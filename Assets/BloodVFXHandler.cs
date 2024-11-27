using UnityEngine;
using UnityEngine.VFX;

public class BloodVFXHandler : MonoBehaviour
{
    [SerializeField] private VisualEffect _bloodVFX; // Asigna el VFX en el Inspector
    [SerializeField] private float _timer = 0f;
    [SerializeField] private const float Interval = 0.5f; // Intervalo de 0.5 segundos

    private void FixedUpdate()
    {
        _timer += Time.fixedDeltaTime;

        if (_timer >= Interval)
        {
            _bloodVFX.SendEvent("OnTakeDamage");
            _timer = 0f; // Reinicia el temporizador
        }
    }
}
