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
    public AnimationScript animator;

    private void Start()
    {
        videoPlayer.enabled = false;
        videoPlayer.playOnAwake = false; // Asegurarse de que no se reproduzca automáticamente al iniciar
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
            isPlaying = true;
        }
    }

    void PlayRandomVideo()
    {
        int randomIndex = Random.Range(0, videoClips.Length);

        videoPlayer.enabled = true;

        videoPlayer.clip = videoClips[randomIndex];

        if (randomIndex == 0)
        {
            animator.anim.SetTrigger("BadApple");
        }

        videoPlayer.Play();
    }
}
