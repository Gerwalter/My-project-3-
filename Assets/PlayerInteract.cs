using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : Player
{
    [Header("<color=#6A89A7>Inputs</color>")]
    [SerializeField] private KeyCode _intKey = KeyCode.F;


    [Header("<color=#6A89A7>Physics - Interaction</color>")]
    [SerializeField] private Transform _intOrigin;
    [SerializeField] private float _intRayDist = 1.0f;
    [SerializeField] private LayerMask _intMask;
    [SerializeField] private Ray _intRay;
    private RaycastHit _intHit;
    public float _sphereIntRadius = 0.5f; // Radio de la esfera

    private void Update()
    {
        if (Input.GetKeyDown(_intKey))
            _anim.SetTrigger("Int");
    }
    public void Start()
    {
        EventManager.Subscribe("OnInteract", OnInteract);
    }

    void OnInteract(params object[] args)
    {
        Interact(); // método original
    }
    public override void Interact()
    {
        _intRay = new Ray(_intOrigin.position, transform.forward);

        if (Physics.SphereCast(_intRay, _sphereIntRadius, out _intHit, _intRayDist, _intMask))
        {
            if (_intHit.collider.TryGetComponent<ButtonBehaviour>(out ButtonBehaviour intObj))
            {
                intObj.OnInteract();
            }
        }
    }

    private void OnDrawGizmos()
    {

        // Dibuja la línea del SphereCast
        Gizmos.color = Color.yellow; // Color del gizmo
        Gizmos.DrawLine(_intOrigin.position, _intOrigin.position + transform.forward * _intRayDist);

        // Dibuja la esfera al final del SphereCast
        Gizmos.color = Color.cyan; // Color de la esfera
        Gizmos.DrawWireSphere(_intRay.origin + _intRay.direction * _intRayDist, _sphereIntRadius);
    }
}
