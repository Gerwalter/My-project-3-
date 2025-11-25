using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerLunge : MonoBehaviour
{
    [Header("Lunge Settings")]
    public KeyCode lungeKey = KeyCode.E;
    public float lungeRange = 6f;
    public LayerMask enemyLayer;
    public float lungeSpeed = 18f;           // Un poco más lento = más controlable
    public float lungeStopDistance = 1.4f;
    public float cooldown = 1.2f;

    [HideInInspector] public bool isLunging = false;
    private bool onCooldown = false;

    private Rigidbody rb;
    private PlayerMovement playerMovement;  // Para desactivar su FixedUpdate durante lunge
    private Vector3 lungeTargetPoint;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerMovement = GetComponent<PlayerController>().movement as PlayerMovement;
    }

    void Update()
    {
        if (isLunging) return;

        if (Input.GetKeyDown(lungeKey) && !onCooldown)
        {
            TryStartLunge();
        }
    }

    void TryStartLunge()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, lungeRange, enemyLayer);
        if (hits.Length == 0) return;

        Collider targetCollider = null;
        float bestDist = float.MaxValue;
        foreach (var hit in hits)
        {
            float d = Vector3.Distance(transform.position, hit.transform.position);
            if (d < bestDist)
            {
                bestDist = d;
                targetCollider = hit;
            }
        }

        if (targetCollider == null) return;

        // Validar EnemyAmbush (sorpresa)
        EnemyAmbush ea = targetCollider.GetComponentInParent<EnemyAmbush>();
        if (ea != null)
        {
            float distToEnemy = Vector3.Distance(transform.position, ea.transform.position);
            if (distToEnemy > ea.radioDeteccion) return;
        }

        // Buscar PatrollingNPC
        PatrollingNPC enemyPatrol = targetCollider.GetComponentInParent<PatrollingNPC>()
                                   ?? targetCollider.GetComponentInChildren<PatrollingNPC>();

        if (enemyPatrol == null)
        {
            Debug.LogWarning("No se encontró PatrollingNPC en el enemigo objetivo");
            return;
        }

        // PARAR AL ENEMIGO
        enemyPatrol.StopFollowingPath();
        if (enemyPatrol.agent != null)
        {
            enemyPatrol.agent.isStopped = true;
            enemyPatrol.agent.velocity = Vector3.zero;
        }

        // INICIAR LUNGE
        StartCoroutine(LungeRoutine(targetCollider.transform, enemyPatrol));
    }

    IEnumerator LungeRoutine(Transform enemyTransform, PatrollingNPC enemyPatrol)
    {
        isLunging = true;
        onCooldown = true;

        // DESACTIVAR movimiento normal del jugador
        rb.isKinematic = true;                    // ← Bloquea física externa
        rb.velocity = Vector3.zero;

        // Calcular punto final (fijo)
        Vector3 dir = (enemyTransform.position - transform.position).normalized;
        lungeTargetPoint = enemyTransform.position - dir * lungeStopDistance;

        float distanceToTarget = Vector3.Distance(transform.position, lungeTargetPoint);
        float duration = distanceToTarget / lungeSpeed;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // Movimiento suave y predecible
            transform.position = Vector3.Lerp(transform.position, lungeTargetPoint, t * 10f * Time.deltaTime);

            // Opcional: mirar al enemigo
            Vector3 lookDir = enemyTransform.position - transform.position;
            if (lookDir != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(lookDir);

            yield return null;
        }

        // Snap final
        transform.position = lungeTargetPoint;

        // ACTIVAR DERROTA DEL ENEMIGO
        enemyPatrol.hasTriggered = true;
        Debug.Log("¡Lunge exitoso! Enemigo derrotado.");

        // Restaurar control del jugador
        rb.isKinematic = false;

        yield return new WaitForSeconds(0.15f);
        isLunging = false;

        yield return new WaitForSeconds(cooldown);
        onCooldown = false;
    }

    // Gizmo
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, lungeRange);
    }
}