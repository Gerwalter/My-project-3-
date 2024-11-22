using UnityEngine;

public class Lock : MonoBehaviour
{

    public GameObject menu;
    public MenuCameraLocker Locker;

    private void Start()
    {
        menu.SetActive(false);
    }

    public void OnDie()
    {
        menu.SetActive(true);
      //  if (Locker != null && menu.activeSelf)
      //  {
      //      // Alternamos el estado de IsCameraFixed
           Locker.LockCamera();
      //
      //  }
    }

    private void Update()
    {

    }
}
