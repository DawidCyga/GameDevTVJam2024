using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepsAudioManager : MonoBehaviour
{
    public static StepsAudioManager Instance { get; private set; }

    public enum AudioName
    {
        Step1,
        Step2,
        Step3,
        Step4
    }

    [SerializeField] private List<AudioClip> _audioClipList = new List<AudioClip>();

    private Dictionary<AudioName, AudioClip> _enumNameAudioClipDictionary = new Dictionary<AudioName, AudioClip>();
    private AudioSource _audioSource;
    private bool _isPlayingWalkSound = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
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

    public void PlayWalkSound()
    {
        if (!_isPlayingWalkSound && _audioSource != null)
        {
            int index = Random.Range((int)AudioName.Step1, (int)AudioName.Step1 + 4); // Assume these are the first four indices
            _audioSource.clip = _enumNameAudioClipDictionary[(AudioName)index];
            _audioSource.Play();
            _isPlayingWalkSound = true;
            StartCoroutine(WaitForSoundToEnd());
        }
    }

    private IEnumerator WaitForSoundToEnd()
    {
        yield return new WaitWhile(() => _audioSource.isPlaying);
        _isPlayingWalkSound = false;
        if (Player.Instance != null && Player.Instance.IsMoving() && Player.Instance.IsGrounded())
        {
            PlayWalkSound();
        }
    }

    public void StopWalkSound()
    {
        _audioSource.Stop();
        _isPlayingWalkSound = false;
    }

    public bool IsPlayingWalkSound() => _isPlayingWalkSound;
}
