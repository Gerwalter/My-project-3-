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

    private void Start()
    {
        if (distractionType == DistractionType.Coin)
        {
            Destroy(gameObject, coinLifetime);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Ignorar colisiones con el jugador
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player")) return;
        PatrollingNPC npc = collision.collider.GetComponent<PatrollingNPC>();

        if (npc != null)
        {
            // Enemigo golpeado  investigar la posición de impacto
            npc.SeeCoin(collision.contacts[0].point);
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