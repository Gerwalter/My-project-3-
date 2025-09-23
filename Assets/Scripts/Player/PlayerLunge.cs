using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLunge : MonoBehaviour
{
    [Header("Lunge Settings")]
    public KeyCode lungeKey = KeyCode.E;
    public float lungeRange = 6f;           // radio en el que buscar enemigos
    public LayerMask enemyLayer;            // capa de enemigos
    public float lungeSpeed = 25f;          // velocidad de la "embestida"
    public float lungeStopDistance = 1.2f;  // distancia al objetivo donde se considera golpeado
    public float damage = 25f;
    public float cooldown = 1.2f;

    [Header("Optional")]
    public MonoBehaviour playerMovementToDisable; // referencia a tu script de movimiento (si quieres desactivarlo durante la embestida)
    public PatrollingNPC patrol; // referencia a tu script de movimiento (si quieres desactivarlo durante la embestida)

    bool onCooldown = false;
    bool isLunging = false;

    void Update()
    {
        if (isLunging) return;

        if (Input.GetKeyDown(lungeKey) && !onCooldown)
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, lungeRange, enemyLayer);
            if (hits.Length == 0)
            {
                // retroalimentación simple: no hay enemigos
                // podrías reproducir un sonido o animación aquí
                return;
            }

            // elegimos el enemigo más cercano
            Transform target = null;
            float bestDist = float.MaxValue;
            foreach (var c in hits)
            {
                float d = Vector3.Distance(transform.position, c.transform.position);
                if (d < bestDist)
                {
                    bestDist = d;
                    target = c.transform;
                }
            }

            if (target != null)
            {
                // Antes de lanzarnos, comprobamos (si existe) el radio de detección del EnemyAmbush
                EnemyAmbush ea = target.GetComponent<EnemyAmbush>();
                if (ea != null)
                {
                    float distToEnemy = Vector3.Distance(transform.position, ea.transform.position);
                    // Si quieres obligar que el enemigo esté "detectando" (por ejemplo el jugador activa la UI),
                    // usamos el radioDeteccion del enemigo como criterio:
                    if (distToEnemy > ea.radioDeteccion)
                    {
                        // fuera del radio de detección del enemigo -> no se puede hacer sorpresa
                        return;
                    }
                }

                StartCoroutine(LungeRoutine(target));
            }
        }
    }

    IEnumerator LungeRoutine(Transform target)
    {
        isLunging = true;
        onCooldown = true;

        if (playerMovementToDisable != null)
            playerMovementToDisable.enabled = false;

        Vector3 startPos = transform.position;
        Vector3 dirToEnemy = (target.position - transform.position).normalized;
        Vector3 targetPoint = target.position - dirToEnemy * lungeStopDistance;

        while (Vector3.Distance(transform.position, targetPoint) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPoint, lungeSpeed * Time.deltaTime);
            yield return null;
        }

        // Aquí marcamos al enemigo como "alertado"
             patrol = target.GetComponent<PatrollingNPC>();
        if (patrol != null)
        {
            patrol.hasTriggered = true;
        }

        target.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
        target.SendMessage("OnAmbushed", SendMessageOptions.DontRequireReceiver);

        yield return new WaitForSeconds(0.12f);

        if (playerMovementToDisable != null)
            playerMovementToDisable.enabled = true;

        isLunging = false;
        yield return new WaitForSeconds(cooldown);
        onCooldown = false;
    }

    // helper para ver el rango en editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, lungeRange);
    }
}
