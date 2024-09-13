using UnityEngine;

public class OutroSceneManager : MonoBehaviour
{
    [Header("Content to type")]
    [TextArea(2, 4)]
    [SerializeField] private string[] _textParagraphs;

    [Header("Fade transition configuration")]
    [SerializeField] private float _fadeInDuration;
    [SerializeField] private float _fadeOutDuration;

    [Header("Cache References")]
    [SerializeField] private TextWriter _textWriter;

    [Header("For debugging only")]
    [SerializeField] private float _timeSinceSceneLoaded;

    private void Start()
    {
        CursorVisibilityHandler.SwitchCursorEnabled(true);
        FadeTransitionHandler.Instance.FadeIn(_fadeInDuration, StartTypingIntroduction);
    }
    private void StartTypingIntroduction() => _textWriter.StartTyping(_textParagraphs, false);
}
