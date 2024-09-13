using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DifficultySelectionMenu : MonoBehaviour
{
    [Header("Buttons References")]
    [SerializeField] private Button _easyButton;
    [SerializeField] private Button _normalButton;
    [SerializeField] private Button _hardButton;
    [Space]
    [SerializeField] private Button _backButton;

    private void Awake()
    {
        BindButtons();
    }
    private void BindButtons()
    {
        _easyButton.onClick.AddListener(() =>
        {
            SaveDifficulty(DifficultyManager.SelectedDifficulty.Easy);

            SetButtonsInteractivityFalse();
            DisableButtonsEventTriggers();

            StartGame();
        });
        _normalButton.onClick.AddListener(() =>
        {
            SaveDifficulty(DifficultyManager.SelectedDifficulty.Normal);

            SetButtonsInteractivityFalse();
            DisableButtonsEventTriggers();

            StartGame();
        });
        _hardButton.onClick.AddListener(() =>
        {
            SaveDifficulty(DifficultyManager.SelectedDifficulty.Hard);

            SetButtonsInteractivityFalse();
            DisableButtonsEventTriggers();

            StartGame();
        });
    }
    private void SaveDifficulty(DifficultyManager.SelectedDifficulty difficulty) => DifficultyManager.Instance.Select(difficulty);
    private void StartGame() => FadeTransitionHandler.Instance.FadeOut(2, LoadNextScene);
    private void LoadNextScene() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

    private void SetButtonsInteractivityFalse()
    {
        _easyButton.interactable = false;
        _normalButton.interactable = false;
        _hardButton.interactable = false;
        _backButton.interactable = false;
    }

    private void DisableButtonsEventTriggers()
    {
        _easyButton.GetComponent<EventTrigger>().enabled = false;
        _normalButton.GetComponent<EventTrigger>().enabled = false;
        _hardButton.GetComponent<EventTrigger>().enabled = false;

        _easyButton.GetComponent<ButtonSoundEmitter>().enabled = false;
        _normalButton.GetComponent<ButtonSoundEmitter>().enabled = false;
        _hardButton.GetComponent<ButtonSoundEmitter>().enabled = false;
        _backButton.GetComponent<ButtonSoundEmitter>().enabled = false;
    }
}
