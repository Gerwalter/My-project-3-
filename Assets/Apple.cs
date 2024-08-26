using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class Apple : MonoBehaviour
{
    public VideoPlayerScript playerScript;
    public GameObject player;

    private void OnTriggerEnter(Collider other)
    {

        print(other.name);
        if (other.gameObject == player)
        {
            playerScript.appleCollected = true;
            Destroy(gameObject);
        }
    }
}
