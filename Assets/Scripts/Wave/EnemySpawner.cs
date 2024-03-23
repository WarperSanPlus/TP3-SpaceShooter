using Extensions;
using Interfaces;
using Serializables;
using Singletons;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class EnemySpawner : MonoBehaviour, IPredicatable
{
    public GameObject[] waves;
    private float timer;

    private void Start()
    {
        PrepareWaves(this.waves);
        this.Trigger();
    }

    public void Trigger()
    {
        this.Remove();
        this.timer = 10;
        this.SpawnWave();
    }

    public void SpawnWave()
    {
        List<GameObject> enemies = LoadWave(this.waves[Random.Range(0, this.waves.Length)]);

        // Wait until all the enemies are dead or timer to run out
        this.Add((_) => !enemies.Any(o => o.activeInHierarchy));
        //this.Add((float elapsed) =>
        //{
        //    this.timer -= elapsed;

        //    return this.timer <= 0;
        //});
    }

    #region Static

    /// <summary>
    /// Places all the enemies in the given prefab
    /// </summary>
    /// <returns>List of the enemies created</returns>
    private static List<GameObject> LoadWave(GameObject prefab)
    {
        var enemies = new List<GameObject>();

        if (prefab == null)
            return enemies;

        // Get all enemies
        foreach (Transform child in prefab.transform)
        {
            Transform sourcePrefab = PrefabUtility.GetCorrespondingObjectFromSource(child);

            if (sourcePrefab == null)
                continue;

            // Get enemy
            GameObject enemy = ObjectPool.GetPooledObject(sourcePrefab.name, Entities.EnemyEntity.NAMESPACE);

            if (enemy == null)
                continue;

            // Place enemy
            enemy.transform.position = child.position;

            // Set up enemy
            EnemyEnter enter = enemy.GetComponentInChildren<EnemyEnter>();
            EnemyEnter childEnter = child.GetComponentInChildren<EnemyEnter>();
            enter.Copy(childEnter);

            // Remove all Predicates
            foreach (Predicates.PredicateScript item in enter.GetComponents<Predicates.PredicateScript>())
                Destroy(item);

            // Copy predicate
            if (childEnter.TryGetComponent(out Predicates.PredicateScript predicate))
                predicate.CopyComponent(enter.gameObject);

            // Reset
            enter.OnReset();

            // Start enemy
            enemy.SetActive(true);

            enemies.Add(enemy);
        }

        return enemies;
    }

    /// <summary>
    /// Calculates the amount of each object needed for <paramref name="waves"/>
    /// </summary>
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
                s.amount = Mathf.CeilToInt(s.amount * 1.2f);

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

    #endregion
}
