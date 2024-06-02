using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BeastProfileUI : MonoBehaviour
{
    [SerializeField] private Button _backButton;

    [Header("Cache References")]
    [SerializeField] private GameObject _bestiaryContainer;

    [Header("For debugging only")]
    [SerializeField] private TextMeshProUGUI _beastName;
    [SerializeField] private TextMeshProUGUI _beastDescription;
    [SerializeField] private Image _beastImage;

    private void Start()
    {
        _backButton.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
            _bestiaryContainer.SetActive(true);
        });
    }

    public void SetupProfile(string name, string description, Sprite image)
    {
        _beastName.text = name;
        _beastDescription.text = description;
        _beastImage.sprite = image;
    }
}
