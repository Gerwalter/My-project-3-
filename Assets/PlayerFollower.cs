using System.Collections;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerFollower : HP
{
    [Header("<color=#6A89A7>Animation</color>")]
    [SerializeField] private string _isMovName = "isMoving";
    [SerializeField] private string _isGroundName = "isGrounded";
    [SerializeField] private string _jumpName = "onJump";
    [SerializeField] private Animator _anim;
    [SerializeField] private string _xName = "xAxis";
    [SerializeField] private string _zName = "zAxis";

    [Header("<color=#6A89A7>Target</color>")]
    [SerializeField] private Transform _target; // Objeto a seguir
    [SerializeField] private float _followSpeed = 3.5f;
    [SerializeField] private float _stoppingDistance = 1.0f;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float raycastDistance = 1.2f;
    [Header("<color=#6A89A7>UI</color>")]
    [SerializeField] private Image healthBar;

    [Header("<color=#6A89A7>Physics - Jumping</color>")]
   // [SerializeField] private float _jumpForce = 5.0f;
    [SerializeField] private float _jumpRayDist = 0.75f;
    [SerializeField] private LayerMask _jumpMask;
    private Rigidbody _rb;
    [SerializeField] private bool groundCheck;
    [SerializeField] private bool _freeze = false;
    [SerializeField] private bool _follow = false;

    public string MovName { get { return _isMovName; } }
    public string GroundName { get { return _isGroundName; } }
    public Animator Animator { get { return _anim; } }
    public string JumpName { get { return _jumpName; } }
    public string XName { get { return _xName; } }
    public float Speed { get { return _followSpeed; } }
    public bool Follow { get { return _follow; } set { _follow = value; } }
    public bool Freeze { get { return _freeze; } set { _freeze = value; } }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    private void Start()
    {
        _anim = GetComponentInChildren<Animator>();
        _rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        GetLife = maxLife;
        UpdateHealthBar();
    }

    private void Update()
    {
        if (_freeze) return;

        groundCheck = IsGrounded();
        _anim.SetBool(_isGroundName, groundCheck);

        if (_follow)
        {
            FollowPlayer();
            RotateTowardsPlayer();
        }
    }

    
    private void FollowPlayer()
    {
        if (_target == null) return;
        gameObject.layer = LayerMask.NameToLayer("Player");
        Vector3 direction = (_target.position - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, _target.position);

        if (distance > _stoppingDistance)
        {
            Vector3 moveDirection = GetSlopeDirection(direction) * _followSpeed;
            _rb.MovePosition(_rb.position + moveDirection * Time.deltaTime);
            _anim.SetBool(_isMovName, true);
            _anim.SetFloat(_zName, 1.0f);
        }
        else
        {
            _anim.SetBool(_isMovName, false);
            _anim.SetFloat(_xName, 0.0f);
            _anim.SetFloat(_zName, 0.0f);
        }
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit slopeHit, raycastDistance, groundMask))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle > 0f && angle < 45f;
        }
        return false;
    }

    private Vector3 GetSlopeDirection(Vector3 direction)
    {
        if (OnSlope())
        {
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit slopeHit, raycastDistance, groundMask))
            {
                return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
            }
        }
        return direction;
    }

    private void RotateTowardsPlayer()
    {
        if (_target == null) return;

        Vector3 direction = (_target.position - transform.position).normalized;
        direction.y = 0; // Evita inclinaciones innecesarias en el eje Y

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }

    /*
     
     
    private void FollowTarget()
    {
        if (_target == null) return;

        Vector3 direction = (_target.position - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, _target.position);

        if (distance > _stoppingDistance)
        {
            MoveTowards(direction);
            _anim.SetBool(_isMovName, true);
            _anim.SetFloat(_zName, 1.0f);
        }
        else
        {
            _anim.SetBool(_isMovName, false);
            _anim.SetFloat(_xName, 0.0f);
            _anim.SetFloat(_zName, 0.0f);
        }
    }

    private void MoveTowards(Vector3 direction)
    {
        direction.y = 0; // Mantener el movimiento en el plano horizontal
        _rb.MovePosition(transform.position + direction * _followSpeed * Time.deltaTime);
        transform.forward = direction;
    }


      
     */
    private void UpdateHealthBar()
    {
        float lifePercent = GetLife / maxLife;
        healthBar.fillAmount = lifePercent;
        healthBar.color = Color.Lerp(Color.red, Color.green, lifePercent);
    }

    public bool IsGrounded()
    {
        Vector3 jumpOffset = new Vector3(transform.position.x, transform.position.y + 0.126f, transform.position.z);
        Ray jumpRay = new Ray(jumpOffset, -transform.up);
        return Physics.Raycast(jumpRay, _jumpRayDist, _jumpMask);
    }

   

    public override void Health(float amount)
    {
        throw new System.NotImplementedException();
    }
}
