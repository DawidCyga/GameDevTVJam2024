using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeTransitionHandler : MonoBehaviour
{
    public static FadeTransitionHandler Instance { get; private set; }

    [Header("Configuration of automatic fade in")]
    [SerializeField] private bool _shouldFadeInAutomatically;
    [SerializeField] private float _timeBeforeDefaultFadeIn;
    [SerializeField] private float _defaultFadeInTime;

    private Image _FadeTransitionImage;

    private void Awake()
    {
        Instance = this;
        _FadeTransitionImage = GetComponent<Image>();
    }

    private void Update()
    {
        if (_shouldFadeInAutomatically)
        {
            _FadeTransitionImage.color = Color.black;
            _timeBeforeDefaultFadeIn -= Time.deltaTime;

            if (_timeBeforeDefaultFadeIn < 0)
            {
                FadeInAutomatically();
                _shouldFadeInAutomatically = false;
            }
        }
    }

    private void FadeInAutomatically() => FadeIn(_defaultFadeInTime);
    public void FadeIn(float duration, Action onFinishCallback = null) => StartCoroutine(FadeRoutine(1, 0, duration, onFinishCallback));

    public void FadeOut(float duration, Action onFinishedCallBack = null)
    {
        if (PlayerInputHandler.Instance is not null)
        {
            PlayerInputHandler.Instance.gameObject.SetActive(false);
        }
        StartCoroutine(FadeRoutine(0, 1, duration, onFinishedCallBack));
    }

    private IEnumerator FadeRoutine(float startAlpha, float targetAlpha, float duration, Action onFinishCallback)
    {
        float timeSinceStarted = 0;

        Color fadeColor = new Color(0, 0, 0);

        while (timeSinceStarted < duration)
        {
            timeSinceStarted += Time.deltaTime;

            fadeColor.a = Mathf.Lerp(startAlpha, targetAlpha, timeSinceStarted / duration);
            _FadeTransitionImage.color = fadeColor;

            yield return new WaitForEndOfFrame();
        }

        if (onFinishCallback is not null)
        {
            onFinishCallback();
        }
    }
}
