using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PoisonUsesUI : MonoBehaviour
{
    [Header("Component Reference")]
    [SerializeField] private TextMeshProUGUI _availableUsesText;
    [SerializeField] private Image _OrbImage;

    [Header("Sprites References")]
    [SerializeField] private Sprite _regularOrbImage;
    [SerializeField] private Sprite _poisonousOrbImage;



    private void Start()
    {
        PoisonOrbsCollector.Instance.OnAvailablePoisonUsesIncreased += (sender, args) => UpdateAvailableUses();
        PoisonOrbsCollector.Instance.OnAvailablePoisonUsesDecreased += (sender, args) => UpdateAvailableUses();
    }

    private void UpdateAvailableUses()
    {
        int availableUses = PoisonOrbsCollector.Instance.GetAvailableUses();

        _availableUsesText.text = availableUses.ToString();

        _OrbImage.sprite = (availableUses == 0) ? _regularOrbImage : _poisonousOrbImage;
    }

}
