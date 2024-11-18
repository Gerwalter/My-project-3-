using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(HealthSystem))]
[RequireComponent(typeof(NavMeshAgent))]
public class Enemigo : MonoBehaviour
{
    [Header("AI Settings")]
    [SerializeField] private Transform target;
    [SerializeField] private float chaseDistance = 10f;
    [SerializeField] private float attackDistance = 2f;
    [SerializeField] private float optimalDistance = 5f; // Distancia preferida para disparar
    [SerializeField] private float stopShootingDistance = 3f; // Distancia en la que dejará de disparar

    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private HealthSystem healthSystem;
    [SerializeField] public Animator _anim;

    private EnemyBehavior enemyBehavior; // Referencia opcional al componente EnemyBehavior

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        healthSystem = GetComponent<HealthSystem>();
        enemyBehavior = GetComponent<EnemyBehavior>(); // Intentar obtener EnemyBehavior si está presente
        target = GameManager.Instance.Player.gameObject.transform;

        healthSystem.OnDie += HandleDeath;
    }

    private void Update()
    {
        if (target == null) return;

        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        if (enemyBehavior != null)
        {
            // Si el enemigo tiene EnemyBehavior, mantén la distancia óptima
            HandleRangedBehavior(distanceToTarget);
        }
        else
        {
            // Lógica estándar de persecución y ataque cuerpo a cuerpo
            HandleMeleeBehavior(distanceToTarget);
        }
    }

    private void HandleMeleeBehavior(float distanceToTarget)
    {
        if (distanceToTarget <= attackDistance)
        {
            Attack();
        }
        else if (distanceToTarget <= chaseDistance)
        {
            ChaseTarget();
        }
        else
        {
            agent.isStopped = true; // Detener al agente si el jugador está fuera del alcance de persecución
        }
    }

    private void HandleRangedBehavior(float distanceToTarget)
    {
        if (distanceToTarget <= stopShootingDistance)
        {
            // Dejar de disparar si el jugador está demasiado cerca
            StopShooting();
        }
        else if (distanceToTarget > optimalDistance)
        {
            // Acercarse si está fuera del rango óptimo
            ChaseTarget();
        }
        else if (distanceToTarget < optimalDistance - 1f)
        {
            // Alejarse si está demasiado cerca
            Vector3 directionAway = (transform.position - target.position).normalized;
            Vector3 newPosition = transform.position + directionAway * agent.speed * Time.deltaTime;
            agent.isStopped = false;
            _anim.SetBool("isMoving", false);
            agent.SetDestination(newPosition);
        }
        else
        {
            // Mantener la posición si está dentro del rango óptimo
            agent.isStopped = true;
            Shoot(); // Si el jugador está en el rango adecuado, dispara
        }
    }

    private void ChaseTarget()
    {
        agent.isStopped = false;
        _anim.SetBool("isMoving", true);
        agent.SetDestination(target.position);
    }

    private void Attack()
    {
        agent.isStopped = true;
        // Aquí puedes invocar un evento o ejecutar la lógica del ataque cuerpo a cuerpo
    }

    private void Shoot()
    {
        // Lógica de disparo. Aquí puedes integrar el disparo o el comportamiento del proyectil
        if (enemyBehavior != null)
        {
            enemyBehavior.ShootAtPlayer(target);
        }
    }

    private void StopShooting()
    {
        // Detener el disparo o cambiar el estado a más defensivo
        if (enemyBehavior != null)
        {
            enemyBehavior.StopShooting();
        }
    }

    private void HandleDeath()
    {
        agent.isStopped = true;
        // Aquí puedes deshabilitar el enemigo o iniciar su destrucción
    }

    
}
