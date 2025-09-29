using UnityEngine;
using System.Collections;

public class WaveSpawner : MonoBehaviour
{
    public Enemy enemyPrefab;
    public Transform parent;

    private EnemyFactory factory;
    public BoxCollider spawnArea;

    public float spawnDelay = 2f;      // tiempo entre indicador y enemigo
    public float WaveTotal = 2f;      // tiempo entre indicador y enemigo

    [Header("Indicator")]
    public GameObject spawnIndicatorPrefab;
    Vector3 GetRandomSpawnPosition()
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

        // Registrar un pool para Goblins
        var goblinPool = new ObjectPool<Enemy>(enemyPrefab, 10, parent);
        factory.RegisterPool("Enemy", goblinPool);

        // Lanzar corrutina de oleadas
        StartCoroutine(SpawnWaves());
    }

    IEnumerator SpawnWaves()
    {
        int wave = 1;
        while (true)
        {
            Debug.Log("Oleada " + wave);

            for (int i = 0; i < WaveTotal; i++)
            {
                var pos = GetRandomSpawnPosition();
                if (spawnIndicatorPrefab != null)
                {
                    GameObject indicator = Instantiate((spawnIndicatorPrefab), (pos + new Vector3 (0, .03f, 0)), Quaternion.Euler(-90f, 0f, 0f));
                    Destroy(indicator, spawnDelay+1);
                }

                yield return new WaitForSeconds(spawnDelay+0.5f);
                factory.Create("Enemy", pos);
                yield return new WaitForSeconds(0.5f);
            }

            wave++;
            yield return new WaitForSeconds(5f);
        }
    }
}
