using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
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

    public void UpdateMasterVolume(float volume) => _audioMixer.SetFloat(MIXER_PARAMETER_MASTER, volume);
    public void UpdateSoundVolume(float volume) => _audioMixer.SetFloat(MIXER_PARAMETER_SOUND, volume);
    public void UpdateMusicVolume(float volume) => _audioMixer.SetFloat(MIXER_PARAMETER_MUSIC, volume);

}
