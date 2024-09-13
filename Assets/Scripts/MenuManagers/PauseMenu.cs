using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Instance { get; private set; }

    public event EventHandler OnGameResumed;

    [SerializeField] private Button _resumeButton;
    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _mainMenuButton;

    [SerializeField] private GameObject _pauseMenuScreenContainer;

    private bool _isShown;

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

        _pauseMenuScreenContainer.SetActive(false);

        _resumeButton.onClick.AddListener(() =>
        {
            ResumeGame();
        });

        _restartButton.onClick.AddListener(() =>
        {
            UnfreezeTime();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            EventSystem.current.SetSelectedGameObject(null);

        });

        _mainMenuButton.onClick.AddListener(() =>
        {
            UnfreezeTime();
            SceneManager.LoadScene(0);
            EventSystem.current.SetSelectedGameObject(null);
        });
    }

    private void ResumeGame()
    {
        OnGameResumed?.Invoke(this, EventArgs.Empty);
        Hide();
    }

    private void UnfreezeTime() => Time.timeScale = 1;
    public void Hide()
    {
        _isShown = false;
        EventSystem.current.SetSelectedGameObject(null);
        _pauseMenuScreenContainer.SetActive(false);
    }
    public void Show()
    {
        _pauseMenuScreenContainer.SetActive(true);
        if (!_isShown)
        {
            EventSystem.current.SetSelectedGameObject(_resumeButton.gameObject);
            _isShown = true;
        }
    }
}
