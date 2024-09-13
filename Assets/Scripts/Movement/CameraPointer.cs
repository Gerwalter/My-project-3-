using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPointer : MonoBehaviour
{
    public Color feedbackColor = Color.red;
    public Color defaultColor = Color.white;
    public float rayDistance = 100f;
    public Color rayColor = Color.green;
    public Renderer targetRenderer;
    public GameObject target; // Nueva variable para el GameObject

    void Update()
    {
        CheckObjectInCenter();
    }

    void CheckObjectInCenter()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        Debug.DrawRay(transform.position, transform.forward * rayDistance, rayColor);

        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            if (hit.collider.gameObject.GetComponent<SpecificScript>() != null)
            {
                if (targetRenderer != null && targetRenderer != hit.collider.GetComponent<Renderer>())
                {
                    targetRenderer.material.color = defaultColor;
                }

                targetRenderer = hit.collider.GetComponent<Renderer>();
                targetRenderer.material.color = feedbackColor;

                // Actualiza la variable target con el GameObject correspondiente
                target = hit.collider.gameObject;
            }
        }
        else if (targetRenderer != null)
        {
            targetRenderer.material.color = defaultColor;
            targetRenderer = null;

            // Reinicia target cuando no hay un objeto detectado
            target = null;
        }
    }
}
