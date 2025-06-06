using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private float moveSpeed = 3.5f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 5.0f;
    [SerializeField] private float groundCheckDistance = 0.75f;
    [SerializeField] private LayerMask groundMask;

    [Header("Wall Run")]
    [SerializeField] private float wallCheckDistance = 1.0f;
    [SerializeField] private LayerMask wallMask;

    [Header("Sprint")]
    [SerializeField] private float staminaMax = 100f;
    [SerializeField] private float staminaDrainRate = 20f;
    [SerializeField] private float staminaRegenRate = 10f;

    // Accesores p�blicos para m�dulos
    public float MoveSpeed => moveSpeed;
    public float JumpForce => jumpForce;
    public float GroundCheckDistance => groundCheckDistance;
    public LayerMask GroundMask => groundMask;
    public float WallCheckDistance => wallCheckDistance;
    public LayerMask WallMask => wallMask;
    public float StaminaMax => staminaMax;
    public float StaminaDrainRate => staminaDrainRate;
    public float StaminaRegenRate => staminaRegenRate;

    public Rigidbody Rigidbody { get; private set; }
    public Transform Transform => transform;
    public Vector3 Direction => new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

    // M�dulos
    private PlayerMovement movement;
    private PlayerJump jump;
    private PlayerWallRun wallRun;

    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();

        movement = new PlayerMovement(this);
        jump = new PlayerJump(this);
        wallRun = new PlayerWallRun(this);
    }

    private void Update()
    {
        movement.Update();
        jump.Update();
        wallRun.Update();
    }

    private void FixedUpdate()
    {
        movement.FixedUpdate();
    }
    private void OnDrawGizmos()
    {
        // Dibujo del Ground Check
        Gizmos.color = Color.yellow;
        Vector3 groundOrigin = transform.position + Vector3.up * 0.125f;
        Gizmos.DrawRay(groundOrigin, Vector3.down * groundCheckDistance);

        // Dibujo del Wall Check en m�ltiples direcciones
        Gizmos.color = Color.cyan;
        Vector3[] directions = new Vector3[]
        {
        Vector3.forward, Vector3.back, Vector3.left, Vector3.right,
        (Vector3.forward + Vector3.right).normalized,
        (Vector3.forward + Vector3.left).normalized,
        (Vector3.back + Vector3.right).normalized,
        (Vector3.back + Vector3.left).normalized
        };

        foreach (var dir in directions)
        {
            Gizmos.DrawRay(transform.position, dir * wallCheckDistance);
        }
    }
}
