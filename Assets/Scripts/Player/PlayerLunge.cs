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
        // objetivo: punto delante del enemigo (para no atravesarlo)
        Vector3 dirToEnemy = (target.position - transform.position).normalized;
        Vector3 targetPoint = target.position - dirToEnemy * lungeStopDistance;

        // Si usas Rigidbody-based movement, considera mover con rb.MovePosition en lugar de transform.
        float dist = Vector3.Distance(transform.position, targetPoint);

        while (Vector3.Distance(transform.position, targetPoint) > 0.1f)
        {
            // mover con Lerp/Slerp para sentir impulso
            transform.position = Vector3.MoveTowards(transform.position, targetPoint, lungeSpeed * Time.deltaTime);
            yield return null;
        }

        // opcional: pequeño "empuje" / retroceso de la cámara o animación aquí

        GetComponent<PatrollingNPC>()?.DefeatEnemy();
        // Esperamos un frame o dos para "terminar" la animación del golpe
        yield return new WaitForSeconds(0.12f);
        // volvemos al punto inicial (o no — depende de tu diseño). Aquí vuelvo ligeramente atrás para dar sensación de rebote
        Vector3 returnPoint = startPos; // podrías cambiarlo por una posición relativa

        float t = 0f;
        float duration = 0.18f;
        Vector3 origin = transform.position;
        while (t < duration)
        {
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(origin, returnPoint, t / duration);
            yield return null;
        }

        if (playerMovementToDisable != null)
            playerMovementToDisable.enabled = true;

        isLunging = false;
        // cooldown
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
