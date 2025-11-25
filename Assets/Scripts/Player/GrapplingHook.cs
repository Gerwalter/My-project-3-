using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GrapplingHook : MonoBehaviour
{
    [Header("<color=#FFD700>Grappling Hook Settings</color>")]
    [SerializeField] private float _grapplingRange = 20f;
    [SerializeField] private float _arcHeight = 5f;
    [SerializeField] private float _stopDistance = 2f;
    [SerializeField] private float _fakeGrappleDuration = 0.3f;
    [SerializeField] private float _grappleDuration = 1f;
    [SerializeField] private bool releaseAtPeak = false;

    [Header("<color=#FFD700>Toggle Movement</color>")]
    [SerializeField] private bool useStraightMovement = false; // NUEVO: alternar entre recto y arco

    [Header("<color=#FFD700>Keybinds</color>")]
    [SerializeField] private KeyCode _grappleKey = KeyCode.F;

    [Header("<color=#FFD700>References</color>")]
    [SerializeField] private Transform _player;
    [SerializeField] private Transform _grappleOrigin; // 🔹 NUEVO: origen del gancho
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private LayerMask _grappleMask;
    [SerializeField] private Image Crosshair;

    private Vector3 _grapplePoint;
    private bool _isGrappling = false;
    private int _playerLayer;
    private Vector3 _initialPosition;
    private Vector3 _previousPosition;
    private Vector3 _currentVelocity;

    private Vector3 _targetPosition;
    private bool _isMoving;

    public float velocitylimiter = 2f;

    private void Start()
    {
        Crosshair.enabled = false;
        _playerLayer = _player.gameObject.layer;
    }

    private void Update()
    {
        if (Input.GetKeyDown(_grappleKey) && !Input.GetKey(KeyCode.LeftControl))
        {
            Crosshair.enabled = true;
        }
        if (Input.GetKeyUp(_grappleKey) && !_isGrappling && !Input.GetKey(KeyCode.LeftControl))
        {
            Crosshair.enabled = false;
          //  StartGrapple();
        }
    }
    public void StartGrappleToPoint(Vector3 targetPoint)
    {
        Crosshair.enabled = false;
        _grapplePoint = targetPoint;
        _isGrappling = true;
        _rb.useGravity = false;

        _initialPosition = _player.position;
        _previousPosition = _initialPosition;

        _lineRenderer.enabled = true;
        _lineRenderer.SetPosition(0, _grappleOrigin ? _grappleOrigin.position : _player.position);
        _lineRenderer.SetPosition(1, _grapplePoint);

        if (useStraightMovement)
            StartCoroutine(GrappleStraightMove());
        else
            StartCoroutine(GrappleArcMove());
    }
    private void StartGrapple()
    {
        RaycastHit hit;
        Vector3 origin = Camera.main.transform.position;
        Vector3 direction = Camera.main.transform.forward;

        int mask = _grappleMask & ~(1 << _playerLayer);

        _lineRenderer.enabled = true;
        _lineRenderer.SetPosition(0, _grappleOrigin ? _grappleOrigin.position : _player.position);

        if (Physics.Raycast(origin, direction, out hit, _grapplingRange, mask))
        {
            _grapplePoint = hit.point;
            _isGrappling = true;
            _rb.useGravity = false;
            _initialPosition = _player.position;
            _previousPosition = _initialPosition;

            _lineRenderer.SetPosition(1, _grapplePoint);

            if (useStraightMovement)
                StartCoroutine(GrappleStraightMove());
            else
                StartCoroutine(GrappleArcMove());
        }
        else
        {
            Vector3 fakeEndPoint = origin + (direction * _grapplingRange);
            _lineRenderer.SetPosition(1, fakeEndPoint);
            StartCoroutine(FakeGrappleEffect());
        }
    }
    private IEnumerator GrappleArcMove()
    {
        float elapsedTime = 0f;
        bool hasReleased = false;
        _isMoving = true;

        while (elapsedTime < _grappleDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / _grappleDuration;

            Vector3 horizontalMovement = Vector3.Lerp(_initialPosition, _grapplePoint, t);
            float height = Mathf.Sin(t * Mathf.PI) * _arcHeight;
            _targetPosition = horizontalMovement + new Vector3(0, height, 0);

            _currentVelocity = (_targetPosition - _previousPosition) / Time.deltaTime;
            _previousPosition = _player.position;

            _lineRenderer.SetPosition(0, _grappleOrigin ? _grappleOrigin.position : _player.position);

            if (releaseAtPeak && !hasReleased && t >= 0.5f)
            {
                hasReleased = true;
                StopGrapple(true);
                yield break;
            }

            if (Vector3.Distance(_player.position, _grapplePoint) <= _stopDistance)
            {
                StopGrapple(false);
                yield break;
            }

            yield return null;
        }

        _isMoving = false;
    }

    private IEnumerator GrappleStraightMove()
    {
        float elapsedTime = 0f;
        _isMoving = true;

        while (elapsedTime < _grappleDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / _grappleDuration;

            // Movimiento lineal directo
            _targetPosition = Vector3.Lerp(_initialPosition, _grapplePoint, t);

            _currentVelocity = (_targetPosition - _previousPosition) / Time.deltaTime;
            _previousPosition = _player.position;

            _lineRenderer.SetPosition(0, _grappleOrigin ? _grappleOrigin.position : _player.position);

            if (Vector3.Distance(_player.position, _grapplePoint) <= _stopDistance)
            {
                StopGrapple(false);
                yield break;
            }

            yield return null;
        }

        _isMoving = false;
    }

    private IEnumerator FakeGrappleEffect()
    {
        yield return new WaitForSeconds(_fakeGrappleDuration);
        _lineRenderer.enabled = false;
    }

    private void FixedUpdate()
    {
        if (_isMoving && _isGrappling)
        {
            if (Physics.Linecast(_player.position, _targetPosition, out RaycastHit hit))
            {
                StopGrapple(false);
                _rb.position = hit.point;
            }
            else
            {
                _rb.MovePosition(_targetPosition);
            }
        }
    }

    private void StopGrapple(bool applyImpulse)
    {
        _isGrappling = false;
        _isMoving = false;
        _rb.useGravity = true;
        _lineRenderer.enabled = false;

        if (applyImpulse)
        {
            float traveledDistance = Vector3.Distance(_initialPosition, _grapplePoint);
            float maxSpeed = Mathf.Clamp(traveledDistance * 2f, 10f, 50f);

            Vector3 limitedVelocity = Vector3.ClampMagnitude(_currentVelocity, maxSpeed);
            _rb.velocity = limitedVelocity / velocitylimiter;
        }
        else
        {
            _rb.velocity = Vector3.zero;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & _grappleMask) != 0)
        {
            float impactForce = collision.relativeVelocity.magnitude;

            if (impactForce > 5f)
            {
                _rb.velocity = Vector3.zero;
                _rb.angularVelocity = Vector3.zero;
                _rb.AddForce(-collision.contacts[0].normal * 2f, ForceMode.Impulse);
            }
        }
    }
}
