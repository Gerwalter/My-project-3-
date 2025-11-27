using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : EnemyHealth
{
    public override void Start()
    {
        base.Start();
        EnemyManager.Instance.RegisterEnemy(this);
    }

    public static System.Action OnEnemyDefeated;

    public override void Die()
    {
        EnemyManager.Instance.UnregisterEnemy(this);
        OnEnemyDefeated?.Invoke();
        base.Die();
    }

    [Header("Enemy Settings")]
    public float detectionRadius = 5f;
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;
    public LayerMask playerLayer;

    private float attackTimer = 0f;
    private Transform targetPlayer;

    void Update()
    {
        attackTimer -= Time.deltaTime;

        Collider[] players = Physics.OverlapSphere(transform.position, detectionRadius, playerLayer);

        if (players.Length > 0)
        {
            targetPlayer = players[0].transform;

            float distance = Vector3.Distance(transform.position, targetPlayer.position);
            if (distance <= attackRange && attackTimer <= 0f)
            {
                anim.SetTrigger("Punch");
                attackTimer = attackCooldown;
            }
        }
        else
        {
            targetPlayer = null;
        }
    }

    public Transform GetTargetPlayer() => targetPlayer;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
