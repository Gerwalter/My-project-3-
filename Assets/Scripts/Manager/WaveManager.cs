using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    [SerializeField] private Queue<EnemyWaveData> _spawnOrder = new Queue<EnemyWaveData>();
    public float _timer;
    public float _spawn;

    [SerializeField] private LootManager _enemyLootManager;


    void Awake()
    {
        _timer = 0;
        _spawnOrder.Clear();
        // Obtiene referencia al EnemyLootManager
        _enemyLootManager = LootManager.Instance;
    }

    private void Start()
    {
        if (_spawnOrder.Count == 0)
        {
            print("a");
            QueueEnemy();
        }
    }
    private void QueueEnemy()
    {
        _spawnOrder.Enqueue(normalEnemyWave);
        _spawnOrder.Enqueue(ligthEnemyWave);
        _spawnOrder.Enqueue(normalEnemyWave);
        _spawnOrder.Enqueue(heavyEnemyWave);
        _spawnOrder.Enqueue(ligthEnemyWave);
    }

    private void Update()
    {
        _timer += Time.deltaTime;

        // Verifica si hay elementos en la cola
        if (_spawnOrder.Count == 0)
        {
            _timer = 0;
            return;
        }

        if (_timer > _spawn)
        {
            _timer = 0;
            var spawnData = _spawnOrder.Dequeue();

            foreach (var enemy in spawnData.enemyToSpawn)
            {
                Transform spawnPoint = GetRandomSpawnPoint();

                // Instanciar el enemigo en la posición del punto de spawn
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
}