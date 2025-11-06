using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class PatrollingNPC : MonoBehaviour
{
    [Header("Patrullaje")]
    public List<Node> patrolNodes = new List<Node>();
    private List<Vector3> patrolPoints = new List<Vector3>();
    [SerializeField] private List<Quaternion> patrolRotations = new List<Quaternion>(); // Nuevo: Lista para rotaciones de nodos
    public float moveSpeed = 2f;
   // public TypeOfPathCalc myPathCalc;

    [Header("Persecucion e Investigacion")]
    public float chaseSpeed = 3.5f;
    [Tooltip("Duracion de investigacion si la investigacion fue causada por una distraccion")]
    public float distractionInvestigateDuration = 6f;

    [Header("Deteccion de Suelo")]
    public LayerMask groundLayers = 0;
    public float groundRayLength = 2f;
    [SerializeField] private Vector3 groundRayOffset = new Vector3(0, 1f, 0);

    [Header("Deteccion y Alerta")]
    public GameObject player;
    public LayerMask playerLayer;
    public LayerMask distractionLayer;
    public FOVAgent FOVAgent;

    // NUEVO: Sistema de Exposición
    [Header("Sistema de Exposición")]
    public float exposureFillRateBase = 10f; // Velocidad base de llenado
    public float exposureDecayRate = 5f;     // Velocidad de decremento cuando no visible
    public Image exposureUI;
    [SerializeField] public float currentExposure = 0f;
    [SerializeField] private float maxExposure = 100f;
    public bool isExposureFull => currentExposure >= maxExposure;
    public float CurrentExposure => currentExposure;
    public float ExposurePercentage => currentExposure / maxExposure;
    public float NormalizedExposure => Mathf.Clamp01(currentExposure / maxExposure);

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [Header("Captura")]
    public float captureRange = 1.5f;

    [HideInInspector] public Vector3 lastSeenPosition;
    public bool isCoinDistraction;

    [SerializeField] private INPCState currentState;
    private bool _isSwitchingState = false;

    private int currentTargetIndex = 0;
    private bool isMoving = false;
    private Coroutine followPathRoutine;
    private Coroutine patrolRoutineHandle;

    private Vector3 debugGroundRayStart;
    private Vector3 debugGroundRayEnd;
    private bool debugGroundHit;

    public bool hasTriggered = false;
    [SerializeField] private string battleSceneName = "BattleScene";

    [Header("Distracciones / Audicion")]
    public LayerMask hearingLayerMask = 0;
    public float hearingRange = 10f;
    [HideInInspector] public Vector3 lastHeardPosition;
    [HideInInspector] public bool heardDistraction = false;

    public NavMeshAgent agent;

    protected void Start()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
        agent.updateRotation = true;

        foreach (var node in patrolNodes)
        {
            if (node != null)
            {
                patrolPoints.Add(node.transform.position);
                patrolRotations.Add(node.transform.rotation); // Nuevo: Agregar rotacin del nodo
            }
        }
        ThiefAlertSystem.instance.Subscribe(new ExposureAlertObserver(this));
        SwitchState(new PatrolState());
        NPCAlertSystem.RegisterNPC(this);
    }

    private void Update()
    {
        currentState?.Update(this);
        UpdateExposure();
        if (exposureUI != null)
        {
            exposureUI.fillAmount = NormalizedExposure;
        }
        if (animator != null)
        {
            animator.SetFloat("Move", isMoving ? 1f : 0f);
        }
        // Detectar monedas visibles
        if (!(currentState is ChaseState || currentState is InvestigateState))
        {
            CheckForVisibleCoin();
        }

        if (hasTriggered)
            DefeatEnemy();
    }
    private void UpdateExposure()
    {
        float alertMultiplier = ThiefAlertSystem.instance.ObtainValue() / ThiefAlertSystem.instance._MaxAlert;
        float fillRate = exposureFillRateBase * (1f + alertMultiplier * 2f); // Se llena 3x más rápido al 100% alert

        if (IsPlayerVisible())
        {
            currentExposure = Mathf.Min(currentExposure + fillRate * Time.deltaTime, maxExposure);
        }
        else
        {
            currentExposure = Mathf.Max(currentExposure - exposureDecayRate * Time.deltaTime, 0f);
        }
    }
    public void DefeatEnemy()
    {
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        BattleManager.Instance.StartBattle(player.transform.position, currentScene, battleSceneName);
        GetComponent<EnemyPersistent>()?.DefeatEnemy();
    }

    public void SeeCoin(Vector3 coinPosition)
    {
        if (currentState is ChaseState) return;

        lastSeenPosition = coinPosition;
        heardDistraction = false;
        isCoinDistraction = true;

        StopPatrolRoutine();
        StopFollowingPath();
        SwitchState(new InvestigateState());
    }

    private void CheckForVisibleCoin()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, FOVAgent.ViewRange, distractionLayer);
        foreach (var hit in hits)
        {
            if (FOVAgent.InFOV(hit.transform.position))
            {
                DistractionObject distraction = hit.GetComponent<DistractionObject>();
                if (distraction != null && distraction.distractionType == DistractionType.Coin)
                {
                    SeeCoin(hit.transform.position);
                    break;
                }
            }
        }
    }

    #region StateManagement
    public void SwitchState(INPCState newState)
    {
        if (newState == null) return;
        if (currentState != null && currentState.GetType() == newState.GetType()) return;
        if (_isSwitchingState) return;

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
        isCoinDistraction = false;
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

    #region Movement / Patrol
    public IEnumerator MoveToRoutine(Vector3 targetPos)
    {
        isMoving = true;
        agent.SetDestination(targetPos);

        float stuckTimer = 0f;
        while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
        {
            Vector3 rayOrigin = transform.position + groundRayOffset;
            if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hitGround, groundRayLength, groundLayers))
            {
                debugGroundRayStart = rayOrigin;
                debugGroundRayEnd = hitGround.point;
                debugGroundHit = true;

                Vector3 pos = transform.position;
                pos.y = hitGround.point.y;
                transform.position = pos;
            }
            else
            {
                debugGroundHit = false;
            }

            stuckTimer += Time.deltaTime;
            if (stuckTimer > 5f) break;

            yield return null;
        }

        isMoving = false;
    }

    public IEnumerator PatrolRoutine()
    {
        while (true)
        {
            if (!isMoving && patrolPoints != null && patrolPoints.Count > 0)
            {
                Vector3 goalPos = patrolPoints[currentTargetIndex];

                if (NavMesh.SamplePosition(goalPos, out NavMeshHit hit, 2f, NavMesh.AllAreas))
                {
                    goalPos = hit.position;
                }
                else
                {
                    if (patrolPoints.Count > 1)
                        currentTargetIndex = (currentTargetIndex + 1) % patrolPoints.Count;
                    yield return null;
                    continue;
                }

                if (followPathRoutine != null) StopCoroutine(followPathRoutine);
                followPathRoutine = StartCoroutine(MoveToRoutine(goalPos));
                yield return followPathRoutine;

                // Nuevo: Aplicar rotacin del nodo al llegar
                if (patrolRotations.Count > currentTargetIndex)
                {
                    transform.rotation = patrolRotations[currentTargetIndex];
                }

                if (patrolPoints.Count > 1)
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
        agent.ResetPath();
        isMoving = false;
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
    private void OnDisable()
    {
        ThiefAlertSystem.instance?.Unsubscribe(new ExposureAlertObserver(this));
    }
}