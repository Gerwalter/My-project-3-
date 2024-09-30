using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraPointer : MonoBehaviour
{
    public float rayDistance = 100f;
    public Color rayColor = Color.green;
    public Image CrossHair;
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
                CrossHair.color = Color.red;
                target = hit.collider.gameObject;
            }
        }
        else if (target != null)
        {
            CrossHair.color = Color.black;
            target = null;
        }
    }
}
