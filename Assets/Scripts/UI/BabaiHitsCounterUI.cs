using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BabaiHitsCounterUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _counterText;

    private void Start()
    {
        HitsCounter.Instance.OnBabaiHitsChanged += BabaiHitsCounter_OnBabaiHitsChanged;
    }

    private void OnDestroy()
    {
        HitsCounter.Instance.OnBabaiHitsChanged -= BabaiHitsCounter_OnBabaiHitsChanged;

    }

    private void BabaiHitsCounter_OnBabaiHitsChanged(object sender, HitsCounter.OnBabaiHitsChangedEventArgs e)
    {
        _counterText.text = e.CurrentBabaiHits.ToString();
    }
}
