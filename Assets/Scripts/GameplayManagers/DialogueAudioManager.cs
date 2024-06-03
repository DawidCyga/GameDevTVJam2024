using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueAudioManager : MonoBehaviour
{
    [SerializeField] private AudioClip _speaker1Audio;
    [SerializeField] private AudioClip _speaker2Audio;

    private int _previousSpeaker;

    [Header("Cache References")]
    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        DialogueManager.Instance.OnStartedNewParagraph += DialogueManager_OnStartedNewParagraph;
        DialogueManager.Instance.OnLastParagraphTyped += DialogueManager_OnLastParagraphTyped;
    }

    private void DialogueManager_OnStartedNewParagraph(object sender, DialogueManager.OnStartedNewParagraphEventArgs e)
    {
        int currentDialogueParagraphIndex = DialogueManager.Instance.GetCurrentDialogueParagraphIndex();

        if (e.CurrentDialogueSpeaker == 1 && e.CurrentDialogueSpeaker != _previousSpeaker && (currentDialogueParagraphIndex == 0 || currentDialogueParagraphIndex == 1))
        {
            _previousSpeaker = e.CurrentDialogueSpeaker;
            _audioSource.Stop();
            _audioSource.PlayOneShot(_speaker1Audio);
        }
        else if (e.CurrentDialogueSpeaker == 2 && e.CurrentDialogueSpeaker != _previousSpeaker && (currentDialogueParagraphIndex == 0 || currentDialogueParagraphIndex == 1))
        {
            _previousSpeaker = e.CurrentDialogueSpeaker;
            _audioSource.Stop();
            _audioSource.PlayOneShot(_speaker2Audio);
        }
    }

    private void DialogueManager_OnLastParagraphTyped(object sender, System.EventArgs e)
    {
        _previousSpeaker = 0;
        _audioSource.Stop();
    }
}
