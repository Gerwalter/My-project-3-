using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrollingNPC : MonoBehaviour
{
    [Header("Patrullaje")]
    public List<Node> patrolPoints = new List<Node>();
    public float moveSpeed = 2f;
    public TypeOfPathfinding pathfindingType;
    public TypeOfPathCalc myPathCalc;

    [Header("Persecución e Investigación")]
    public float chaseSpeed = 3.5f;
    public float investigateDuration = 4f;

    [Header("Detección de Suelo")]
    public LayerMask groundLayers = ~0;
    public float groundRayLength = 2f;
    [SerializeField] private Vector3 groundRayOffset = new Vector3(0, 1f, 0);

    [Header("Detección y Alerta")]
    public GameObject player;
    public LayerMask playerLayer;
    public FOVAgent FOVAgent;

    [Header("Captura")]
    public float captureRange = 1.5f;

    [HideInInspector] public Vector3 lastSeenPosition;

    // Estado y path
    [SerializeField] private INPCState currentState;
    private bool _isSwitchingState = false;

    private int currentTargetIndex = 0;
    private bool isMoving = false;
    public List<Node> currentPath = new List<Node>();
    private Pathfinding pathfinder;

    private Coroutine followPathRoutine;
    private Coroutine patrolRoutineHandle;

    // Debug
    private Vector3 debugGroundRayStart;
    private Vector3 debugGroundRayEnd;
    private bool debugGroundHit;

    // Cached nodes
    private Node[] cachedNodes;

    // Otros
    public bool hasTriggered = false;
    [SerializeField] private string battleSceneName = "BattleScene";

    [Header("Distracciones / Audición")]
    [Tooltip("Capas que pueden escuchar el sonido (por ejemplo: NPCs en layer 'NPC')")]
    public LayerMask hearingLayerMask = ~0;
    [Tooltip("Distancia máxima a la que el NPC puede oír (si usas OverlapSphere desde SoundEmitter)")]
    public float hearingRange = 10f; // valor por defecto, SoundEmitter normalmente define su propio radio

    [HideInInspector] public Vector3 lastHeardPosition;
    [HideInInspector] public bool heardDistraction = false;
    [Tooltip("Duración de investigación si la investigación fue causada por una distracción")]
    public float distractionInvestigateDuration = 6f; // override para distracciones

    protected void Start()
    {
        pathfinder = new Pathfinding();
        // Cachear nodos (mejor que buscar cada vez)
        cachedNodes = FindObjectsOfType<Node>();

        // Aplicar configuración global de pathcalc
        PathfindingGameManager.instance.myPathCalc = myPathCalc;

        SwitchState(new PatrolState());
        NPCAlertSystem.RegisterNPC(this);
    }

    private void Update()
    {
        currentState?.Update(this);

        if (hasTriggered)
            DefeatEnemy();
    }

    public void DefeatEnemy()
    {
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        BattleManager.Instance.StartBattle(player.transform.position, currentScene, battleSceneName);
        GetComponent<EnemyPersistent>()?.DefeatEnemy();
    }
    public void HearNoise(Vector3 sourcePosition, float intensity = 1f)
    {
        print("Chocó");
        // Si está persiguiendo al jugador intensamente, quizá no se distrae.
        // Ajusta la condición según tu diseño (ejemplo: si está en ChaseState lo ignora).
        if (currentState is ChaseState)
        {
            // Ignorar si estás en persecución. Cambiar comportamiento si quieres que los NPCs puedan ser distraídos en chase.
            return;
        }

        // Registrar fuente y marcar como distracción
        lastHeardPosition = sourcePosition;
        heardDistraction = true;

        // Cambiar a investigar inmediatamente
        StopPatrolRoutine();
        StopFollowingPath();

        // Reutilizamos InvestigateState (que leerá heardDistraction para usar la duración especifica)
        SwitchState(new InvestigateState());
    }
    #region StateManagement (simplificado)
    public void SwitchState(INPCState newState)
    {
        if (newState == null) return;
        if (currentState != null && currentState.GetType() == newState.GetType()) return;
        if (_isSwitchingState)
        {
            // Evitamos reentradas: simplemente ignoramos cambios simultáneos
            return;
        }

        _isSwitchingState = true;
        currentState?.Exit(this);
        currentState = newState;
        currentState.Enter(this);
        _isSwitchingState = false;
    }
    #endregion

    #region Detección
    public bool IsPlayerVisible()
    {
        if (player == null) return false;
        if (((1 << player.layer) & playerLayer) == 0) return false;
        return FOVAgent != null && FOVAgent.InFOV(player.transform.position);
    }

    public void OnPlayerSpotted(Vector3 playerPosition)
    {
        if (player == null) return;
        if (((1 << player.layer) & playerLayer) == 0) return;

        lastSeenPosition = playerPosition;
        if (currentState is not InvestigateState)
        {
            StopFollowingPath();
            SwitchState(new InvestigateState());
        }
    }

    public void OnAlertCleared()
    {
        if (!(currentState is PatrolState))
        {
            StopFollowingPath();
            SwitchState(new PatrolState());
        }
    }
    #endregion

    #region Pathfinding
    public void GeneratePath(Node start, Node goal)
    {
        if (start == null || goal == null) { currentPath.Clear(); return; }

        // Asegurar la misma configuración global que tenías antes
        PathfindingGameManager.instance.myPathCalc = myPathCalc;

        switch (pathfindingType)
        {
            case TypeOfPathfinding.BFS:
                currentPath = pathfinder.CalculateBFS(start, goal);
                break;
            case TypeOfPathfinding.Dijkstra:
                currentPath = pathfinder.CalculateDijkstra(start, goal);
                break;
            case TypeOfPathfinding.GreedyBFS:
                currentPath = pathfinder.CalculateGreedyBFS(start, goal);
                break;
            case TypeOfPathfinding.AStar:
                currentPath = pathfinder.CalculateAStar(start, goal);
                break;
            case TypeOfPathfinding.ThetaStar:
                currentPath = pathfinder.CalculateThetaStar(start, goal);
                break;
            default:
                currentPath = new List<Node>();
                break;
        }
    }
    #endregion

    #region Movement / Patrol
    public IEnumerator FollowPath()
    {
        if (currentPath == null || currentPath.Count == 0) yield break;
        isMoving = true;

        foreach (var node in currentPath)
        {
            Vector3 targetPos = node.transform.position;

            // Ajustar Y al suelo del nodo (si hay suelo cerca)
            if (Physics.Raycast(targetPos + Vector3.up * 2f, Vector3.down, out RaycastHit hitNode, 5f, groundLayers))
                targetPos.y = hitNode.point.y;

            // Mover hasta el objetivo
            float stuckTimer = 0f;
            while (Vector3.Distance(transform.position, targetPos) > 0.1f)
            {
                // Raycast suelo desde el NPC para adaptarse a pendientes una sola vez por frame
                Vector3 rayOrigin = transform.position + groundRayOffset;
                if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hitGround, groundRayLength, groundLayers))
                {
                    debugGroundRayStart = rayOrigin;
                    debugGroundRayEnd = hitGround.point;
                    debugGroundHit = true;

                    // Ajustar altura al suelo
                    Vector3 pos = transform.position;
                    pos.y = hitGround.point.y;
                    transform.position = pos;
                }
                else
                {
                    debugGroundHit = false;
                }

                // Movimiento horizontal hacia el objetivo (sin afectar demasiado la y)
                Vector3 moveTarget = new Vector3(targetPos.x, transform.position.y, targetPos.z);
                transform.position = Vector3.MoveTowards(transform.position, moveTarget, moveSpeed * Time.deltaTime);

                Vector3 lookDir = (moveTarget - transform.position);
                if (lookDir.sqrMagnitude > 0.001f)
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir.normalized), 10f * Time.deltaTime);

                stuckTimer += Time.deltaTime;
                if (stuckTimer > 5f) break;

                yield return null;
            }
        }

        isMoving = false;
    }

    public IEnumerator PatrolRoutine()
    {
        // Requiere al menos 2 puntos para patrullar en ciclo
        while (true)
        {
            if (!isMoving && patrolPoints != null && patrolPoints.Count > 1)
            {
                Node startNode = GetClosestNode();
                Node goalNode = patrolPoints[currentTargetIndex];
                GeneratePath(startNode, goalNode);

                if (followPathRoutine != null) StopCoroutine(followPathRoutine);
                followPathRoutine = StartCoroutine(FollowPath());
                yield return followPathRoutine;

                currentTargetIndex = (currentTargetIndex + 1) % patrolPoints.Count;
            }
            else
            {
                yield return null;
            }
        }
    }

    public void StartPatrolRoutine()
    {
        if (patrolRoutineHandle == null)
            patrolRoutineHandle = StartCoroutine(PatrolRoutine());
    }

    public void StopPatrolRoutine()
    {
        if (patrolRoutineHandle != null)
        {
            StopCoroutine(patrolRoutineHandle);
            patrolRoutineHandle = null;
        }
    }

    public void StopFollowingPath()
    {
        if (followPathRoutine != null)
        {
            StopCoroutine(followPathRoutine);
            followPathRoutine = null;
        }
        isMoving = false;
        currentPath.Clear();
    }
    #endregion

    #region Node helpers (usa cache)
    public Node GetClosestNode()
    {
        Node closest = null;
        float minDist = Mathf.Infinity;
        if (cachedNodes == null) cachedNodes = FindObjectsOfType<Node>();

        foreach (var node in cachedNodes)
        {
            if (node == null || node.Block) continue;
            float d = Vector3.Distance(transform.position, node.transform.position);
            if (d < minDist)
            {
                minDist = d;
                closest = node;
            }
        }
        return closest;
    }

    public Node GetNodeAtPosition(Vector3 pos)
    {
        Node closest = null;
        float minDist = Mathf.Infinity;
        if (cachedNodes == null) cachedNodes = FindObjectsOfType<Node>();

        foreach (var node in cachedNodes)
        {
            if (node == null) continue;
            float d = Vector3.Distance(pos, node.transform.position);
            if (d < minDist)
            {
                minDist = d;
                closest = node;
            }
        }
        return closest;
    }
    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, captureRange);
        Gizmos.color = Color.grey;
        Gizmos.DrawWireSphere(transform.position, hearingRange);
        if (debugGroundHit)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(debugGroundRayStart, debugGroundRayEnd);
            Gizmos.DrawSphere(debugGroundRayEnd, 0.05f);
        }
        else
        {
            Gizmos.color = Color.yellow;
            Vector3 start = transform.position + groundRayOffset;
            Vector3 endPos = start + Vector3.down * groundRayLength;
            Gizmos.DrawLine(start, endPos);
        }

        if (currentPath != null && currentPath.Count > 0)
        {
            for (int i = 0; i < currentPath.Count; i++)
            {
                if (currentPath[i] == null) continue;
                Vector3 nodePos = currentPath[i].transform.position;
                nodePos.y += 0.1f;
                Gizmos.DrawSphere(nodePos, 0.12f);
                if (i < currentPath.Count - 1 && currentPath[i + 1] != null)
                {
                    Vector3 nextPos = currentPath[i + 1].transform.position;
                    nextPos.y += 0.1f;
                    Gizmos.DrawLine(nodePos, nextPos);
                }
            }
        }
    }
}
