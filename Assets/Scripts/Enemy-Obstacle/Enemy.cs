using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : Entity
{
    [SerializeField] private Animator _animator;
    [SerializeField] private Image healthBar;
    private float lastHitTime;
    private float hitCooldown = 0.5f;

    private Rigidbody rb;

    [Header("<color=red>AI</color>")]
    [SerializeField] private float _chaseDist = 6.0f;
    [SerializeField] private float _atkDist = 2.0f;
    [SerializeField] private float _changeNodeDist = 0.5f;
    [SerializeField] private bool _enableRoam;


    protected override void Awake()
    {
        base.Awake();
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

    private NavMeshAgent _agent;



    public void Initialize()
    {
        _target = GameManager.Instance.Player.gameObject.transform;

        _actualNode = GetNewNode();

        _agent.SetDestination(_actualNode.position);

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

    protected override void HandleDamage(float damage)
    {
        base.HandleDamage(damage);
        if (_animator != null && Time.time >= lastHitTime + hitCooldown)
        {
            _animator.SetTrigger("Hit");
            lastHitTime = Time.time;  // Actualizar el tiempo del último hit
        }
        // Actualización de barra de vida específica del enemigo, si es necesario
    }

    protected override void HandleDeath()
    {
        base.HandleDeath();
        _agent.isStopped = true;
        _animator.SetTrigger("Die");
        GameManager.Instance.Enemies.Remove(this);
        Destroy(gameObject, 2.0f); // Destruir después de la animación de muerte
    }



    private void FixedUpdate()
    {
        UpdateHealthBar();

        if (!_target)

        {
            {
                Debug.LogError($"<color=red>NullReferenceException</color>: No asignaste un objetivo, boludo.");
                return;
            }
        }

        if (_enableRoam)
        {
            if (Vector3.SqrMagnitude(transform.position - _target.position) <= (_chaseDist * _chaseDist))
            {
                if (Vector3.SqrMagnitude(transform.position - _target.position) <= (_atkDist * _atkDist))
                {
                    if (!_agent.isStopped)
                    { // _agent.isStopped = true;}
                        print("non");
                    }
                    _animator.SetBool("isMoving", false);
                    _animator.SetTrigger("Punch");
                }
                else
                {
                    if (_agent.isStopped)
                    {
                        print("oal");
                    }// _agent.isStopped = false;

                    _animator.SetBool("isMoving", true);
                    _animator.ResetTrigger("Punch");
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


    private void UpdateHealthBar()
    {
        float lifePercent = GetLife / maxLife;

        healthBar.fillAmount = lifePercent;

        healthBar.color = Color.Lerp(Color.red, Color.green, lifePercent);
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
    }
    }
