using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoPlayerScript : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public bool appleCollected = false;
    public bool isPlaying = false;
    public bool playerdectected = false;
    private void Start()
    {
        videoPlayer.enabled = false;
    }
    public GameObject player;

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
        if (appleCollected && playerdectected) 
        {
            videoPlayer.enabled = true; videoPlayer.Play();
        }
    }
}
