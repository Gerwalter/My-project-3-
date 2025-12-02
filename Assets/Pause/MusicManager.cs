using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private string background;
    public SoundManager soundManager;



    private void Start()
    {
        if (soundManager == null)
        {
            soundManager = SoundManager.instance;
        }
        // Inicializa la m�sica de fondo usando SFXManager
        soundManager.PlaySound(background);
    }


}
