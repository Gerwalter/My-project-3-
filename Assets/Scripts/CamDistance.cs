using UnityEngine;

public class CamDistance : MonoBehaviour
{
    [SerializeField] private Cam _cam;

    [SerializeField] private Transform _lookingAt;


    [SerializeField] private float _rayDistance;

    [SerializeField] private Vector3 _origin;


    [SerializeField] private Vector3 _point;


    private void Update()
    {

        _origin = new Vector3(transform.position.x, transform.position.y + 2, transform.position.z);


        Vector3 _ray = (_lookingAt.position - _origin).normalized;


        Debug.DrawRay(_origin, _ray * _rayDistance, Color.green);

        RaycastHit hit;


        if (Physics.Raycast(_origin, _ray.normalized, out hit, _rayDistance, LayerMask.GetMask("Default")))
        {
            _point = hit.point;
            _cam.usePoint = true;
        }
        else if (_cam.usePoint)
        {
            _cam.usePoint = false;
        }
    }

    private void LateUpdate()
    {
        if (_cam.usePoint)
        {
            _cam.point = _point;
        }
    }
}
