using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IntroductionSceneManager : MonoBehaviour
{
    [Header("Content to type")]
    [TextArea(2, 4)]
    [SerializeField] private string[] _textParagraphs;

    [Header("Button text depending on state")]
    [SerializeField] private string _nextButtonText;
    [SerializeField] private string _startGameButtonText;

    [Space]
    [SerializeField] private float _timeToShowNextButton;

    [Header("Cache References")]
    [SerializeField] private TextWriter _textWriter;
    [SerializeField] private GameObject _textInteractionButtonObject;
    [SerializeField] private TextInteractionButton _textInteractionButton;

    [Header("For debugging only")]
    [SerializeField] private float _timeSinceSceneLoaded;

    private void Start()
    {
        _textInteractionButton.UpdateSelf(_nextButtonText, DisplayAllParagraphs);
        _textInteractionButtonObject.SetActive(false);

        _textWriter.OnAllParagraphsTyped += TextWriter_OnAllParagraphsTyped;
        _textWriter.StartTyping(_textParagraphs, false);
    }

    private void Update()
    {
        if (_timeSinceSceneLoaded > _timeToShowNextButton)
        {
            _textInteractionButtonObject.SetActive(true);
        }
        else
        {
            _timeSinceSceneLoaded += Time.deltaTime;
        }
    }

    private void TextWriter_OnAllParagraphsTyped(object sender, EventArgs e)
    {
        _textInteractionButton.UpdateSelf(_startGameButtonText, StartGame);
    }


    private void StartGame() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

    private void DisplayAllParagraphs() => _textWriter.FinishTypingAllParagraphs(_textParagraphs);
}