using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
            Time.timeScale = 1;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        });

        _mainMenuButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(0);
        });
    }

    private void ResumeGame()
    {
        OnGameResumed?.Invoke(this, EventArgs.Empty);
        Hide();
    }

    public void Hide()
    {
        _pauseMenuScreenContainer.SetActive(false);
    }

    public void Show()
    {
        _pauseMenuScreenContainer.SetActive(true);
    }

   
}
