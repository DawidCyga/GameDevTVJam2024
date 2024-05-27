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

    private void Awake()
    {
        _gameState = GameState.Playing;
    }

    private void Start()
    {
        PlayerHitBox.Instance.OnPlayerDeath += PlayerHitBox_OnPlayerDeath;
    }

    private void Update()
    {
        switch (_gameState)
        {
            case GameState.Playing:
                SetupGame();
                break;
            case GameState.PauseMenu:
                DisplayPauseMenuScreen();
                break;
            case GameState.GameOver:
                DisplayGameOverScreen();
                break;
            case GameState.GameWin:
                //
                break;
        }
    }

    private void SetupGame()
    {
        Time.timeScale = 1;
    }

    private void DisplayPauseMenuScreen()
    {
        //
    }

    private void DisplayGameOverScreen()
    {
        GameOverScreen.Instance.Show();
        PauseGame();
    }

    private void PlayerHitBox_OnPlayerDeath(object sender, System.EventArgs e)
    {
        _gameState = GameState.GameOver;
    }

    private void PauseGame()
    {
        Time.timeScale = 0f;
        // additional conditions based on what we're going to use in the game
    }
}
