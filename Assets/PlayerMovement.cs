using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerMovement : MonoBehaviour, IObservable
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
    [SerializeField] private float _sprintWallCheckDistance = 0.6f;
    [SerializeField] private LayerMask _wallLayer;
    //   [SerializeField] private Image _staminaBar;

    [SerializeField] List<IObserver> _observers = new List<IObserver>();


    [Header("<color=#6A89A6>Physics - Speed</color>")]
    [SerializeField] private float originalSpeed;
//    [SerializeField] private float speedMultiplier = 2.0f;
//    [SerializeField] private float duration = 5.0f;

    public bool groundCheck;
    public bool freeze;

    

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.constraints = RigidbodyConstraints.FreezeRotation;

        // GameManager.Instance.Player = this;

    }
    public void Subscribe(IObserver x)
    {
        if (_observers.Contains(x)) return;

        _observers.Add(x);
    }

    public void Unsubscribe(IObserver x)
    {
        if (_observers.Contains(x))
            _observers.Remove(x);
    }
    private void Start()
    {
        _camTransform = Camera.main.transform;
        _rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        _wallHitStatus = new bool[_wallCheckDirections.Length];
        EventManager.Subscribe("OnDisableMovement", Disable);
        EventManager.Subscribe("OnEnableMovement", Enable);
        EventManager.Subscribe("OnJump", OnJump);
    }

    void Disable(params object[] args)
    {
        DisableMovement();
    }
    void Enable(params object[] args)
    {
        EnableMovement();
    }
    void OnJump(params object[] args)
    {
        Jump();
    }
    public void DisableMovement()
    {
        freeze = true;
    }

    public void EnableMovement()
    {
        freeze = false;
    }
    private void Update()
    {
        if (freeze)
        {
            EventManager.Trigger("Float", _zName, 0.0f);
            EventManager.Trigger("Float", _xName, 0.0f);
            return;
        }
        _dir.x = Input.GetAxisRaw("Horizontal");
        _dir.z = Input.GetAxisRaw("Vertical");

        if (!IsBlocked(_dir.x, _dir.z))
        {
            EventManager.Trigger("Float", _xName, _dir.x);
            EventManager.Trigger("Float", _zName, _dir.z);
        }
        else
        {
            EventManager.Trigger("Float", _zName, 0.0f);
            EventManager.Trigger("Float", _xName, 0.0f);
        }

        EventManager.Trigger("Bool", _isGroundName, IsGrounded()); 
        EventManager.Trigger("Bool", _isMovName, _dir.x != 0 || _dir.z != 0); 


        if (Input.GetKeyDown(_jumpKey) && IsGrounded())
        {
           EventManager.Trigger("Input", _jumpName);
        }
        else if (Input.GetKeyDown(_jumpKey) && _isWallDetected)
        {
            Jump();
        }
        groundCheck = IsGrounded();
        DetectWall();

        UpdateWallCheck();
    }

    private void FixedUpdate()
    {
        if (freeze) return; // Si el movimiento está desactivado, no hacer nada

        if ((_dir.x != 0.0f || _dir.z != 0.0f) && !IsBlocked(_dir.x, _dir.z))
        {
            Movement(_dir);
        }
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
                    wallNormal += -dir; // Sumamos la dirección opuesta a la detección
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
        float speed = _movSpeed;
        if (OnSlope()) // Si está en una pendiente, ajusta el movimiento
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

        // Ignorar el suelo en esta comprobación para evitar interferencias con OnSlope()
        int layerMask2 = _movMask & ~_jumpMask; // Excluir la capa de suelo

        return Physics.Raycast(_movRay, _movRayDist, layerMask2);
    }
/*
    [Header("<color=yellow>Sprint</color>")]
    [SerializeField] private float _sprintMultiplier = 2.0f;  // Multiplicador de velocidad al esprintar
    [SerializeField] private float _staminaMax = 100f;  // Máxima cantidad de stamina
    [SerializeField] private float _staminaDrainRate = 20f;  // Cuánta stamina se gasta por segundo corriendo
    [SerializeField] private float _staminaRegenRate = 10f;  // Cuánta stamina se regenera por segundo


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
                foreach (var observer in _observers)
                    observer.Notify(_currentStamina, _staminaMax);
            }
        }
        else
        {
            _isSprinting = false;
            _anim.SetFloat(_sprint, 1f);
            RegenerateStamina();
        }

        _currentStamina = Mathf.Clamp(_currentStamina, 0, _staminaMax);
    }*/
/*
    // Regeneración de Stamina
    private void RegenerateStamina()
    {
        // Si el jugador NO está corriendo, regenera stamina
        float regenRate = (_dir.x == 0 && _dir.z == 0) ? _staminaRegenRate * 1.5f : _staminaRegenRate;
        _currentStamina += regenRate * Time.deltaTime;
        foreach (var observer in _observers)
            observer.Notify(_currentStamina, _staminaMax);
    }
    
    */

    [Header("<color=#6A89A7>Wall Running</color>")]
    [SerializeField] private float _wallCheckDistance = 1.0f; // Distancia de los Raycasts
    [SerializeField] private LayerMask _wallMask;             // Capa de detección de paredes
    private bool _isWallDetected = false;
    private bool _isWallRunning = false;
    private bool[] _wallHitStatus;
    // Direcciones de los Raycasts en un círculo
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
            if (Physics.Raycast((transform.position), dir, _wallCheckDistance, _wallMask))
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
            if (Physics.Raycast((transform.position), _wallCheckDirections[i], out RaycastHit hit, _wallCheckDistance, _wallMask))
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
            Gizmos.DrawLine((transform.position), (transform.position) + dir * _wallCheckDistance);
        }
        if (_wallCheckDirections == null) return;
        if (Application.isPlaying)
        {
            for (int i = 0; i < _wallCheckDirections.Length; i++)
            {
                Gizmos.color = _wallHitStatus != null && _wallHitStatus[i] ? Color.green : Color.red;
                Gizmos.DrawRay((transform.position), _wallCheckDirections[i] * _wallCheckDistance);
            }
        }
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(_jumpRay);

    }
}


