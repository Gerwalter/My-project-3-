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
    [SerializeField] private Vector2 spawnAreaSize = new Vector2(10f, 10f);
    [SerializeField] private float spawnYPosition = 0f;

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

            foreach (var item in spawnData.enemyToSpawn)
            {
                Vector3 spawnPosition = GetRandomSpawnPosition();

                // Instanciar el enemigo en la posici�n aleatoria
                Instantiate(item, spawnPosition, Quaternion.identity);
            }
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        float randomX = UnityEngine.Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2);
        float randomZ = UnityEngine.Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2);
        return new Vector3(randomX, spawnYPosition, randomZ);
    }

    private void OnDrawGizmos()
    {
        // Establecer el color del Gizmo (puedes cambiarlo seg�n lo que prefieras)
        Gizmos.color = Color.green;

        // Posici�n central del �rea de spawn
        Vector3 centerPosition = new Vector3(transform.position.x, spawnYPosition, transform.position.z);

        // Dibujar el �rea de spawn como un rect�ngulo (usando WireCube para dibujar solo los bordes)
        Gizmos.DrawWireCube(centerPosition, new Vector3(spawnAreaSize.x, 0, spawnAreaSize.y));
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
