using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrollingNPC : MonoBehaviour
{
    [Header("Patrullaje")]
    public List<Node> patrolPoints;
    public float moveSpeed = 2f;
    public Color NPCColor;
    public TypeOfPathfinding pathfindingType;
    public TypeOfPathCalc myPathCalc;

    [Header("Persecución e Investigación")]
    public float chaseSpeed = 3.5f;
    public float investigateDuration = 4f;

    [Header("Detección de Suelo")]
    public LayerMask groundLayers = ~0; // Todas por defecto
    public float groundRayLength = 2f;
    public Vector3 groundRayOffset = new Vector3(0, 1f, 0);

    // Debug gizmo
    private Vector3 debugGroundRayStart;
    private Vector3 debugGroundRayEnd;
    private bool debugGroundHit;
    [Header("Tiempos de Comportamiento")]
    public float timeBeforeReturningToPatrol = 2f;
    [SerializeField] private INPCState currentState;

    [Header("Alerta")]
    public bool hasBeenAlerted = false;

    [Header("Detección y Alerta")]
    public GameObject player;
    public FOVAgent FOVAgent;

    [Header("Captura")]
    public float captureRange = 1.5f;

    [HideInInspector] public Vector3 lastSeenPosition;

    private int currentTargetIndex = 0;
    private bool isMoving = false;
    public List<Node> currentPath = new List<Node>();
    private Pathfinding pathfinder;

    private Coroutine followPathRoutine;
    private Coroutine patrolRoutineHandle;

    // Control de transición de estados
    private bool _isTransitioning;
    private INPCState _pendingState;

    protected void Start()
    {
        pathfinder = new Pathfinding();
        FOVAgent.ChangeColor(NPCColor);
        SwitchState(new PatrolState());
        NPCAlertSystem.RegisterNPC(this);
    }

    private void Update()
    {
        currentState?.Update(this);
    }

    public void SwitchState(INPCState newState)
    {
        // Evita cambiar al mismo estado
        if (currentState != null && currentState.GetType() == newState.GetType())
            return;

        if (_isTransitioning)
        {
            _pendingState = newState;
            return;
        }

        StartCoroutine(SwitchStateDeferred(newState));
    }

    private IEnumerator SwitchStateDeferred(INPCState newState)
    {
        _isTransitioning = true;

        // Salir del estado actual
        currentState?.Exit(this);

        // Yield para cortar posibles llamadas recursivas
        yield return null;

        // Entrar al nuevo estado
        currentState = newState;
        currentState.Enter(this);

        _isTransitioning = false;

        // Si se pidió otro cambio mientras tanto, lo aplicamos
        if (_pendingState != null)
        {
            var next = _pendingState;
            _pendingState = null;
            SwitchState(next);
        }
    }

    public bool IsPlayerVisible()
    {
        return FOVAgent.InFOV(player.transform.position);
    }

    public void OnPlayerSpotted(Vector3 playerPosition)
    {
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

    public void GeneratePath(Node start, Node goal)
    {
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
        }

        foreach (var node in currentPath)
        {
            node.ChangeColor(NPCColor);
        }
    }

    public IEnumerator FollowPath()
    {
        isMoving = true;

        foreach (Node node in currentPath)
        {
            Vector3 targetPos = node.transform.position;

            // Ajustar altura al suelo del nodo
            if (Physics.Raycast(targetPos + Vector3.up * 2f, Vector3.down, out RaycastHit hitNode, 5f, groundLayers))
            {
                targetPos.y = hitNode.point.y;
            }

            float stuckTimer = 0f;

            while (Vector3.Distance(transform.position, targetPos) > 0.1f)
            {
                Vector3 moveDir = (targetPos - transform.position).normalized;

                // --- Raycast hacia abajo para adaptarse a pendientes ---
                Vector3 rayOrigin = transform.position + groundRayOffset;
                if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hitGround, groundRayLength, groundLayers))
                {
                    // Guardar info para gizmo
                    debugGroundRayStart = rayOrigin;
                    debugGroundRayEnd = hitGround.point;
                    debugGroundHit = true;

                    // Proyectar movimiento en el plano de la pendiente
                    moveDir = Vector3.ProjectOnPlane(moveDir, hitGround.normal).normalized;

                    // Ajustar posición Y al suelo
                    Vector3 pos = transform.position;
                    pos.y = hitGround.point.y;
                    transform.position = pos;
                }
                else
                {
                    debugGroundHit = false;
                }

                // Mover NPC
                transform.position += moveDir * moveSpeed * Time.deltaTime;

                // Orientar hacia la dirección de movimiento
                if (moveDir != Vector3.zero)
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveDir), 10f * Time.deltaTime);

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
            if (!isMoving && patrolPoints.Count > 1)
            {
                Node startNode = GetClosestNode();
                Node goalNode = patrolPoints[currentTargetIndex];

                GeneratePath(startNode, goalNode);
                followPathRoutine = StartCoroutine(FollowPath());
                yield return followPathRoutine;

                currentTargetIndex = (currentTargetIndex + 1) % patrolPoints.Count;
            }

            yield return null;
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
    }

    public Node GetClosestNode()
    {
        Node[] allNodes = FindObjectsOfType<Node>();
        Node closest = null;
        float minDist = Mathf.Infinity;

        foreach (var node in allNodes)
        {
            if (node.Block) continue;
            float dist = Vector3.Distance(transform.position, node.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = node;
            }
        }

        return closest;
    }

    public Node GetNodeAtPosition(Vector3 pos)
    {
        Node[] allNodes = FindObjectsOfType<Node>();
        Node closest = null;
        float minDist = Mathf.Infinity;

        foreach (var node in allNodes)
        {
            float dist = Vector3.Distance(pos, node.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = node;
            }
        }

        return closest;
    }
    private void OnDrawGizmos()
    {
        // Gizmo del rango de captura
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, captureRange);

        // Gizmo del raycast de suelo
        if (debugGroundHit)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(debugGroundRayStart, debugGroundRayEnd);
            Gizmos.DrawSphere(debugGroundRayEnd, 0.05f);
        }
        else
        {
            Gizmos.color = Color.yellow;
            Vector3 endPos = debugGroundRayStart + Vector3.down * groundRayLength;
            Gizmos.DrawLine(debugGroundRayStart, endPos);
        }

        if (currentPath != null && currentPath.Count > 0)
        {
            Gizmos.color = NPCColor;

            for (int i = 0; i < currentPath.Count; i++)
            {
                Vector3 nodePos = currentPath[i].transform.position;
                nodePos.y += 0.1f; // un poco arriba para que no se mezcle con el suelo
                Gizmos.DrawSphere(nodePos, 0.15f);

                if (i < currentPath.Count - 1)
                {
                    Vector3 nextPos = currentPath[i + 1].transform.position;
                    nextPos.y += 0.1f;
                    Gizmos.DrawLine(nodePos, nextPos);
                }
            }
        }
    }
}
