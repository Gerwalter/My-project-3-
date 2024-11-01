using UnityEngine;
using UnityEngine.UI;

public class CameraPointer : MonoBehaviour
{
    public float rayDistance = 100f;
    public Color rayColor = Color.green;
    public Image CrossHair;
    public GameObject target;
    public TargetMove targetMove; // Referencia al script TargetMove
    public LayerMask layerMask; // Máscara de capas para ignorar la capa del jugador

    private void Start()
    {
        CrossHair.enabled = false;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.T))
        {
            CheckObjectInCenter();
        }
        else if (Input.GetKeyUp(KeyCode.T))
        {
            // Solo inicia el movimiento cuando se suelta la tecla T y hay un objetivo válido
            if (target != null)
            {
                targetMove.MoveTowardsTarget();
            }
        }
    }

    void CheckObjectInCenter()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        Debug.DrawRay(transform.position, transform.forward * rayDistance, rayColor);

        // Usar el layerMask para ignorar la capa del jugador
        if (Physics.Raycast(ray, out hit, rayDistance, layerMask))
        {
            target = hit.collider.gameObject;
            CrossHair.enabled = true;
            CrossHair.color = Color.green;
        }
        else
        {
            CrossHair.color = Color.red;
            target = null;
        }
    }

    public void TargetNull()
    {
        target = null;
        CrossHair.enabled = false;
    }
}
