using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasMenu : ButtonBehaviour
{
    public GameObject menu;
    public MenuCameraLocker Lock;


    public override void OnInteract()
    {
        menu.SetActive(true);
        if (Lock != null && menu.activeSelf)
        {
            // Alternamos el estado de IsCameraFixed
            Lock.LockCamera();
        }
    }   
}
