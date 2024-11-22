using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasMenu : ButtonBehaviour
{
    public GameObject menu;
    public Player player;
    public MenuCameraLocker Lock;

    private void Start()
    {
        player = GameManager.Instance.Player;
    }

    public override void OnInteract()
    {
        menu.SetActive(true);
        player.freeze = true;
        if (menu.activeSelf == true)
        {
            // Alternamos el estado de IsCameraFixed
            Lock.LockCamera();
        }
    }   
}
