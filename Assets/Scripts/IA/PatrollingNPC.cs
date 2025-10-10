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

    [Header("Persecucion e Investigacion")]
    public float chaseSpeed = 3.5f;
    public float investigateDuration = 4f;

    [Header("Deteccion de Suelo")]
    public LayerMask groundLayers = 0;
    public float groundRayLength = 2f;
    [SerializeField] private Vector3 groundRayOffset = new Vector3(0, 1f, 0);

    [Header("Deteccion y Alerta")]
    public GameObject player;
    public LayerMask playerLayer;
    public FOVAgent FOVAgent;

    [Header("Captura")]
    public float captureRange = 1.5f;

    [HideInInspector] public Vector3 lastSeenPosition;

    [SerializeField] private INPCState currentState;
    private bool _isSwitchingState = false;

    private int currentTargetIndex = 0;
    private bool isMoving = false;
    public List<Node> currentPath = new List<Node>();
    private Pathfinding pathfinder;

    private Coroutine followPathRoutine;
    private Coroutine patrolRoutineHandle;

    private Vector3 debugGroundRayStart;
    private Vector3 debugGroundRayEnd;
    private bool debugGroundHit;

    private Node[] cachedNodes;

    // Otros
    public bool hasTriggered = false;
    [SerializeField] private string battleSceneName = "BattleScene";

    [Header("Distracciones / Audicion")]

    public LayerMask hearingLayerMask = 0;

    public float hearingRange = 10f; 

    [HideInInspector] public Vector3 lastHeardPosition;
    [HideInInspector] public bool heardDistraction = false;
    [Tooltip("Duraci0n de investigaci0n si la investigacion fue causada por una distraccion")]
    public float distractionInvestigateDuration = 6f; // override para distracciones

    protected void Start()
    {
        pathfinder = new Pathfinding();
        cachedNodes = FindObjectsOfType<Node>();

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
        if (currentState is ChaseState)
        { 
            return;
        }

        lastHeardPosition = sourcePosition;
        heardDistraction = true;

        // Cambiar a investigar inmediatamente
        StopPatrolRoutine();
        StopFollowingPath();

        SwitchState(new InvestigateState());
    }
    #region StateManagement (simplificado)
    public void SwitchState(INPCState newState)
    {
        if (newState == null) return;
        if (currentState != null && currentState.GetType() == newState.GetType()) return;
        if (_isSwitchingState)
        {
            return;
        }

        _isSwitchingState = true;
        currentState?.Exit(this);
        currentState = newState;
        currentState.Enter(this);
        _isSwitchingState = false;
    }
    #endregion

    #region Deteccion
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

            if (Physics.Raycast(targetPos + Vector3.up * 2f, Vector3.down, out RaycastHit hitNode, 5f, groundLayers))
                targetPos.y = hitNode.point.y;

            // Mover hasta el objetivo
            float stuckTimer = 0f;
            while (Vector3.Distance(transform.position, targetPos) > 0.1f)
            {
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
    }
}
