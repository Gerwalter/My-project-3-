using UnityEngine;
using UnityEngine.UI;

public class Player : HP
{
    [Header("<color=#6A89A7>Animation</color>")]
    [SerializeField] private string _isMovName = "isMoving";
    [SerializeField] private string _isGroundName = "isGrounded";
    [SerializeField] private string _jumpName = "onJump";
    [SerializeField] private string _xName = "xAxis";
    [SerializeField] private string _zName = "zAxis";

    [Header("<color=#6A89A7>Camera</color>")]
    [SerializeField] private Transform _camTarget;

    public Transform GetCamTarget { get { return _camTarget; } }

    [Header("<color=#6A89A7>Inputs</color>")]
    [SerializeField] private KeyCode _intKey = KeyCode.F;
    [SerializeField] private KeyCode _jumpKey = KeyCode.Space;

    [Header("<color=#6A89A7>UI</color>")]
    [SerializeField] private Image healthBar;

    [Header("<color=#6A89A7>Physics</color>")]
    [SerializeField] private Transform _intOrigin;
    [SerializeField] private float _intRayDist = 1.0f;
    [SerializeField] private LayerMask _intMask;
    [SerializeField] private float _jumpForce = 5.0f;
    [SerializeField] private float _jumpRayDist = 0.75f;
    [SerializeField] private LayerMask _jumpMask;
    [SerializeField] private float _movRayDist = 0.75f;
    [SerializeField] private LayerMask _movMask;
    [SerializeField] private float _movSpeed = 3.5f;
    [SerializeField] private Vector3 velocityToSet;
    [SerializeField] private float velocity;

    [Header("<color=yellow>Attack</color>")]
    [SerializeField] private Transform _atkOrigin;
    [SerializeField] private float _atkRayDist = 1.0f;
    [SerializeField] private LayerMask _atkMask;
    [SerializeField] private int _atkDmg = 20;
    private Ray _atkRay;
    private RaycastHit _atkHit;

    public Vector3 _camForwardFix = new(), _camRightFix = new(), _dir = new(), _jumpOffset = new(), _movRayDir = new();
    private Vector3 _dirFix = new();

    public Animator _anim;
    private Rigidbody _rb;
    protected Transform _camTransform;

    private Ray _intRay, _jumpRay, _movRay;
    private RaycastHit _intHit;
    public bool groundCheck;
    public Grappling grapple;

    // Nueva variable para desactivar el movimiento
    [SerializeField] public bool freeze = false;
    [SerializeField] public bool activeGrapple = false;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.constraints = RigidbodyConstraints.FreezeRotation;

