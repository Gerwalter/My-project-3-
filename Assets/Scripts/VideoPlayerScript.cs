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

    private void Start()
    {
        videoPlayer.enabled = false;
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
        // Seleccionar un video al azar
        int randomIndex = Random.Range(0, videoClips.Length);
    }
}
