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
   // public float damage = 25f;
    public float cooldown = 1.2f;

    bool onCooldown = false;
    bool isLunging = false;

    void Update()
    {
        if (isLunging) return;

        if (Input.GetKeyDown(lungeKey) && !onCooldown)
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, lungeRange, enemyLayer);
            if (hits.Length == 0) return;

            // elegimos el enemigo más cercano (guardamos el collider)
            Collider targetCollider = null;
            float bestDist = float.MaxValue;
            foreach (var c in hits)
            {
                float d = Vector3.Distance(transform.position, c.transform.position);
                if (d < bestDist)
                {
                    bestDist = d;
                    targetCollider = c;
                }
            }

            if (targetCollider != null)
            {
                // comprobamos (si existe) el radio de detección del EnemyAmbush en cualquiera de los padres
                EnemyAmbush ea = targetCollider.GetComponentInParent<EnemyAmbush>();
                if (ea != null)
                {
                    float distToEnemy = Vector3.Distance(transform.position, ea.transform.position);
                    if (distToEnemy > ea.radioDeteccion)
                    {
                        // fuera del radio de detección -> no se puede hacer sorpresa
                        return;
                    }
                }

                StartCoroutine(LungeRoutine(targetCollider));
            }
        }
    }

    IEnumerator LungeRoutine(Collider targetCollider)
    {
        isLunging = true;
        onCooldown = true;

        Vector3 startPos = transform.position;
        Vector3 dirToEnemy = (targetCollider.transform.position - transform.position).normalized;
        Vector3 targetPoint = targetCollider.transform.position - dirToEnemy * lungeStopDistance;

        while (Vector3.Distance(transform.position, targetPoint) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPoint, lungeSpeed * Time.deltaTime);
            yield return null;
        }

        // --> Aquí está la corrección importante: buscamos el PatrollingNPC EN el enemigo, no en el jugador.
        PatrollingNPC enemyPatrol = targetCollider.GetComponentInParent<PatrollingNPC>();
        if (enemyPatrol == null) enemyPatrol = targetCollider.GetComponentInChildren<PatrollingNPC>();

        if (enemyPatrol != null)
        {
            enemyPatrol.hasTriggered = true;
            Debug.Log($"PlayerLunge: alertado PatrollingNPC en {targetCollider.gameObject.name}");
        }
        else
        {
            Debug.LogWarning($"PlayerLunge: no se encontró PatrollingNPC en el objetivo {targetCollider.gameObject.name}");
        }

        yield return new WaitForSeconds(0.12f);

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
