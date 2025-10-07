using UnityEngine;

public class SoundEmitter : MonoBehaviour
{
    [Tooltip("Radio del sonido en unidades Unity")]
    public float radius = 8f;
    [Tooltip("Capa usada para filtrar NPCs (por ejemplo: layer 'NPC')")]
    public LayerMask affectedLayers;
    [Tooltip("Si true, se reproduce un clip localmente (opcional)")]
    public AudioClip soundClip;
    [Tooltip("Opcional: volumen si reproduces clip localmente")]
    public float volume = 1f;
    [Tooltip("Si true, el emitter se destruye después de emitir (útil para objetos lanzables)")]
    public bool destroyAfterEmit = true;

    // Llama a Emit() para notificar NPCs en el radio.
    [ContextMenu("Emit sound now")]
    public void Emit()
    {
        // Reproducir sonido local (opcional)
        if (soundClip != null)
            AudioSource.PlayClipAtPoint(soundClip, transform.position, volume);

        // Detectar NPCs (que tengan PatrollingNPC)
        Collider[] hits = Physics.OverlapSphere(transform.position, radius, affectedLayers, QueryTriggerInteraction.Ignore);

        foreach (var col in hits)
        {
            PatrollingNPC npc = col.GetComponentInParent<PatrollingNPC>();
            if (npc != null)
            {
                npc.HearNoise(transform.position);
            }
            else
            {
                // Si no está en el mismo GameObject, intentar buscar en hijos/parent (por si el collider está en un child)
                npc = col.GetComponent<PatrollingNPC>();
                if (npc != null) npc.HearNoise(transform.position);
            }
        }

        if (destroyAfterEmit)
            Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.25f);
        Gizmos.DrawSphere(transform.position, radius);
    }
}
