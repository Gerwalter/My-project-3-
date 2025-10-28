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
    public float spawnDelay = 2f;

    [Header("Wave Settings")]
    public int totalWaves = 5;
    public List<int> enemiesPerWave = new List<int> { 3, 5, 7, 9, 12 };
    public float timeBetweenWaves = 5f;

    [Header("Indicator")]
    public GameObject spawnIndicatorPrefab;

    public bool allWavesCompleted { get; private set; } = false;

    // NUEVO: Contador global de enemigos
    private int totalEnemiesToSpawn = 0;
    private int enemiesDefeated = 0;

    void Start()
    {
        factory = new EnemyFactory();

        // Calcular total de enemigos
        totalEnemiesToSpawn = 0;
        foreach (int count in enemiesPerWave)
            totalEnemiesToSpawn += count;

        // Ajustar si hay menos oleadas que elementos en la lista
        totalEnemiesToSpawn = Mathf.Min(totalEnemiesToSpawn, enemiesPerWave.Count * totalWaves);

        // Pool más grande si es necesario
        int poolSize = Mathf.Max(20, totalEnemiesToSpawn);
        var pool = new ObjectPool<Enemy>(enemyPrefab, poolSize, parent);
        factory.RegisterPool("Enemy", pool);

        // Suscribir al evento de muerte
        Enemy.OnEnemyDefeated += OnEnemyDefeated;

        StartCoroutine(SpawnWaves());
    }

    private void OnDestroy()
    {
        Enemy.OnEnemyDefeated -= OnEnemyDefeated;
    }
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
    private void OnEnemyDefeated()
    {
        enemiesDefeated++;
        Debug.Log($"Enemigos derrotados: {enemiesDefeated}/{totalEnemiesToSpawn}");

        if (enemiesDefeated >= totalEnemiesToSpawn && allWavesCompleted)
        {
            Debug.Log("¡Todos los enemigos derrotados! Batalla ganada.");
            FindObjectOfType<BattleEnd>()?.OnBattleWon();
        }
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
