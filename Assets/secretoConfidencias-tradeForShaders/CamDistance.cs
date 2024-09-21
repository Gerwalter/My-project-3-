using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamDistance : MonoBehaviour
{
    [SerializeField] Cam _cam;
    
    //destino del raycast
    [SerializeField] Transform _lookingAt;
    
    //largo del rayo
    [SerializeField] float _rayDistance;

    Vector3 _origin;

    //pinto de colision
    Vector3 _point;


    private void Update()
    {
        //guardar pos dle pj, en teoria pos del mesh pero no queria usar mesh de una porque soy especial
        _origin = new Vector3(transform.position.x, transform.position.y + 2, transform.position.z);
        
        //direccion del ray
        Vector3 _ray = (_lookingAt.position - _origin).normalized;

        //crea rayo verde solo para el editor en unity
        Debug.DrawRay(_origin, _ray * _rayDistance, Color.green);

        RaycastHit hit;
        
        //Crea Raycast desde el player al camera holder (pos default de la camara)
        //Cuando el raycast es cortado por la pared entre medio se pone a hacer magia

        // Crea Raycast     origen  / direccion   /devuelve algo/  largo del ray/  layer que lo puede cortar/
        if (Physics.Raycast(_origin, _ray.normalized, out hit, _rayDistance, LayerMask.GetMask("Default")))
        {
            //Debug de testeo
            //Debug.Log(hit.transform.name);

            //Al colicionar con una pared, guarda el liugar y activa el uso de punto para la posicion d ela camara
            _point = hit.point;
            _cam.usePoint=true;
            

        }
        else if(_cam.usePoint)
        {
            //Desactiva uso de punto
            _cam.usePoint = false;
        }       
    }

    private void LateUpdate()
    {
        if (_cam.usePoint)
        {
            /*
             * Intento de evitar spam de cambio de camara
            if ((_point - _cam.point).magnitude > 0.1f)
            {
                _cam.point = _point;
            }
            */

            //Da cordenadas del punto en la pared
            _cam.point = _point;
        }
    }
}
