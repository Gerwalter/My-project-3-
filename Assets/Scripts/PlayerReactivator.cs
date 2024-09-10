using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerReactivator : HipsTracker
{

    [Range(0, 7)] public float Seconds;

    // Método que se llama desde el script HipsTracker
    public void ReactivatePlayerAfterDelay()
    {
        // Inicia la corrutina que reactivará al player después de 7 segundos
        StartCoroutine(ReactivatePlayerCoroutine());
    }

    // Corrutina que espera 7 segundos antes de reactivar al player
    private IEnumerator ReactivatePlayerCoroutine()
    {
        yield return new WaitForSeconds(Seconds);

        // Verifica si el player está desactivado y lo activa
        if (!player.activeSelf && !player2.activeSelf && !player3.activeSelf)
        {
            player.SetActive(true);
            player2.SetActive(true);
            player3.SetActive(true);
        }
    }
}
