using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BabaiHitsCounterUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _counterText;

    private void Start()
    {
        BabaiHitsCounter.Instance.OnBabaiHitsChanged += BabaiHitsCounter_OnBabaiHitsChanged;
    }

    private void OnDestroy()
    {
        BabaiHitsCounter.Instance.OnBabaiHitsChanged -= BabaiHitsCounter_OnBabaiHitsChanged;

    }

    private void BabaiHitsCounter_OnBabaiHitsChanged(object sender, BabaiHitsCounter.OnBabaiHitsChangedEventArgs e)
    {
        _counterText.text = e.CurrentBabaiHits.ToString();
    }
}
