using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameTimerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _gameTimerText;

    private void Update()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        string timeToDisplay = GameTimer.Instance.GetTimeSinceStartedPlaying().ToString("00:00");
        _gameTimerText.text = timeToDisplay;
    }
}
