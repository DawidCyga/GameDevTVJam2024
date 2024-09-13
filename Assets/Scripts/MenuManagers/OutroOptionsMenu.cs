using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OutroOptionsMenu : MonoBehaviour
{
    [SerializeField] private Button _playAgainButton;
    [SerializeField] private Button _mainMenuButton;

    private void Awake()
    {
        _mainMenuButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(0);
        });
        _playAgainButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(1);
        });

        EventSystem.current.SetSelectedGameObject(_playAgainButton.gameObject);
    }
}
