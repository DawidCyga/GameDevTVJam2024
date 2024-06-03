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

    [SerializeField] private List<int> _clearedWavesIndexList = new List<int>();
    private int _currentDialogueIndex;

    private bool _isGamePaused;

    public event EventHandler<OnTimeToStartDialogueEventArgs> OnTimeToStartDialogue;
    public class OnTimeToStartDialogueEventArgs { public int DialogueIndex { get; set; } }

    public event EventHandler<OnGameStateChangedEventArgs> OnGameStateChanged;
    public class OnGameStateChangedEventArgs { public GameState GameState { get; set; } }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {

        if (arg0.buildIndex == 3 || arg0.buildIndex == 4)
        {
            ChangeState(GameState.Playing);
            _currentDialogueIndex = 0;
            SubscribeEvents();
            TryStartDialogue();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDisable() => UnsubscribeEvents();

    private void SubscribeEvents()
    {
        if (PlayerHitBox.Instance != null)
            PlayerHitBox.Instance.OnPlayerDeath += PlayerHitBox_OnPlayerDeath;
        if (PlayerInputHandler.Instance != null)
            PlayerInputHandler.Instance.OnPauseButtonPressed += PlayerInputHandler_OnPauseButtonPressed;
        if (PauseMenu.Instance != null)
            PauseMenu.Instance.OnGameResumed += PauseMenu_OnGameResumed;
        if (WaveSpawner.Instance != null)
            WaveSpawner.Instance.OnWaveCleared += WaveSpawner_OnWaveCleared;
        if (DialogueUI.Instance != null)
            DialogueUI.Instance.OnHide += DialogueUI_OnHide;
        TreeScript.OnAnyTreeBurned += TreeScript_OnAnyTreeBurned;
    }

    private void UnsubscribeEvents()
    {
        SceneManager.sceneLoaded -= SceneManager_sceneLoaded;

        if (PlayerHitBox.Instance != null)
            PlayerHitBox.Instance.OnPlayerDeath -= PlayerHitBox_OnPlayerDeath;
        if (PlayerInputHandler.Instance != null)
            PlayerInputHandler.Instance.OnPauseButtonPressed -= PlayerInputHandler_OnPauseButtonPressed;
        if (PauseMenu.Instance != null)
            PauseMenu.Instance.OnGameResumed -= PauseMenu_OnGameResumed;
        if (WaveSpawner.Instance != null)
            WaveSpawner.Instance.OnWaveCleared -= WaveSpawner_OnWaveCleared;
        if (DialogueUI.Instance != null)
            DialogueUI.Instance.OnHide -= DialogueUI_OnHide;
        TreeScript.OnAnyTreeBurned -= TreeScript_OnAnyTreeBurned;
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

        TryStartDialogue();
    }

    private void TreeScript_OnAnyTreeBurned(object sender, EventArgs e)
    {
        _gameState = GameState.GameOver;
    }

    private void TryStartDialogue()
    {
        if (_clearedWavesIndexList.Contains(_currentDialogueIndex))
        {
            if (_currentDialogueIndex == WaveSpawner.Instance.GetTotalWaveCount() - 1)
            {
                // START WIN GAME
                HandleStartDialogue();
            }
            else
            {
                HandleStartNextWave();
            }
        }
        else
        {
            HandleStartDialogue();
            _clearedWavesIndexList.Add(_currentDialogueIndex);
        }
    }

    private void HandleStartDialogue() => StartCoroutine(StartDialogueRoutine());

    private IEnumerator StartDialogueRoutine(float delayInSeconds = 0.1f)
    {
        yield return new WaitForSeconds(delayInSeconds);

        OnTimeToStartDialogue?.Invoke(this, new OnTimeToStartDialogueEventArgs { DialogueIndex = _currentDialogueIndex });

        ChangeState(GameState.Dialogue);
    }

    private void DialogueUI_OnHide(object sender, EventArgs e)
    {
        if (_currentDialogueIndex == WaveSpawner.Instance.GetTotalWaveCount())
        {
            ChangeState(GameState.GameWin);
            _clearedWavesIndexList.Clear();
            StartCoroutine(LoadNextSceneRoutine());
        }
        HandleStartNextWave();
    }

    private IEnumerator LoadNextSceneRoutine(float delayInSeconds = 0.3f)
    {
        yield return new WaitForSeconds(delayInSeconds);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
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
                if (Player.Instance.isPaused())
                {
                    ChangePlayerPaused(false);
                }
                if (_isGamePaused)
                {
                    ResumeGame();
                }
                break;
            case GameState.Dialogue:
                {
                    if (!Player.Instance.isPaused())
                    {
                        ChangePlayerPaused(true);
                    }
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
                if (!Player.Instance.isPaused())
                {
                    ChangePlayerPaused(true);
                }
                if (!_isGamePaused)
                {
                    PauseGame();
                }
                DisplayGameOverScreen();
                break;
            case GameState.GameWin:
                Debug.Log("I'm winning");
                break;
        }
    }

    private void ResumeGame()
    {
        Time.timeScale = 1;
        _isGamePaused = false;
    }

    private void DisplayPauseMenuScreen() => PauseMenu.Instance.Show();

    private void DisplayGameOverScreen() => GameOverScreen.Instance.Show();

    private void PauseGame()
    {
        Time.timeScale = 0f;
        _isGamePaused = true;
    }

    private void ChangeState(GameState newState)
    {
        _gameState = newState;
        OnGameStateChanged?.Invoke(this, new OnGameStateChangedEventArgs { GameState = newState });
    }

    private void ChangePlayerPaused(bool state)
    {
        if (state == true)
        {
            Player.Instance.Pause();
        }
        else
        {
            Player.Instance.Resume();
        }
    }

    public GameState GetCurrentGameState() => _gameState;
}
