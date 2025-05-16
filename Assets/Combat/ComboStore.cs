using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboStore : MonoBehaviour
{    public static ComboStore Instance { get; private set; }

    public float Points = 100; // Puedes asignar el valor inicial desde aquí

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
}
