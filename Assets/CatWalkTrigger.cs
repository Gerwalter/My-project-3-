using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatWalkTrigger : MonoBehaviour
{
    public void OnPlayerStanding()
    {
        CameraController.Instance.SetMinRotation(true);
        Debug.Log("El jugador está sobre el CatWalk");
    }
    public void OnPlayerExit()
    {
        CameraController.Instance.SetMinRotation(false);
        Debug.Log("El jugador ya no está sobre el CatWalk");
    }
}
