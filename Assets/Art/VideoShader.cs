using UnityEngine;
using UnityEngine.Video;

public class VideoShader : MonoBehaviour
{
    public VideoClip videoClip;  // El VideoClip que se usará
    [SerializeField] private Renderer objectRenderer;  // Renderer del objeto
    [SerializeField] private VideoPlayer videoPlayer;  // Componente VideoPlayer para reproducir el VideoClip
    public string shaderTexturePropertyName = "_MainTex";
    [SerializeField] private Animator anim; // Nombre de la propiedad del Shader que controla la textura

    void Start()
    {
        // Obtén el componente Renderer del objeto 3D
        objectRenderer = GetComponent<Renderer>();

        // Si no se ha asignado, busca el VideoPlayer en otro GameObject
        if (videoPlayer == null)
        {
            videoPlayer = FindObjectOfType<VideoPlayer>();
        }

        // Establece el VideoClip en el VideoPlayer
        if (videoPlayer != null)
        {
            videoPlayer.clip = videoClip;
            videoPlayer.renderMode = VideoRenderMode.APIOnly;
        }
        else
        {
            Debug.LogError("No se encontró un VideoPlayer en la escena.");
        }
    }

    void Update()
    {
        // Espera a que el video tenga una textura disponible
        if (videoPlayer != null && videoPlayer.texture != null)
        {
            // Establece la textura del video en el Shader
            objectRenderer.material.SetTexture(shaderTexturePropertyName, videoPlayer.texture);
        }
        else return;
    }

    public void VideoPlay()
    {
        if (anim != null)
        {
            anim.SetBool("appear", true);
        }
    }

    public void Player()
    {
        if (videoPlayer != null)
        {
            videoPlayer.Play();
        }
    }
}
