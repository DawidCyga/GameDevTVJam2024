using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextWriter : MonoBehaviour
{
    public static TextWriter Instance { get; private set; }

    [TextArea(2, 4)]
    [SerializeField] private string[] _textParagraphs;
    //[SerializeField] private int _currentParagraphIndex;

    [SerializeField] private float _timeToTypeSingleCharacter;
    [SerializeField] private float _timeBetweenParagraphs;
    [SerializeField] private int _numberOfSpacesBetweenParagraphs;

    [SerializeField] private TextMeshProUGUI _textField;

    private Coroutine _activeTypeRoutine;

    public event EventHandler OnAllParagraphsTyped;

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

        //_currentParagraphIndex = 0;
    }

    public void StartTyping()
    {
        if (_activeTypeRoutine == null)
        {
            _activeTypeRoutine = StartCoroutine(TypeTextRoutine());
        }
    }

    private IEnumerator TypeTextRoutine()
    {
        for (int i = 0; i < _textParagraphs.Length; ++i)
        {
            for (int j = 0; j < _textParagraphs[i].ToCharArray().Length; j++)
            {
                char currentlyProcessedCharacter = _textParagraphs[i].ToCharArray()[j];
                _textField.text += currentlyProcessedCharacter;
                yield return new WaitForSeconds(_timeToTypeSingleCharacter);
            }

            if (i < _textParagraphs.Length - 1)
            {
                AddSpacesBetweenParagraphs();
                yield return new WaitForSeconds(_timeBetweenParagraphs);
            }
            else
            {
                yield return new WaitForEndOfFrame();
            }
        }
        OnAllParagraphsTyped?.Invoke(this, EventArgs.Empty);
        Debug.Log("Typed all");
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

    public void DisplayAllParagraphs()
    {
        if (_activeTypeRoutine is not null)
        {
            StopCoroutine(_activeTypeRoutine);
        }
        _activeTypeRoutine = null;
        ClearTextField();

        for (int i = 0; i < _textParagraphs.Length; ++i)
        {
            _textField.text += _textParagraphs[i];

            if (i < _textParagraphs.Length - 1)
            {
                AddSpacesBetweenParagraphs();
            }
        }
        OnAllParagraphsTyped?.Invoke(this, EventArgs.Empty);
    }

    private void ClearTextField() => _textField.text = string.Empty;

}
