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
        float totalSeconds = GameTimer.Instance.GetTimeSinceStartedPlaying();
        int minutes = Mathf.FloorToInt(totalSeconds / 60);
        int seconds = Mathf.FloorToInt(totalSeconds % 60);

        string timeToDisplay = string.Format("{0:00}:{1:00}", minutes, seconds);
        _gameTimerText.text = timeToDisplay;
    }
}
