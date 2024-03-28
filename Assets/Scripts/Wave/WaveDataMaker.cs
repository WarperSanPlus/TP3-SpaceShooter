using Serializables;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class WaveDataMaker : MonoBehaviour
{
    public WaveData data;

#if UNITY_EDITOR
    [SerializeField]
    private bool overrideCompiling = false;

    private void Start() => CompileData(this);

    private static void CompileData(WaveDataMaker instance)
    {
        if (instance.overrideCompiling)
        {
            print("The data compiling was stopped due to it's override");
            return;
        }

        var hasData = instance.data != null;

        // If data not set, create data
        if (!hasData)
            instance.data = ScriptableObject.CreateInstance<WaveData>();

        print($"Saving the data for '{instance.gameObject.name}'");

        // Compile data
        Dictionary<Transform, Transform> enemies = GetEnemies(instance.transform);
        instance.data.enemySetting = GetEnemySettings(enemies.Values.AsEnumerable());
        _ = GetLinks(enemies.Keys.AsEnumerable(), out List<(int own, int to)> links);

        instance.data.links = new WaveLink[links.Count];
        for (var i = 0; i < links.Count; i++)
        {
            (var own, var to) = links[i];

            instance.data.links[i] = new WaveLink()
            {
                From = own,
                To = to,
            };
        }

        // Save data
        if (!hasData)
            AssetDatabase.CreateAsset(instance.data, $"Assets/Data/Waves/{instance.gameObject.name}.asset");

        // Save prefab
        EditorUtility.SetDirty(instance);
        Undo.RecordObject(instance, "Changed " + instance.GetType().Name);
        PrefabUtility.RecordPrefabInstancePropertyModifications(instance);
    }

    private static Dictionary<Transform, Transform> GetEnemies(Transform parent)
    {
        // Get all enemies
        var enemies = new Dictionary<Transform, Transform>();

        foreach (Transform child in parent)
        {
            if (!child.gameObject.activeInHierarchy)
                continue;

            Transform sourcePrefab = PrefabUtility.GetCorrespondingObjectFromSource(child);

            if (sourcePrefab == null)
                continue;

            enemies.Add(child, sourcePrefab);
        }

        return enemies;
    }

    private static PoolingSetting[] GetEnemySettings(IEnumerable<Transform> prefabs)
    {
        // Get all enemies
        var enemies = new Dictionary<string, (int, GameObject)>();

        foreach (Transform prefab in prefabs)
        {
            (int, GameObject) c = (1, prefab.gameObject);

            if (enemies.ContainsKey(prefab.name))
            {
                c = enemies[prefab.name];
                c.Item1++;
            }

            enemies[prefab.name] = c;
        }

        // Set enemy settings
        var settings = new PoolingSetting[enemies.Count];
        var index = 0;

        foreach (KeyValuePair<string, (int, GameObject)> item in enemies)
        {
            settings[index] = new PoolingSetting()
            {
                Prefab = item.Value.Item2,
                amount = item.Value.Item1
            };

            index++;
        }

        return settings;
    }
    private static Dictionary<Transform, int> GetLinks(IEnumerable<Transform> children, out List<(int own, int to)> links)
    {
        // Attribute an ID to every children
        var ids = new Dictionary<Transform, int>();

        foreach (Transform child in children)
            ids.Add(child, ids.Count);

        links = new();

        // Make links
        foreach (Transform child in children)
        {
            // Get IPredicateEntity script
            EnemyEnter enter = child.GetComponentInChildren<EnemyEnter>();

            if (enter == null)
                continue;

            if (!enter.TryGetComponent(out Interfaces.IPredicateEntity predicateEntity))
                continue;

            // Get the linked entities
            Entities.BaseEntity[] linkedEntities = predicateEntity.GetEntities();

            // Create a list of links 
            foreach (Entities.BaseEntity entity in linkedEntities)
            {
                // Get own id
                var own = ids[child];

                if (!ids.TryGetValue(entity.transform, out var to))
                    continue;

                // Make link
                links.Add((own, to));
            }
        }

        return ids;
    }
#endif
}
