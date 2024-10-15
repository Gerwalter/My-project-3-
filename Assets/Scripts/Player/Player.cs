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

    public Vector3 _camForwardFix = new(), _camRightFix = new(), _dir = new(), _jumpOffset = new(), _movRayDir = new();
    private Vector3 _dirFix = new();

    private Animator _anim;
    private Rigidbody _rb;
    protected Transform _camTransform;


    private Ray _intRay, _jumpRay, _movRay;
    private RaycastHit _intHit;

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
            //Interact();
        }

        if (Input.GetKeyDown(_jumpKey) && IsGrounded())
        {
            _anim.SetTrigger(_jumpName);
            Jump();
        }

        UpdateHealthBar();
    }

    private void FixedUpdate()
    {
        if ((_dir.x != 0.0f || _dir.z != 0.0f) && !IsBlocked(_dir.x, _dir.z))
        {
            Movement(_dir);
        }
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

        _dirFix = (_camRightFix * dir.x + _camForwardFix * dir.z);

        _rb.MovePosition(transform.position + _dirFix * _movSpeed * Time.fixedDeltaTime);

    }

    private void Rotate(Vector3 dir)
    {
        transform.forward = dir;
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

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(_intRay);
    }


    [SerializeField] private GameObject gameObje;
    [SerializeField] private Lock Handle;

    public void Die()
    { 
        Handle.OnDie();
        gameObje.SetActive(false);
       // Rend1.enabled = false; Rend2.enabled = false;
    }
}
