using UnityEngine;
using UnityEngine.Video;

public class VideoTexture : MonoBehaviour
{
    [Header("Configuración del video")]
    public VideoClip videoClip;          // El video que querés reproducir
    public bool loop = true;             // Si el video se repite

    [Header("Materiales de la TV")]
    public Material[] tvMaterials;       // Varios materiales
    public string textureProperty = "_BaseMap"; // Nombre de la propiedad del shader

    private VideoPlayer videoPlayer;
    private RenderTexture renderTexture;

    void Awake()
    {
        // Crear RenderTexture
        renderTexture = new RenderTexture(1920, 1080, 0);
        renderTexture.Create();

        // Configurar VideoPlayer
        videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.playOnAwake = false;
        videoPlayer.isLooping = loop;
        videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        videoPlayer.targetTexture = renderTexture;

        if (videoClip != null)
            videoPlayer.clip = videoClip;

        // Asignar la textura a todos los materiales
        foreach (var mat in tvMaterials)
        {
            if (mat != null)
                mat.SetTexture(textureProperty, renderTexture);
        }
    }

    void Start()
    {
        videoPlayer.Play();
    }

    private void OnDestroy()
    {
        if (renderTexture != null)
            renderTexture.Release();
    }
}
