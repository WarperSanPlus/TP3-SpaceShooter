using Extensions;
using Serializables;
using Singletons;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject[] waves;
    private float timer;

    private void Start() => PrepareWaves(this.waves);

    private void LoadWave(GameObject prefab)
    {
        // Get all enemies
        foreach (Transform child in prefab.transform)
        {
            Transform sourcePrefab = PrefabUtility.GetCorrespondingObjectFromSource(child);

            if (sourcePrefab == null)
                continue;

            // Get enemy
            GameObject enemy = ObjectPool.GetPooledObject(sourcePrefab.name, "Enemies");

            // Place enemy
            enemy.transform.position = child.transform.position;

            // Set up enemy
            enemy.GetComponent<EnemyEnter>().Copy(child.GetComponent<EnemyEnter>());
            enemy.GetComponent<EnemyEnter>().OnReset();

            // Start enemy
            enemy.SetActive(true);
        }
    }

    private void FixedUpdate()
    {
        this.timer -= Time.fixedDeltaTime;

        if (this.timer > 0)
            return;

        this.LoadWave(this.waves[Random.Range(0, this.waves.Length)]);
        this.timer = 25f;
    }

    private static void PrepareWaves(GameObject[] waves)
    {
        // Compile waves settings
        var settings = new Dictionary<string, PoolingSetting>();

        foreach (GameObject wave in waves)
        {
            if (!wave.TryGetComponent(out WaveDataMaker waveData))
                continue;

            PoolingSetting[] enemySetting = waveData.data.enemySetting;
            foreach (PoolingSetting enemy in enemySetting)
            {
                PoolingSetting s = enemy;
                s.amount = Mathf.FloorToInt(s.amount * 1.2f);

                var key = enemy.Prefab.name;

                if (settings.ContainsKey(key))
                    s.amount = Mathf.Max(settings[key].amount, s.amount);

                settings[key] = s;
            }
        }

        // Prepare settings
        foreach (KeyValuePair<string, PoolingSetting> item in settings)
            ObjectPool.PreparePoolSetting(item.Value, Entities.EnemyEntity.NAMESPACE, false);
    }
}
