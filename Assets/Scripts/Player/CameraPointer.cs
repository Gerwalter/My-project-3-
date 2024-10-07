using UnityEngine;
using UnityEngine.UI;

public class CameraPointer : MonoBehaviour
{
    public float rayDistance = 100f;
    public Color rayColor = Color.green;
    public Image CrossHair;
    public GameObject target;

    private void Start()
    {
        CrossHair.enabled = false;
    }

    void Update()
    {
        CheckObjectInCenter();


        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (target != null)
            {
                isLockedOnTarget = true;
            }
        }
    }
    public bool isLockedOnTarget = false;
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
                CrossHair.enabled = true;
            }

            else if (isLockedOnTarget == true) 
            {
                target = null;
                CrossHair.enabled = false;
                isLockedOnTarget = false;
            }
            else
            {
                CrossHair.enabled = false;
            }
        }
        else if (target != null)
        {
            target = null;
            CrossHair.enabled = false;
        }
    }

    public void TargetNull()
    {
        target = null;
        CrossHair.enabled = false;
        isLockedOnTarget = false;
    }
}
