using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class PlayerMovement : Player
{
    [Header("<color=#6A89A7>Animation</color>")]
    [SerializeField] private string _isMovName = "isMoving";
    [SerializeField] private string _isGroundName = "isGrounded";
    [SerializeField] private string _jumpName = "onJump";
    [SerializeField] private string _xName = "xAxis";
    [SerializeField] private string _zName = "zAxis";
    [SerializeField] private string _sprint = "Sprinting";
    [Header("<color=#6A89A7>Camera</color>")]
    [SerializeField] private Transform _camTarget;
    public Transform GetCamTarget { get { return _camTarget; } }
    public Vector3 _camForwardFix = new(), _camRightFix = new();
    protected Transform _camTransform;

    [Header("<color=#6A89A7>Inputs</color>")]
    private Rigidbody _rb;
    [SerializeField] private KeyCode _jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode _sprintKey = KeyCode.LeftShift;  // Tecla para correr

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
    [SerializeField] private Image _staminaBar;



    [Header("<color=#6A89A6>Physics - Speed</color>")]
    [SerializeField] private float originalSpeed;
    [SerializeField] private float speedMultiplier = 2.0f;
    [SerializeField] private float duration = 5.0f;

    public bool groundCheck;



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
        _rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        _wallHitStatus = new bool[_wallCheckDirections.Length];
        _currentStamina = _staminaMax;
        UpdateStaminaUI();
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


        if (Input.GetKeyDown(_jumpKey) && IsGrounded())
        {
            _anim.SetTrigger(_jumpName);
        }
        else if (Input.GetKeyDown(_jumpKey) && _isWallDetected)
        {
            Jump();
        }
        groundCheck = IsGrounded();
        DetectWall();

        UpdateWallCheck();
        HandleSprint();
        UpdateStaminaUI();
    }

    private void FixedUpdate()
    {
        if (freeze) return; // Si el movimiento est� desactivado, no hacer nada

        if ((_dir.x != 0.0f || _dir.z != 0.0f) && !IsBlocked(_dir.x, _dir.z))
        {
            Movement(_dir);
        }

        if (_isSprinting)
        {
            PreventWallClipping();
        }

    }
    public override void Jump()
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

            _rb.AddForce((wallNormal + Vector3.up).normalized * _jumpForce, ForceMode.Impulse);
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

        // Aplicamos la velocidad normal o con sprint
        float speed = _isSprinting ? _movSpeed * _sprintMultiplier : _movSpeed;
        if (OnSlope()) // Si est� en una pendiente, ajusta el movimiento
        {
            _dirFix = GetSlopeDirection(_dirFix);
        }

        _rb.MovePosition(transform.position + _dirFix * speed * Time.fixedDeltaTime);
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

    [Header("<color=yellow>Sprint</color>")]
    [SerializeField] private float _sprintMultiplier = 2.0f;  // Multiplicador de velocidad al esprintar
    [SerializeField] private float _staminaMax = 100f;  // M�xima cantidad de stamina
    [SerializeField] private float _staminaDrainRate = 20f;  // Cu�nta stamina se gasta por segundo corriendo
    [SerializeField] private float _staminaRegenRate = 10f;  // Cu�nta stamina se regenera por segundo
    [SerializeField] private float _sprintWallCheckDistance = 0.6f;
    [SerializeField] private LayerMask _wallLayer;
    [SerializeField] private float _currentStamina;
    private bool _isSprinting = false;

    private void HandleSprint()
    {
        bool isMoving = _dir.x != 0 || _dir.z != 0;

        if (Input.GetKey(_sprintKey) && _currentStamina > 0 && isMoving)
        {
            if (!IsNearWall())
            {
                _isSprinting = true;
                _anim.SetFloat(_sprint, 0f);
                _currentStamina -= _staminaDrainRate * Time.deltaTime;
            }
        }
        else
        {
            _isSprinting = false;
            _anim.SetFloat(_sprint, 1f);
            RegenerateStamina();
        }

        _currentStamina = Mathf.Clamp(_currentStamina, 0, _staminaMax);
    }
    private bool IsNearWall()
    {
        RaycastHit hit;
        return Physics.Raycast(transform.position, transform.forward, out hit, _sprintWallCheckDistance, _wallLayer);
    }

    private void PreventWallClipping()
    {
        Vector3 moveDirection = transform.forward; // Direcci�n en la que se mueve el jugador
        RaycastHit hit;

        if (Physics.Raycast(transform.position, moveDirection, out hit, _sprintWallCheckDistance, _wallLayer))
        {
            // Si hay una pared cerca, reducimos la velocidad para evitar atravesarla
            _isSprinting = false; // Detenemos el sprint si est� muy cerca de una pared
        }
    }
    // Regeneraci�n de Stamina
    private void RegenerateStamina()
    {
        // Si el jugador NO est� corriendo, regenera stamina
        float regenRate = (_dir.x == 0 && _dir.z == 0) ? _staminaRegenRate * 1.5f : _staminaRegenRate;
        _currentStamina += regenRate * Time.deltaTime;
    }
    private void UpdateStaminaUI()
    {
        if (_staminaBar != null)
        {
            float fillAmount = _currentStamina / _staminaMax;
            _staminaBar.fillAmount = fillAmount;

            // Lerp de verde (stamina llena) a rojo (stamina vac�a)
            _staminaBar.color = Color.Lerp(Color.red, Color.green, fillAmount);
            _staminaBar.gameObject.SetActive(fillAmount < 1.0f);
        }
    }

    [Header("<color=#6A89A7>Wall Running</color>")]
    [SerializeField] private float _wallCheckDistance = 1.0f; // Distancia de los Raycasts
    [SerializeField] private LayerMask _wallMask;             // Capa de detecci�n de paredes
    private bool _isWallDetected = false;
    private bool _isWallRunning = false;
    private bool[] _wallHitStatus;
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
        if (Application.isPlaying)
        {
            for (int i = 0; i < _wallCheckDirections.Length; i++)
            {
                Gizmos.color = _wallHitStatus != null && _wallHitStatus[i] ? Color.green : Color.red;
                Gizmos.DrawRay(transform.position, _wallCheckDirections[i] * _wallCheckDistance);
            }
        }
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(_jumpRay);

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


}


