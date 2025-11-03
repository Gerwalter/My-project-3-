using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhipAttack : MonoBehaviour
{
    [Header("Referencias")]
    public WhipTipController tip;       // punta del látigo
    public Transform basePoint;         // base del látigo (opcional para cálculos de dirección)
    public Collider tipCollider;        // collider de daño
    public Animator animator;           // opcional: animaciones del personaje

    [Header("Parámetros generales")]
    public LayerMask enemyLayer;        // capa de enemigos
    public float hitRadius = 0.5f;      // radio de detección adicional si usas OverlapSphere
    public float comboResetTime = 1.0f; // tiempo máximo entre ataques para mantener combo

    [Header("Daños y fuerzas por ataque")]
    public float jabDamage = 8f;
    public float jabKnockback = 2f;

    public float throwDamage = 14f;
    public float throwKnockback = 8f;
    public Vector3 throwLaunchForce = new Vector3(0, 6f, 10f);

    public float spinDamage = 5f;
    public float spinRadius = 2.2f;
    public float spinDuration = 0.9f;  // duración total del spin
    public int spinHitsPerSecond = 10;
    public float spinEndDamage = 20f;
    public float spinEndKnockback = 12f;

    private bool isAttacking = false;
    private bool canAttack = true;
    private int comboStep = 0;
    private float lastAttackTime;

    void Start()
    {
        if (tip == null) tip = GetComponentInChildren<WhipTipController>();
        if (tipCollider == null && tip != null) tipCollider = tip.GetComponent<Collider>();
        if (tipCollider != null) tipCollider.enabled = false;
    }

    void Update()
    {
        // Ejemplo simple de input (puedes cambiar a tu sistema):
        if (Input.GetMouseButtonDown(0) && canAttack)
            StartCoroutine(DoAttack("Jab"));

        if (Input.GetMouseButtonDown(1) && canAttack)
            StartCoroutine(DoAttack("Throw"));

        if (Input.GetKeyDown(KeyCode.E) && canAttack)
            StartCoroutine(DoAttack("SpinCrack"));
    }

    IEnumerator DoAttack(string type)
    {
        isAttacking = true;
        canAttack = false;
        comboStep++;
        lastAttackTime = Time.time;

        switch (type)
        {
            case "Jab": yield return StartCoroutine(JabAttack()); break;
            case "Throw": yield return StartCoroutine(ThrowAttack()); break;
            case "SpinCrack": yield return StartCoroutine(SpinCrackAttack()); break;
        }

        isAttacking = false;
        yield return new WaitForSeconds(0.2f); // pequeño delay antes del siguiente ataque
        canAttack = true;
    }

    // ============================================================
    // ATAQUES INDIVIDUALES
    // ============================================================

    IEnumerator JabAttack()
    {
        if (animator) animator.SetTrigger("Jab");
        tipCollider.enabled = true;
        tip.Launch();
        float attackDuration = 0.3f;
        float elapsed = 0f;
        while (elapsed < attackDuration)
        {
            DetectHits(jabDamage, jabKnockback);
            elapsed += Time.deltaTime;
            yield return null;
        }

        tipCollider.enabled = false;
    }

    IEnumerator ThrowAttack()
    {
        if (animator) animator.SetTrigger("Throw");
        tipCollider.enabled = true;
        tip.Launch();
        // Lanza la punta hacia adelante brevemente
        float attackDuration = 0.8f;
        float elapsed = 0f;
        while (elapsed < attackDuration)
        {
            DetectHits(throwDamage, throwKnockback, throwLaunchForce);
            elapsed += Time.deltaTime;
            yield return null;
        }

        tipCollider.enabled = false;
    }

    IEnumerator SpinCrackAttack()
    {
        if (animator) animator.SetTrigger("Spin");
        float elapsed = 0f;
        float hitInterval = 1f / spinHitsPerSecond;
        tip.Launch();
        // 1. Giro de látigo (multi-hit)
        while (elapsed < spinDuration)
        {
            DetectHits(spinDamage, 0f); // sin knockback fuerte durante el giro
            elapsed += hitInterval;
            yield return new WaitForSeconds(hitInterval);
        }

        // 2. Golpe final hacia adelante
        tipCollider.enabled = true;
        float finisherTime = 0.3f;
        float finisherElapsed = 0f;
        while (finisherElapsed < finisherTime)
        {
            DetectHits(spinEndDamage, spinEndKnockback);
            finisherElapsed += Time.deltaTime;
            yield return null;
        }
        tipCollider.enabled = false;
    }

    // ============================================================
    // SISTEMA DE DETECCIÓN DE GOLPES
    // ============================================================

    void DetectHits(float damage, float knockback, Vector3? launchForce = null)
    {
        if (tip == null) return;

        Collider[] hits = Physics.OverlapSphere(tip.transform.position, hitRadius, enemyLayer);
        foreach (Collider h in hits)
        {
            // Buscar componente Enemy o similar
            var enemy = h.GetComponent<Enemy>();
            if (enemy != null)
            {

                // Knockback o Launch (si tiene Rigidbody)
                Rigidbody rb = h.attachedRigidbody;
                if (rb != null)
                {
                    Vector3 dir = (h.transform.position - basePoint.position).normalized;
                    if (launchForce.HasValue)
                        rb.velocity = launchForce.Value;
                    else
                        rb.AddForce(dir * knockback, ForceMode.VelocityChange);
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (tip != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(tip.transform.position, hitRadius);
        }
    }
}
