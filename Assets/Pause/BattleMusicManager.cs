using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleMusicManager : MonoBehaviour
{
    [SerializeField] private string backgroundA;
    [SerializeField] private string backgroundB;

    public SoundManager soundManager;

    private void Start()
    {
        if (soundManager == null)
            soundManager = SoundManager.instance;

        // Seleccionar aleatoriamente una de las dos
        string selected = (Random.value < 0.5f) ? backgroundA : backgroundB;

        soundManager.PlaySound(selected);
    }
}
