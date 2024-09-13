using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : Entity
{
    [Header("<color=red>AI</color>")]
    [SerializeField] private float _chaseDist = 6.0f;
    [SerializeField] private float _atkDist = 2.0f;
    [SerializeField] private float _changeNodeDist = 0.5f;
    [SerializeField] public float speed;
    [SerializeField] private Player _player;

    [SerializeField] private GameObject sword;
    public float health;
    public float maxHealth;

    public Transform _target, _actualNode;
    private List<Transform> _navMeshNodes = new();
    public List<Transform> NavMeshNodes
    {
        get { return _navMeshNodes; }
        set { _navMeshNodes = value; }
    }

    private NavMeshAgent _agent;

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _target = GameManager.Instance.Player.gameObject.transform;
        GameManager.Instance.Enemies.Add(this);
        health = maxHealth;
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
            Debug.LogError($"<color=red>NullReferenceException</color>: No asignaste un objetivo, boludo.");
            return;
        }

        if (Vector3.SqrMagnitude(transform.position - _target.position) <= (_chaseDist * _chaseDist))
        {
            if (Vector3.SqrMagnitude(transform.position - _target.position) <= (_atkDist * _atkDist))
            {
                if (!_agent.isStopped) _agent.isStopped = true;

                Debug.Log($"<color=red>{name}</color>: Japish.");
                _player.Damage();
                
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
        // Color para el chase distance
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _chaseDist);

        // Color para el attack distance
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _atkDist);

        // Color para el change node distance
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _changeNodeDist);
    }


    private void OnTriggerEnter(Collider other)
    {
        // Verifica si el objeto que colisiona es la espada
        if (other.gameObject == sword)
        {
            health -= 1;

            if (health <= 0)
            {
                GameManager.Instance.Enemies.Remove(this);
                Destroy(this.gameObject);
            }
        }
    }
}
