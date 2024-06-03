using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuStateManager : MonoBehaviour
{
    private enum MainMenuState
    {
        MainMenu,
        Difficulty,
        Options,
        Bestiary,
        Credits
    }

    private MainMenuState _mainMenuState;

    private Dictionary<MainMenuState, GameObject> _mainMenuStateTransformDictionary;

    [Header("Main Window Buttons")]
    [SerializeField] private Button _startButton;
    //   [SerializeField] private Button _optionsButton;
    [SerializeField] private Button _bestiaryButton;
    [SerializeField] private Button _creditsButton;

    [Header("Sub-windows Back Buttons")]
    [SerializeField] private Button _backFromDifficultyButton;
    [SerializeField] private Button _backFromOptionsButton;
    [SerializeField] private Button _backFromBestiaryButton;
    [SerializeField] private Button _backFromCreditsButton;

    [Header("Windows")]
    [SerializeField] private GameObject _mainMenuContainer;
    [SerializeField] private GameObject _difficultyContainer;
    //   [SerializeField] private GameObject _optionsContainer;
    [SerializeField] private GameObject _bestiaryContainer;
    [SerializeField] private GameObject _creditsContainer;

    private void Awake()
    {
        _mainMenuStateTransformDictionary = new Dictionary<MainMenuState, GameObject>

        {
            {MainMenuState.MainMenu, _mainMenuContainer },
            {MainMenuState.Difficulty, _difficultyContainer },
            {MainMenuState.Bestiary, _bestiaryContainer },
            {MainMenuState.Credits, _creditsContainer }
        };

        _mainMenuState = MainMenuState.MainMenu;

        _startButton.onClick.AddListener(() =>
        {
            ChangeState(MainMenuState.Difficulty);
            UpdateEventSystemCurrent(_backFromDifficultyButton.gameObject);
        });

        _bestiaryButton.onClick.AddListener(() =>
        {
            ChangeState(MainMenuState.Bestiary);
            UpdateEventSystemCurrent(_backFromBestiaryButton.gameObject);
            
        });
        _creditsButton.onClick.AddListener(() =>
        {
            ChangeState(MainMenuState.Credits);
            UpdateEventSystemCurrent(_backFromCreditsButton.gameObject);
        });
        _backFromDifficultyButton.onClick.AddListener(() =>
        {
            ChangeState();
            UpdateEventSystemCurrent();
        });
        _backFromBestiaryButton.onClick.AddListener(() =>
        {
            ChangeState();
            UpdateEventSystemCurrent();

        });
        _backFromCreditsButton.onClick.AddListener(() =>
        {
            ChangeState();
            UpdateEventSystemCurrent();
        });
    }

    private void Start()
    {
        UpdateEventSystemCurrent(_startButton.gameObject);
    }

    private void ChangeState() => ChangeState(MainMenuState.MainMenu);
    private void ChangeState(MainMenuState state)
    {
        _mainMenuStateTransformDictionary[_mainMenuState].SetActive(false);
        _mainMenuState = state;
        _mainMenuStateTransformDictionary[_mainMenuState].SetActive(true);
    }
    private void UpdateEventSystemCurrent() => UpdateEventSystemCurrent(_startButton.gameObject);
    private void UpdateEventSystemCurrent(GameObject selected) => EventSystem.current.SetSelectedGameObject(selected);

}
