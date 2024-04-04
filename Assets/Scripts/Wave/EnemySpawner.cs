using Entities;
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

    private Vector2 scaleMultiplier;

    private void Start()
    {
        Vector2 defaultScale = new(48.02f, 30.00f); // aka 16:10 aspect
        Vector2 currentScale = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

        this.scaleMultiplier = currentScale / defaultScale;

        _ = this.StartCoroutine(this.Prepare_Spawner());
    }

    private System.Collections.IEnumerator Prepare_Spawner()
    {
        PrepareWaves(this.waves);
        yield return new WaitForEndOfFrame();
        this.Trigger();
    }

    public void Trigger()
    {
        this.RemoveAll();
        this.timer = 10;
        this.SpawnWave();
    }

    public void SpawnWave()
    {
        List<GameObject> enemies = LoadWave(this.waves[Random.Range(0, this.waves.Length)], this.scaleMultiplier);

        // Wait until all the enemies are dead or timer to run out
        _ = this.Add(_ => !enemies.Any(o => o.activeInHierarchy));
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
    private static List<GameObject> LoadWave(GameObject prefab, Vector2 scaleMultiplier)
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
            enemy.transform.position = child.position * scaleMultiplier;

            // Set up enemy
            EnemyEnter enter = enemy.GetComponentInChildren<EnemyEnter>();
            EnemyEnter childEnter = child.GetComponentInChildren<EnemyEnter>();
            enter.Copy(childEnter);
            enter.Scale(scaleMultiplier);

            // Remove all Predicates
            foreach (Predicates.Predicate item in enter.GetComponents<Predicates.Predicate>())
                Destroy(item);

            // Copy predicate
            if (childEnter.TryGetComponent(out Predicates.Predicate predicate))
                _ = predicate.CopyComponent(enter.gameObject);

            // Reset
            enter.OnReset();

            // Start enemy
            enemy.SetActive(true);

            enemies.Add(enemy);
        }

        SetUpLinks(enemies, prefab.GetComponent<WaveDataMaker>());

        return enemies;
    }

    private static void SetUpLinks(List<GameObject> enemies, WaveDataMaker wave)
    {
        var references = new List<(BaseEntity entity, IPredicateEntity listener)>();

        foreach (GameObject item in enemies)
        {
            BaseEntity entity = item.GetComponent<BaseEntity>();
            EnemyEnter enter = item.GetComponentInChildren<EnemyEnter>();
            IPredicateEntity predicate = enter == null ? null : enter.GetComponent<IPredicateEntity>();

            references.Add((entity, predicate));
        }

        // Get links
        WaveLink[] links = wave.data.links;
        var entitiesForListeners = new Dictionary<IPredicateEntity, List<BaseEntity>>();

        foreach (WaveLink item in links)
        {
            // Get own
            IPredicateEntity listener = references[item.From].listener;

            // Get to
            BaseEntity target = references[item.To].entity;

            // Set to
            if (listener == null)
                continue;

            if (!entitiesForListeners.ContainsKey(listener))
                entitiesForListeners[listener] = new();

            entitiesForListeners[listener].Add(target);
        }

        // Notify each others
        foreach (KeyValuePair<IPredicateEntity, List<BaseEntity>> item in entitiesForListeners)
            item.Key.SetEntities(item.Value.ToArray());
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

    #endregion Static
}