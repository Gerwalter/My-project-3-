using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveSpawner : MonoBehaviour
{
    [Header("Enemy Settings")]
    public Enemy enemyPrefab;
    public Transform parent;
    private EnemyFactory factory;

    [Header("Spawn Settings")]
    public BoxCollider spawnArea;
    public float spawnDelay = 2f; // tiempo entre indicador y aparición del enemigo

    [Header("Wave Settings")]
    public int totalWaves = 5; // número total de oleadas
    public List<int> enemiesPerWave = new List<int> { 3, 5, 7, 9, 12 }; // cantidad por oleada
    public float timeBetweenWaves = 5f; // tiempo entre oleadas

    [Header("Indicator")]
    public GameObject spawnIndicatorPrefab;
    public bool allWavesCompleted { get; private set; } = false;
    private Vector3 GetRandomSpawnPosition()
    {
        if (spawnArea == null) return Vector3.zero;

        Vector3 bounds = spawnArea.size;
        Vector3 localPos = new Vector3(
            Random.Range(-bounds.x / 2, bounds.x / 2),
            0,
            Random.Range(-bounds.z / 2, bounds.z / 2)
        );

        return spawnArea.transform.TransformPoint(localPos);
    }

    void OnDrawGizmos()
    {
        if (spawnArea != null)
        {
            Gizmos.color = new Color(1, 1, 0, 0.3f);
            Gizmos.DrawWireCube(spawnArea.transform.position, spawnArea.size);
        }
    }

    void Start()
    {
        factory = new EnemyFactory();

        // Crear un pool base
        var pool = new ObjectPool<Enemy>(enemyPrefab, 20, parent);
        factory.RegisterPool("Enemy", pool);

        StartCoroutine(SpawnWaves());
    }

    IEnumerator SpawnWaves()
    {
        for (int wave = 0; wave < totalWaves; wave++)
        {
            int enemiesToSpawn = (wave < enemiesPerWave.Count) ? enemiesPerWave[wave] : enemiesPerWave[enemiesPerWave.Count - 1];
            Debug.Log($"Oleada {wave + 1} - Enemigos: {enemiesToSpawn}");

            for (int i = 0; i < enemiesToSpawn; i++)
            {
                var pos = GetRandomSpawnPosition();
                if (spawnIndicatorPrefab != null)
                {
                    GameObject indicator = Instantiate(spawnIndicatorPrefab, pos + new Vector3(0, .03f, 0), Quaternion.Euler(-90f, 0f, 0f));
                    Destroy(indicator, spawnDelay + 1);
                }

                yield return new WaitForSeconds(spawnDelay);
                factory.Create("Enemy", pos);
                yield return new WaitForSeconds(0.5f);
            }

            yield return new WaitForSeconds(timeBetweenWaves);
        }

        Debug.Log("Todas las oleadas han terminado.");
        allWavesCompleted = true;
    }
}
