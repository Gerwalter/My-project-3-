using UnityEngine;

public class VehicleInteraction : MonoBehaviour
{
    public GameObject player;
    public Transform vehicleSeat;
    public VehicleController vehicleController;
    public Player playerController; // Script de movimiento del jugador

    private bool inVehicle = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) // Tecla para entrar o salir del vehículo
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

    void EnterVehicle()
    {
        player.transform.SetParent(vehicleSeat);
        player.transform.localPosition = Vector3.zero;
        player.transform.localRotation = Quaternion.identity;
        player.SetActive(false); // Desactiva el modelo del jugador

        if (playerController != null)
            playerController.enabled = false; // Desactiva el script del jugador

        vehicleController.enabled = true; // Activa el control del vehículo
        inVehicle = true;
    }

    void ExitVehicle()
    {
        player.transform.SetParent(null);
        player.SetActive(true); // Reactiva el modelo del jugador

        if (playerController != null)
            playerController.enabled = true; // Reactiva el script del jugador

        vehicleController.enabled = false; // Desactiva el control del vehículo
        inVehicle = false;
    }
}