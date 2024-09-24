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

    [SerializeField] private Animator anim;

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

        if (_actualNode == null)
            return;
    }

    public void Initialize()
    {
        _actualNode = GetNewNode();

        _agent.SetDestination(_actualNode.position);

        GameManager.Instance.Enemies.Add(this);

        if (_actualNode == null)
        {
            _actualNode = _target.transform;
        }
    }

    private void Update()
    {
        if (_actualNode == null)
        {
            _actualNode = _target.transform;
        }

        if (Vector3.SqrMagnitude(transform.position - _target.position) <= (_chaseDist * _chaseDist))
        {
            if (Vector3.SqrMagnitude(transform.position - _target.position) <= (_atkDist * _atkDist))
            {
                if (!_agent.isStopped) _agent.isStopped = true;

                _player.ReciveDamage(.05f);
            }
            else
            {
                if (_agent.isStopped) _agent.isStopped = false;

                _agent.SetDestination(_target.position);

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
        anim.SetBool("isMoving", true);
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
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, _shootDist);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Sword")) 
        {
            ReciveDamage(10);
            GameManager.Instance.Enemies.Remove(this);
        }
    }
}
