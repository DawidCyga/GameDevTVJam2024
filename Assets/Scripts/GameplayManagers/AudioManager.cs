using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    public enum AudioName
    {
        // This enum will contain explicitly declared enumerator list. 
        //The elements values correspond to positions of audio clips in audio clip list
        //Enum elements are keys in enumNameAudioDictionary
        // Manager will subscribe to events triggered on objects producing sounds
    }


    [SerializeField] private List<AudioClip> _audioClipList = new List<AudioClip>();

    private Dictionary<AudioName, AudioClip> _enumNameAudioClipDictionary = new Dictionary<AudioName, AudioClip>();

    private AudioSource _audioSource;

    private void Awake()
    {
        Instance = this;

        _audioSource = GetComponent<AudioSource>();

        foreach (AudioName audioName in System.Enum.GetValues(typeof(AudioName)))
        {
            _enumNameAudioClipDictionary[audioName] = _audioClipList[(int)audioName];
        }

    }
    private void PlaySound(AudioName name) => _audioSource.PlayOneShot(_enumNameAudioClipDictionary[name]);

}
