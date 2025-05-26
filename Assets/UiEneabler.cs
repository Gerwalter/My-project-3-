using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiEneabler : MonoBehaviour
{
    public GameObject targetObject; // Asigna el GameObject desde el inspector
   
    public PlayerCombat combat;

    void Update()
    {
        if (targetObject != null)
        {
            combat.CanCombo = !targetObject.activeSelf;
        }
    }
}
