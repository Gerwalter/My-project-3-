using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager instance;

    [SerializeField] private AudioSource soundSFXObject;
    [SerializeField] private AudioSource musicObject;

    private void Awake()
    {
            if (instance == null) { instance = this; }
    }
    public void PlaySFXClip(AudioClip clip, Transform spawnTransform, float volume)
    {
        AudioSource audioSource = Instantiate(soundSFXObject, spawnTransform.position, Quaternion.identity);

        audioSource.clip = clip;

        audioSource.volume = volume;

        audioSource.Play();

        float clipLength = audioSource.clip.length;

        Destroy(audioSource.gameObject, clipLength);
    }

    public void PlayMusic(AudioClip clip, Transform spawnTransform, float volume)
    {
        AudioSource audioSource = Instantiate(musicObject, spawnTransform.position, Quaternion.identity);

        audioSource.clip = clip;

        audioSource.volume = volume;

        audioSource.Play();

        float clipLength = audioSource.clip.length;

        Destroy(audioSource.gameObject, clipLength);
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
