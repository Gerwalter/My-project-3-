using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [Header("<color=#6A89A7>Inputs</color>")]
    [SerializeField] private KeyCode _intKey = KeyCode.F;

    [Header("<color=#6A89A7>Physics - Interaction</color>")]
    [SerializeField] private Transform _intOrigin;
    [SerializeField] private float _intRadius = 0.5f;
    [SerializeField] private float _intRange = 1.0f;
    [SerializeField] private LayerMask _intMask;

    private ButtonBehaviour _currentTarget;

    private void Start()
    {
        EventManager.Subscribe("OnInteract", OnInteract);
    }

    private void Update()
    {
        if (Input.GetKeyDown(_intKey))
            EventManager.Trigger("Input", "Int");

        DetectInteractable();
    }

    private void DetectInteractable()
    {
        _currentTarget = null;

        Collider[] hits = Physics.OverlapSphere(
            _intOrigin.position + transform.forward * _intRange,
            _intRadius,
            _intMask
        );

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<ButtonBehaviour>(out ButtonBehaviour intObj))
            {
                _currentTarget = intObj;
                break;
            }
        }
    }

    void OnInteract(params object[] args)
    {
        if (_currentTarget != null)
        {
            _currentTarget.OnInteract();
        }
    }

    private void OnDrawGizmos()
    {
        if (_intOrigin == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(_intOrigin.position, _intOrigin.position + transform.forward * _intRange);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(_intOrigin.position + transform.forward * _intRange, _intRadius);
    }

    private void OnDestroy()
    {
        EventManager.Unsubscribe("OnInteract", OnInteract);
    }
}
