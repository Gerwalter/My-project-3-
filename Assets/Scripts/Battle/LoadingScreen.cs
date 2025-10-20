using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class LoadingScreen : MonoBehaviour
{
    public static string nextScene;

    [Header("UI")]
    public Animator loadingAnimator; // Animator que controla la animación de carga
    public TextMeshProUGUI loadingText;
    public CanvasGroup fadePanel;

    [Header("Fade Settings")]
    public float fadeDuration = 1f;
    public float extraDelayAfterFull = 1.5f; // Espera adicional para que se vea la animación

    private void Start()
    {
        StartCoroutine(LoadAsync());
    }

    IEnumerator LoadAsync()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;

        while (!op.isDone)
        {
            float progress = Mathf.Clamp01(op.progress / 0.9f);
            loadingText.text = (progress * 100f).ToString("F0") + "%";

            // Pasamos el valor de progreso al Animator (float parameter "Progress")
            loadingAnimator.SetFloat("Progress", progress);

            if (op.progress >= 0.9f)
            {
                loadingText.text = "100%";
                loadingAnimator.SetFloat("Progress", 1f);

                // Dispara el trigger de finalización
                loadingAnimator.SetTrigger("LoadFinished");

                // Espera un tiempo adicional antes de activar la escena
                yield return new WaitForSeconds(extraDelayAfterFull);

                // Opcional: fade out antes de entrar a la escena
                yield return StartCoroutine(FadeOut());

                op.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    IEnumerator FadeOut()
    {
        fadePanel.alpha = 0;
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            fadePanel.alpha = t / fadeDuration;
            yield return null;
        }
        fadePanel.alpha = 1;
    }
}
