using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private string _nextButtonText;
    [SerializeField] private string _forwardText;
    [SerializeField] private string _finishButtonText;

    [Header("Cache References")]
    [SerializeField] private GameObject _dialogueUIContainer;
    [SerializeField] private DialogueManager _dialogueManager;

    private TextInteractionButton _textInteractionButton;

    private void Awake()
    {
        _textInteractionButton = GetComponentInChildren<TextInteractionButton>();

        _dialogueUIContainer.SetActive(false);

    }
    private void Start()
    {
        _dialogueManager.OnStartNewDialogue += DialogueManager_OnStartNewDialogue;
        _dialogueManager.OnStartedNewParagraph += DialogueManager_OnStartedNewParagraph;
        _dialogueManager.OnSingleParagraphTyped += DialogueManager_OnSingleParagraphTyped;
        _dialogueManager.OnLastParagraphTyped += DialogueManager_OnLastParagraphTyped;
    }

    private void DialogueManager_OnStartNewDialogue(object sender, System.EventArgs e)
    {
        _dialogueUIContainer?.SetActive(true);

    }

    private void DialogueManager_OnStartedNewParagraph(object sender, System.EventArgs e)
    {
        _textInteractionButton.UpdateSelf(_forwardText, _dialogueManager.FinishTypingCurrentParagraph);
    }

    private void DialogueManager_OnSingleParagraphTyped(object sender, System.EventArgs e)
    {
        _textInteractionButton.UpdateSelf(_nextButtonText, _dialogueManager.StartTypingNextParagraph);
    }

    private void DialogueManager_OnLastParagraphTyped(object sender, System.EventArgs e)
    {
        _textInteractionButton.UpdateSelf(_finishButtonText, Hide);
    }

    private void Hide()
    {
        _dialogueUIContainer.SetActive(false);
    }

}
