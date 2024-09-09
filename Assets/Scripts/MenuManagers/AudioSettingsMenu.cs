using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AudioSettingsMenu : MonoBehaviour
{
    [SerializeField] private AudioMixer _audioMixer;

    [SerializeField] private Slider _masterVolumeSlider;
    [SerializeField] private Slider _soundVolumeSlider;
    [SerializeField] private Slider _musicVolumeSlider;

    private const string MIXER_PARAMETER_MASTER = "MasterVolume";
    private const string MIXER_PARAMETER_SOUND = "SFXVolume";
    private const string MIXER_PARAMETER_MUSIC = "MusicVolume";

    private void OnEnable()
    {
        Debug.Log("Called changing values");
        _masterVolumeSlider.value = VolumeSettingsHolder.Instance.MasterVolume;
        _soundVolumeSlider.value = VolumeSettingsHolder.Instance.SFXVolume;
        _musicVolumeSlider.value = VolumeSettingsHolder.Instance.MusicVolume;
    }

    public void UpdateMasterVolume()
    {
        float volume = _masterVolumeSlider.value;
        SetMixerValue(MIXER_PARAMETER_MASTER, _masterVolumeSlider.value);

        VolumeSettingsHolder.Instance.MasterVolume = volume;
    }
    public void UpdateSoundVolume()
    {
        float volume = _soundVolumeSlider.value;
        SetMixerValue(MIXER_PARAMETER_SOUND, volume);

        VolumeSettingsHolder.Instance.SFXVolume = volume;
    }
    public void UpdateMusicVolume()
    {
        float volume = _musicVolumeSlider.value;
        SetMixerValue(MIXER_PARAMETER_MUSIC, volume);

        VolumeSettingsHolder.Instance.MusicVolume = volume;
    }

    private void SetMixerValue(string name, float volume) =>
        _audioMixer.SetFloat(name, Mathf.Log10(volume) * 20);

}
