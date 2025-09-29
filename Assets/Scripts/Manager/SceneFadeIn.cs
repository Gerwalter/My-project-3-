using UnityEngine;
using System.Collections;

public class SceneFadeIn : MonoBehaviour
{
    [Header("Fade Settings")]
    public CanvasGroup fadePanel; // Panel negro con CanvasGroup
    public float fadeDuration = 1f;

    [Header("Bloqueo de sripts")]
    public MonoBehaviour[] scriptsToDisable; // Scripts que deben estar deshabilitados durante el fade

    private void Start()
    {
        // Deshabilitar scripts antes del fade
        foreach (var script in scriptsToDisable)
        {
            if (script != null) script.enabled = false;
        }

        StartCoroutine(FadeInRoutine());
    }

    IEnumerator FadeInRoutine()
    {
        fadePanel.alpha = 1; // empieza completamente negro
        float t = 0;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            fadePanel.alpha = 1 - (t / fadeDuration);
            yield return null;
        }

        fadePanel.alpha = 0;

        // Reactivar scripts al terminar
        foreach (var script in scriptsToDisable)
        {
            if (script != null) script.enabled = true;
        }
    }
}
