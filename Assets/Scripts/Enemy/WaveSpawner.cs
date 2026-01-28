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
    public List<int> baseEnemiesPerWave = new List<int> { 4, 5, 6 };
    public float timeBetweenWaves = 5f;

    [Header("Wave Progression Mode")]
    [Tooltip("Si está en true → espera a que mueran TODOS los enemigos de la oleada actual antes de la siguiente")]
    public bool waitForWaveClear = false;           // ← Este es el booleano que controlas

    [Header("Indicator")]
    public GameObject spawnIndicatorPrefab;

    public bool allWavesCompleted { get; private set; } = false;

    // Contadores globales
    private int totalEnemiesToSpawn = 0;
    private int enemiesDefeated = 0;
    private int totalWaves;

    // Contadores por oleada (solo se usan cuando waitForWaveClear = true)
    private int enemiesInCurrentWave = 0;
    private int enemiesDefeatedInCurrentWave = 0;

    void Start()
    {
        factory = new EnemyFactory();

        // Cálculo de dificultad según alerta (igual que antes)
        float currentAlert = ThiefAlertSystem.instance?.ObtainValue() ?? 0f;
        float maxAlert = ThiefAlertSystem.instance?._MaxAlert ?? 100f;
        float alertNorm = currentAlert / maxAlert;

        int extraWaves = GetExtraWaves(alertNorm);
        totalWaves = baseWaves + extraWaves;

        Debug.Log($"[WaveSpawner] Alerta: {currentAlert:F0}/{maxAlert} ({alertNorm:P0}) → +{extraWaves} oleadas extra → Total: {totalWaves}");

        // Construir lista final de enemigos por oleada
        List<int> finalEnemiesPerWave = new List<int>(baseEnemiesPerWave);
        int lastBaseCount = baseEnemiesPerWave.Count > 0 ? baseEnemiesPerWave[^1] : 5;

        for (int i = 0; i < extraWaves; i++)
        {
            int extraEnemies = lastBaseCount + 1 + i;
            finalEnemiesPerWave.Add(extraEnemies);
        }

        totalEnemiesToSpawn = 0;
        foreach (int count in finalEnemiesPerWave)
            totalEnemiesToSpawn += count;

        Debug.Log($"[WaveSpawner] Enemigos totales: {totalEnemiesToSpawn}");

        // Pool
        int poolSize = Mathf.Max(25, totalEnemiesToSpawn + 15);
        var pool = new ObjectPool<Enemy>(enemyPrefab, poolSize, parent);
        factory.RegisterPool("Enemy", pool);

        Enemy.OnEnemyDefeated += OnEnemyDefeated;
        StartCoroutine(SpawnWaves(finalEnemiesPerWave));
    }

    private int GetExtraWaves(float alert01)
    {
        if (alert01 >= 1.00f) return 2;
        if (alert01 >= 0.80f) return 2;
        if (alert01 >= 0.60f) return 1;
        if (alert01 >= 0.30f) return 1;
        return 0;
    }

    void OnDestroy()
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
        enemiesDefeatedInCurrentWave++;

        Debug.Log($"Progreso global: {enemiesDefeated}/{totalEnemiesToSpawn} | Oleada actual: {enemiesDefeatedInCurrentWave}/{enemiesInCurrentWave}");

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
            Debug.Log($"Iniciando Oleada {wave + 1}{(isExtra ? " (EXTRA)" : "")} → {count} enemigos");

            enemiesInCurrentWave = count;
            enemiesDefeatedInCurrentWave = 0;

            // Spawnear enemigos de esta oleada
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

                yield return new WaitForSeconds(0.6f);   // pequeño delay entre spawns dentro de la misma oleada
            }

            // Si está activado el modo "esperar limpieza de oleada"
            if (waitForWaveClear)
            {
                Debug.Log($"[Wave {wave + 1}] Esperando a que se derroten los {count} enemigos...");

                // Esperamos hasta que todos los enemigos de ESTA oleada hayan muerto
                yield return new WaitUntil(() => enemiesDefeatedInCurrentWave >= enemiesInCurrentWave);

                Debug.Log($"[Wave {wave + 1}] ¡Oleada limpia! Esperando {timeBetweenWaves}s antes de la siguiente...");

                yield return new WaitForSeconds(timeBetweenWaves);
            }
            else
            {
                // Modo original: solo espera entre oleadas sin importar si siguen vivos
                if (wave < enemiesPerWave.Count - 1)
                    yield return new WaitForSeconds(timeBetweenWaves);
            }
        }

        allWavesCompleted = true;
        Debug.Log("¡Todas las oleadas han sido lanzadas! (esperando limpieza final si corresponde)");
    }
}