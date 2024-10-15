using UnityEngine;

public class Lock : MonoBehaviour
{

    public GameObject menu;
    public MenuCameraLocker Locker;

    public void OnDie()
    {
        print("AAA");
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
