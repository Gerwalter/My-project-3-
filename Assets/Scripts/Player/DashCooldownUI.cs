using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DashCooldownUI : MonoBehaviour
{
    public Image cooldownImage; // Imagen que representa la barra de cooldown
    public Color cooldownColor = Color.red; // Color de la barra durante el cooldown
    public Color readyColor = Color.green; // Color de la barra cuando el cooldown ha terminado
    public float dashCooldown = 2.0f; // Duración del cooldown en segundos

    private float cooldownTimer;
    private bool isCooldownActive = false;

    void Update()
    {
        if (isCooldownActive)
        {
            // Actualizar el temporizador
            cooldownTimer -= Time.deltaTime;

            if (cooldownTimer <= 0)
            {
                // Cooldown terminado
                isCooldownActive = false;
                cooldownTimer = 0;
            }

            // Actualizar la barra de cooldown
            float fillAmount = cooldownTimer / dashCooldown;
            cooldownImage.fillAmount = fillAmount;

            // Cambiar el color de la barra según el estado del cooldown
            cooldownImage.color = Color.Lerp(cooldownColor, readyColor, fillAmount);
        }
        else
        {
            // La barra está llena y es verde
            cooldownImage.fillAmount = 1.0f;
            cooldownImage.color = readyColor;
        }
    }

    public void StartCooldown()
    {
        // Iniciar el cooldown
        isCooldownActive = true;
        cooldownTimer = dashCooldown;
    }
}
