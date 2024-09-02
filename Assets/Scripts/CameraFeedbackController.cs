using UnityEngine;

public class CameraFeedbackController : MonoBehaviour
{
    public Color feedbackColor = Color.red;
    public Color defaultColor = Color.white;
    public float rayDistance = 100f;
    public Color rayColor = Color.green;
    private Renderer targetRenderer;
    public CameraFollow cameraFollow;
    public PlayerMoveToTarget playerMove;


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

                if (Input.GetKeyDown(KeyCode.Q) && targetRenderer != null)
                {
                    cameraFollow.LockOnTarget(hit.transform);
                    playerMove.SetTarget(hit.transform);
                }
            }
        }
        else if (targetRenderer != null)
        {
            targetRenderer.material.color = defaultColor;
            targetRenderer = null;
        }
    }
}
