using UnityEngine;

public class Enemy : EnemyHealth
{
    public override void Start()
    {
        base.Start();
        // EnemyManager.Instance.RegisterEnemy(this);

    }

    public static System.Action OnEnemyDefeated;



    public override void Die()
    {
        if (hasAttackSlot && EnemyManager.Instance != null)
        {
            EnemyManager.Instance.ReleaseSlot(attackSlotType);
            hasAttackSlot = false;
        }

        EnemyManager.Instance.UnregisterEnemy(this);
        OnEnemyDefeated?.Invoke();
        base.Die();
    }

    [Header("Enemy Settings")]
    public float detectionRadius = 5f;
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;
    public LayerMask playerLayer;

    private float attackTimer = 0f;
    private Transform targetPlayer;
    [SerializeField] private bool hasAttackSlot = false;
    [Header("Attack Type")]
    public AttackSlotType attackSlotType = AttackSlotType.Melee;
    void Update()
    {
        switch (currentState)
        {
            case EnemyStates.Idle:
                UpdateIdle();
                break;

            case EnemyStates.Aware:
                UpdateAware();
                break;

            case EnemyStates.Chasing:
                UpdateChasing();
                break;

            case EnemyStates.WaitingForAttack:
                UpdateWaitingForAttack();
                break;

            case EnemyStates.Attacking:

                UpdateAttacking();
                break;

            case EnemyStates.Recovering:
                UpdateRecovering();
                break;
        }
    }

  [SerializeField]  float recoverTimer;

    void UpdateRecovering()
    {
        recoverTimer -= Time.deltaTime;
        if (recoverTimer <= 0f)
        {
            ChangeState(EnemyStates.WaitingForAttack);
        }
    }
    void UpdateIdle()
    {
        if (DetectPlayer())
        {
            ChangeState(EnemyStates.Aware);
        }
    }
    [SerializeField] float awareTimer;

    void UpdateAware()
    {
        awareTimer -= Time.deltaTime;
        if (awareTimer <= 0f)
        {
            ChangeState(EnemyStates.Chasing);
        }
    }
    void UpdateChasing()
    {
        if (targetPlayer == null) return;

        float dist = Vector3.Distance(transform.position, targetPlayer.position);
        if (dist <= attackRange + 0.5f)
        {
            ChangeState(EnemyStates.WaitingForAttack);
        }
    }
    void UpdateWaitingForAttack()
    {
        if (EnemyManager.Instance == null) return;
        if (targetPlayer == null) return;

        float distance = Vector3.Distance(transform.position, targetPlayer.position);

        // Distancia distinta según tipo
        if (attackSlotType == AttackSlotType.Melee && distance > attackRange)
            return;

        if (attackSlotType == AttackSlotType.Shooter && distance < attackRange)
            return;

        if (!hasAttackSlot && EnemyManager.Instance.CanRequestSlot(attackSlotType))
        {
            if (EnemyManager.Instance.RequestSlot(attackSlotType))
            {
                hasAttackSlot = true;
                ChangeState(EnemyStates.Attacking);
            }
        }
    }


    void UpdateAttacking()
    {
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0f)
        {
            if (hasAttackSlot)
            {
                EnemyManager.Instance.ReleaseSlot(attackSlotType);
                hasAttackSlot = false;
            }
            attackTimer = attackCooldown;
            ChangeState(EnemyStates.Recovering);
        }
    }
    bool DetectPlayer()
    {
        Collider[] players = Physics.OverlapSphere(transform.position, detectionRadius, playerLayer);
        if (players.Length > 0)
        {
            targetPlayer = players[0].transform;
            return true;
        }

        targetPlayer = null;
        return false;
    }
    [SerializeField] private EnemyStates currentState;
    public EnemyStates CurrentState => currentState;
    public Transform GetTargetPlayer() => targetPlayer;
    private void ChangeState(EnemyStates newState)
    {
        if (currentState == newState) return;

        currentState = newState;
        Debug.Log($"[{name}] Estado = {currentState}");

        OnEnterState(newState);
    }
    private void OnEnterState(EnemyStates state)
    {
        switch (state)
        {
            case EnemyStates.Idle:
                print(state);
                break;

            case EnemyStates.Aware:
                print(state);
                awareTimer = 0.5f;
                break;

            case EnemyStates.Chasing:
                break;

            case EnemyStates.WaitingForAttack:
                print(state);
                break;

            case EnemyStates.Attacking:

                attackTimer = attackCooldown;

                if (attackSlotType == AttackSlotType.Melee)
                    anim.SetTrigger("Punch");
                else if (attackSlotType == AttackSlotType.Shooter)
                    anim.SetTrigger("Shoot");

                attackTimer = 0.8f;
                break;

            case EnemyStates.Recovering:
                recoverTimer = 1.2f;
                break;

            case EnemyStates.Dead:
                break;
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
