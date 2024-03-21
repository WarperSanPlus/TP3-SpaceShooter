using Serializables;
using System.Collections.Generic;
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
        instance.data.enemySetting = GetEnemySettings(instance.transform);

        // Save data
        if (!hasData)
            AssetDatabase.CreateAsset(instance.data, $"Assets/Data/Waves/{instance.gameObject.name}.asset");

        // Save prefab
        EditorUtility.SetDirty(instance);
        Undo.RecordObject(instance, "Changed " + instance.GetType().Name);
        PrefabUtility.RecordPrefabInstancePropertyModifications(instance);
    }

    private static PoolingSetting[] GetEnemySettings(Transform parent)
    {
        // Get all enemies
        var enemies = new Dictionary<string, (int, GameObject)>();

        foreach (Transform child in parent)
        {
            Transform sourcePrefab = PrefabUtility.GetCorrespondingObjectFromSource(child);

            if (sourcePrefab == null)
                continue;

            (int, GameObject) c = (1, sourcePrefab.gameObject);

            if (enemies.ContainsKey(sourcePrefab.name))
            {
                c = enemies[sourcePrefab.name];
                c.Item1++;
            }

            enemies[sourcePrefab.name] = c;
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
#endif
}
