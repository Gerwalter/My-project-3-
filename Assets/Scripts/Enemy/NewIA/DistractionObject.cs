using UnityEngine;

public enum DistractionType { Stone, Coin }

public class DistractionObject : MonoBehaviour
{
    [Header("Configuracion de Distraccion")]
    public DistractionType distractionType = DistractionType.Stone;
    public float noiseRadius = 10f;  // Solo para Stone
    public float intensity = 1f;     // Solo para Stone
    public float coinLifetime = 10f; // Tiempo que la moneda permanece visible
    public LayerMask enemyLayers;
    private bool hasTriggered = false;

    private void Start()
    {
        if (distractionType == DistractionType.Coin)
        {
           // Destroy(gameObject, coinLifetime);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Ignorar colisiones con el jugador
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player")) return;

        if (distractionType == DistractionType.Stone && !hasTriggered)
        {
            hasTriggered = true; // Evitar multiples triggers
            EmitNoise(transform.position);
          //  Destroy(gameObject, 1f); // Delay para efectos
        }
    }

    private void EmitNoise(Vector3 noisePosition)
    {
        Collider[] hits = Physics.OverlapSphere(noisePosition, noiseRadius, enemyLayers);
        foreach (var hit in hits)
        {
            PatrollingNPC npc = hit.GetComponent<PatrollingNPC>();
            if (npc != null)
            {
                npc.HearNoise(noisePosition, intensity);
            }
        }
    }

    public void Dest()
    {
        print("A");
        gameObject.SetActive(false);
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, noiseRadius);
    }
}