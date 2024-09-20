using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy2 : Entity
{
    [Header("<color=red>AI</color>")]
    [SerializeField] private float _chaseDist = 10.0f;  // Distancia para empezar a perseguir
    [SerializeField] private float _shootDist = 5.0f;   // Distancia a la que el enemigo se detiene y dispara
    [SerializeField] private float _changeNodeDist = 0.5f;
    [SerializeField] public float speed;
    [SerializeField] private Player _player;

    [SerializeField] private GameObject projectilePrefab; // Proyectil que disparará el enemigo
    [SerializeField] private Transform shootPoint;        // Punto de origen del proyectil
    [SerializeField] private float shootCooldown = 2.0f;  // Intervalo entre disparos
    private float lastShootTime;

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
        //GameManager.Instance.Enemies.Add(this);
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

        float distanceToPlayer = Vector3.SqrMagnitude(transform.position - _target.position);

        if (distanceToPlayer <= (_chaseDist * _chaseDist))
        {
            // Si está dentro del rango de persecución
            if (distanceToPlayer <= (_shootDist * _shootDist))
            {
                // Si está dentro del rango de disparo
                if (!_agent.isStopped) _agent.isStopped = true;

                if (Time.time >= lastShootTime + shootCooldown)
                {
                    Shoot();
                    lastShootTime = Time.time;
                }
            }
            else
            {
                // Si está fuera del rango de disparo, pero dentro del rango de persecución
                if (_agent.isStopped) _agent.isStopped = false;
                _agent.SetDestination(_target.position);
            }
        }
        else
        {
            // Si está fuera del rango de persecución
            if (_agent.destination != _actualNode.position) _agent.SetDestination(_actualNode.position);

            if (Vector3.SqrMagnitude(transform.position - _actualNode.position) <= (_changeNodeDist * _changeNodeDist))
            {
                _actualNode = GetNewNode(_actualNode);
                _agent.SetDestination(_actualNode.position);
            }
        }

        _agent.speed = speed;
    }

    private void Shoot()
    {
        // Crear un proyectil y dispararlo hacia el jugador
        GameObject projectile = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);
        Vector3 direction = (_target.position - shootPoint.position).normalized;
        projectile.GetComponent<Rigidbody>().velocity = direction * 20f; // Ajusta la velocidad según sea necesario
        Debug.Log($"{name} ha disparado.");
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

        // Color para el shoot distance
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _shootDist);

        // Color para el change node distance
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _changeNodeDist);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Verifica si el objeto que colisiona es el proyectil
        if (other.gameObject.name == "Projectile")
        {
            health -= 1;

            if (health <= 0)
            {
                //GameManager.Instance.Enemies.Remove(this);
                Destroy(this.gameObject);
            }
        }
    }
}
