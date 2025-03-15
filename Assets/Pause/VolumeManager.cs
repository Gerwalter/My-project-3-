using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeManager : MonoBehaviour
{
    public AudioMixer AudioMixer;
    public Slider VolumeSlider;
    public Slider SfxSlider;
    public Slider MasterSlider;

   public const string MIXER_MUSIC = "Music Volume";
    public const string MIXER_SFX = "SFX Volume";
    public const string MIXER_Master = "Master Volume";

    private void Awake()
    {
        VolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        SfxSlider.onValueChanged.AddListener(SetSFXVolume);
        MasterSlider.onValueChanged.AddListener(SetMasterVolume);
    }
    private void Start()
    {
       VolumeSlider.value = PlayerPrefs.GetFloat(SoundManager.MUSIC_KEY, .75f);
        SfxSlider.value = PlayerPrefs.GetFloat(SoundManager.SFX_KEY, .75f);
        MasterSlider.value = PlayerPrefs.GetFloat(SoundManager.MASTER_KEY, .75f);
    }
    private void OnDisable()
    {
        PlayerPrefs.SetFloat(SoundManager.MUSIC_KEY, VolumeSlider.value);
        PlayerPrefs.SetFloat(SoundManager.SFX_KEY, SfxSlider.value);
        PlayerPrefs.SetFloat(SoundManager.MASTER_KEY, MasterSlider.value);
    }
    void SetMusicVolume(float volume)
    {
        AudioMixer.SetFloat(MIXER_MUSIC, Mathf.Log10(volume)* 20);
    }
    void SetSFXVolume(float volume)
    {
        AudioMixer.SetFloat(MIXER_SFX, Mathf.Log10(volume) * 20);
    }

    void SetMasterVolume(float volume)
    {
        AudioMixer.SetFloat(MIXER_Master, Mathf.Log10(volume) * 20);
    }
}
