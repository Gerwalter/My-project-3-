using UnityEngine;
using UnityEngine.UI;

public class Grappling : MonoBehaviour
{
    [Header("Referencias")]
    public Player controller;
    public Transform cam;
    public Transform graplletip;
    public LineRenderer lineRenderer;
    public LayerMask whatisGrappable;

    [Header("Grappling")]
    public float maxDistance;
    public float delayTime;
    public float overshootYAxis;

    public Vector3 grapplePoint;

    [Header("Cooldown")]
    public float grappleCD;
    public float grappleCDTimer;

    [Header("Input")]
    public KeyCode grappleKey;
    public bool grappling;

    [Header("UI")]
    public Image grappleCDImage; // Referencia al objeto de UI que mostrará el progreso de cooldown

    private void Start()
    {
        controller = GameManager.Instance.Player;
        grappleCDImage = CanvasReferencesManager.Instance.HookTimer;
    }

    private void LateUpdate()
    {
        if (grappling) lineRenderer.SetPosition(0, graplletip.position);
    }

    private void Update()
    {
        // Actualizar la barra de cooldown en la UI
        if (grappleCDImage != null)
        {
            grappleCDImage.fillAmount = 1 - (grappleCDTimer / grappleCD);
        }

        if (Input.GetKeyDown(grappleKey))
        {
            if (grappleCDTimer > 0) return;
            controller.freeze = true;
        }

        if (Input.GetKeyUp(grappleKey))
        {
            startGrapple();
        }

        if (grappleCDTimer > 0)
        {
            grappleCDTimer -= Time.deltaTime;
        }
    }

    public void startGrapple()
    {
        if (grappleCDTimer > 0) return;

        grappling = true;

        RaycastHit hit;

        if (Physics.Raycast(cam.position, cam.forward, out hit, maxDistance, whatisGrappable))
        {
            grapplePoint = hit.point;
            Invoke(nameof(executeGrapple), delayTime);
        }
        else
        {
            grapplePoint = cam.position + cam.forward * maxDistance;
            Invoke(nameof(stopGrapple), delayTime);
        }

        lineRenderer.enabled = true;
        lineRenderer.SetPosition(1, grapplePoint);
    }

    public void executeGrapple()
    {
        Vector3 lowestPoint = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);
        float grapplePointRelativeToY = grapplePoint.y - lowestPoint.y;
        float highestPointInArc = grapplePointRelativeToY + overshootYAxis;

        if (grapplePointRelativeToY < 0) highestPointInArc = overshootYAxis;

        controller.JumpToPosition(grapplePoint, highestPointInArc);
        Invoke(nameof(stopGrapple), 1f);
    }

    public void stopGrapple()
    {
        grappling = false;
        controller.freeze = false;
        grappleCDTimer = grappleCD;
        lineRenderer.enabled = false;
    }
}
