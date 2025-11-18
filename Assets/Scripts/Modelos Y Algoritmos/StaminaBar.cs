using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour, IStaminaObserver
{
    [SerializeField] private Image staminaFill;
    private PlayerStamina stamina;

    private void Start()
    {
        // Buscar el PlayerController en escena y suscribirse
    //    var player = FindObjectOfType<PlayerController>();
   //     stamina = player.Stamina;

    //    stamina.Subscribe(this);

        // Inicializar UI
      //  Notify(stamina.CurrentStamina, stamina.MaxStamina);
    }

    public void Notify(float value, float maxValue)
    {
        float fill = Mathf.Clamp01(value / maxValue);
        staminaFill.fillAmount = fill;

        // Cambiar color dinámicamente
        if (fill > 0.5f)
            staminaFill.color = Color.green;
        else if (fill > 0.25f)
            staminaFill.color = Color.yellow;
        else
            staminaFill.color = Color.red;
    }

    private void OnDestroy()
    {
        if (stamina != null)
            stamina.Unsubscribe(this);
    }
}
