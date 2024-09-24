using UnityEngine;

public class SecurityCamera : MonoBehaviour
{
    public Transform player; // Referencia al jugador
    public float detectionRange = 10f; // Rango de detecci�n de la c�mara
    public float fieldOfViewAngle = 45f; // �ngulo del cono de visi�n de la c�mara
    public LayerMask playerLayer; // Capa en la que est� el jugador
    public GameObject[] enemyPrefabs; // Prefabs de enemigos a invocar
    public Transform[] spawnPoints; // Puntos donde se invocar�n los enemigos

    private bool playerDetected = false;

    void Update()
    {
        DetectPlayer();
    }

    void DetectPlayer()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        // Verificar si el jugador est� dentro del �ngulo de visi�n y rango
        if (angleToPlayer < fieldOfViewAngle / 2f && directionToPlayer.magnitude <= detectionRange)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, directionToPlayer.normalized, out hit, detectionRange, playerLayer))
            {
                if (hit.transform == player && !playerDetected)
                {
                    playerDetected = true;
                    SpawnEnemies();
                }
            }
        }
    }

    void SpawnEnemies()
    {
        foreach (Transform spawnPoint in spawnPoints)
        {
            int randomIndex = Random.Range(0, enemyPrefabs.Length);
            GameObject enemyInstance = Instantiate(enemyPrefabs[randomIndex], spawnPoint.position, spawnPoint.rotation);

            // A�adir el enemigo a la lista del GameManager
            Enemy enemyScript = enemyInstance.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                GameManager.Instance.Enemies.Add(enemyScript);
            }
        }
    }

    void OnDrawGizmos()
    {
        // Visualizaci�n del rango de detecci�n y el cono de visi�n en la escena
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Vector3 leftBoundary = Quaternion.Euler(0, -fieldOfViewAngle / 2f, 0) * transform.forward * detectionRange;
        Vector3 rightBoundary = Quaternion.Euler(0, fieldOfViewAngle / 2f, 0) * transform.forward * detectionRange;

        Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary);
    }
}
