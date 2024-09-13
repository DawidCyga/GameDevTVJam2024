using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class TextWriter : MonoBehaviour
{
    public static TextWriter Instance { get; private set; }

    [Header("Typing Configuration")]
    [SerializeField] private float _timeToTypeSingleCharacter;
    [SerializeField] private float _timeBetweenParagraphs;

    [Header("Non Dialogue Specific Configuration")]
    [SerializeField] private int _numberOfSpacesBetweenParagraphs;

    [Header("Dialogue Specific Configuration")]
    private bool _canType;

    [Header("Cache reference")]
    [SerializeField] private TextMeshProUGUI _textField;

    private Coroutine _activeTypeRoutine;

    public event EventHandler OnStartedTypingNewParagraph;
    public event EventHandler OnSingleDialogueParagraphTyped;
    public event EventHandler OnLastDialogueParagraphTyped;
    public event EventHandler OnAllParagraphsTyped;

    private bool _shouldFinishTypingCurrentParagraph;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        SetShouldFinishTypingCurrentParagraph(false);
    }

    public void StartTyping(string[] textParagraphs, bool isDialogue)
    {
        ClearTextField();
        if (_activeTypeRoutine == null)
        {
            _activeTypeRoutine = StartCoroutine(TypeTextRoutine(textParagraphs, isDialogue));
        }
    }

    private IEnumerator TypeTextRoutine(string[] textParagraphs, bool isDialogue)
    {
        for (int i = 0; i < textParagraphs.Length; ++i)
        {

            OnStartedTypingNewParagraph?.Invoke(this, EventArgs.Empty);


            for (int j = 0; j < textParagraphs[i].ToCharArray().Length; j++)
            {
                if (_shouldFinishTypingCurrentParagraph)
                {
                    FinishTypingCurrentlyProcessedParagraph(textParagraphs[i]);
                    SetShouldFinishTypingCurrentParagraph(false);
                    break;
                }

                char currentlyProcessedCharacter = textParagraphs[i].ToCharArray()[j];
                _textField.text += currentlyProcessedCharacter;
                yield return new WaitForSeconds(_timeToTypeSingleCharacter);
            }

            if (isDialogue)
            {
                yield return HandleDialogueSpecificLogic(i, textParagraphs.Length);
            }
            else
            {
                yield return HandleNonDialogueSpecificLogic(i, textParagraphs.Length);
            }

        }
        OnAllParagraphsTyped?.Invoke(this, EventArgs.Empty);
        _activeTypeRoutine = null;
    }

    private void FinishTypingCurrentlyProcessedParagraph(string currentlyProcessedParagraph)
    {
        _textField.text = string.Empty;
        _textField.text = currentlyProcessedParagraph;
    }

    private IEnumerator HandleDialogueSpecificLogic(int index, int totalParagraphs)
    {
        if (index == totalParagraphs - 1)
        {
            OnLastDialogueParagraphTyped?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            OnSingleDialogueParagraphTyped?.Invoke(this, EventArgs.Empty);
            _canType = false;

            while (!_canType)
            {
                yield return new WaitForEndOfFrame();
            }
            ClearTextField();
        }
    }

    private IEnumerator HandleNonDialogueSpecificLogic(int index, int totalParagraphs)
    {
        if (index < totalParagraphs - 1)
        {
            AddSpacesBetweenParagraphs();
            yield return new WaitForSeconds(_timeBetweenParagraphs);
        }
        else
        {
            yield return new WaitForEndOfFrame();
        }
    }

    private void AddSpacesBetweenParagraphs()
    {
        int spacesAdded = 0;
        while (_numberOfSpacesBetweenParagraphs > spacesAdded)
        {
            _textField.text += "\n";
            spacesAdded++;
        }
    }

    public void FinishTypingAllParagraphs(string[] textParagraphs)
    {
        if (_activeTypeRoutine is not null)
        {
            StopCoroutine(_activeTypeRoutine);
        }
        _activeTypeRoutine = null;
        ClearTextField();

        for (int i = 0; i < textParagraphs.Length; ++i)
        {
            _textField.text += textParagraphs[i];

            if (i < textParagraphs.Length - 1)
            {
                AddSpacesBetweenParagraphs();
            }
        }
        OnAllParagraphsTyped?.Invoke(this, EventArgs.Empty);
    }

    private void ClearTextField() => _textField.text = string.Empty;
    public void SetShouldFinishTypingCurrentParagraph(bool state) => _shouldFinishTypingCurrentParagraph = state;  
    public void SetCanType(bool state) => _canType = state;
}
