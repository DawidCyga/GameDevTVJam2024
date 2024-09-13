using System;
using UnityEngine;

[Serializable]
public class DialogueSection
{
    [SerializeField] private string _sectionName;
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
    public static DialogueManager Instance { get; private set; }

    [SerializeField] private DialogueSection[] _dialogueSections;

    [Header("For debugging only")]
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

    private void Awake() => Instance = this; 

    private void Start()
    {
        if (TutorialGameManager.Instance is not null)
        {
            TutorialGameManager.Instance.OnTimeToStartDialogue += TutorialGameManager_OnTimeToStartDialogue;
        }
        if (GameStateManager.Instance is not null)
        {
            GameStateManager.Instance.OnTimeToStartDialogue += GameStateManager_OnTimeToStartDialogue;
        }
        TextWriter.Instance.OnStartedTypingNewParagraph += TextWriter_OnStartedTypingNewParagraph;
        TextWriter.Instance.OnSingleDialogueParagraphTyped += TextWriter_OnSingleDialogueParagraphTyped;
        TextWriter.Instance.OnLastDialogueParagraphTyped += TextWriter_OnLastDialogueParagraphTyped;
    }

    private void OnDestroy()
    {
        if (TutorialGameManager.Instance is not null)
        {
            TutorialGameManager.Instance.OnTimeToStartDialogue -= TutorialGameManager_OnTimeToStartDialogue;
        }
        if (GameStateManager.Instance is not null)
        {
            GameStateManager.Instance.OnTimeToStartDialogue -= GameStateManager_OnTimeToStartDialogue;
        }
        TextWriter.Instance.OnStartedTypingNewParagraph -= TextWriter_OnStartedTypingNewParagraph;
        TextWriter.Instance.OnSingleDialogueParagraphTyped -= TextWriter_OnSingleDialogueParagraphTyped;
        TextWriter.Instance.OnLastDialogueParagraphTyped -= TextWriter_OnLastDialogueParagraphTyped;
    }

    private void TutorialGameManager_OnTimeToStartDialogue(object sender, TutorialGameManager.OnTimeToStartDialogueEventArgs e)
    {
        if(Player.Instance is not null)
        {
            Player.Instance.Pause();
        }
        StartDialogue(e.DialogueIndex);
    }

    private void GameStateManager_OnTimeToStartDialogue(object sender, GameStateManager.OnTimeToStartDialogueEventArgs e) => StartDialogue(e.DialogueIndex);

    private void TextWriter_OnStartedTypingNewParagraph(object sender, EventArgs e)
    {
        UpdateCurrentDialogueSpeaker();
        OnStartedNewParagraph?.Invoke(this, new OnStartedNewParagraphEventArgs { CurrentDialogueSpeaker = _currentDialogueSpeaker });
        _currentDialogueParagraphIndex++;
    }

    private void TextWriter_OnSingleDialogueParagraphTyped(object sender, EventArgs e) => OnSingleParagraphTyped?.Invoke(this, EventArgs.Empty);

    private void TextWriter_OnLastDialogueParagraphTyped(object sender, EventArgs e)
    {
        OnLastParagraphTyped?.Invoke(sender, EventArgs.Empty);
        if (Player.Instance is not null)
        {
            Player.Instance.Resume();
        }
    }

    private void StartDialogue(int dialogueIndex)
    {
        _currentDialogueSectionIndex = dialogueIndex;
        _currentDialogueParagraphIndex = 0;
        _textWriter.StartTyping(_dialogueSections[_currentDialogueSectionIndex].GetTextParagraphsContents(), true);
        OnStartNewDialogue?.Invoke(this, EventArgs.Empty);
    }

    private void UpdateCurrentDialogueSpeaker() => _currentDialogueSpeaker = _dialogueSections[_currentDialogueSectionIndex].GetTextParagraph(_currentDialogueParagraphIndex).GetSpeakerNumber();
    public void FinishTypingCurrentParagraph() => TextWriter.Instance.SetShouldFinishTypingCurrentParagraph(true);
    public void StartTypingNextParagraph() => TextWriter.Instance.SetCanType(true);
    public int GetCurrentDialogueParagraphIndex() => _currentDialogueParagraphIndex;
}
