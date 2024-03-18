using Extensions;
using Singletons;
using UnityEditor;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject[] waves;
    private float timer;

    private void LoadWave(GameObject prefab)
    {
        // Get all enemies
        foreach (Transform child in prefab.transform)
        {
            Transform sourcePrefab = PrefabUtility.GetCorrespondingObjectFromSource(child);

            if (sourcePrefab == null)
                continue;

            // Get enemy
            GameObject enemy = ObjectPool.Instance.GetPooledObject(sourcePrefab.name, "Enemies");

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
}
