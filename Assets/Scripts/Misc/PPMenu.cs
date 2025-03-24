using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PPMenu : MonoBehaviour
{
    public GameObject menu;
    public Player player;
    public MenuCameraLocker _lock;

    private void Start()
    {
        player = GameManager.Instance.Player;
    }

    public void Menudisable()
    {
        menu.SetActive(false);
       // player.freeze = false;

        _lock.UnlockCamera();
        
    }
}
