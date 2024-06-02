using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    public enum AudioName
    {
        Menu,
        Tutorial,
        MainGame
    }


    [SerializeField] private List<AudioClip> _audioClipList = new List<AudioClip>();

    private Dictionary<AudioName, AudioClip> _enumNameAudioClipDictionary = new Dictionary<AudioName, AudioClip>();

    private AudioSource _audioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }

        _audioSource = GetComponent<AudioSource>();

        foreach (AudioName audioName in System.Enum.GetValues(typeof(AudioName)))
        {
            _enumNameAudioClipDictionary[audioName] = _audioClipList[(int)audioName];
        }

    }

    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if (arg0.buildIndex == 0)
        {
            PlaySound(AudioName.Menu);
        }
        else if (arg0.buildIndex == 2 || arg0.buildIndex == 4)
        {
            PlaySound(AudioName.Tutorial);
        }
        else if (arg0.buildIndex == 3)
        {
            PlaySound(AudioName.MainGame);
        }
    }

    private void PlaySound(AudioName name)
    {
        _audioSource.Stop();
        _audioSource.PlayOneShot(_enumNameAudioClipDictionary[name]);
    }
}
