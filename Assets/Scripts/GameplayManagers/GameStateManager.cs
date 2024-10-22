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

    [Header("Scripted Player Movement Parameters")]
    [SerializeField] private float _autoMovementTimeReturnFromLevel;

    [Header("Fading Between Levels Parameter")]
    [SerializeField] private float _fadeBeforeSceneChangeDuration;

    private int _currentDialogueIndex;
    private bool _isGamePaused;
    private bool _isPlayingWaveUnrelatedDialogue;

    private const int EXIT_SCENE_MONOLOGUE_INDEX = 6;

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

    private void OnDisable() => UnsubscribeEvents();

    private void SceneManager_sceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 3 || scene.buildIndex == 4)
        {
            InitializeForNewLevel();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeForNewLevel()
    {
        ChangeState(GameState.Playing);
        _currentDialogueIndex = 0;
        SubscribeEvents();
        TryStartWaveRelatedDialogue();
    }

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
        if (ExitSceneMonologueTrigger.Instance != null)
            ExitSceneMonologueTrigger.Instance.OnPlayerAttemptsLeavingScene += ExitSceneMonologueTrigger_OnPlayerAttemptsLeavingScene;
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
        if (ExitSceneMonologueTrigger.Instance != null)
            ExitSceneMonologueTrigger.Instance.OnPlayerAttemptsLeavingScene -= ExitSceneMonologueTrigger_OnPlayerAttemptsLeavingScene;
        TreeScript.OnAnyTreeBurned -= TreeScript_OnAnyTreeBurned;
    }

    private void PlayerHitBox_OnPlayerDeath(object sender, EventArgs e) => ChangeState(GameState.GameOver);

    private void PlayerInputHandler_OnPauseButtonPressed(object sender, EventArgs e)
    {
        switch (_gameState)
        {
            case GameState.Playing:
                ChangeState(GameState.PauseMenu);
                break;
            case GameState.PauseMenu:
                PauseMenu.Instance.Hide();
                ChangeState(GameState.Playing);
                break;
        }
    }

    private void PauseMenu_OnGameResumed(object sender, EventArgs e) => ChangeState(GameState.Playing);

    private void WaveSpawner_OnWaveCleared(object sender, WaveSpawner.OnWaveClearedEventArgs e)
    {
        _currentDialogueIndex = e.CurrentWaveIndex;
        TryStartWaveRelatedDialogue();
    }

    private void ExitSceneMonologueTrigger_OnPlayerAttemptsLeavingScene(object sender, EventArgs e) => StartExitSceneMonologue();

    private void TreeScript_OnAnyTreeBurned(object sender, EventArgs e) => ChangeState(GameState.GameOver);

    private void TryStartWaveRelatedDialogue()
    {
        if (_clearedWavesIndexList.Contains(_currentDialogueIndex))
        {
            if (_currentDialogueIndex == WaveSpawner.Instance.GetTotalWaveCount() - 1)
            {
                HandleStartWaveRelatedDialogue();
            }
            else
            {
                HandleStartNextWave();
            }
        }
        else
        {
            HandleStartWaveRelatedDialogue();
            _clearedWavesIndexList.Add(_currentDialogueIndex);
        }
    }

    private void StartExitSceneMonologue()
    {
        HandleStartWaveUnrelatedDialogue(EXIT_SCENE_MONOLOGUE_INDEX);
        _isPlayingWaveUnrelatedDialogue = true;
    }

    private void HandleStartWaveRelatedDialogue() => StartCoroutine(StartDialogueRoutine(_currentDialogueIndex));
    private void HandleStartWaveUnrelatedDialogue(int dialogueIndex) => StartCoroutine(StartDialogueRoutine(dialogueIndex));

    private IEnumerator StartDialogueRoutine(int dialogueIndex, float delayInSeconds = 0.1f)
    {
        yield return new WaitForSeconds(delayInSeconds);
        OnTimeToStartDialogue?.Invoke(this, new OnTimeToStartDialogueEventArgs { DialogueIndex = dialogueIndex });
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
        else if (!_isPlayingWaveUnrelatedDialogue)
        {
            HandleStartNextWave();
        }
        else
        {
            HandleBackFromWaveUnrelatedDialogue();
        }
    }

    private IEnumerator LoadNextSceneRoutine(float delayInSeconds = 0.3f)
    {
        yield return new WaitForSeconds(delayInSeconds);
        FadeTransitionHandler.Instance.FadeOut(_fadeBeforeSceneChangeDuration, LoadNextLevel);
    }

    private void LoadNextLevel() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

    private void HandleStartNextWave()
    {
        WaveSpawner.Instance.StartBreakBeforeNextWave();
        ChangeState(GameState.Playing);
    }

    private void HandleBackFromWaveUnrelatedDialogue()
    {
        _isPlayingWaveUnrelatedDialogue = false;
        Player.Instance.MovePlayerAuto(_autoMovementTimeReturnFromLevel);
        ChangeState(GameState.Playing);
    }

    private void Update()
    {
        switch (_gameState)
        {
            case GameState.Playing:
                HandlePlayingState();
                break;
            case GameState.Dialogue:
                HandleDialogueState();
                break;
            case GameState.PauseMenu:
                HandlePauseMenuState();
                break;
            case GameState.GameOver:
                HandleGameOverState();
                break;
            case GameState.GameWin:
                HandleGameWinState();
                break;
        }
    }

    private void HandlePlayingState()
    {
        if (Player.Instance.isPaused())
        {
            ChangePlayerPaused(false);
        }
        if (_isGamePaused)
        {
            ResumeGame();
        }
    }

    private void HandleDialogueState()
    {
        if (!Player.Instance.isPaused())
        {
            ChangePlayerPaused(true);
        }
    }

    private void HandlePauseMenuState()
    {
        if (!_isGamePaused)
        {
            if (!Player.Instance.isPaused())
            {
                ChangePlayerPaused(true);
            }
            PauseGame();
        }
        DisplayPauseMenuScreen();
    }

    private void HandleGameOverState()
    {
        if (!Player.Instance.isPaused())
        {
            ChangePlayerPaused(true);
        }
        if (!_isGamePaused)
        {
            PauseGame();
        }
        DisplayGameOverScreen();
    }

    private void HandleGameWinState()
    {
        Debug.Log("I'm winning");
    }

    private void ResumeGame()
    {
        CursorVisibilityHandler.SwitchCursorEnabled(false);
        Time.timeScale = 1;
        _isGamePaused = false;
    }

    private void DisplayPauseMenuScreen() => PauseMenu.Instance.Show();
    private void DisplayGameOverScreen() => GameOverScreen.Instance.Show();

    private void PauseGame()
    {
        CursorVisibilityHandler.SwitchCursorEnabled(true);
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
        if (state)
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
