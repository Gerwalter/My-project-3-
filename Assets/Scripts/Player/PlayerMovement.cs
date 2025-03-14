using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("<color=#6A89A7>Camera</color>")]
    [SerializeField] private Transform _camTarget;
    public Transform GetCamTarget { get { return _camTarget; } }

    [Header("<color=#6A89A7>Inputs</color>")]
    [SerializeField] private KeyCode _jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode _dashKey = KeyCode.LeftShift;

    [Header("<color=#6A89A7>Physics</color>")]
    [SerializeField] private float _jumpForce = 5.0f;
    [SerializeField] private float _jumpRayDist = 0.75f;
    [SerializeField] private LayerMask _jumpMask;
    [SerializeField] private float _movRayDist = 0.75f;
    [SerializeField] private LayerMask _movMask;
    [SerializeField] public float _movSpeed = 3.5f;
    [SerializeField] private float _dashSpeed = 10f;
    [SerializeField] private float _dashDuration = 0.2f;
    [SerializeField] private float _dashCooldown = 1f;

    private Vector3 _camForwardFix = new(), _camRightFix = new(), _dir = new(), _jumpOffset = new(), _movRayDir = new();
    private Vector3 _dirFix = new();

    private Rigidbody _rb;
    private Transform _camTransform;
    private bool _isDashing = false;
    private float _lastDashTime = -Mathf.Infinity;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    private void Start()
    {
        _camTransform = Camera.main.transform;
    }

    private void Update()
    {
        _dir.x = Input.GetAxisRaw("Horizontal");
        _dir.z = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(_jumpKey) && IsGrounded())
        {
            Jump();
        }

        if (Input.GetKeyDown(_dashKey) && Time.time >= _lastDashTime + _dashCooldown)
        {
            StartCoroutine(Dash());
        }
    }

    private void FixedUpdate()
    {
        if (!_isDashing && (_dir.x != 0.0f || _dir.z != 0.0f) && !IsBlocked(_dir.x, _dir.z))
        {
            Movement(_dir);
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

    private bool IsGrounded()
    {
        _jumpOffset = new Vector3(transform.position.x, transform.position.y + 0.125f, transform.position.z);
        Ray _jumpRay = new Ray(_jumpOffset, -transform.up);
        return Physics.Raycast(_jumpRay, _jumpRayDist, _jumpMask);
    }

    private bool IsBlocked(float x, float z)
    {
        _movRayDir = (transform.right * x + transform.forward * z);
        Ray _movRay = new Ray(transform.position, _movRayDir);
        return Physics.Raycast(_movRay, _movRayDist, _movMask);
    }

    private IEnumerator Dash()
    {
        _isDashing = true;
        _lastDashTime = Time.time;

        Vector3 dashDirection = transform.forward;
        float startTime = Time.time;

        while (Time.time < startTime + _dashDuration)
        {
            _rb.velocity = dashDirection * _dashSpeed;
            yield return null;
        }

        _rb.velocity = Vector3.zero;
        _isDashing = false;
    }
}
