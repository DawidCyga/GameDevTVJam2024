using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public enum GameState
    {
        Playing,
        PauseMenu,
        GameOver,
        GameWin
    }

    [SerializeField] public GameState _gameState;

    private bool _isGamePaused;

    private void Awake()
    {
        _gameState = GameState.Playing;
    }

    private void Start()
    {
        PlayerHitBox.Instance.OnPlayerDeath += PlayerHitBox_OnPlayerDeath;
        PlayerInputHandler.Instance.OnPauseButtonPressed += PlayerInputHandler_OnPauseButtonPressed;
        PauseMenu.Instance.OnGameResumed += PauseMenu_OnGameResumed;
    }
    private void PlayerHitBox_OnPlayerDeath(object sender, EventArgs e)
    {
        _gameState = GameState.GameOver;
    }

    private void PlayerInputHandler_OnPauseButtonPressed(object sender, EventArgs e)
    {
        switch (_gameState)
        {
            case GameState.Playing:
                _gameState = GameState.PauseMenu;
                break;
            case GameState.PauseMenu:
                PauseMenu.Instance.Hide();
                _gameState = GameState.Playing;
                break;
            default:
                Debug.Log("CANNOT USE PAUSE AT THIS STATE");
                break;
        }
    }

    private void PauseMenu_OnGameResumed(object sender, EventArgs e)
    {
        _gameState = GameState.Playing;
        Debug.Log("Received: game resumed");
    }

    private void Update()
    {
        switch (_gameState)
        {
            case GameState.Playing:
                if (_isGamePaused)
                {
                    ResumeGame();
                }
                break;
            case GameState.PauseMenu:
                if (!_isGamePaused)
                {
                    PauseGame();
                }
                DisplayPauseMenuScreen();
                break;
            case GameState.GameOver:
                if (!_isGamePaused)
                {
                    PauseGame();
                }
                DisplayGameOverScreen();
                break;
            case GameState.GameWin:
                //
                break;
        }
    }

    private void ResumeGame()
    {
        Time.timeScale = 1;
        _isGamePaused = false;
    }

    private void DisplayPauseMenuScreen()
    {
        PauseMenu.Instance.Show();
    }



    private void DisplayGameOverScreen()
    {
        GameOverScreen.Instance.Show();      
    }

    private void PauseGame()
    {
        Time.timeScale = 0f;
        _isGamePaused = true;
        // additional conditions based on what we're going to use in the game
    }
}
