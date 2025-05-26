using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    [SerializeField] AudioMixer audioMixer;
    public static SoundManager instance;
    public Sound[] Music, sfx;
    public AudioSource musicSource, sfxSource;

    public const string MUSIC_KEY = "Music";
    public const string SFX_KEY = "SFX";
    public const string MASTER_KEY = "Master";

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return; // Detiene la ejecución para evitar conflictos.
        }

        // Escuchar cambios de escena
       // SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StopMusic(); // Detiene la música al cambiar de escena
    }
    public void StopMusic()
    {
        if (musicSource.isPlaying)
        {
            musicSource.Stop(); // Detiene la música actual
        }
    }
    public void PlaySound(string name)
    {
        Sound s = Array.Find(Music, x => x.name == name);

        if (s == null)
        {
            Debug.Log("nada");
        }
        else
        {
            musicSource.clip = s.audioClip;
            musicSource.Play();
        }
    }
    void loadVolume()
    {
        float Musicvolume = PlayerPrefs.GetFloat(MUSIC_KEY, .75f);
        float SFXvolume = PlayerPrefs.GetFloat(SFX_KEY, .75f);
        float Mastervolume = PlayerPrefs.GetFloat(MASTER_KEY, .75f);

        audioMixer.SetFloat(MUSIC_KEY, Mathf.Log10(Musicvolume) * 20);
        audioMixer.SetFloat(SFX_KEY, Mathf.Log10(SFXvolume) * 20);
        audioMixer.SetFloat(MASTER_KEY, Mathf.Log10(Mastervolume) * 20);
    }
    public void PlaySFX(string name)
    {
        Sound s = Array.Find(sfx, x => x.name == name);

        if (s == null)
        {
            Debug.Log("SFX no encontrado: " + name);
            return;
        }

        sfxSource.PlayOneShot(s.audioClip); // Usa PlayOneShot para evitar sobreescrituras
    }
}