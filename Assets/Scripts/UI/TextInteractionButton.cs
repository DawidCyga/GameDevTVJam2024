using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextInteractionButton : MonoBehaviour
{
    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    private void Start()
    {
        PlayerInputHandler.Instance.OnDialogueInteractionPressed += PlayerInputHandler_OnDialogueInteractionPressed;
    }

    private void PlayerInputHandler_OnDialogueInteractionPressed(object sender, EventArgs e)
    {
        _button.onClick.Invoke();
    }

    public void UpdateSelf(string buttonText, Action buttonFunctionCallBack)
    {
        if (_button is null)
        {
            _button = GetComponent<Button>();
        }

        TextMeshProUGUI textComponent = GetComponentInChildren<TextMeshProUGUI>();
        textComponent.text = buttonText;

        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(() =>
        {
            buttonFunctionCallBack();
        });
        
    }
}
