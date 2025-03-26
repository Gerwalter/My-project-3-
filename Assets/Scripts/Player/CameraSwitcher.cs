using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{

    [SerializeField] private Camera alternateCamera; // Asigna esta cámara desde el inspector

    private void Start()
    {
        if (CameraController.Instance != null)
        {
            CameraController.Instance.AlternateCamera = alternateCamera;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            CameraController.Instance.SwitchCamera();
        }
     /*   if (Input.GetKeyUp(KeyCode.X))
        {
            CameraController.Instance.SwitchCamera();
        }*/
    }
}
