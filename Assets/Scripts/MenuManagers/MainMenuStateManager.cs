using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuStateManager : MonoBehaviour
{
    private enum MainMenuState
    {
        MainMenu,
        Options,
        Achievements,
        Credits
    }

    private MainMenuState _mainMenuState;

    private Dictionary<MainMenuState, GameObject> _mainMenuStateTransformDictionary;

    [Header("Main Window Buttons")]
    [SerializeField] private Button _startButton;
    [SerializeField] private Button _optionsButton;
    [SerializeField] private Button _achievementsButton;
    [SerializeField] private Button _creditsButton;

    [Header("Sub-windows Back Buttons")]
    [SerializeField] private Button _backFromOptionsButton;
    [SerializeField] private Button _backFromAchievementsButton;
    [SerializeField] private Button _backFromCreditsButton;

    [Header("Windows")]
    [SerializeField] private GameObject _mainMenuContainer;
    [SerializeField] private GameObject _optionsContainer;
    [SerializeField] private GameObject _achievementsContainer;
    [SerializeField] private GameObject _creditsContainer;

    private void Awake()
    {
        _mainMenuStateTransformDictionary = new Dictionary<MainMenuState, GameObject>

        {
            {MainMenuState.MainMenu, _mainMenuContainer },
            {MainMenuState.Options, _optionsContainer },
            {MainMenuState.Achievements, _achievementsContainer },
            {MainMenuState.Credits, _creditsContainer }
        };

        _mainMenuState = MainMenuState.MainMenu;

        _startButton.onClick.AddListener(() =>
        {
            FadeTransitionHandler.Instance.FadeOut(2, LoadNextScene);
        });
        _optionsButton.onClick.AddListener(() =>
        {
            ChangeState(MainMenuState.Options);
        });
        _achievementsButton.onClick.AddListener(() =>
        {
            ChangeState(MainMenuState.Achievements);
        });
        _creditsButton.onClick.AddListener(() =>
        {
            ChangeState(MainMenuState.Credits);
        });

        _backFromOptionsButton.onClick.AddListener(() =>
        {
            ChangeState();
        });
        _backFromAchievementsButton.onClick.AddListener(() =>
        {
            ChangeState();
        });
        _backFromCreditsButton.onClick.AddListener(() =>
        {
            ChangeState();
        });
    }

    //To be extracted into SceneManager class if time allows
    private void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private void ChangeState()
    {
        ChangeState(MainMenuState.MainMenu);
    }
    private void ChangeState(MainMenuState state)
    {
        _mainMenuStateTransformDictionary[_mainMenuState].SetActive(false);
        _mainMenuState = state;
        _mainMenuStateTransformDictionary[_mainMenuState].SetActive(true);
    }
}
