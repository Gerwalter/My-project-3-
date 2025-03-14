using System.Collections;
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
    public Image CrossHair; // Referencia al objeto de UI que mostrará el progreso de cooldown

    public CameraController Cameracontroller;
    public float originaldistance;
    public int Newdistance;

    private void Start()
    {
        Cameracontroller = CameraController.Instance;
        controller = GameManager.Instance.Player;
        grappleCDImage = CanvasReferencesManager.Instance.HookTimer;
        CrossHair = CanvasReferencesManager.Instance.CrossHair;
        originaldistance = Cameracontroller._maxDistance;
        CrossHair.enabled = false;
        cam = CameraController.Instance.transform;
    }

    private void LateUpdate()
    {
        if (grappling) lineRenderer.SetPosition(0, graplletip.position);
    }
   [SerializeField] private float requiredHoldTime = 0.2f;
   [SerializeField] private float grappleHoldTime = 0f; // Tiempo que se mantiene presionada la tecla
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
            grappleHoldTime = 0f;
            controller.freeze = true;
        }

        // Aumentar el tiempo que se mantiene presionada la tecla
        if (Input.GetKey(grappleKey))
        {
            grappleHoldTime = Mathf.Min(grappleHoldTime + Time.deltaTime, requiredHoldTime);
            Cameracontroller._maxDistance = Newdistance;
            CrossHair.enabled = true;
        }

        // Detectar si se suelta la tecla
        if (Input.GetKeyUp(grappleKey))
        {
            Cameracontroller._maxDistance = originaldistance;
            CrossHair.enabled = false;
            if (grappleHoldTime >= requiredHoldTime) // Solo activa si el tiempo de presión es suficiente
            {
                startGrapple();
            }
            else
            {
                controller.freeze = false; // Libera al personaje si no se activó el gancho
            }
        }

        if (grappleCDTimer > 0)
        {
            grappleCDTimer = Mathf.Min(grappleCDTimer - Time.deltaTime, grappleCD);
            //grappleCDTimer -= Time.deltaTime;
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
