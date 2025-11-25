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

    [Header("Wave Settings - Base (Demo)")]
    public int baseWaves = 3;
    public List<int> baseEnemiesPerWave = new List<int> { 4, 5, 6 };  // Total base: 15 enemigos
    public float timeBetweenWaves = 5f;  // Más rápido para demo

    [Header("Indicator")]
    public GameObject spawnIndicatorPrefab;

    public bool allWavesCompleted { get; private set; } = false;

    // Contadores
    private int totalEnemiesToSpawn = 0;
    private int enemiesDefeated = 0;
    private int totalWaves;

    void Start()
    {
        factory = new EnemyFactory();

        // === CALCULAR DIFICULTAD SEGÚN ALERTA (DEMO MODE) ===
        float currentAlert = ThiefAlertSystem.instance?.ObtainValue() ?? 0f;
        float maxAlert = ThiefAlertSystem.instance?._MaxAlert ?? 100f;
        float alertNorm = currentAlert / maxAlert;

        int extraWaves = GetExtraWaves(alertNorm);
        totalWaves = baseWaves + extraWaves;

        Debug.Log($"[WaveSpawner] Alerta: {currentAlert:F0}/{maxAlert} ({alertNorm:P0}) → +{extraWaves} oleadas extra → Total oleadas: {totalWaves}");

        // === CONSTRUIR LISTA FINAL ===
        List<int> finalEnemiesPerWave = new List<int>(baseEnemiesPerWave);
        int lastBaseCount = baseEnemiesPerWave[baseEnemiesPerWave.Count - 1];

        for (int i = 0; i < extraWaves; i++)
        {
            // Progresión suave para demo: 7, 8...
            int extraEnemies = lastBaseCount + 1 + i;
            finalEnemiesPerWave.Add(extraEnemies);
        }

        // Total enemigos
        totalEnemiesToSpawn = 0;
        foreach (int count in finalEnemiesPerWave)
            totalEnemiesToSpawn += count;

        Debug.Log($"[WaveSpawner] Enemigos totales: {totalEnemiesToSpawn} (¡Perfecto para demo!)");

        // Pool pequeño para demo
        int poolSize = Mathf.Max(25, totalEnemiesToSpawn + 15);
        var pool = new ObjectPool<Enemy>(enemyPrefab, poolSize, parent);
        factory.RegisterPool("Enemy", pool);

        Enemy.OnEnemyDefeated += OnEnemyDefeated;
        StartCoroutine(SpawnWaves(finalEnemiesPerWave));
    }

    private int GetExtraWaves(float alert01)
    {
        // 5 niveles: 0(<30%), 1(30-59%), 2(60-79%), 3(80-99%), 4(100%)
        // Extra oleadas escaladas para demo (máx. 2 extra → 5 totales)
        if (alert01 >= 1.00f) return 2;   // Nivel 4: +2 (5 oleadas)
        if (alert01 >= 0.80f) return 2;   // Nivel 3: +2 (5 oleadas)
        if (alert01 >= 0.60f) return 1;   // Nivel 2: +1 (4 oleadas)
        if (alert01 >= 0.30f) return 1;   // Nivel 1: +1 (4 oleadas)
        return 0;                         // Nivel 0: 0 (3 oleadas)
    }

    private void OnDestroy()
    {
        Enemy.OnEnemyDefeated -= OnEnemyDefeated;
    }

    private Vector3 GetRandomSpawnPosition()
    {
        if (spawnArea == null) return transform.position;

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
        Debug.Log($"Progreso: {enemiesDefeated}/{totalEnemiesToSpawn} enemigos");

        if (enemiesDefeated >= totalEnemiesToSpawn && allWavesCompleted)
        {
            Debug.Log("¡DEMO VICTORIA! Todos los enemigos eliminados.");
            FindObjectOfType<BattleEnd>()?.OnBattleWon();
        }
    }

    IEnumerator SpawnWaves(List<int> enemiesPerWave)
    {
        for (int wave = 0; wave < enemiesPerWave.Count; wave++)
        {
            int count = enemiesPerWave[wave];
            bool isExtra = wave >= baseWaves;
            Debug.Log($"Oleada {wave + 1}{(isExtra ? " (EXTRA)" : "")} → {count} enemigos");

            for (int i = 0; i < count; i++)
            {
                Vector3 pos = GetRandomSpawnPosition();

                if (spawnIndicatorPrefab != null)
                {
                    var ind = Instantiate(spawnIndicatorPrefab, pos + new Vector3(0, 0.03f, 0), Quaternion.Euler(-90f, 0f, 0f));
                    Destroy(ind, spawnDelay + 1f);
                }

                yield return new WaitForSeconds(spawnDelay);
                factory.Create("Enemy", pos);
                yield return new WaitForSeconds(0.6f);
            }

            if (wave < enemiesPerWave.Count - 1)
                yield return new WaitForSeconds(timeBetweenWaves);
        }

        allWavesCompleted = true;
        Debug.Log("¡Oleadas completadas! (Demo lista para victoria)");
    }
}