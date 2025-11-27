using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimController : MonoBehaviour
{
    [Header("Damage Settings")]
    public float attackDamage = 10f;
    public float attackRange = 2f;
    public LayerMask playerLayer;

    public Enemy enemy;

    // ESTE MÉTODO ES LLAMADO POR LA ANIMACIÓN
    public void Attack()
    {
        if (enemy == null) return;

        Transform target = enemy.GetTargetPlayer();
        if (target == null) return;

        float distance = Vector3.Distance(transform.position, target.position);

        if (distance <= attackRange)
        {
            Health h = target.GetComponent<Health>();
            if (h != null)
            {
                h.OnTakeDamage(attackDamage);
            }
        }
    }

}
