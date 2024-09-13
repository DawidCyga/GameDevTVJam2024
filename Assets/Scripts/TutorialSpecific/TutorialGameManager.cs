using System;
using UnityEngine;

public class TutorialGameManager : MonoBehaviour
{
    public static TutorialGameManager Instance { get; private set; }

    public enum GameState
    {
        Playing,
        Dialogue,
        PauseMenu,
    }

    [SerializeField] public GameState _gameState;

    [Header("For debugging only")]
    [SerializeField] private int _currentDialogueIndex;
    [SerializeField] private bool _isGamePaused;

    public event EventHandler<OnTimeToStartDialogueEventArgs> OnTimeToStartDialogue;
    public class OnTimeToStartDialogueEventArgs { public int DialogueIndex { get; set; } }

    private void Awake() => Instance = this;
    private void Start() => SubscribeEvents();
    private void OnDestroy() => UnsubscribeEvents();

    private void SubscribeEvents()
    {
        PlayerInputHandler.Instance.OnPauseButtonPressed += PlayerInputHandler_OnPauseButtonPressed;
        PauseMenu.Instance.OnGameResumed += PauseMenu_OnGameResumed;
        DialogueUI.Instance.OnHide += DialogueUI_OnHide;
    }

    private void UnsubscribeEvents()
    {
        PlayerInputHandler.Instance.OnPauseButtonPressed -= PlayerInputHandler_OnPauseButtonPressed;
        PauseMenu.Instance.OnGameResumed -= PauseMenu_OnGameResumed;
        DialogueUI.Instance.OnHide -= DialogueUI_OnHide;
    }

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
    private void DialogueUI_OnHide(object sender, EventArgs e) => ChangeState(GameState.Playing);

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
        }
    }

    private void HandlePlayingState()
    {
        if (Player.Instance.isPaused())
        {
            Player.Instance.Resume();
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
            Player.Instance.Pause();
        }
    }

    private void HandlePauseMenuState()
    {
        if (!_isGamePaused)
        {
            PauseGame();
        }
        DisplayPauseMenuScreen();
    }

    private void ResumeGame()
    {
        Time.timeScale = 1;
        CursorVisibilityHandler.SwitchCursorEnabled(true);
        Player.Instance.Resume();
        _isGamePaused = false;
    }

    private void DisplayPauseMenuScreen() => PauseMenu.Instance.Show();

    private void PauseGame()
    {
        CursorVisibilityHandler.SwitchCursorEnabled(false);
        Time.timeScale = 0f;
        Player.Instance.Pause();
        _isGamePaused = true;
    }

    private void ChangeState(GameState newState) => _gameState = newState;
    public GameState GetCurrentState() => _gameState;

    public void StartDialogue(int dialogueIndex)
    {
        _currentDialogueIndex = dialogueIndex;
        ChangeState(GameState.Dialogue);
        OnTimeToStartDialogue?.Invoke(this, new OnTimeToStartDialogueEventArgs { DialogueIndex = _currentDialogueIndex });
    }
}
