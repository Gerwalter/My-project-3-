using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Texturee : MonoBehaviour
{

    // Referencias al material y bot�n
    public Material material; // Asigna el material que usar� el shader
    public Button toggleButton; // El bot�n que controlar� el cambio

    private bool isOverlayActive = true; // Estado actual de la keyword


    // M�todo que intercambia la keyword en el shader
    public void ToggleOverlay()
    {
        // Cambia entre activar o desactivar la keyword
        if (isOverlayActive)
        {
            material.DisableKeyword("_TEXTURE");
        }
        else
        {
            material.EnableKeyword("_TEXTURE");
        }

        // Actualiza el estado de la keyword
        isOverlayActive = !isOverlayActive;
    }

    public void ToggleOverlay2()
    {
        // Cambia entre activar o desactivar la keyword
        if (isOverlayActive)
        {
            material.DisableKeyword("_ACTIVATE");
        }
        else
        {
            material.EnableKeyword("_ACTIVATE");
        }

        // Actualiza el estado de la keyword
        isOverlayActive = !isOverlayActive;
    }
}
