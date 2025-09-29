using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class LoadingScreen : MonoBehaviour
{
    public static string nextScene;

    [Header("UI")]
    public Slider loadingBar;
    public TextMeshProUGUI loadingText;
    public CanvasGroup fadePanel; // El panel negro con CanvasGroup

    [Header("Fade Settings")]
    public float fadeDuration = 1f;

    private void Start()
    {
        StartCoroutine(FadeIn()); // Al entrar, fade desde negro a transparente
        StartCoroutine(LoadAsync());
    }

    IEnumerator LoadAsync()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;

        while (!op.isDone)
        {
            float progress = Mathf.Clamp01(op.progress / 0.9f);
            loadingBar.value = progress;
            loadingText.text = (progress * 100f).ToString("F0") + "%";

            if (op.progress >= 0.9f)
            {
                loadingBar.value = 1f;
                loadingText.text = "100%";

                // Pequeña pausa y fade out antes de entrar a la escena
                yield return new WaitForSeconds(0.5f);
                yield return StartCoroutine(FadeOut());
                op.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    IEnumerator FadeIn()
    {
        fadePanel.alpha = 1;
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            fadePanel.alpha = 1 - (t / fadeDuration);
            yield return null;
        }
        fadePanel.alpha = 0;
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
