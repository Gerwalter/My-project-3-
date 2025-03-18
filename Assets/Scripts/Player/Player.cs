using System.Collections;
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
    public Animator _anim;

    [Header("<color=#6A89A7>Camera</color>")]
    [SerializeField] private Transform _camTarget;

    public Transform GetCamTarget { get { return _camTarget; } }
    public Vector3 _camForwardFix = new(), _camRightFix = new();
    protected Transform _camTransform;

    [Header("<color=#6A89A7>Inputs</color>")]
    [SerializeField] private KeyCode _intKey = KeyCode.F;
    [SerializeField] private KeyCode _jumpKey = KeyCode.Space;

    [Header("<color=#6A89A7>UI</color>")]
    [SerializeField] private Image healthBar;


    [Header("<color=#6A89A7>Physics - Interaction</color>")]
    [SerializeField] private Transform _intOrigin;
    [SerializeField] private float _intRayDist = 1.0f;
    [SerializeField] private LayerMask _intMask;
    [SerializeField] private Ray _intRay;
    private RaycastHit _intHit;
    public float _sphereIntRadius = 0.5f; // Radio de la esfera

    [Header("<color=#6A89A7>Physics - Jumping</color>")]
    [SerializeField] private float _jumpForce = 5.0f;
    [SerializeField] private float _jumpRayDist = 0.75f;
    [SerializeField] private LayerMask _jumpMask;
    public Vector3 _jumpOffset = new();
    [SerializeField] private Ray _jumpRay;

    [Header("<color=#6A89A7>Physics - Movement</color>")]
    [SerializeField] private float _movRayDist = 0.75f;
    [SerializeField] private LayerMask _movMask;
    [SerializeField] private float _movSpeed = 3.5f;
    public Vector3 _dir = new(), _movRayDir = new();
    private Vector3 _dirFix = new();
    [SerializeField] private Ray _movRay;




    [Header("<color=#6A89A6>Physics - Speed</color>")]
    [SerializeField] private float originalSpeed;
    [SerializeField] private float speedMultiplier = 2.0f;
    [SerializeField] private float duration = 5.0f;
    private Rigidbody _rb;
    public bool groundCheck;
    [SerializeField] public bool freeze = false;


    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.constraints = RigidbodyConstraints.FreezeRotation;

        // GameManager.Instance.Player = this;

    }

    private void Start()
    {
        _camTransform = Camera.main.transform;
        _anim = GetComponentInChildren<Animator>();

        GetLife = maxLife;
        UpdateHealthBar();
        _wallHitStatus = new bool[_wallCheckDirections.Length];
    }
    private void Update()
    {
        if (freeze)
        {
            _anim.SetFloat(_xName, 0.0f);
            _anim.SetFloat(_zName, 0.0f);

            return;
        }
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
        }
        else if (Input.GetKeyDown(_jumpKey) && _isWallDetected)
        {
            Jump();
        }

        UpdateHealthBar();
        groundCheck = IsGrounded();
        DetectWall();

        if (_isWallRunning && Input.GetKeyDown(KeyCode.G))
        {
            StopWallRun();
        }

        if (groundCheck)
        {
            _wallDetectionTimer = 0f;
        }
        else
        {
            // Si no est� en el suelo, sumamos el tiempo transcurrido
            _wallDetectionTimer += Time.deltaTime;
        }

        if (_wallDetectionTimer >= _wallDetectionDelay)
        {
            UpdateWallCheck(); // Actualiza la detecci�n de la pared
        }

    }

   


    private void FixedUpdate()
    {
        if (freeze) return; // Si el movimiento est� desactivado, no hacer nada

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
    public void Jump()
    {
        if (_isWallRunning)
        {
            Vector3 wallNormal = Vector3.zero;
            int wallCount = 0;

            foreach (Vector3 dir in _wallCheckDirections)
            {
                if (Physics.Raycast(transform.position, dir, _wallCheckDistance, _wallMask))
                {
                    wallNormal += -dir; // Sumamos la direcci�n opuesta a la detecci�n
                    wallCount++;
                }
            }

            if (wallCount > 0)
            {
                wallNormal /= wallCount; // Promediamos si hay varias colisiones
                wallNormal.Normalize(); // Normalizamos para evitar fuerzas exageradas
            }

            _rb.AddForce((wallNormal + Vector3.up).normalized * _jumpForce , ForceMode.Impulse);
        }
        else
        {
            _rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
        }
    }
    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit slopeHit, _jumpRayDist + 0.1f, _jumpMask))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle > 0f && angle < 45f; // Considera pendientes menores de 45 grados como transitables
        }
        return false;
    }

    private Vector3 GetSlopeDirection(Vector3 direction)
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit slopeHit, _jumpRayDist + 0.1f, _jumpMask))
        {
            return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
        }
        return direction;
    }

    private void Movement(Vector3 dir)
    {
        _camForwardFix = _camTransform.forward;
        _camRightFix = _camTransform.right;
        _camForwardFix.y = 0.0f;
        _camRightFix.y = 0.0f;
        Rotate(_camForwardFix);

        _dirFix = (_camRightFix * dir.x + _camForwardFix * dir.z).normalized;

        if (OnSlope()) // Si est� en una pendiente, ajusta el movimiento
        {
            _dirFix = GetSlopeDirection(_dirFix);
        }

        _rb.MovePosition(transform.position + _dirFix * _movSpeed * Time.fixedDeltaTime);
    }

    private void Rotate(Vector3 dir)
    {
        transform.forward = dir;
    }

    public bool IsGrounded()
    {
        _jumpOffset = new Vector3(transform.position.x, transform.position.y + 0.125f, transform.position.z);

        _jumpRay = new Ray(_jumpOffset, -transform.up);

        return Physics.Raycast(_jumpRay, _jumpRayDist, _jumpMask);
    }

    private bool IsBlocked(float x, float z)
    {
        _movRayDir = (transform.right * x + transform.forward * z).normalized;
        _movRay = new Ray(transform.position, _movRayDir);

        // Ignorar el suelo en esta comprobaci�n para evitar interferencias con OnSlope()
        int layerMask2 = _movMask & ~_jumpMask; // Excluir la capa de suelo

        return Physics.Raycast(_movRay, _movRayDist, layerMask2);
    }
    public void Interact()
    {
        _intRay = new Ray(_intOrigin.position, transform.forward);

        if (Physics.SphereCast(_intRay, _sphereIntRadius, out _intHit, _intRayDist, _intMask))
        {
            if (_intHit.collider.TryGetComponent<ButtonBehaviour>(out ButtonBehaviour intObj))
            {
                intObj.OnInteract();
            }
        }
    }

    [Header("<color=#6A89A7>Wall Running</color>")]
    [SerializeField] private float _wallCheckDistance = 1.0f; // Distancia de los Raycasts
    [SerializeField] private LayerMask _wallMask;             // Capa de detecci�n de paredes
    private bool _isWallDetected = false;
    private bool _isWallRunning = false;
    [SerializeField] private bool[] _wallHitStatus;
    // Direcciones de los Raycasts en un c�rculo
    [SerializeField]
    private Vector3[] _wallCheckDirections = new Vector3[]
    {
    Vector3.forward, Vector3.back, Vector3.right, Vector3.left,
    (Vector3.forward + Vector3.right).normalized,
    (Vector3.forward + Vector3.left).normalized,
    (Vector3.back + Vector3.right).normalized,
    (Vector3.back + Vector3.left).normalized
    };
    [SerializeField] private float climbSpeed = 3.0f; // Velocidad de subida en la pared
    [SerializeField] private bool isClimbing = false; // Indica si el jugador est� escalando
    [SerializeField] private float _wallDetectionDelay = 1.0f; // Tiempo de retraso en segundos
    private float _wallDetectionTimer = 0f; // Temporizador para la detecci�n
    private void DetectWall()
    {
        _isWallDetected = false;

        foreach (Vector3 dir in _wallCheckDirections)
        {
            if (Physics.Raycast(transform.position, dir, _wallCheckDistance, _wallMask))
            {
                _isWallDetected = true;
                break; // Si detectamos una pared, no hace falta seguir verificando
            }
        }

        if (!IsGrounded() && _isWallDetected)
        {
            StartWallRun();
        }
        else if (_isWallRunning && (!_isWallDetected || IsGrounded()))
        {
            StopWallRun();
        }
    }
    private void UpdateWallCheck()
    {
        for (int i = 0; i < _wallCheckDirections.Length; i++)
        {
            if (Physics.Raycast(transform.position, _wallCheckDirections[i], out RaycastHit hit, _wallCheckDistance, _wallMask))
            {
                _wallHitStatus[i] = true;
            }
            else
            {
                _wallHitStatus[i] = false;
            }
        }
    }

    private void StartWallRun()
    {
        if (_isWallRunning) return;

        _isWallRunning = true;
        _rb.useGravity = false;
        _rb.velocity = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);
        Debug.Log("Wall Run Activado!");
    }

    private void StopWallRun()
    {
        if (!_isWallRunning) return;

        _isWallRunning = false;
        _rb.useGravity = true;
        Debug.Log("Wall Run Desactivado.");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = (!_isWallDetected || IsGrounded()) ? Color.red : Color.green;

        foreach (Vector3 dir in _wallCheckDirections)
        {
            Gizmos.DrawLine(transform.position, transform.position + dir * _wallCheckDistance);
        }
        if (_wallCheckDirections == null) return;

        for (int i = 0; i < _wallCheckDirections.Length; i++)
        {
            Gizmos.color = _wallHitStatus != null && _wallHitStatus[i] ? Color.green : Color.red;
            Gizmos.DrawRay(transform.position, _wallCheckDirections[i] * _wallCheckDistance);
        }
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(_jumpRay);

        // Dibuja la l�nea del SphereCast
        Gizmos.color = Color.yellow; // Color del gizmo
        Gizmos.DrawLine(_intOrigin.position, _intOrigin.position + transform.forward * _intRayDist);

        // Dibuja la esfera al final del SphereCast
        Gizmos.color = Color.cyan; // Color de la esfera
        Gizmos.DrawWireSphere(_intRay.origin + _intRay.direction * _intRayDist, _sphereIntRadius);
    }
    private IEnumerator ApplySpeedBoost()
    {
        originalSpeed = _movSpeed; // Guarda la velocidad original
        _movSpeed *= speedMultiplier; // Aplica el multiplicador

        yield return new WaitForSeconds(duration); // Espera el tiempo de duraci�n

        _movSpeed = originalSpeed; // Restaura la velocidad original
    }
    public void SpeedBooster()
    {
        StartCoroutine(ApplySpeedBoost());
    }
    public void Die()
    {
        //Handle.OnDie();
        freeze = true;
        gameObje.SetActive(false);

        // Rend1.enabled = false; Rend2.enabled = false;
    }
    [SerializeField] private GameObject gameObje;
    private bool isDead = false;
    public override void ReciveDamage(float damage)
    {
        GetLife -= damage;

        if (isDead) return; // Si ya est� muerto, no hacer nada

        if (GetLife <= 0)
        {
            if (_anim != null)
            {
                _anim.SetTrigger("Die");
            }
            isDead = true; // Marcar como muerto
        }
        else
        {
            if (_anim != null)
            {
                _anim.SetTrigger("Hit");
                // _bloodVFX.SendEvent("OnTakeDamage");
            }
        }
    }

    public void AnimationMoveImpulse(float force)
    {
        Vector3 forwardDirection = transform.forward; // Direcci�n actual del jugador
        _rb.AddForce(forwardDirection * force, ForceMode.Impulse);
    }

    public void ApplyForwardJumpImpulse(float forwardForce, float jumpForce)
    {
        // Direcci�n hacia adelante basada en la orientaci�n actual del jugador
        Vector3 forwardDirection = transform.forward * forwardForce;

        // Impulso en el eje Y para el salto
        Vector3 upwardImpulse = Vector3.up * jumpForce;

        // Aplica ambas fuerzas al Rigidbody
        _rb.AddForce(forwardDirection + upwardImpulse, ForceMode.Impulse);
    }
    public override void Health(float amount)
    {
        GetLife += amount;
    }
}
