using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using static Player;

public enum EnemyType
{
    MELEE,
    RANGE,
    TANK,
    BOSS
}


[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : Entity
{

    [Header("<color=red>Settings</color>")]
    [SerializeField] private float _chaseDist = 6.0f; // Distancia de persecución
    [SerializeField] private float _atkDist = 2.0f; // Distancia de ataque
    [SerializeField] public int _speed; // Velocidad de movimiento
    [SerializeField] public EnemyType enemyType; // Tipo de enemigo
    [SerializeField] private Transform _target; // Objetivo del enemigo

    [Header("<color=red>Behaviours</color>")]
    [SerializeField] private Animator _animator;
    [SerializeField] private AudioClip[] clips;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private float liftForce = 10.0f;
    [SerializeField] private bool isDead;

    [Header("<color=#6A89A7>UI</color>")]
    [SerializeField] private Image healthBar;

    [Header("<color=yellow>Attack</color>")]
    [SerializeField] private Transform _atkOrigin;
    [SerializeField] private float _atkRayDist = 1.0f;
    [SerializeField] private LayerMask _atkMask;
    [SerializeField] private int _atkDmg = 2;

    [Header("<color=yellow>Heal</color>")]
   // [SerializeField] private float healCooldown = 7.0f; // Tiempo entre curaciones
   // private float lastHealTime = -Mathf.Infinity; // Tiempo de la última curación

    [SerializeField] private ElementType weakness; // Tipo de debilidad
    [SerializeField] private float elementalMultiplier = 2.0f;

   // private bool isTakingContinuousDamage = false;
    //private float _groundCheckDistance = 1.1f;

    private void Awake()
    {
        GameManager.Instance.Enemies.Add(this);
        _rb = GetComponent<Rigidbody>();
        _rb.isKinematic = true;
        _rb.useGravity = true;
        _rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        _target = GameManager.Instance.Player.gameObject.transform;
        GetLife = maxLife;
    }

    private void Update()
    {
        UpdateHealthBar();
        if (isDead) return;


        if (Vector3.SqrMagnitude(transform.position - _target.position) <= (_chaseDist * _chaseDist))
        {
            ChaseAndAttack();
        }
    }

    private void UpdateHealthBar()
    {
        float lifePercent = GetLife / maxLife;
        healthBar.fillAmount = lifePercent;
        healthBar.color = Color.Lerp(Color.red, Color.green, lifePercent);
    }

    private void ChaseAndAttack()
    {
        float distanceToTarget = Vector3.Distance(transform.position, _target.position);

        // Calcular dirección hacia el jugador
        Vector3 direction = (_target.position - transform.position).normalized;

        // Mantener el eje Y nivelado para evitar inclinaciones
        direction.y = 0;

        // Girar hacia el jugador sin retrasar el movimiento
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * 360.0f); // Ajusta la velocidad de giro con "360.0f"

        // Mover al enemigo hacia el jugador si está fuera del rango de ataque
        if (distanceToTarget > _atkDist)
        {
            _animator.SetBool("isMoving", true);
            transform.Translate(direction * _speed * Time.deltaTime, Space.World);
        }
        else
        {
            _animator.SetBool("isMoving", false);
            _animator.SetTrigger("Punch");
        }
    }

    public void ApplyLiftImpulse()
    {
        _rb.isKinematic = false;
        _rb.AddForce(Vector3.up * liftForce, ForceMode.Impulse);
        StartCoroutine(CheckGroundAfterLift());
    }

    private IEnumerator CheckGroundAfterLift()
    {
        yield return new WaitForSeconds(1);
        _rb.isKinematic = true;
    }

    public void Attack()
    {
        Ray atkRay = new Ray(_atkOrigin.position, transform.forward);

        if (Physics.Raycast(atkRay, out RaycastHit atkHit, _atkRayDist, _atkMask))
        {
            if (atkHit.collider.TryGetComponent<Player>(out Player player))
            {
                player.ReciveDamage(_atkDmg);
            }
        }
    }

    public void ReceiveDamage(float dmg, ElementType damageType)
    {
        if (damageType == weakness)
        {
            dmg *= elementalMultiplier;
        }
        GetLife -= dmg;

        if (GetLife <= 0)
        {
            Die();
        }
        else
        {
            _bloodVFX.SendEvent("OnTakeDamage");
        }
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;
        _animator.SetBool("isMoving", false);
        _animator.SetTrigger("Die");
        GameManager.Instance.Enemies.Remove(this);

        // Lógica de loot aquí
        Destroy(gameObject, 1.5f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _chaseDist);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _atkDist);
    }
}
