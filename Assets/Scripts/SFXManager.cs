using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager instance;

    [SerializeField] private AudioSource soundSFXObject;

    private void Awake()
    {
            if (instance == null) { instance = this; }
    }

    public void PlayRandSFXClip (AudioClip[] clip, Transform spawnTransform, float volume)
    {
        AudioSource audioSource = Instantiate(soundSFXObject, spawnTransform.position, Quaternion.identity);

        int rand = Random.Range(0, clip.Length);

        audioSource.clip = clip[rand];

        audioSource.volume = volume;

        audioSource.Play();

        float clipLength = audioSource.clip.length;

        Destroy(audioSource.gameObject, clipLength);
    }
}
