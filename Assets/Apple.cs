using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Apple : MonoBehaviour
{
    // Asigna este campo en el Inspector para referenciar el bot�n
    public Button myButton;
    public GameObject mySlider;

    // Este m�todo ser� llamado cuando el bot�n sea presionado
    public void Click()
    {
        mySlider.SetActive(true);
    }
}
