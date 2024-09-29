using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPointer : MonoBehaviour
{
    public float rayDistance = 100f;
    public Color rayColor = Color.green;
    public Renderer targetRenderer;
    public GameObject target;

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
                target = hit.collider.gameObject;
            }
        }
        else if (target != null)
        {
            target = null;
        }
    }
}
