using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[Serializable]
public class DialogueSection
{
    [SerializeField] private SingleParagraph[] _textParagraphs;

    public string[] GetTextParagraphsContents()
    {
        string[] content = new string[_textParagraphs.Length];

        for (int i = 0; i < _textParagraphs.Length; i++)
        {
            content[i] = _textParagraphs[i].GetText();
        }
        return content;
    }

    public SingleParagraph GetTextParagraph(int index) => _textParagraphs[index];
}

[Serializable]
public class SingleParagraph
{
    [TextArea(2, 4)]
    [SerializeField] private string _text;
    [SerializeField] private int _speakerNumber;

    public string GetText() => _text;
    public int GetSpeakerNumber() => _speakerNumber;
}

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private DialogueSection[] _dialogueSections;

    [SerializeField] private int _currentDialogueSectionIndex;
    [SerializeField] private int _currentDialogueParagraphIndex;
    [SerializeField] private int _currentDialogueSpeaker;

    [Header("Cache Reference")]
    [SerializeField] private TextWriter _textWriter;

    public event EventHandler OnStartNewDialogue;
    public event EventHandler<OnStartedNewParagraphEventArgs> OnStartedNewParagraph;
    public class OnStartedNewParagraphEventArgs { public int CurrentDialogueSpeaker { get; set; } }
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
        _currentDialogueSectionIndex = e.DialogueIndex;
        _currentDialogueParagraphIndex = 0;
        _textWriter.StartTyping(_dialogueSections[_currentDialogueSectionIndex].GetTextParagraphsContents(), true);
        OnStartNewDialogue?.Invoke(this, EventArgs.Empty);
    }

    private void TextWriter_OnStartedTypingNewParagraph(object sender, EventArgs e)
    {
        UpdateCurrentDialogueSpeaker();
        OnStartedNewParagraph?.Invoke(this, new OnStartedNewParagraphEventArgs { CurrentDialogueSpeaker = _currentDialogueSpeaker });
        _currentDialogueParagraphIndex++;
    }

    private void TextWriter_OnSingleDialogueParagraphTyped(object sender, EventArgs e)
    {
        OnSingleParagraphTyped?.Invoke(this, EventArgs.Empty);
    }

    private void UpdateCurrentDialogueSpeaker()
    {
        _currentDialogueSpeaker = _dialogueSections[_currentDialogueSectionIndex].GetTextParagraph(_currentDialogueParagraphIndex).GetSpeakerNumber();
    }

    public void FinishTypingCurrentParagraph() => TextWriter.Instance.SetShouldFinishTypingCurrentParagraph(true);
    public void StartTypingNextParagraph() => TextWriter.Instance.SetCanType(true);
}
