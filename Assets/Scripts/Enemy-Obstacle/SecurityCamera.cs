using UnityEngine;

public class SecurityCamera : MonoBehaviour
{
    public Transform player;
    public float detectionRange = 10f;
    public float fieldOfViewAngle = 45f;
    public LayerMask playerLayer;
    public GameObject[] enemyPrefabs;
    public Transform[] spawnPoints;
    public IANodeManager iANode;
    [SerializeField] private bool playerDetected = false;


    private void Start()
    {

    }
    void Update()
    {
        if (iANode == null)
        {
            iANode = GetComponent<IANodeManager>();
        }
        DetectPlayer();
    }

    void DetectPlayer()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

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


    void NodesConfirm()
    {

    }
    void SpawnEnemies()
    {
        foreach (Transform spawnPoint in spawnPoints)
        {
            int randomIndex = Random.Range(0, enemyPrefabs.Length);
            GameObject enemyInstance = Instantiate(enemyPrefabs[randomIndex], spawnPoint.position, spawnPoint.rotation);

            Enemy enemyScript = enemyInstance.GetComponent<Enemy>();
            enemyScript._nodeManager = iANode;

            enemyScript.Initialize();
           // iANode.NodesExtraConfirm();
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Vector3 leftBoundary = Quaternion.Euler(0, -fieldOfViewAngle / 2f, 0) * transform.forward * detectionRange;
        Vector3 rightBoundary = Quaternion.Euler(0, fieldOfViewAngle / 2f, 0) * transform.forward * detectionRange;

        Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary);
    }
}
