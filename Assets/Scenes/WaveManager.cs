using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEditor.Progress;

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
    [SerializeField] private Vector2 spawnAreaSize = new Vector2(10f, 10f);
    [SerializeField] private float spawnYPosition = 0f;

    private Dictionary<EnemyType, LootData> _enemyLoot = new Dictionary<EnemyType, LootData>();

    public static WaveManager Instance;

    private Queue<EnemyWaveData> _spawnOrder = new Queue<EnemyWaveData>();

    public float _timer;


    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this);

        _enemyLoot.Add(EnemyType.MELEE, new LootData { gold = 30, xp = 10 });

        //Manera de ver si no existe esta key
        if (!_enemyLoot.ContainsKey(EnemyType.MELEE))
            _enemyLoot.Add(EnemyType.MELEE, new LootData { gold = 30, xp = 10 });

        //Opcion automatica
        _enemyLoot.TryAdd(EnemyType.MELEE, new LootData { gold = 30, xp = 10 });

        _enemyLoot.Add(EnemyType.RANGE, new LootData { gold = 45, xp = 10 });
        _enemyLoot.Add(EnemyType.TANK, new LootData { gold = 25, xp = 40 });
        _enemyLoot.Add(EnemyType.BOSS, new LootData { gold = 100, xp = 100 });

        //Sobreescribir value
        _enemyLoot[EnemyType.MELEE] = new LootData { gold = 60, xp = 20 };

        //Remover valor
        //_enemyLoot.Remove(EnemyType.MELEE);

        //Manera de limpiar el diccionario
        //_enemyLoot.Clear();


        _spawnOrder.Enqueue(normalEnemyWave);
        //_spawnOrder.Enqueue(ligthEnemyWave);
       // _spawnOrder.Enqueue(normalEnemyWave);
        //_spawnOrder.Enqueue(heavyEnemyWave);
        //_spawnOrder.Enqueue(ligthEnemyWave);
        //_spawnOrder.Enqueue(bossEnemyWave);
    }

    private void Update()
    {
        _timer += Time.deltaTime;

        if (_timer > 1)
        {

            _timer = 0;
            var spawnData = _spawnOrder.Dequeue();

            foreach (var item in spawnData.enemyToSpawn)
            {
                Instantiate(item);
            }
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        float randomX = UnityEngine.Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2);
        float randomZ = UnityEngine.Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2);
        return new Vector3(randomX, spawnYPosition, randomZ);
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
