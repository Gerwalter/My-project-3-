using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDitherController : MonoBehaviour
{
    public static PlayerDitherController Instance;

    [SerializeField] private Material[] playerMaterials;

    private void Awake()
    {
        Instance = this;
    }

    public void SetDither(float value)
    {
        foreach (var mat in playerMaterials)
        {
            mat.SetFloat("_Dither", value);
        }
    }
}
