using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    public enum GameState
    {
        Playing,
        Dialogue,
        PauseMenu,
        GameOver,
        GameWin
    }

    [SerializeField] public GameState _gameState;

    private List<int> _clearedWavesIndexList = new List<int>();
    private int _currentDialogueIndex;

    private bool _isGamePaused;

    public event EventHandler<OnTimeToStartDialogueEventArgs> OnTimeToStartDialogue;
    public class OnTimeToStartDialogueEventArgs { public int DialogueIndex { get; set; } }

    public event EventHandler<OnGameStateChangedEventArgs> OnGameStateChanged;
    public class OnGameStateChangedEventArgs { public GameState GameState { get; set; } }

    private void Awake()
    {
        Instance = this;
        _currentDialogueIndex = 0;
    }

    private void Start()
    {
        PlayerHitBox.Instance.OnPlayerDeath += PlayerHitBox_OnPlayerDeath;
        PlayerInputHandler.Instance.OnPauseButtonPressed += PlayerInputHandler_OnPauseButtonPressed;
        PauseMenu.Instance.OnGameResumed += PauseMenu_OnGameResumed;
        WaveSpawner.Instance.OnWaveCleared += WaveSpawner_OnWaveCleared;
        DialogueUI.Instance.OnHide += DialogueUI_OnHide;

        HandleStartDialogue();
    }

    private void OnDestroy()
    {
        PlayerHitBox.Instance.OnPlayerDeath -= PlayerHitBox_OnPlayerDeath;
        PlayerInputHandler.Instance.OnPauseButtonPressed -= PlayerInputHandler_OnPauseButtonPressed;
        PauseMenu.Instance.OnGameResumed -= PauseMenu_OnGameResumed;
        WaveSpawner.Instance.OnWaveCleared -= WaveSpawner_OnWaveCleared;
        DialogueUI.Instance.OnHide -= DialogueUI_OnHide;
    }

    private void PlayerHitBox_OnPlayerDeath(object sender, EventArgs e)
    {
        ChangeState(GameState.GameOver);
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
                ChangeState(GameState.Playing);
                break;
            default:
                Debug.Log("CANNOT USE PAUSE AT THIS STATE");
                break;
        }
    }

    private void PauseMenu_OnGameResumed(object sender, EventArgs e)
    {
        ChangeState(GameState.Playing);
    }

    private void WaveSpawner_OnWaveCleared(object sender, WaveSpawner.OnWaveClearedEventArgs e)
    {
        _currentDialogueIndex = e.CurrentWaveIndex;

        if (_clearedWavesIndexList.Contains(_currentDialogueIndex))
        {
            if (_currentDialogueIndex == _clearedWavesIndexList.Count - 1)
            {
                // START WIN GAME
                HandleStartDialogue();
                Debug.Log("Game State Manager starts last dialogue");
            }
            else
            {
                Debug.Log("Game State Manager already have this state");
                HandleStartNextWave();
            }
        }
        else
        {
            Debug.Log("Game State Manager don't yet have this wave. Start dialogue");
            HandleStartDialogue();
            _clearedWavesIndexList.Add(_currentDialogueIndex);
        }
    }

    private void HandleStartDialogue()
    {
        Debug.Log("Send event");
        OnTimeToStartDialogue?.Invoke(this, new OnTimeToStartDialogueEventArgs { DialogueIndex = _currentDialogueIndex });
        ChangeState(GameState.Dialogue);
    }

    private void DialogueUI_OnHide(object sender, EventArgs e)
    {
        if (_currentDialogueIndex == WaveSpawner.Instance.GetTotalWaveCount())
        {
            ChangeState(GameState.GameWin);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            Debug.Log("Game Win");
        }
        HandleStartNextWave();
    }

    private void HandleStartNextWave()
    {
        WaveSpawner.Instance.StartBreakBeforeNextWave();
        ChangeState(GameState.Playing);
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
            case GameState.Dialogue:
                {
                    //Decide what to pause
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

    private void ChangeState(GameState newState)
    {
        _gameState = newState;
        OnGameStateChanged?.Invoke(this, new OnGameStateChangedEventArgs { GameState = newState });
    }
}
