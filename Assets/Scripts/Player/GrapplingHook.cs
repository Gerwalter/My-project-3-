using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private Camera alternateCamera;
    [Header("<color=#FFD700>Keybinds</color>")]
    [SerializeField] private KeyCode _grappleKey = KeyCode.F;

    [Header("<color=#FFD700>References</color>")]
    [SerializeField] private Transform _player;
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

    private void Start()
    {
        Crosshair.enabled = false;
        _playerLayer = _player.gameObject.layer;

    }

    private void Update()
    {
        if (Input.GetKeyDown(_grappleKey))
        {
            Crosshair.enabled = true;
        }
        if (Input.GetKeyUp(_grappleKey) && !_isGrappling)
        {
            Crosshair.enabled = false;
           // CameraController.Instance.SwitchCamera();
            StartGrapple();
        }
    }

    private void StartGrapple()
    {
        RaycastHit hit;
        Vector3 origin = Camera.main.transform.position;
        Vector3 direction = Camera.main.transform.forward;

        int mask = _grappleMask & ~(1 << _playerLayer);

        _lineRenderer.enabled = true;
        _lineRenderer.SetPosition(0, _player.position);

        if (Physics.Raycast(origin, direction, out hit, _grapplingRange, mask))
        {
            _grapplePoint = hit.point;
            _isGrappling = true;
            _rb.useGravity = false;
            _initialPosition = _player.position;
            _previousPosition = _initialPosition;

            _lineRenderer.SetPosition(1, _grapplePoint);
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

        while (elapsedTime < _grappleDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / _grappleDuration;

            // Movimiento interpolado con altura
            Vector3 horizontalMovement = Vector3.Lerp(_initialPosition, _grapplePoint, t);
            float height = Mathf.Sin(t * Mathf.PI) * _arcHeight;
            Vector3 newPosition = horizontalMovement + new Vector3(0, height, 0);

            // Calcular la velocidad basada en la posición previa
            _currentVelocity = (newPosition - _previousPosition) / Time.deltaTime;
            _previousPosition = _player.position;

            _rb.MovePosition(newPosition);
            _lineRenderer.SetPosition(0, _player.position);

            // Soltar en el punto más alto
            if (releaseAtPeak && !hasReleased && t >= 0.5f)
            {
                hasReleased = true;
                StopGrapple(true);  // True para aplicar el impulso
                yield break;
            }

            // Parar si llegamos al destino
            if (Vector3.Distance(_player.position, _grapplePoint) <= _stopDistance)
            {
                StopGrapple(false);
                yield break;
            }

            yield return null;
        }
    }

    private IEnumerator FakeGrappleEffect()
    {
        yield return new WaitForSeconds(_fakeGrappleDuration);
        _lineRenderer.enabled = false;
    }
    public float velocitylimiter;
    private void StopGrapple(bool applyImpulse)
    {
        _isGrappling = false;
        _rb.useGravity = true;
        _lineRenderer.enabled = false;

        if (applyImpulse)
        {
            // Cálculo de la velocidad máxima basada en la distancia recorrida
            float traveledDistance = Vector3.Distance(_initialPosition, _grapplePoint);
            float maxSpeed = Mathf.Clamp(traveledDistance * 2f, 10f, 50f); // Ajusta los valores según necesidad

            // Aplicar velocidad con límite
            Vector3 limitedVelocity = Vector3.ClampMagnitude(_currentVelocity, maxSpeed);
            _rb.velocity = limitedVelocity/ velocitylimiter;
        }
        else
        {
            _rb.velocity = Vector3.zero; // Detiene completamente si se cancela manualmente
        }
    }

    private void OnCollisionEnter(Collision collision)
    { // Verificar si la colisión es con una pared usando la LayerMask
        if (((1 << collision.gameObject.layer) & _grappleMask) != 0)
        {
            // Opcional: Detectar si el impacto es fuerte (basado en la velocidad)
            float impactForce = collision.relativeVelocity.magnitude;

            if (impactForce > 5f) // Ajusta este valor según pruebas
            {
                // Detener completamente el movimiento
                _rb.velocity = Vector3.zero;
                _rb.angularVelocity = Vector3.zero;

                // Opcional: Añadir un pequeño empuje hacia atrás para que no se "pegue" a la pared
                _rb.AddForce(-collision.contacts[0].normal * 2f, ForceMode.Impulse);
            }
        }
    }
}
