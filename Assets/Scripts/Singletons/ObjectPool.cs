using Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Singletons
{
    public class ObjectPool : Singleton<ObjectPool>
    {
        #region Singleton

        /// <inheritdoc/>
        protected override bool DestroyOnLoad => true;

        /// <inheritdoc/>
        protected override void OnAwake() => this.InitializePooledTypes();

        #endregion

        #region Evaluation

        [SerializeField]
        [Tooltip("Keeps tracks of the number of projectiles spawned. Usefull to determine the amount needed")]
        private bool isEvaluating = false;

        /// <summary>
        /// Increases the counter for <paramref name="prefabName"/> in <paramref name="namespace"/>
        /// </summary>
        /// <param name="prefabName">Name of the prefab</param>
        /// <param name="namespace">Name of the category of the prefab</param>
        private void IncreaseEvaluate(string prefabName, string @namespace)
        {
            // Find type
            var typeIndex = -1;
            PoolingType poolingType = default;
            for (var i = 0; i < this.pooledTypes.Length; i++)
            {
                if (this.pooledTypes[i].Namespace != @namespace)
                    continue;

                typeIndex = i;
                poolingType = this.pooledTypes[i];
                break;
            }

            // Find setting
            var settingIndex = 0;
            for (var i = 0; i < poolingType.Settings.Length; i++)
            {
                if (poolingType.Settings[i].Prefab.name != prefabName)
                    continue;

                settingIndex = i;
                break;
            }

            // Reput
            this.pooledTypes[typeIndex].Settings[settingIndex].amountSpawned++;
        }

        #endregion Evaluation

        #region Pooled Object

        /// <param name="prefabName">
        /// Name of the <see cref="GameObject"/> to get
        /// </param>
        /// <returns>
        /// First instance of the <see cref="GameObject"/> that has the given name
        /// </returns>
        public GameObject GetPooledObject(string prefabName, string @namespace)
        {
            var key = GetKey(prefabName, @namespace);

            if (!this.pooledInfos.TryGetValue(key, out (Transform parent, GameObject prefab) infos))
            {
                Debug.LogError($"The prefab \'{prefabName}\' was not pooled.");
                return null;
            }

            GameObject child = this.FindChildren(infos.parent);

            if (child == null)
            {
                if (!this.isEvaluating)
                    Debug.LogWarning($"Consider increasing the initial amount of \'{prefabName}\'.");

                child = this.CreateObject(infos.parent, infos.prefab);
            }

            foreach (IResetable item in child.GetComponents<IResetable>())
                item.OnReset();

            return child;
        }

        public GameObject GetRandomObject(string @namespace)
        {
            // Find namespace child
            Transform namespaceParent = this.transform.Find(@namespace);

            // Get random child
            var prefabName = namespaceParent.GetChild(UnityEngine.Random.Range(0, namespaceParent.childCount)).name;

            // Get instance
            return this.GetPooledObject(prefabName, @namespace);
        }

        private GameObject FindChildren(Transform parent)
        {
            // Find the first instance of a disabled object
            foreach (Transform child in parent)
            {
                if (child.gameObject.activeInHierarchy)
                    continue;

                return child.gameObject;
            }

            return null;
        }

        /// <summary>
        /// Creates a new instance of the given <see cref="GameObject"/>
        /// </summary>
        /// <param name="prefab"><see cref="GameObject"/> to create</param>
        /// <param name="namespace">Name of the category of this prefab</param>
        /// <returns><see cref="GameObject"/> created</returns>
        private GameObject CreateObject(Transform parent, GameObject prefab)
        {
            if (this.isEvaluating)
                this.IncreaseEvaluate(prefab.name, parent.parent.name);

            // Create object
            return GameObject.Instantiate(prefab, parent);
        }

        /// <returns>Key of the prefab in <see cref="pooledInfos"/></returns>
        private static string GetKey(string prefabName, string @namespace) => @namespace + "/" + prefabName;

        /// <summary>
        /// Disables every objects under <paramref name="namespace"/> 
        /// that matches <paramref name="predicate"/>
        /// </summary>
        /// <param name="namespace">Name of the category of the object</param>
        /// <param name="predicate">Condition to disable the objects</param>
        public void DisableObjects(string @namespace, Predicate<GameObject> predicate)
        {
            // Skip all if the condition is invalid
            if (predicate == null)
                return;

            // Get type
            PoolingType type = this.pooledTypes.First(t => t.Namespace == @namespace);

            foreach (PoolingSetting item in type.Settings)
            {
                // Get parent
                Transform parent = this.pooledInfos[GetKey(item.Prefab.name, @namespace)].parent;

                // Get children
                foreach (Transform item1 in parent)
                {
                    // Skip if not active
                    if (!item1.gameObject.activeInHierarchy)
                        continue;

                    // If the gameobject meets the condition
                    if (predicate.Invoke(item1.gameObject))
                        item1.gameObject.SetActive(false);
                }
            }
        }

        #endregion Pooled Object

        #region Pooling Type

        private void InitializePooledTypes()
        {
            foreach (PoolingType item in this.pooledTypes)
            {
                // Create the namespace parent
                var newParent = new GameObject()
                {
                    name = item.Namespace,
                };
                newParent.transform.parent = this.transform;

                foreach (PoolingSetting setting in item.Settings)
                {
                    // Create the prefab parent
                    var newSetting = new GameObject()
                    {
                        name = setting.Prefab.name,
                    };

                    newSetting.transform.parent = newParent.transform;

                    this.pooledInfos.Add(GetKey(setting.Prefab.name, item.Namespace), (newSetting.transform, setting.Prefab));

                    // Create every prefab
                    for (var i = 0; i < setting.amount; i++)
                    {
                        GameObject newPrefab = this.CreateObject(newSetting.transform, setting.Prefab);
                        newPrefab.SetActive(false);
                    }
                }
            }
        }

        [SerializeField]
        private PoolingType[] pooledTypes;

        // "Namespace/PrefabName" => (Parent, Prefab)
        private readonly Dictionary<string, (Transform parent, GameObject prefab)> pooledInfos = new();

        /// <summary>
        /// Defines the category under which the settings are
        /// </summary>
        [Serializable]
        public struct PoolingType
        {
            [Tooltip("Under which category those objects are")]
            public string Namespace;

            [Tooltip("List of settings associated with this type")]
            public PoolingSetting[] Settings;
        }

        /// <summary>
        /// Defines how this object pool should initializes
        /// </summary>
        [Serializable]
        public struct PoolingSetting
        {
            [Tooltip("GameObject to spawn")]
            public GameObject Prefab;

            [Min(0), Tooltip("Amount of objects to create at the start of this script")]
            public int amount;

            [Tooltip("Shows how many objects of this type has been spawned")]
            public int amountSpawned;
        }

        #endregion Pooling Type
    }
}