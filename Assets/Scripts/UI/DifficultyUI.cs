using TMPro;
using UnityEngine;

public class DifficultyUI : MonoBehaviour
{
    public static DifficultyUI Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI _difficultyText;

    private void Awake() => Instance = this;

    public void DisplayDifficulty(string choice) => _difficultyText.text = "Difficulty: " + choice;
}
