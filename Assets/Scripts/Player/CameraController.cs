using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("<color=#6A89A7>Cursor</color>")]
    [SerializeField] private CursorLockMode _lockMode = CursorLockMode.Locked;
    [SerializeField] private bool _isCursorVisible = false;
    [SerializeField] private bool _isCameraFixed = false; // Nuevo: estado de cámara fija o no

    [Header("<color=#6A89A7>Physics</color>")]
    [Range(.01f, 1f)][SerializeField] private float _detectionRadius = .1f;
    [SerializeField] private float _hitOffset = 0.25f;

    [Header("<color=#6A89A7>Settings</color>")]
    [Range(1f, 1000f)][SerializeField] private float _mouseSensitivity = 500f;
    [Range(.125f, 1f)][SerializeField] private float _minDistance = .25f;
    [Range(1f, 10f)][SerializeField] private float _maxDistance = 5f;
    [Range(-90f, 0f)][SerializeField] private float _minRotation = -45f;
    [Range(0f, 90f)][SerializeField] private float _maxRotation = 80f;

    [SerializeField] private bool _isCamBlocked = false;
    [SerializeField] private float _mouseX = 0.0f, _mouseY = 0.0f;
    [SerializeField] private Vector3 _dir = new(), _dirTest = new(), _camPos = new();

    [SerializeField] private Camera _cam;
    [SerializeField] private Transform _target;

    [SerializeField] private Ray _camRay;
    [SerializeField] private RaycastHit _camRayHit;

    private void Awake()
    {

    }

    private void Start()
    {
        _target = GameManager.Instance.Player.GetCamTarget;
        _cam = Camera.main;

        LockCursor();  // Usamos el nuevo método LockCursor para inicializar
        transform.forward = _target.forward;

        _mouseX = transform.eulerAngles.y;
        _mouseY = transform.eulerAngles.x;
    }

    private void Update()
    {
        // Alternar entre la cámara fija y la liberada al presionar Escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _isCameraFixed = !_isCameraFixed;  // Cambia el estado de la cámara fija
            ToggleCursorMode(_isCameraFixed);
        }

        // Solo actualizar la rotación de la cámara si no está fija
        if (!_isCameraFixed)
        {
            UpdateCamRot(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        }
    }

    private void FixedUpdate()
    {
        _camRay = new Ray(transform.position, _dir);

        _isCamBlocked = Physics.SphereCast(_camRay, _detectionRadius, out _camRayHit, _maxDistance);
    }

    private void LateUpdate()
    {
        if (!_isCameraFixed)
        {
            UpdateSpringArm();
        }
    }

    public bool IsCameraFixed
    {
        get { return _isCameraFixed; }
        set
        {
            _isCameraFixed = value;
            ToggleCursorMode(_isCameraFixed); // Ajustamos el estado del cursor
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

            if (_dirTest.sqrMagnitude <= _minDistance * _minDistance)
            {
                _camPos = transform.position + _dir * _minDistance;
            }
            else
            {
                _camPos = transform.position + _dirTest;
            }
        }
        else
        {
            _camPos = transform.position + _dir * _maxDistance;
        }

        _cam.transform.position = _camPos;
        _cam.transform.LookAt(transform.position);
    }

    // Método para alternar entre los modos de cursor
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
        Cursor.lockState = _lockMode;
        Cursor.visible = _isCursorVisible;
    }
}
