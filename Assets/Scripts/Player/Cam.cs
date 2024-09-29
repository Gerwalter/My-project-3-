using UnityEngine;

public class Cam : MonoBehaviour
{
    [SerializeField] public Vector3 point;
    [SerializeField] Transform _target;

    public bool usePoint = false;

    private void Start()
    {
        GameManager.Instance.Camera = this;
    }

    private void LateUpdate()
    {
        transform.position = _target.position;

        transform.forward = _target.forward;
    }
}
