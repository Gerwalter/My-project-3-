using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HipsTracker : MonoBehaviour
{
    public Transform hipsTransform;
    public Transform playerTransform;

    public void TrackerHips()
    {
        if (hipsTransform != null && playerTransform != null)
        {
            // Guardar la posición original de las animaciones
            Vector3 originalAnimationPosition = hipsTransform.localPosition;

            // Mover el objeto player a la posición de hipsTransform
            playerTransform.position = hipsTransform.position;

            // Restaurar la posición local de las animaciones
            //hipsTransform.localPosition = originalAnimationPosition;
        }
        else
        {
            Debug.LogWarning("Hips Transform or Player Transform not assigned!");
        }
    }
}
