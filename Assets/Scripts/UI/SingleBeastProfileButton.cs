using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SingleBeastProfileButton : MonoBehaviour
{
    [Header("Profile Setup")]
    [SerializeField] private string _beastName;
    [TextArea(3,9)]
    [SerializeField] private string _beastDescription;
    [SerializeField] private Sprite _beastImage;
    [SerializeField] private Enemy.EnemyType _enemyType;

    [Header("Cache References")]
    [SerializeField] private Button _button;
    [SerializeField] private GameObject _bestiaryContainer;
    [SerializeField] private GameObject _beastProfileDisplayContainer;
    [SerializeField] private GameObject _backFromProfileButton;

    private BeastProfileUI _beastProfileUI;

    private void Start()
    {
        _beastProfileUI = _beastProfileDisplayContainer.GetComponent<BeastProfileUI>();

        _button.onClick.AddListener(() =>
        {
            int killCount = BeastKillCounter.Instance.GetCount(_enemyType);
            bool hasKillCount = (_enemyType == Enemy.EnemyType.Story) ? false : true;
            _bestiaryContainer.SetActive(false);
            _beastProfileDisplayContainer.SetActive(true);
           _beastProfileUI.SetupProfile(_beastName, killCount, _beastDescription, _beastImage, hasKillCount);
            EventSystem.current.SetSelectedGameObject(_backFromProfileButton);
        });
    }
}
