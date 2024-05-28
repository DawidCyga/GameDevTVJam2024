using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[Serializable]
public class DialogueSection
{
    [TextArea(2,4)]
    [SerializeField] private string[] _textParagraphs;

    public string[] GetTextParagraphs => _textParagraphs;
}

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private DialogueSection[] _dialogueSections;

    [Header("Cache Reference")]
    [SerializeField] private TextWriter _textWriter;

    public event EventHandler OnStartNewDialogue;
    public event EventHandler OnStartedNewParagraph;
    public event EventHandler OnSingleParagraphTyped;
    public event EventHandler OnLastParagraphTyped;


    private void Start()
    {
        TestingDialogue.Instance.OnTimeToStartDialogue += TestingDialogue_OnTimeToStartDialogue;
        TextWriter.Instance.OnStartedTypingNewParagraph += TextWriter_OnStartedTypingNewParagraph;
        TextWriter.Instance.OnSingleDialogueParagraphTyped += TextWriter_OnSingleDialogueParagraphTyped;
        TextWriter.Instance.OnLastDialogueParagraphTyped += TextWriter_OnLastDialogueParagraphTyped;
    }

    private void TextWriter_OnLastDialogueParagraphTyped(object sender, EventArgs e)
    {
        OnLastParagraphTyped?.Invoke(sender, EventArgs.Empty);
    }

    private void TestingDialogue_OnTimeToStartDialogue(object sender, TestingDialogue.OnTimeToDisplayDialogueEventArgs e)
    {
        _textWriter.StartTyping(_dialogueSections[e.DialogueIdentifier].GetTextParagraphs, true);
        OnStartNewDialogue?.Invoke(this, EventArgs.Empty);
    }

    private void TextWriter_OnStartedTypingNewParagraph(object sender, EventArgs e)
    {
        OnStartedNewParagraph?.Invoke(this, EventArgs.Empty);
    }

    private void TextWriter_OnSingleDialogueParagraphTyped(object sender, EventArgs e)
    {
        OnSingleParagraphTyped?.Invoke(this, EventArgs.Empty);
    }

    public void FinishTypingCurrentParagraph() => TextWriter.Instance.SetShouldFinishTypingCurrentParagraph(true);
    public void StartTypingNextParagraph() => TextWriter.Instance.SetCanType(true);
}
