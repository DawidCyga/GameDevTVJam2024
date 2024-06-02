using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DifficultyUI : MonoBehaviour
{
    public static DifficultyUI Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    [SerializeField] private TextMeshProUGUI _difficultyText;

    public void DisplayDifficulty(string choice) => _difficultyText.text = "Difficulty: " + choice;
}
