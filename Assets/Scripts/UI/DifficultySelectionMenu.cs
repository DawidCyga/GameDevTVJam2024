using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DifficultySelectionMenu : MonoBehaviour
{
    [SerializeField] private Button _easyButton;
    [SerializeField] private Button _normalButton;
    [SerializeField] private Button _hardButton;

    private void Awake()
    {
        _easyButton.onClick.AddListener(() =>
        {
            SaveDifficulty(DifficultyManager.SelectedDifficulty.Easy);
            StartGame();
        });
        _normalButton.onClick.AddListener(() =>
        {
            SaveDifficulty(DifficultyManager.SelectedDifficulty.Normal);
            StartGame();
        });
        _hardButton.onClick.AddListener(() =>
        {
            SaveDifficulty(DifficultyManager.SelectedDifficulty.Hard);
            StartGame();
        });

    }
    private void SaveDifficulty(DifficultyManager.SelectedDifficulty difficulty)
    {
        DifficultyManager.Instance.Select(difficulty);

    }
    private void StartGame()
    {
        FadeTransitionHandler.Instance.FadeOut(2, LoadNextScene);
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
