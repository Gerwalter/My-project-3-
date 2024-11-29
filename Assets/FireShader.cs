using System.Collections;
using UnityEngine;

public class FireShader : MonoBehaviour
{
    [SerializeField] public Material material; // El material que quieres controlar
    [SerializeField] private string noisePowerProperty = "_NoisePower_"; // Nombre del parámetro en el material
    [SerializeField] private float duration = 2f; // Duración en segundos para animar el valor
    [SerializeField] private float timeElapsed = 0f;
    private void Start()
    {
        material.SetFloat(noisePowerProperty, 0);
    }

    private bool isAnimating = false; // Controla si una animación ya está en curso

    public void ActivateFire()
    {
        StartCoroutine(AnimateNoisePower());
    }

    IEnumerator AnimateNoisePower()
    {
        isAnimating = true;

        // Obtiene el valor actual de NoisePower_
        float currentValue = material.GetFloat(noisePowerProperty);
        float targetValue = (currentValue == 0f) ? 0.64f : 0f; // Si está en 0, va a 0.64; si está en 0.64, va a 0
        
        float startValue = currentValue;

        // Animar el valor de NoisePower_ a su valor objetivo
        while (timeElapsed < duration)
        {
            float value = Mathf.Lerp(startValue, targetValue, timeElapsed / duration);
            material.SetFloat(noisePowerProperty, value);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // Asegura que el valor final sea el objetivo (0 o 0.64)
        material.SetFloat(noisePowerProperty, targetValue);

        isAnimating = false;
    }

}
