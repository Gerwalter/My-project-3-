using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.VFX;
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
    [Header("<color=red>AI</color>")]
    [SerializeField] private float _chaseDist = 6.0f;
    [SerializeField] private float _atkDist = 2.0f;
    [SerializeField] private float _changeNodeDist = 0.5f;
    [SerializeField] private float _healDist = 5.0f;
    [SerializeField] private float _shootDist = 6.0f;
    [SerializeField] public int _speed;
    [SerializeField] public EnemyType enemyType;
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private Transform _target, _actualNode;
    [SerializeField] private List<Transform> _navMeshNodes = new();
    [SerializeField] private IANodeManager _iaNodeManager;
    [SerializeField] public bool _enableRoam = true;


    [Header("<color=red>Behaviours</color>")]
    [SerializeField] private Animator _animator;
    [SerializeField] AudioClip[] clips;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float liftForce = 10.0f;
    [SerializeField] private bool isdead;



    [Header("<color=#6A89A7>UI</color>")]
    [SerializeField] private Image healthBar;


    private void Awake()
    {
        GameManager.Instance.Enemies.Add(this);
        _iaNodeManager = GameManager.Instance.Nodes;

        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }


    public List<Transform> NavMeshNodes
    {
        get { return _navMeshNodes; }
        set { _navMeshNodes = value; }
    }



    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        NavMeshNodes.AddRange(_iaNodeManager._nodes);

        _target = GameManager.Instance.Player.gameObject.transform;

        _agent = GetComponent<NavMeshAgent>();

        _actualNode = GetNewNode();

        _agent.SetDestination(_actualNode.position);

        GetLife = maxLife;

    }

    public bool air;

    public void ApplyLiftImpulse()
    {
        _groundCheckDistance = 0;
        _enableRoam = false;
        _agent.enabled = false;
        rb.isKinematic = false;
        rb.AddForce(Vector3.up * liftForce, ForceMode.Impulse);
        StartCoroutine(CheckDistance());
    }

    IEnumerator CheckDistance()
    {
        yield return new WaitForSeconds(1);
        _groundCheckDistance = 1.1f;
    }

    [SerializeField] private float _groundCheckDistance = 1.1f;

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, _groundCheckDistance);
    }

    private void groundcheck()
    {
        if (IsGrounded())
        {
            _enableRoam = true;
            _agent.enabled = true;
            rb.isKinematic = true;
        }
    }

    private void UpdateHealthBar()
    {
        float lifePercent = GetLife / maxLife;

        healthBar.fillAmount = lifePercent;

        healthBar.color = Color.Lerp(Color.red, Color.green, lifePercent);
    }




    private void FixedUpdate()
    {
        UpdateHealthBar();
        groundcheck();
        _agent.speed = _speed;

        if (!isdead)
        {
            if (_enableRoam)
            {
                if (!_target)
                {
                    Debug.LogError($"<color=red>NullReferenceException</color>: No asignaste un objetivo, boludo.");
                    return;
                }

                _animator.SetBool("isMoving", true);

                if (Vector3.SqrMagnitude(transform.position - _target.position) <= (_chaseDist * _chaseDist))
                {
                    if (Vector3.SqrMagnitude(transform.position - _target.position) <= (_atkDist * _atkDist))
                    {
                        if (!_agent.isStopped) _agent.isStopped = true;

                        _animator.SetBool("isMoving", false);
                        _animator.SetTrigger("Punch");
                    }
                    else
                    {
                        if (_agent.isStopped) _agent.isStopped = false;

                        _animator.SetBool("isMoving", true);
                        _animator.ResetTrigger("Punch");

                        _agent.SetDestination(_target.position);
                    }
                }
                else
                {
                    if (_agent.destination != _actualNode.position) _agent.SetDestination(_actualNode.position);

                    if (Vector3.SqrMagnitude(transform.position - _actualNode.position) <= (_changeNodeDist * _changeNodeDist))
                    {
                        _actualNode = GetNewNode();
                        _agent.SetDestination(_actualNode.position);
                    }
                }
            }
            else
            {
                return;
            }

            if (_isHealer)
            {
                Enemy nearbyAlly = FindAllyToHeal();
                if (nearbyAlly != null)
                {
                    HealAlly(nearbyAlly);
                    return;
                }
            }

            if (_isShooter)
            {
                if (PlayerInShootRange())
                {
                    if (Time.time >= lastShootTime + shootCooldown)
                    {
                        Shoot();
                        lastShootTime = Time.time;
                    }
                }
            }
        }
    }
    private bool PlayerInShootRange()
    {
        // Verificar que la distancia esté dentro del rango de disparo
        if (Vector3.Distance(transform.position, _target.position) <= _shootDist)
        {
            // Obtener la dirección hacia el objetivo
            Vector3 directionToTarget = (_target.position - transform.position).normalized;

            // Calcular el ángulo entre el forward del enemigo y la dirección hacia el objetivo
            float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);

            // Asegurarse de que el enemigo esté mirando hacia el objetivo (por ejemplo, un ángulo de 30 grados)
            if (angleToTarget <= 30.0f)
            {
                return true;
            }
        }
        return false;
    }

    void Shoot()
    {
        GameObject projectile = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);
        Vector3 direction = (_target.position - shootPoint.position).normalized;
        projectile.GetComponent<Rigidbody>().velocity = direction * 40f;
    }

    [Header("<color=yellow>Attack</color>")]
    [SerializeField] private Transform _atkOrigin;
    [SerializeField] private float _atkRayDist = 1.0f;
    [SerializeField] private LayerMask _atkMask;
    [SerializeField] private int _atkDmg = 2;    
    private bool isTakingContinuousDamage = false;

    private Ray _atkRay;
    private RaycastHit _atkHit;
    public void Attack()
    {
        _atkRay = new Ray(_atkOrigin.position, transform.forward);

        if (Physics.Raycast(_atkRay, out _atkHit, _atkRayDist, _atkMask))
        {
            if (_atkHit.collider.TryGetComponent<Player>(out Player player))
            {
                player.ReciveDamage(_atkDmg);
            }
        }
    }

    private Transform GetNewNode()
    {
        Transform newNode = _navMeshNodes[Random.Range(0, _navMeshNodes.Count)];

        return newNode;
    }

    private Enemy FindAllyToHeal()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, _healDist);
        foreach (var hit in hits)
        {
            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy != null && enemy != this && enemy.GetLife < enemy.maxLife)
            {

                return enemy;
            }
        }

        return null;
    }
    [Header("<color=yellow>Heal</color>")]
    [SerializeField] private float healCooldown = 7.0f; // Tiempo en segundos entre curaciones
    private float lastHealTime = -Mathf.Infinity; // Tiempo de la última curación

    public void HealAlly(Enemy ally)
    {
        // Comprobar si ha pasado suficiente tiempo desde la última curación
        if (Time.time - lastHealTime < healCooldown)
            return;

        _agent.SetDestination(ally.transform.position);

        // Verificar la distancia al aliado
        if (Vector3.Distance(transform.position, ally.transform.position) <= 1.0f)
        {
            ally.Health(8); // Aplicar curación
            lastHealTime = Time.time; // Registrar el tiempo de la curación
        }
    }
    [SerializeField] private ElementType weakness; // Tipo de debilidad del enemigo
    [SerializeField] private float elementalMultiplier = 2.0f;

    public void ReciveDamage(float dmg, ElementType damageType)
    {
        if (damageType == weakness)
        {
            dmg *= elementalMultiplier; // Aumenta el daño si coincide con la debilidad
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

        //SFXManager.instance.PlayRandSFXClip(clips, transform, 1f);
    }


    public void ApplyContinuousDamageFromPlayer(float totalDamage, float duration, ElementType damageType)
    {
        if (isTakingContinuousDamage) return;

        isTakingContinuousDamage = true;
        StartCoroutine(ContinuousDamageRoutine(totalDamage, duration, damageType));
    }

    private IEnumerator ContinuousDamageRoutine(float totalDamage, float duration, ElementType damageType)
    {
        float damagePerTick = totalDamage / duration;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            // Verifica si el tipo de daño coincide con la debilidad
            float actualDamage = (damageType == weakness) ? damagePerTick * 2 : damagePerTick;


            int roundedDamage = Mathf.RoundToInt(actualDamage * Time.deltaTime);
            // Aplica el daño calculado
            ReciveDamage(roundedDamage, damageType);

            elapsed += Time.deltaTime;
            yield return null;
        }

        isTakingContinuousDamage = false;
    }


    private void Die()
    {
        _animator.SetBool("isMoving", false);
        _animator.ResetTrigger("Punch");
        _agent.speed = 0;
        GameManager.Instance.Enemies.Remove(this);

        

        _animator.SetTrigger("Die");

        if (!isdead)
        {

            LootData loot = LootManager.Instance.GetLoot(enemyType);

            if (FindObjectOfType<PlayerStats>() is PlayerStats playerStats)
            {
                playerStats.AddLoot(loot);
            }
        }
        isdead = true;
        Destroy(gameObject, 1.5f);
    }

    public void triggerReset()
    {
        _animator.ResetTrigger("Hit");
    }
    public void FalseBool()
    {
        _animator.ResetTrigger("Punch");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(_atkRay);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _chaseDist);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _atkDist);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _changeNodeDist);

        if (_enemyClass == EnemyClass.Healer)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _healDist);
        }

        if (_enemyClass == EnemyClass.Shooter)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, _shootDist);
        }
    }
}
