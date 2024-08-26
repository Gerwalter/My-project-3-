using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoPlayerScript : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public VideoClip[] videoClips; // Array de videos
    public bool appleCollected = false;
    public bool isPlaying = false;
    public bool playerdectected = false;
    public GameObject player;
    private int currentVideoIndex;
    public AnimationScript animator;
    public GameObject camer;

    private void Start()
    {
        videoPlayer.enabled = false;
        videoPlayer.loopPointReached += OnVideoFinished; // Suscribirse al evento de finalización del video
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            playerdectected = true;
        }
    }

    private void Update()
    {
        playVideo();
    }

    void playVideo()
    {
        if (appleCollected && playerdectected && !isPlaying)
        {
            PlayRandomVideo();
            isPlaying = true; // Marcar que el video se está reproduciendo
        }
    }

    void PlayRandomVideo()
    {
        // Seleccionar un video al azar
        int randomIndex = Random.Range(0, videoClips.Length);
        if (currentVideoIndex == 0) // Si el video seleccionado es el número 4 (índice 3)
        {
            animator.anim.SetTrigger("BadApple");
        }
            videoPlayer.clip = videoClips[randomIndex];

        videoPlayer.enabled = true;
        videoPlayer.Play();
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        animator.anim.SetTrigger("BadApple");
    }
}
