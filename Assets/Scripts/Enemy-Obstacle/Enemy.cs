using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.VFX;

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
    [SerializeField] public EnemyType _enemyType;
    [Header("<color=red>AI</color>")]
    [SerializeField] private float _chaseDist = 6.0f;
    [SerializeField] private float _atkDist = 2.0f;
    [SerializeField] private float _changeNodeDist = 0.5f;
    [SerializeField] private float _healDist = 5.0f;
    [SerializeField] private float _shootDist = 6.0f;
    [SerializeField] private float _shieldDist = 5.0f;
    [SerializeField] private int _speed;

    [Header("<color=red>Behaviours</color>")]
    [SerializeField] private Animator _animator;

    [SerializeField] private Material mat;


    [SerializeField] private NavMeshAgent _agent;

    [SerializeField] AudioClip[] clips;

    [Header("<color=#6A89A7>UI</color>")]
    [SerializeField] private Image healthBar;

    [SerializeField] private Rigidbody rb;
    [SerializeField] private float liftForce = 10.0f;
    private void Awake()
    {
        GameManager.Instance.Enemies.Add(this);

        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }
    [SerializeField] private Transform _target, _actualNode;
    [SerializeField] private List<Transform> _navMeshNodes = new();
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
        _target = GameManager.Instance.Player.gameObject.transform;

        _agent = GetComponent<NavMeshAgent>();

        _actualNode = GetNewNode();

        _agent.SetDestination(_actualNode.position);

        GetLife = maxLife;

    }
    private Transform GetNewNode(Transform lastNode = null)
    {
        Transform newNode = _navMeshNodes[Random.Range(0, _navMeshNodes.Count)];

        while (lastNode == newNode)
        {
            newNode = _navMeshNodes[Random.Range(0, _navMeshNodes.Count)];
        }

        return newNode;
    }
    public bool air;

    public void ApplyLiftImpulse()
    {
        Debug.Log("Applying lift impulse");
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

    // Llama a groundcheck en Update o FixedUpdate
    void Update()
    {

    }
    private void UpdateHealthBar()
    {
        float lifePercent = GetLife / maxLife;

        healthBar.fillAmount = lifePercent;

        healthBar.color = Color.Lerp(Color.red, Color.green, lifePercent);
    }

    [SerializeField] private bool _enableRoam = true;
    private void FixedUpdate()
    {
        UpdateHealthBar(); 
        groundcheck();

     //   if (IsGrounded() && !air)
     //   {
     //       _enableRoam = true;
     //       _agent.enabled = true;
     //       rb.isKinematic = true;
     //   }

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



        if (_isShielder)
        {

            if (PlayerInShieldRange() && _shieldLife >= 0)
            {
                ActivateShield();
                return;
            }
            else
            {
                DeactivateShield();
                return;
            }
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
    private bool PlayerInShieldRange()
    {
        return Vector3.Distance(transform.position, _target.position) <= _shieldDist;
    }

    void ActivateShield()
    {
        _shieldInstance.SetActive(true);
    }

    void DeactivateShield()
    {
        _shieldInstance.SetActive(false);
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

    private void HealAlly(Enemy ally)
    {
        _agent.SetDestination(ally.transform.position);
        if (Vector3.Distance(transform.position, ally.transform.position) <= 1.0f)
        {
            ally.Health(10);
        }
    }

    public void ReciveDamage(int dmg)
    {

        GetLife -= dmg;
        if (GetLife <= 0)
        {
            GameManager.Instance.Enemies.Remove(this);

            Destroy(gameObject);
            
        }
        else
        {

                _bloodVFX.SendEvent("OnTakeDamage");
            
        }

        //SFXManager.instance.PlayRandSFXClip(clips, transform, 1f);

        
        
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

    [SerializeField] private VisualEffect[] vfxArray;

    // Nombre del parámetro booleano en el VFX Graph, si es necesario
    [SerializeField] private string vfxParameter = "PlayVFX";

    public void PlayVFX()
    {
        foreach (var vfx in vfxArray)
        {
            if (vfx != null)
            {
                // Si el VFX Graph tiene un parámetro booleano para activar


                vfx.SetBool(vfxParameter, true);


                // Reiniciar el VFX para que las partículas comiencen de nuevo
                vfx.Reinit();
            }
            else
            {
                Debug.LogWarning("Un VisualEffect no está asignado en el array.");
            }
        }
    }
}