        GameManager.Instance.Player = this;
    }

    private void Start()
    {
        _camTransform = Camera.main.transform;

        _anim = GetComponentInChildren<Animator>();

        GetLife = maxLife;
        UpdateHealthBar();
    }

    private void Update()
    {
        if (freeze)
        {
            _anim.SetFloat(_xName, 0.0f);
            _anim.SetFloat(_zName, 0.0f);

            return;
        }// Si el movimiento está desactivado, no hacer nada
        _dir.x = Input.GetAxisRaw("Horizontal");
        _dir.z = Input.GetAxisRaw("Vertical");

        if (!IsBlocked(_dir.x, _dir.z))
        {
            _anim.SetFloat(_xName, _dir.x);
            _anim.SetFloat(_zName, _dir.z);
        }
        else
        {
            _anim.SetFloat(_xName, 0.0f);
            _anim.SetFloat(_zName, 0.0f);
        }

        _anim.SetBool(_isGroundName, IsGrounded());
        _anim.SetBool(_isMovName, _dir.x != 0 || _dir.z != 0);

        if (Input.GetKeyDown(_intKey))
        {
            _anim.SetTrigger("Int");
        }

        if (Input.GetKeyDown(_jumpKey) && IsGrounded())
        {
            _anim.SetTrigger(_jumpName);
            Jump();
        }

        UpdateHealthBar();
        groundCheck = IsGrounded();
        ElementalCast();
    }

    private void FixedUpdate()
    {
        if (freeze) return; // Si el movimiento está desactivado, no hacer nada

        if ((_dir.x != 0.0f || _dir.z != 0.0f) && !IsBlocked(_dir.x, _dir.z))
        {
            Movement(_dir);
        }
    }

    public void DisableMovement()
    {
        freeze = true;
    }

    public void EnableMovement()
    {
        freeze = false;
    }

    private void UpdateHealthBar()
    {
        float lifePercent = GetLife / maxLife;
        healthBar.fillAmount = lifePercent;
        healthBar.color = Color.Lerp(Color.red, Color.green, lifePercent);
    }

    public void Interact()
    {
        _intRay = new Ray(_intOrigin.position, transform.forward);

        if (Physics.SphereCast(_intRay, .25f, out _intHit, _intRayDist, _intMask))
        {
            if (_intHit.collider.TryGetComponent<ButtonBehaviour>(out ButtonBehaviour intObj))
            {
                intObj.OnInteract();
            }
            else
            {
                return;
            }
        }
    }



    private void Jump()
    {
        _rb.AddForce(transform.up * _jumpForce, ForceMode.Impulse);
    }

    private void Movement(Vector3 dir)
    {
        _camForwardFix = _camTransform.forward;
        _camRightFix = _camTransform.right;

        _camForwardFix.y = 0.0f;
        _camRightFix.y = 0.0f;

        Rotate(_camForwardFix);

        _dirFix = (_camRightFix * dir.x + _camForwardFix * dir.z).normalized;

        _rb.MovePosition(transform.position + _dirFix * _movSpeed * Time.fixedDeltaTime);
    }

    private void Rotate(Vector3 dir)
    {
        transform.forward = dir;
    }

    public bool EnableMovementAfterCollision = true;

    public bool IsGrounded()
    {
        _jumpOffset = new Vector3(transform.position.x, transform.position.y + 0.125f, transform.position.z);

        _jumpRay = new Ray(_jumpOffset, -transform.up);

        return Physics.Raycast(_jumpRay, _jumpRayDist, _jumpMask);
    }

    private bool IsBlocked(float x, float z)
    {
        _movRayDir = (transform.right * x + transform.forward * z);

        _movRay = new Ray(transform.position, _movRayDir);

        return Physics.Raycast(_movRay, _movRayDist, _movMask);
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(_jumpRay);

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(_intRay);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(_atkRay);
    }
    public void Attack()
    {
        _atkRay = new Ray(_atkOrigin.position, transform.forward);

        if (Physics.Raycast(_atkRay, out _atkHit, _atkRayDist, _atkMask))
        {
            if (_atkHit.collider.TryGetComponent<Enemy>(out Enemy enemy))
            {
                enemy.ReciveDamage(_atkDmg);
            }
        }
    }

    public void PerformLiftAttack()
    {
        _atkRay = new Ray(_atkOrigin.position, transform.forward);

        if (Physics.Raycast(_atkRay, out _atkHit, _atkRayDist, _atkMask))
        {
            if (_atkHit.collider.TryGetComponent<Enemy>(out Enemy enemy))
            {
                enemy.ReciveDamage(_atkDmg);
                //enemy.ApplyLiftImpulse();
            }
        }
    }

    public void Cast()
    {
        _anim.SetTrigger("Cast");
    }

    [SerializeField] private GameObject gameObje;
    [SerializeField] private Lock Handle;

    public void Die()
    {
        Handle.OnDie();
        gameObje.SetActive(false);
        // Rend1.enabled = false; Rend2.enabled = false;
    }

    public Vector3 CalculateJumpVelocity(Vector3 startpoint, Vector3 endpoint, float trayectoryHeight)
    {
        float gravity = Physics.gravity.y;
        float displacementY = endpoint.y - startpoint.y;
        Vector3 displacementZX = new Vector3(endpoint.x - startpoint.x, 0f, endpoint.z - startpoint.z);

        Vector3 velocity = Vector3.up * Mathf.Sqrt(-2 * gravity * trayectoryHeight);
        Vector3 velocityZX = displacementZX / (Mathf.Sqrt(-2 * trayectoryHeight / gravity)
            + Mathf.Sqrt(2 * (displacementY - trayectoryHeight) / gravity));

        return velocity + velocityZX;
    }

    public void JumpToPosition(Vector3 targetposition, float trajectoryHeight)
    {
        activeGrapple = true;
        velocityToSet = CalculateJumpVelocity(transform.position, targetposition, trajectoryHeight);
        Invoke(nameof(Setvelocity), 0.1f);
    }


    public void ResetRestrictions()
    {
        activeGrapple = false;
    }

    private void Setvelocity()
    {
        EnableMovementAfterCollision = true;
        _rb.velocity = velocityToSet * velocity;
    }

    public void PrintNum(float num)
    {
        print(num);
    }

    public void MovePlayer(float force)
    {
        Vector3 forwardDirection = transform.forward; // Dirección actual del jugador
        _rb.AddForce(forwardDirection * force, ForceMode.Impulse);
    }

    public void ApplyForwardJumpImpulse(float forwardForce, float jumpForce)
    {
        // Dirección hacia adelante basada en la orientación actual del jugador
        Vector3 forwardDirection = transform.forward * forwardForce;

        // Impulso en el eje Y para el salto
        Vector3 upwardImpulse = Vector3.up * jumpForce;

        // Aplica ambas fuerzas al Rigidbody
        _rb.AddForce(forwardDirection + upwardImpulse, ForceMode.Impulse);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (EnableMovementAfterCollision)
        {
            EnableMovementAfterCollision = false;
            ResetRestrictions();

            grapple.stopGrapple();
        }
    }

    [SerializeField] private ElementType selectedElement;
    private enum ElementType
    {
        Normal,
        Fire,
        Electric,
        Ice,
        Water,
    }
    public void ElementalCast()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            _anim.SetTrigger("Cast");
            selectedElement = (ElementType)Random.Range(0, System.Enum.GetValues(typeof(ElementType)).Length);
            Debug.Log("Elemento seleccionado: " + selectedElement);
        }
    }
}