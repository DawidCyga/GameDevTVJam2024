using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        PlayerInputHandler.Instance.OnPauseButtonPressed += PlayerInputHandler_OnPauseButtonPressed;
        PauseMenu.Instance.OnGameResumed += PauseMenu_OnGameResumed;
        DialogueUI.Instance.OnHide += DialogueUI_OnHide;
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

    private void DialogueUI_OnHide(object sender, EventArgs e)
    {
        ChangeState(GameState.Playing);
    }

    private void Update()
    {
        switch (_gameState)
        {
            case GameState.Playing:
                if (Player.Instance.isPaused())
                {
                    Player.Instance.Resume();
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
                        Player.Instance.Pause();
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
        }
    }
    
    private void ResumeGame()
    {
        Time.timeScale = 1;
        _isGamePaused = false;
    }

    private void DisplayPauseMenuScreen() => PauseMenu.Instance.Show();

    private void PauseGame()
    {
        Time.timeScale = 0f;
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
