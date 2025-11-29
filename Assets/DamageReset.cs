using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageReset : MonoBehaviour
{

    [Header("Asignar Material con el parámetro _ScreenBorderAmount")]
    [SerializeField] private Material screenBorderMaterial;

    [Header("Límites del efecto")]
    [SerializeField] private float maxBorderValue = 5f; // Vida completa
    [SerializeField] private float minBorderValue = 1f; // Vida mínima



    private void Start()
    {
        screenBorderMaterial.SetFloat("_ScreenBorderAmount", maxBorderValue);
    }


}
