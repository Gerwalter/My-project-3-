using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

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
    [SerializeField] private float _shieldDist = 5.0f;
    [SerializeField] private float _shootDist = 6.0f;
    [SerializeField] private int _speed;

    [SerializeField] private bool _canStart = false;
    [SerializeField] public IANodeManager _nodeManager;

    [Header("<color=red>Behaviours</color>")]
    [SerializeField] private Animator _animator;
    [SerializeField] private Transform _target, _actualNode;
    [SerializeField] private Material mat;

    [SerializeField] private List<Transform> _navMeshNodes = new();
    public List<Transform> NavMeshNodes
    {
        get { return _navMeshNodes; }
        set { _navMeshNodes = value; }
    }

    [SerializeField] private NavMeshAgent _agent;

    [SerializeField] AudioClip[] clips;

    [Header("<color=#6A89A7>UI</color>")]
    [SerializeField] private Image healthBar;

    private void Awake()
    {
        Initialize();
        _nodeManager = GameManager.Instance.NodeManager;
    }
    private void Start()
    {

        GetLife = maxLife;
        _agent = GetComponent<NavMeshAgent>();

       // _speed = Random.Range(3, 8);

        _agent.speed = _speed;

        GameManager.Instance.Enemies.Add(this);

        StartCoroutine(WaitOneFrame());
       
    }
    private IEnumerator WaitOneFrame()
    {
        for (int i = 0; i < 3; i++)
        {
            yield return null; // Espera un frame
        }
        _canStart = true;
        Finalizer();
    }
    public void Initialize()
    {
        _target = GameManager.Instance.Player.gameObject.transform;
        
    }

    public void Finalizer()
    {
        _actualNode = GetNewNode();

        _agent.SetDestination(_actualNode.position);
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

        if (!_canStart) return; // Espera hasta que se haya habilitado

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


        if (_isHealer)
        {
            Enemy nearbyAlly = FindAllyToHeal();
            if (nearbyAlly != null)
            {
                HealAlly(nearbyAlly);
                return;

            }
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

    private bool PlayerInShootRange()
    {
        return Vector3.Distance(transform.position, _target.position) <= _shootDist;
    }
    void Shoot()
    {
        GameObject projectile = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);
        Vector3 direction = (_target.position - shootPoint.position).normalized;
        projectile.GetComponent<Rigidbody>().velocity = direction * 40f;
    }
    void ActivateShield()
    {
        _shieldInstance.SetActive(true);
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
                Debug.Log("Japish");
                player.ReciveDamage(_atkDmg);
            }
        }
    }
    void DeactivateShield()
    {
        _shieldInstance.SetActive(false);
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
                return enemy; // Retorna el primer enemigo con vida incompleta
            }
        }
        return null;
    }

    // Curar al aliado detectado
    private void HealAlly(Enemy ally)
    {
        _agent.SetDestination(ally.transform.position);
        if (Vector3.Distance(transform.position, ally.transform.position) <= 1.0f) // Rango para curar
        {
            ally.Health(10); // Curar 10 unidades de vida (puedes ajustar el valor)
        }
    }

    public void ReciveDamage(int dmg)
    {
        if (_isShielder && _shieldLife >= 0)
        {
            _shieldLife -= dmg;

            if (_shieldLife <= 0)
            { _shieldInstance.SetActive(false); }
        }
        else
        {
            GetLife -= dmg;
        }

        if (GetLife <= 0)
        {
            Debug.Log($"<color=red>{name}</color>: oh no *Explota*");

            SFXManager.instance.PlayRandSFXClip(clips, transform, 1f);

            GameManager.Instance.Enemies.Remove(this);

            Destroy(gameObject);

        }
        else
        {
            Debug.Log($"<color=red>{name}</color>: Comí <color=black>{dmg}</color> puntos de daño. Me quedan <color=green>{GetLife}</color> puntos.");
        }
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

        if (_enemyClass == EnemyClass.Shielder)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, _shieldDist);

        }

        if (_enemyClass == EnemyClass.Shooter)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, _shootDist);
        }
    }
}

