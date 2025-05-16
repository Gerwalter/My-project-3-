using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboAttack : MonoBehaviour
{
    [Header("<color=yellow>Attack</color>")]
    [SerializeField] private Transform _atkOrigin;
    [SerializeField] private float _atkRayDist = 1.0f;
    [SerializeField] private LayerMask _atkMask;
    [SerializeField] private int _atkDmg = 1;
    private Ray _atkRay;
    private RaycastHit _atkHit;
    public float _sphereAtkRadius = 0.5f;

    private void OnDrawGizmos()
    {

        // Dibuja la línea del SphereCast
        Gizmos.color = Color.white; // Color del gizmo
        Gizmos.DrawLine(_atkOrigin.position, _atkOrigin.position + transform.forward * _atkRayDist);

        // Dibuja la esfera al final del SphereCast
        Gizmos.color = Color.green; // Color de la esfera
        Gizmos.DrawWireSphere(_atkRay.origin + _atkRay.direction * _atkRayDist, _sphereAtkRadius);
    }
    private void Awake()
    {
        EventManager.Subscribe("Attack", Attacks);
    }

    public void Attacks(params object[] args)
    {
        Attack();
    }
    public void Attack()
    {
        _atkRay = new Ray(_atkOrigin.position, transform.forward);
        Gizmos.color = Color.red;
        if (Physics.SphereCast(_atkRay, _sphereAtkRadius, out _atkHit, _atkRayDist, _atkMask))
        {
            if (_atkHit.collider.TryGetComponent<HP>(out HP enemy))
            {
                enemy.ReciveDamage(_atkDmg);
            }
        }

        else if (Physics.Raycast(_atkRay, out _atkHit, _atkRayDist, _atkMask))
        {
            if (_atkHit.collider.TryGetComponent<HP>(out HP enemy))
            {
                enemy.ReciveDamage(_atkDmg);
            }
        }
    }

}
