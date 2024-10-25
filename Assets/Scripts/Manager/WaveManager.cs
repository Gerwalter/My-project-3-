using System;
using System.Collections.Generic;
using UnityEngine;

public struct LootData
{
    public int gold;
    public int xp;
}

[Serializable]
public struct EnemyWaveData
{
    public List<Enemy> enemyToSpawn;
}

public class WaveManager : MonoBehaviour
{
    [SerializeField] private EnemyWaveData normalEnemyWave;
    [SerializeField] private EnemyWaveData heavyEnemyWave;
    [SerializeField] private EnemyWaveData ligthEnemyWave;
    [SerializeField] private EnemyWaveData bossEnemyWave;

    [Header("Spawn Area Settings")]
    [SerializeField] private List<Transform> spawnPoints; // Lista de puntos de spawn

    private Dictionary<EnemyType, LootData> _enemyLoot = new Dictionary<EnemyType, LootData>();
    public static WaveManager Instance;

    private Queue<EnemyWaveData> _spawnOrder = new Queue<EnemyWaveData>();

    public float _timer;
    public float _spawn;


    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this);

        // Agrega valores al diccionario de loot
        _enemyLoot.Add(EnemyType.MELEE, new LootData { gold = 60, xp = 20 });
        _enemyLoot.Add(EnemyType.RANGE, new LootData { gold = 45, xp = 10 });
        _enemyLoot.Add(EnemyType.TANK, new LootData { gold = 25, xp = 40 });
        _enemyLoot.Add(EnemyType.BOSS, new LootData { gold = 100, xp = 100 });

        // A�adir oleadas al orden de spawn
        _spawnOrder.Enqueue(normalEnemyWave);
        _spawnOrder.Enqueue(ligthEnemyWave);
        _spawnOrder.Enqueue(normalEnemyWave);
        _spawnOrder.Enqueue(heavyEnemyWave);
        _spawnOrder.Enqueue(ligthEnemyWave);
        _spawnOrder.Enqueue(bossEnemyWave);
    }

    private void Update()
    {
        _timer += Time.deltaTime;

        // Verifica si hay elementos en la cola
        if (_spawnOrder.Count == 0)
        {
            // Si no hay m�s oleadas que spawnar, det�n la ejecuci�n
            return;
        }

        if (_timer > _spawn)
        {
            _timer = 0;
            var spawnData = _spawnOrder.Dequeue();

            foreach (var enemy in spawnData.enemyToSpawn)
            {
                Transform spawnPoint = GetRandomSpawnPoint();

                // Instanciar el enemigo en la posici�n del punto de spawn
                Instantiate(enemy, spawnPoint.position, Quaternion.identity);
            }
        }
    }

    // Devuelve un punto de spawn aleatorio de la lista
    private Transform GetRandomSpawnPoint()
    {
        int randomIndex = UnityEngine.Random.Range(0, spawnPoints.Count);
        return spawnPoints[randomIndex];
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        foreach (var spawnPoint in spawnPoints)
        {
            // Dibujar los puntos de spawn como esferas en la escena para visualizarlos mejor
            Gizmos.DrawSphere(spawnPoint.position, 0.5f);
        }
    }

    public LootData GetLoot(EnemyType enemyType)
    {
        if (_enemyLoot.TryGetValue(enemyType, out var lootData))
        {
            return lootData;
        }

        return new LootData { gold = 0, xp = 0 };
    }
}
