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
    [SerializeField] private float _shieldDist = 5.0f;
    [SerializeField] private float _healDist = 5.0f;
    [SerializeField] private float _changeNodeDist = 0.5f;
    [SerializeField] public float speed;
    [SerializeField] private Player _player;
    [SerializeField] private float _damageInterval = 1.0f; // Intervalo de tiempo para aplicar daño
    [SerializeField] private bool isShielder = false;
    [SerializeField] private bool isHealer = false;
    [SerializeField] private GameObject sword;
    [SerializeField] private GameObject shieldPrefab; // Prefab del escudo a instanciar

    public Transform _target, _actualNode;
    private List<Transform> _navMeshNodes = new();
    public List<Transform> NavMeshNodes
    {
        get { return _navMeshNodes; }
        set { _navMeshNodes = value; }
    }

    private NavMeshAgent _agent;
    private Coroutine _damageCoroutine;
    private GameObject _shieldInstance;

    private void Start()
    {
        GameManager.Instance.Enemies.Add(this);

        _agent = GetComponent<NavMeshAgent>();
        _target = GameManager.Instance.Player.gameObject.transform;
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

        if (isHealer)
        {
            Enemy nearbyAlly = FindAllyToHeal();
            if (nearbyAlly != null)
            {
                HealAlly(nearbyAlly);
                return; // No continuar con la lógica de persecución si está curando a otro
            }
        }

        // Si el escudo está activo, detener el movimiento del NavMeshAgent
        if (_shieldInstance != null && _shieldInstance.activeSelf)
        {
            if (!_agent.isStopped)
            {
                _agent.isStopped = true;
            }
            return; // No continuar con la lógica de movimiento mientras el escudo esté activo
        }
        else
        {
            if (_agent.isStopped)
            {
                _agent.isStopped = false;
            }
        }

        // Si es del tipo Shielder, detecta al jugador y activa el escudo
        if (isShielder)
        {
            if (PlayerInShieldRange())
            {
                ActivateShield();
                return; // No continuar con la lógica de persecución si está protegiéndose con el escudo
            }
        }

        // Comportamiento de persecución y ataque al jugador
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

    // Detecta si el jugador está en el rango de activación del escudo
    private bool PlayerInShieldRange()
    {
        return Vector3.Distance(transform.position, _target.position) <= _shieldDist;
    }

    // Activar el escudo cuando el jugador esté en rango
    private void ActivateShield()
    {
        if (_shieldInstance == null)
        {
            _shieldInstance = Instantiate(shieldPrefab, transform.position, Quaternion.identity);
            _shieldInstance.transform.SetParent(transform); // El escudo sigue al Shielder
        }
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

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _healDist);

        // Visualización del área de activación del escudo
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, _shieldDist);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == sword)
        {
            ReciveDamage(5);
        }
    }
}
