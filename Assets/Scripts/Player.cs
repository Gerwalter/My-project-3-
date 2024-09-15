using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{

    [Header("<color=#6A89A7>Animation</color>")]
    [SerializeField] private string _isMovName = "isMoving";
    [SerializeField] private string _isGroundName = "isGrounded";
    [SerializeField] private string _jumpName = "onJump";
    [SerializeField] private string _xName = "xAxis";
    [SerializeField] private string _zName = "zAxis";

    [Header("<color=#6A89A7>Inputs</color>")]
    [SerializeField] private KeyCode _jumpKey = KeyCode.Space;

    [Header("<color=#6A89A7>Physics</color>")]
    [SerializeField] private float _jumpForce = 5.0f;
    [SerializeField] private float _jumpRayDist = 0.75f;
    [SerializeField] private LayerMask _jumpMask;
    [SerializeField] private float _movRayDist = 0.75f;
    [SerializeField] private LayerMask _movMask;
    [SerializeField] private float _movSpeed = 3.5f;

    private bool _isOnAir = false;
    private float _xAxis = 0f, _zAxis = 0f;
    private Vector3 _dir = new(), _jumpOffset = new(), _movRayDir = new();

    private Animator _anim;
    private Rigidbody _rb;

    private Ray _jumpRay, _intRay, _movRay;
    private RaycastHit _intHit;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.constraints = RigidbodyConstraints.FreezeRotation;

        //_rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        //_rb.angularDrag = 1f;        
    }

    private void Start()
    {
        GameManager.Instance.Player = this;

        _anim = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        _xAxis = Input.GetAxis("Horizontal");
        _zAxis = Input.GetAxis("Vertical");

        if (!IsBlocked(_xAxis, _zAxis))
        {
            _anim.SetFloat(_xName, _xAxis);
            _anim.SetFloat(_zName, _zAxis);
        }
        else
        {
            _anim.SetFloat(_xName, 0.0f);
            _anim.SetFloat(_zName, 0.0f);
        }

        _anim.SetBool(_isGroundName, IsGrounded());

        _anim.SetBool(_isMovName, _xAxis != 0 || _zAxis != 0);


        if (Input.GetKeyDown(_jumpKey) && IsGrounded())
        {
            _anim.SetTrigger(_jumpName);
            Jump();
        }
    }

    private void FixedUpdate()
    {
        if ((_xAxis != 0.0f || _zAxis != 0.0f) && !IsBlocked(_xAxis, _zAxis))
        {
            Movement(_xAxis, _zAxis);
        }
    }

    private void Jump()
    {
        _rb.AddForce(transform.up * _jumpForce, ForceMode.Impulse);
    }

    private void Movement(float x, float z)
    {
        _dir = (transform.right * x + transform.forward * z).normalized;

        _rb.MovePosition(transform.position + _dir * _movSpeed * Time.fixedDeltaTime);
    }

    private bool IsGrounded()
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
    }
}
