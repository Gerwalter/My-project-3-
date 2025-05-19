using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraController : MonoBehaviour
{
    [Header("<color=#6A89A7>Cursor</color>")]
    [SerializeField] public bool _isCameraFixed = false;

    [Header("<color=#6A89A7>Physics</color>")]
    [Range(.01f, 1f)][SerializeField] private float _detectionRadius = .1f;
    [SerializeField] private float _hitOffset = 0.25f;

    [Header("<color=#6A89A7>Settings</color>")]
    [Range(1f, 1000f)][SerializeField] private float _mouseSensitivity = 500f;
    [Range(.125f, 1f)][SerializeField] private float _minDistance = .25f;
    [Range(1f, 10f)][SerializeField] public float _maxDistance = 5f;
    [Range(-90f, 0f)][SerializeField] private float _minRotation = -45f;
    [Range(0f, 90f)][SerializeField] private float _maxRotation = 80f;

    [SerializeField] private bool _isCamBlocked = false;
    [SerializeField] private float _mouseX = 0.0f, _mouseY = 0.0f;
    [SerializeField] private Vector3 _dir = new(), _dirTest = new(), _camPos = new();

    [SerializeField] private Camera _cam;
    [SerializeField] private Transform _target;

    [SerializeField] private Ray _camRay;
    [SerializeField] private RaycastHit _camRayHit;

    [Header("<color=#6A89A7>UI Settings</color>")]
    [SerializeField] private float scroll;
    [Header("<color=#6A89A7>Layer Settings</color>")]
    [SerializeField] private LayerMask _ignoreLayerMask;
    public static CameraController Instance;
    public PauseManager pauseManager;
    [SerializeField] private Transform camTransform;

   

    public Transform CamTransform
    {
        get { return camTransform; }
        set { camTransform = value; }
    }

    private void Awake()
    {
        Instance = this;
        CamTransform = transform;
        _ignoreLayerMask = LayerMask.GetMask("Player");
    }

    private void Start()
    {
        InitializeCamera();
    }

    private void Update()
    {
        scroll = Input.GetAxisRaw("Mouse ScrollWheel");

        if (!_isCameraFixed)
        {
            UpdateCamRot(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
            if (scroll != 0f)
            {
                _maxDistance = Mathf.Clamp(_maxDistance - scroll, _minDistance + 2, 10f);
            }
        }
        _isCameraFixed = pauseManager.isPaused;
        ToggleCursorMode(_isCameraFixed);
    }

    private void FixedUpdate()
    {
        _camRay = new Ray(transform.position, _dir);
        _isCamBlocked = Physics.SphereCast(_camRay, _detectionRadius, out _camRayHit, _maxDistance, ~_ignoreLayerMask);
    }

    private void LateUpdate()
    {
        if (!_isCameraFixed)
        {
            UpdateSpringArm();
        }
    }

    
    private void UpdateCamRot(float x, float y)
    {
        transform.position = _target.position;

        if (x == 0.0f && y == 0.0f) return;

        if (x != 0.0f)
        {
            _mouseX += x * _mouseSensitivity * Time.deltaTime;
            if (_mouseX > 360.0f || _mouseX < -360.0f)
            {
                _mouseX -= 360.0f * Mathf.Sign(_mouseX);
            }
        }

        if (y != 0.0f)
        {
            _mouseY += y * _mouseSensitivity * Time.deltaTime;
            _mouseY = Mathf.Clamp(_mouseY, _minRotation, _maxRotation);
        }

        transform.rotation = Quaternion.Euler(-_mouseY, _mouseX, 0.0f);
    }

    private void UpdateSpringArm()
    {
        _dir = -transform.forward;

        if (_isCamBlocked)
        {
            _dirTest = (_camRayHit.point - transform.position) + (_camRayHit.normal * _hitOffset);
            _camPos = (_dirTest.sqrMagnitude <= _minDistance * _minDistance) ?
                        transform.position + _dir * _minDistance :
                        transform.position + _dirTest;
        }
        else
        {
            _camPos = transform.position + _dir * _maxDistance;
        }

        _cam.transform.position = _camPos;
        _cam.transform.LookAt(transform.position);
    }

    private void ToggleCursorMode(bool isFixed)
    {
        if (isFixed)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            LockCursor();
        }
    }

    private void LockCursor()
    {
        Cursor.lockState = pauseManager.isPaused ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = pauseManager.isPaused;
    }

    private void InitializeCamera()
    {
        _cam = Camera.main;
        LockCursor();
        transform.forward = _target.forward;
        _mouseX = transform.eulerAngles.y;
        _mouseY = transform.eulerAngles.x;
    }
}
