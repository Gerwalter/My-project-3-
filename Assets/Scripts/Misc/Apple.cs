using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Apple : MonoBehaviour
{
    // Asigna este campo en el Inspector para referenciar el botón
    public Button myButton;
    public GameObject mySlider;

    // Este método será llamado cuando el botón sea presionado
    public void Click()
    {
        mySlider.SetActive(true);
    }
}
