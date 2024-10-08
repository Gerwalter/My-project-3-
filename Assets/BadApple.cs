using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class BadApple : MonoBehaviour
{
    [SerializeField, Range(0f, 1)] private float vignette = 1;
    [SerializeField] private Texture _mainTexture;
    [SerializeField] private VideoPlayer videoPlayer;
    public string shaderTexturePropertyName = "_MainTex";
    public VideoClip videoClip;

    private void Start()
    {
        // Configurar la viñeta inicialmente
        AppleController.Instance.VignettePostProcess.SetFloat(AppleController.Instance.VignetteAmountName, 5.0f);

        // Establece el VideoClip en el VideoPlayer
        if (videoPlayer != null)
        {
            videoPlayer.clip = videoClip;
            videoPlayer.renderMode = VideoRenderMode.APIOnly; // Render only to Texture (API)
        }
    }

    private void Update()
    {
        // Calcular la cantidad de viñeta inversa: máximo con vida máxima, mínimo con vida baja
        float vignetteAmount = Mathf.Lerp(5.0f, 0.0f, vignette);

        // Asignar el valor calculado a la viñeta
        AppleController.Instance.VignettePostProcess.SetFloat(AppleController.Instance.VignetteAmountName, vignetteAmount);

        // Verificar si la viñeta es exactamente 5 para reproducir el video
        if (vignette == 1.0f)
        {
            if (videoPlayer != null && !videoPlayer.isPlaying)
            {
                videoPlayer.Play(); // Reproduce el video si no se está reproduciendo
            }

            if (videoPlayer != null && videoPlayer.texture != null)
            {
                // Usar la textura del VideoPlayer en lugar de _mainTexture
                AppleController.Instance.VignettePostProcess.SetTexture(shaderTexturePropertyName, videoPlayer.texture);
            }
        }
        else
        {
            if (videoPlayer != null && videoPlayer.isPlaying)
            {
                videoPlayer.Pause(); // Pausa el video si la viñeta no es 5
            }

            // Usa la textura _mainTexture como alternativa si el video está pausado
            if (_mainTexture != null)
            {
                AppleController.Instance.VignettePostProcess.SetTexture(shaderTexturePropertyName, _mainTexture);
            }
        }
    }
}
