using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SingleBeastProfileButton : MonoBehaviour
{

    [Header("Profile Setup")]
    [SerializeField] private string _beastName;
    [SerializeField] private string _beastDescription;
    [SerializeField] private Sprite _beastImage;

    [Header("Cache References")]
    [SerializeField] private Button _button;
    [SerializeField] private GameObject _bestiaryContainer;
    [SerializeField] private GameObject _beastProfileDisplayContainer;

    private BeastProfileUI _beastProfileUI;

    private void Start()
    {
        _beastProfileUI = _beastProfileDisplayContainer.GetComponent<BeastProfileUI>();

        _button.onClick.AddListener(() =>
        {
            _bestiaryContainer.SetActive(false);
            _beastProfileDisplayContainer.SetActive(true);
            _beastProfileUI.SetupProfile(_beastName, _beastDescription, _beastImage);
        });
    }
}
