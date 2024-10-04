using UnityEngine;
using UnityEngine.Video;

public class VideoShader : MonoBehaviour
{
    public VideoClip videoClip;  // El VideoClip que se usará
    [SerializeField] private Renderer objectRenderer;  // Renderer del objeto
    [SerializeField] private VideoPlayer videoPlayer;  // Componente VideoPlayer para reproducir el VideoClip
    public string shaderTexturePropertyName = "_MainTex";
    [SerializeField] private Animator anim;// Nombre de la propiedad del Shader que controla la textura

    void Start()
    {
        // Obtén el componente Renderer del objeto 3D
        objectRenderer = GetComponent<Renderer>();

        // Añade el componente VideoPlayer si no está presente
        if (videoPlayer == null)
        {
            videoPlayer = gameObject.AddComponent<VideoPlayer>();
        }

        // Establece el VideoClip en el VideoPlayer
        videoPlayer.clip = videoClip;

        // Reproduce el video en modo renderizado en una textura
        videoPlayer.renderMode = VideoRenderMode.APIOnly;

    }

    void Update()
    {
        // Espera a que el video tenga una textura disponible
        if (videoPlayer.texture != null)
        {
            // Establece la textura del video en el Shader
            objectRenderer.material.SetTexture(shaderTexturePropertyName, videoPlayer.texture);
        }
    }
    public void VideoPlay()
    {
        anim.SetBool("appear", true);
    }

    public void Player() 
    {
        videoPlayer.Play();
    }
}
