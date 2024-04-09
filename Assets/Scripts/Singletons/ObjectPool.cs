using Interfaces;
using Serializables;
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

        #endregion Singleton

        #region Evaluation

#if UNITY_EDITOR

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

#endif

        #endregion Evaluation

        #region Getters

        /// <param name="prefabName">
        /// Name of the <see cref="GameObject"/> to get
        /// </param>
        /// <returns>
        /// First instance of the <see cref="GameObject"/> that has the given name
        /// </returns>
        public static GameObject GetPooledObject(string prefabName, string @namespace)
        {
            var key = GetKey(prefabName, @namespace);

            if (!Instance.pooledInfos.TryGetValue(key, out (Transform parent, GameObject prefab) infos))
            {
                Debug.LogError($"The prefab \'{prefabName}\' was not pooled.");
                return null;
            }

            GameObject child = Instance.FindChildren(infos.parent);

            if (child == null)
            {
#if UNITY_EDITOR
                if (!Instance.isEvaluating)
#endif
                    Debug.LogWarning($"Consider increasing the initial amount of \'{prefabName}\'.");

                child = Instance.CreateObject(infos.parent, infos.prefab);
            }

            foreach (IResetable item in child.GetComponents<IResetable>())
                item.OnReset();

            return child;
        }

        public static GameObject GetRandomObject(string @namespace)
        {
            // Find namespace child
            Transform namespaceParent = Instance.transform.Find(@namespace);

            // Get random child
            var prefabName = namespaceParent.GetChild(UnityEngine.Random.Range(0, namespaceParent.childCount)).name;

            // Get instance
            return GetPooledObject(prefabName, @namespace);
        }

        public static int GetActiveObjectCount(string @namespace)
        {
            Transform namespaceParent = Instance.transform.Find(@namespace);

            if (namespaceParent == null)
                return 0;

            var amount = 0;

            // Get every prefab
            foreach (Transform prefabParent in namespaceParent.transform)
                amount += GetActiveObjectCount(@namespace, prefabParent.name);

            return amount;
        }

        public static int GetActiveObjectCount(string @namespace, string prefabName)
        {
            var key = GetKey(prefabName, @namespace);

            if (!Instance.pooledInfos.ContainsKey(key))
                return 0;

            var amount = 0;

            // Get active children
            foreach (Transform item in Instance.pooledInfos[key].parent)
                amount++;

            return amount;
        }

        #endregion Getters

        #region Pooled Object

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
#if UNITY_EDITOR
            if (this.isEvaluating)
                this.IncreaseEvaluate(prefab.name, parent.parent.name);
#endif

            // Create object
            var newObj = GameObject.Instantiate(prefab, parent);
            newObj.name = parent.name;

            return newObj;
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
                this.CreateType(item);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="type"></param>
        /// <param name="amountInAddition">
        /// Determines if the setting should add <see cref="PoolingSetting.amount"/>
        /// or if it should have at least <see cref="PoolingSetting.amount"/>
        /// </param>
        public static void PreparePoolSetting(PoolingSetting setting, string @namespace, bool amountInAddition = true)
        {
            // Get parent
            Transform parent = Instance.transform.Find(@namespace);

            // If namespace not found, create
            if (parent == null)
            {
                Instance.CreateType(new PoolingType()
                {
                    Namespace = @namespace,
                    Settings = new PoolingSetting[] { setting }
                });
                return;
            }

            var currentCount = parent.Find(setting.Prefab.name)?.childCount ?? 0;

            // If there are enough items, skip
            if (!amountInAddition && currentCount >= setting.amount)
                return;

            // Get the amount to spawn
            setting.amount = amountInAddition ? setting.amount : setting.amount - currentCount;

            // Create the setting
            Instance.CreateSetting(setting, parent, @namespace);
        }

        private void CreateType(PoolingType type)
        {
            // Create the namespace parent
            var newParent = new GameObject()
            {
                name = type.Namespace,
            };
            newParent.transform.parent = this.transform;

            foreach (PoolingSetting setting in type.Settings)
                this.CreateSetting(setting, newParent.transform, newParent.name);
        }

        private void CreateSetting(PoolingSetting setting, Transform parent, string @namespace)
        {
            // Create the prefab parent
            var newSetting = new GameObject()
            {
                name = setting.Prefab.name,
            };

            newSetting.transform.parent = parent;

            this.pooledInfos.Add(
                GetKey(setting.Prefab.name, @namespace),
                (newSetting.transform, setting.Prefab)
                );

            // Create every prefab
            for (var i = 0; i < setting.amount; i++)
            {
                GameObject newPrefab = this.CreateObject(newSetting.transform, setting.Prefab);
                newPrefab.SetActive(false);
            }
        }

        [SerializeField]
        private PoolingType[] pooledTypes;

        // "Namespace/PrefabName" => (Parent, Prefab)
        private readonly Dictionary<string, (Transform parent, GameObject prefab)> pooledInfos = new();

        #endregion Pooling Type
    }
}