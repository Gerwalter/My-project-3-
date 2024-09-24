using UnityEngine;

public class SecurityCamera : MonoBehaviour
{
    public Transform player; // Referencia al jugador
    public float detectionRange = 10f; // Rango de detección de la cámara
    public float fieldOfViewAngle = 45f; // Ángulo del cono de visión de la cámara
    public LayerMask playerLayer; // Capa en la que está el jugador
    public GameObject[] enemyPrefabs; // Prefabs de enemigos a invocar
    public Transform[] spawnPoints; // Puntos donde se invocarán los enemigos

    private bool playerDetected = false;

    void Update()
    {
        DetectPlayer();
    }

    void DetectPlayer()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        // Verificar si el jugador está dentro del ángulo de visión y rango
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

            // Añadir el enemigo a la lista del GameManager
            Enemy enemyScript = enemyInstance.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                GameManager.Instance.Enemies.Add(enemyScript);
            }
        }
    }

    void OnDrawGizmos()
    {
        // Visualización del rango de detección y el cono de visión en la escena
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Vector3 leftBoundary = Quaternion.Euler(0, -fieldOfViewAngle / 2f, 0) * transform.forward * detectionRange;
        Vector3 rightBoundary = Quaternion.Euler(0, fieldOfViewAngle / 2f, 0) * transform.forward * detectionRange;

        Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary);
    }
}
