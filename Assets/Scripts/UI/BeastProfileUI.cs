using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BeastProfileUI : MonoBehaviour
{
    [SerializeField] private Button _backButton;

    [Header("Cache References")]
    [SerializeField] private GameObject _bestiaryContainer;
    [SerializeField] private Button _backFromBestiaryButton;

    [Header("For debugging only")]
    [SerializeField] private TextMeshProUGUI _beastName;
    [SerializeField] private TextMeshProUGUI _killCount;
    [SerializeField] private TextMeshProUGUI _beastDescription;
    [SerializeField] private Image _beastImage;

    private void Start()
    {
        _backButton.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
            _bestiaryContainer.SetActive(true);
            EventSystem.current.SetSelectedGameObject(_backFromBestiaryButton.gameObject);
        });
    }

    public void SetupProfile(string name, int killCount, string description, Sprite image, bool hasKillCount)
    {
        UpdateKillCountVisibility(hasKillCount);
        _beastName.text = name;
        _killCount.text = "Killed: " + killCount;
        _beastDescription.text = description;
        _beastImage.sprite = image;
    }

    private void UpdateKillCountVisibility(bool hasKillCount)
    {
        if (hasKillCount)
        {
            _killCount.gameObject.SetActive(true);
        }
        else
        {
            _killCount.gameObject.SetActive(false);
        }
    }
}
