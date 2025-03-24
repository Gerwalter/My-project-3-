using UnityEngine;
using UnityEngine.UIElements;

public class VehicleInteraction : ButtonBehaviour
{
    public GameObject player;
    public Transform vehicleSeat;
    public VehicleController vehicleController;
    public Player playerController; // Script de ugador
   [SerializeField] private CapsuleCollider playerCollider;
   [SerializeField] private Rigidbody playerRigidbody;
    private bool inVehicle = false;

    private void Start()
    {
        if (!inVehicle) 
        {
            vehicleController.enabled = false;
        }
    }

    void EnterVehicle()
    {
        player.transform.SetParent(vehicleSeat);
        player.transform.localPosition = Vector3.zero;
        player.transform.localRotation = Quaternion.identity;
        playerCollider.enabled = false;
        playerRigidbody.isKinematic = true;

        if (playerController != null)
            playerController.enabled = false; // Desactiva el script del jugador

        vehicleController.enabled = true; // Activa el control del vehículo
        inVehicle = true;
    }

    void ExitVehicle()
    {
        player.transform.SetParent(null);
        player.SetActive(true); // Reactiva el modelo del jugador
        playerCollider.enabled = true;
        playerRigidbody.isKinematic = false;
        if (playerController != null)
            playerController.enabled = true; // Reactiva el script del jugador

        vehicleController.enabled = false; // Desactiva el control del vehículo
        inVehicle = false;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ExitVehicle();
        }
    }
    public override void OnInteract()
    {
        if (!inVehicle)
        {
            EnterVehicle();
        }
        else
        {
            ExitVehicle();
        }
    }
}