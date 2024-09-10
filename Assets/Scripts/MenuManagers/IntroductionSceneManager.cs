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

    [Header("Fade transition configuration")]
    [SerializeField] private float _fadeInDuration;
    [SerializeField] private float _fadeOutDuration;

    [Header("Cache References")]
    [SerializeField] private TextWriter _textWriter;
    [SerializeField] private GameObject _textInteractionButtonObject;
    [SerializeField] private TextInteractionButton _textInteractionButton;

    [Header("For debugging only")]
    [SerializeField] private float _timeSinceSceneLoaded;

    private void Start()
    {
        CursorVisibilityHandler.SwitchCursorEnabled(false);

        _textInteractionButton.UpdateSelf(_nextButtonText, DisplayAllParagraphs);
        _textInteractionButtonObject.SetActive(false);

        _textWriter.OnAllParagraphsTyped += TextWriter_OnAllParagraphsTyped;
        FadeTransitionHandler.Instance.FadeIn(_fadeInDuration, StartTypingIntroduction);
    }

    private void OnDestroy()
    {
        _textWriter.OnAllParagraphsTyped -= TextWriter_OnAllParagraphsTyped;
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
        _textInteractionButton.UpdateSelf(_startGameButtonText, StartSceneTransitionSequence);
    }
    private void StartTypingIntroduction() => _textWriter.StartTyping(_textParagraphs, false);
    private void StartSceneTransitionSequence() => FadeTransitionHandler.Instance.FadeOut(_fadeOutDuration, StartGame);
    private void StartGame() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    private void DisplayAllParagraphs() => _textWriter.FinishTypingAllParagraphs(_textParagraphs);
}
