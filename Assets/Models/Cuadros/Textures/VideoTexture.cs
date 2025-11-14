using UnityEngine;
using UnityEngine.Video;
using UnityEngine.Audio;

[RequireComponent(typeof(VideoPlayer))]
[RequireComponent(typeof(AudioSource))]
public class VideoTexture : MonoBehaviour
{
    [Header("Video Settings")]
    public VideoClip videoClip;
    public bool playOnAwake = true;
    public string textureProperty = "_BaseMap"; // o "_MainTex" según tu shader

    [Header("Audio Settings")]
    public AudioMixerGroup musicGroup; // arrastra aquí el canal "Music" de tu mixer

    private VideoPlayer videoPlayer;
    public MeshRenderer rend;
    private AudioSource audioSource;

    void Awake()
    {
        // Obtener componentes
        videoPlayer = GetComponent<VideoPlayer>();
        rend = GetComponent<MeshRenderer>();
        audioSource = GetComponent<AudioSource>();

        // Configuración del VideoPlayer
        videoPlayer.playOnAwake = playOnAwake;
        videoPlayer.isLooping = true;

        // --- VIDEO ---
        videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        RenderTexture rt = new RenderTexture(512, 512, 0);
        videoPlayer.targetTexture = rt;
        rend.material.SetTexture(textureProperty, rt);

        // --- AUDIO ---
        videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
        videoPlayer.EnableAudioTrack(0, true);
        videoPlayer.SetTargetAudioSource(0, audioSource);

        // Configuración del AudioSource
        audioSource.playOnAwake = false;
        audioSource.loop = true;
        if (musicGroup != null)
            audioSource.outputAudioMixerGroup = musicGroup;

        // Asignar el clip
        if (videoClip != null)
            videoPlayer.clip = videoClip;

        if (playOnAwake && videoClip != null)
            videoPlayer.Play();
    }

    public void PlayVideo()
    {
        if (!videoPlayer.isPlaying)
            videoPlayer.Play();
    }

    public void StopVideo()
    {
        if (videoPlayer.isPlaying)
            videoPlayer.Stop();
    }
}
