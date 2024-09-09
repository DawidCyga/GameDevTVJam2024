using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DifficultyDescriptionUI : MonoBehaviour
{
    [TextArea(4, 8)][SerializeField] private string _easyDescription;
    [TextArea(4, 8)][SerializeField] private string _normalDescription;
    [TextArea(4, 8)][SerializeField] private string _hardDescription;

    private TextMeshProUGUI _descriptionText;

    private void Awake()
    {
        _descriptionText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void ProcessUpdateDescription(int difficulty)
    {
        switch (difficulty)
        {
            case 0:
                SetNewDescription(_easyDescription);
                break;
            case 1:
                SetNewDescription(_normalDescription);
                break;
            case 2:
                SetNewDescription(_hardDescription);
                break;
        }
    }

    private void SetNewDescription(string description) => _descriptionText.text = description;

}
