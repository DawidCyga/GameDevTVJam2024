using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public enum GameState
    {
        Playing,
        Paused,
        GameOver,
        GameWin
    }

    public GameState _gameState;

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
                //
                break;
            case GameState.Paused:
                //
                break;
            case GameState.GameOver:
                //
                break;
            case GameState.GameWin:
                //
                break;
        }
    }

    private void PlayerHitBox_OnPlayerDeath(object sender, System.EventArgs e)
    {
        _gameState = GameState.GameOver;
    }
}
