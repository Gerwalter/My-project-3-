using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingHook : MonoBehaviour
{
    [Header("<color=#FFD700>Grappling Hook Settings</color>")]
    [SerializeField] private float _grapplingRange = 20f;
    [SerializeField] private float _pullSpeed = 15f;
    [SerializeField] private float _stopDistance = 2f;
    [SerializeField] private float _fakeGrappleDuration = 0.3f; // Tiempo que la cuerda "falsa" se mantiene visible
    [SerializeField] private KeyCode _grappleKey = KeyCode.F;
    [SerializeField] private KeyCode _cancelKey = KeyCode.Q;
    [SerializeField] private LayerMask _grappleMask;

    [Header("<color=#FFD700>References</color>")]
    [SerializeField] private Transform _player;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private LineRenderer _lineRenderer;

    private Vector3 _grapplePoint;
    private bool _isGrappling = false;
    private int _playerLayer;

    private void Start()
    {
        _playerLayer = _player.gameObject.layer;
    }

    private void Update()
    {
        if (Input.GetKeyDown(_grappleKey) && !_isGrappling)
        {
            StartGrapple();
        }
        if (Input.GetKeyDown(_cancelKey) && _isGrappling)
        {
            StopGrapple();
        }
    }

    private void StartGrapple()
    {
        RaycastHit hit;
        Vector3 origin = Camera.main.transform.position;
        Vector3 direction = Camera.main.transform.forward;

        int mask = _grappleMask & ~(1 << _playerLayer);

        _lineRenderer.enabled = true;  // Siempre activa la línea
        _lineRenderer.SetPosition(0, _player.position);

        if (Physics.Raycast(origin, direction, out hit, _grapplingRange, mask))
        {
            _grapplePoint = hit.point;
            _isGrappling = true;
            _rb.useGravity = false;

            _lineRenderer.SetPosition(1, _grapplePoint);
            StartCoroutine(GrappleMove());
        }
        else
        {
            // Si no colisiona con nada, dibuja la cuerda en la dirección del disparo
            Vector3 fakeEndPoint = origin + (direction * _grapplingRange);
            _lineRenderer.SetPosition(1, fakeEndPoint);

            StartCoroutine(FakeGrappleEffect());
        }
    }

    private IEnumerator GrappleMove()
    {
        while (_isGrappling)
        {
            Vector3 direction = (_grapplePoint - _player.position).normalized;
            float distance = Vector3.Distance(_player.position, _grapplePoint);

            if (distance <= _stopDistance)
            {
                StopGrapple();
                yield break;
            }

            _rb.velocity = direction * _pullSpeed;
            _lineRenderer.SetPosition(0, _player.position);

            yield return null;
        }
    }

    private IEnumerator FakeGrappleEffect()
    {
        yield return new WaitForSeconds(_fakeGrappleDuration);
        _lineRenderer.enabled = false; // Desactiva la línea después de un tiempo corto
    }

    private void StopGrapple()
    {
        _isGrappling = false;
        _rb.useGravity = true;
        _lineRenderer.enabled = false;
        _rb.velocity = Vector3.zero;
    }
}
