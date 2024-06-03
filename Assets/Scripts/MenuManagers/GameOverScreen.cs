using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverScreen : MonoBehaviour
{
    public static GameOverScreen Instance { get; private set; }

    [SerializeField] private Button _playAgainButton;
    [SerializeField] private Button _mainMenuButton;

    [SerializeField] private GameObject _gameOverScreenContainer;

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

        _gameOverScreenContainer.SetActive(false);

        _playAgainButton.onClick.AddListener(() =>
        {
            UnfreezeTime();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        });

        _mainMenuButton.onClick.AddListener(() =>
        {
            UnfreezeTime();
            SceneManager.LoadScene(0);
        });
    }

    private void UnfreezeTime() => Time.timeScale = 1;
    public void Show()
    {
        if (!_isShown)
        {
            _gameOverScreenContainer.SetActive(true);
            EventSystem.current.SetSelectedGameObject(_playAgainButton.gameObject);
            _isShown = true;
        }
    }
}
