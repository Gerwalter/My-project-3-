using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : HP
{
    [Header("<color=red>AI</color>")]
    [SerializeField] private float _chaseDist = 6.0f;
    [SerializeField] private float _atkDist = 2.0f;
    [SerializeField] private float _changeNodeDist = 0.5f;
    [SerializeField] public float speed;
    [SerializeField] private Player _player;
    [SerializeField] private float _damageInterval = 1.0f;
    [SerializeField] private GameObject sword;

    [SerializeField] private float _healDist = 5.0f;
    [SerializeField] private float _shieldDist = 5.0f;
    [SerializeField] private float _shootDist = 5.0f;


    public Transform _target, _actualNode;
    private List<Transform> _navMeshNodes = new();
    public List<Transform> NavMeshNodes
    {
        get { return _navMeshNodes; }
        set { _navMeshNodes = value; }
    }

    private NavMeshAgent _agent;
    private Coroutine _damageCoroutine;


    public enum EnemyType
    {
        Normal,
        Healer,
        Shielder,
        Shooter,
    }

    [SerializeField] private EnemyType _enemyType;


    private void Start()
    {
        GameManager.Instance.Enemies.Add(this);

        _agent = GetComponent<NavMeshAgent>();
        _target = GameManager.Instance.Player.gameObject.transform;

        Entity entity = GetComponent<Entity>();
        if (entity != null)
        {
            entity.SetEntityType(_enemyType);
            entity._target = _target;
            entity._healDist = _healDist;
            entity._shieldDist = _shieldDist;
            entity._shootDist = _shootDist;
            entity._agent = _agent;
        }
    }

    public void Initialize()
    {
        _actualNode = GetNewNode();

        _agent.SetDestination(_actualNode.position);
    }

    private void Update()
    {
        if (!_target)
        {
            Debug.LogError($"<color=red>NullReferenceException</color>: No asignaste un objetivo.");
            return;
        }

        if (Vector3.SqrMagnitude(transform.position - _target.position) <= (_chaseDist * _chaseDist))
        {
            if (Vector3.SqrMagnitude(transform.position - _target.position) <= (_atkDist * _atkDist))
            {
                if (!_agent.isStopped) _agent.isStopped = true;

                if (_damageCoroutine == null)
                {
                    _damageCoroutine = StartCoroutine(ApplyDamageOverTime());
                }
            }
            else
            {
                if (_agent.isStopped) _agent.isStopped = false;

                _agent.SetDestination(_target.position);

                if (_damageCoroutine != null)
                {
                    StopCoroutine(_damageCoroutine);
                    _damageCoroutine = null;
                }
            }
        }
        else
        {
            if (_agent.destination != _actualNode.position) _agent.SetDestination(_actualNode.position);

            if (Vector3.SqrMagnitude(transform.position - _actualNode.position) <= (_changeNodeDist * _changeNodeDist))
            {
                _actualNode = GetNewNode(_actualNode);

                _agent.SetDestination(_actualNode.position);
            }
        }

        _agent.speed = speed;
    }

    
    private IEnumerator ApplyDamageOverTime()
    {
        while (true)
        {
            Debug.Log($"<color=red>{name}</color>: Japish.");
            _player.ReciveDamage(2);

            yield return new WaitForSeconds(_damageInterval);
        }
    }

    public Transform GetNewNode(Transform lastNode = null)
    {
        Transform newNode = _navMeshNodes[Random.Range(0, _navMeshNodes.Count)];

        while (lastNode == newNode)
        {
            newNode = _navMeshNodes[Random.Range(0, _navMeshNodes.Count)];
        }

        return newNode;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _chaseDist);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _atkDist);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _changeNodeDist);

        if (_enemyType == EnemyType.Healer)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _healDist);
        }

        // Dibuja el área de escudo solo si el enemigo es un Shielder
        if (_enemyType == EnemyType.Shielder)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, _shieldDist);
        }

        if (_enemyType == EnemyType.Shooter)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, _shootDist);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == sword)
        {
            ReciveDamage(5);
        }
    }
}
