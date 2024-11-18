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
    [SerializeField] private float stopShootingDistance = 3f; // Distancia en la que dejar� de disparar

    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private HealthSystem healthSystem;
    [SerializeField] public Animator _anim;

    private EnemyBehavior enemyBehavior; // Referencia opcional al componente EnemyBehavior

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        healthSystem = GetComponent<HealthSystem>();
        enemyBehavior = GetComponent<EnemyBehavior>(); // Intentar obtener EnemyBehavior si est� presente
        target = GameManager.Instance.Player.gameObject.transform;

        healthSystem.OnDie += HandleDeath;
    }

    private void Update()
    {
        if (target == null) return;

        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        if (enemyBehavior != null)
        {
            // Si el enemigo tiene EnemyBehavior, mant�n la distancia �ptima
            HandleRangedBehavior(distanceToTarget);
        }
        else
        {
            // L�gica est�ndar de persecuci�n y ataque cuerpo a cuerpo
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
            agent.isStopped = true; // Detener al agente si el jugador est� fuera del alcance de persecuci�n
        }
    }

    private void HandleRangedBehavior(float distanceToTarget)
    {
        if (distanceToTarget <= stopShootingDistance)
        {
            // Dejar de disparar si el jugador est� demasiado cerca
            StopShooting();
        }
        else if (distanceToTarget > optimalDistance)
        {
            // Acercarse si est� fuera del rango �ptimo
            ChaseTarget();
        }
        else if (distanceToTarget < optimalDistance - 1f)
        {
            // Alejarse si est� demasiado cerca
            Vector3 directionAway = (transform.position - target.position).normalized;
            Vector3 newPosition = transform.position + directionAway * agent.speed * Time.deltaTime;
            agent.isStopped = false;
            _anim.SetBool("isMoving", false);
            agent.SetDestination(newPosition);
        }
        else
        {
            // Mantener la posici�n si est� dentro del rango �ptimo
            agent.isStopped = true;
            Shoot(); // Si el jugador est� en el rango adecuado, dispara
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
        // Aqu� puedes invocar un evento o ejecutar la l�gica del ataque cuerpo a cuerpo
    }

    private void Shoot()
    {
        // L�gica de disparo. Aqu� puedes integrar el disparo o el comportamiento del proyectil
        if (enemyBehavior != null)
        {
            enemyBehavior.ShootAtPlayer(target);
        }
    }

    private void StopShooting()
    {
        // Detener el disparo o cambiar el estado a m�s defensivo
        if (enemyBehavior != null)
        {
            enemyBehavior.StopShooting();
        }
    }

    private void HandleDeath()
    {
        agent.isStopped = true;
        // Aqu� puedes deshabilitar el enemigo o iniciar su destrucci�n
    }

    
}
