using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Pipes;
using UnityEngine;

public class DialogueUI : MonoBehaviour
{
    public static DialogueUI Instance { get; private set; }

    [SerializeField] private string _nextButtonText;
    [SerializeField] private string _forwardText;
    [SerializeField] private string _finishButtonText;

    [Header("Cache References")]
    [SerializeField] private GameObject _dialogueUIContainer;
    [SerializeField] private DialogueManager _dialogueManager;

    [SerializeField] private GameObject _portraitFirst;
    [SerializeField] private GameObject _portraitSecond;

    [SerializeField] private TextInteractionButton _textInteractionButton;

    public event EventHandler OnHide;

    private void Awake()
    {
        Instance = this;
        _dialogueUIContainer.SetActive(false);
    }
    private void Start()
    {
        _dialogueManager.OnStartNewDialogue += DialogueManager_OnStartNewDialogue;
        _dialogueManager.OnStartedNewParagraph += DialogueManager_OnStartedNewParagraph;
        _dialogueManager.OnSingleParagraphTyped += DialogueManager_OnSingleParagraphTyped;
        _dialogueManager.OnLastParagraphTyped += DialogueManager_OnLastParagraphTyped;
    }

    private void OnDestroy()
    {
        _dialogueManager.OnStartNewDialogue -= DialogueManager_OnStartNewDialogue;
        _dialogueManager.OnStartedNewParagraph -= DialogueManager_OnStartedNewParagraph;
        _dialogueManager.OnSingleParagraphTyped -= DialogueManager_OnSingleParagraphTyped;
        _dialogueManager.OnLastParagraphTyped -= DialogueManager_OnLastParagraphTyped;
    }

    private void DialogueManager_OnStartNewDialogue(object sender, System.EventArgs e)
    {
        _dialogueUIContainer?.SetActive(true);
    }

    private void DialogueManager_OnStartedNewParagraph(object sender, DialogueManager.OnStartedNewParagraphEventArgs e)
    {
        _textInteractionButton.UpdateSelf(_forwardText, _dialogueManager.FinishTypingCurrentParagraph);
        UpdatePortraitsDisplay(e.CurrentDialogueSpeaker);
    }

    private void DialogueManager_OnSingleParagraphTyped(object sender, System.EventArgs e)
    {
        _textInteractionButton.UpdateSelf(_nextButtonText, _dialogueManager.StartTypingNextParagraph);
    }

    private void DialogueManager_OnLastParagraphTyped(object sender, System.EventArgs e)
    {
        _textInteractionButton.UpdateSelf(_finishButtonText, Hide);
    }

    private void UpdatePortraitsDisplay(int numberPortraitToDisplay)
    {
        switch (numberPortraitToDisplay)
        {
            case 1:
                _portraitFirst.SetActive(true);
                _portraitSecond.SetActive(false);
                break;
            case 2:
                _portraitFirst.SetActive(false);
                _portraitSecond.SetActive(true);
                break;
            default:
                Debug.LogError("Invalid speaker number provided");
                break;
        }
    }

    private void Hide()
    {
        _dialogueUIContainer.SetActive(false);

        OnHide?.Invoke(this, EventArgs.Empty);
    }

}
