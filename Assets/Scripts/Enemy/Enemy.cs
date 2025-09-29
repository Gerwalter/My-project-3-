using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : EnemyHealth
{
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        EnemyManager.Instance.RegisterEnemy(this);
    }


    public override void Die()
    {
        EnemyManager.Instance.UnregisterEnemy(this);
        base.Die();
    }
    [Header("Enemy Settings")]
    public float detectionRadius = 5f;   // Rango en el que detecta al jugador
    public float attackRange = 2f;       // Rango en el que puede atacar
    public float attackDamage = 10f;     // Da�o que inflige al jugador
    public float attackCooldown = 1.5f;  // Tiempo entre ataques
    public LayerMask playerLayer;

    private float attackTimer = 0f;
    private Transform targetPlayer;

    void Update()
    {
        attackTimer -= Time.deltaTime;

        // Buscar jugador en rango de detecci�n
        Collider[] players = Physics.OverlapSphere(transform.position, detectionRadius, playerLayer);

        if (players.Length > 0)
        {
            targetPlayer = players[0].transform;

            // Verificar si est� lo suficientemente cerca para atacar
            float distance = Vector3.Distance(transform.position, targetPlayer.position);
            if (distance <= attackRange && attackTimer <= 0f)
            {
                AttackPlayer(players[0].GetComponent<Health>());
                attackTimer = attackCooldown;
            }
        }
        else
        {
            targetPlayer = null;
        }
    }

    private void AttackPlayer(Health playerHealth)
    {
        if (playerHealth != null)
        {
            playerHealth.OnTakeDamage(attackDamage);
            Debug.Log($"{gameObject.name} golpe� al jugador y le hizo {attackDamage} de da�o");
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
