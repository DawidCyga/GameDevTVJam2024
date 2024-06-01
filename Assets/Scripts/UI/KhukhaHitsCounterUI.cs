using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KhukhaHitsCounterUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _counterText;

    private void Start()
    {
        HitsCounter.Instance.OnKhukhaHitsChanged += HitsCounter_OnKhukhaHitsChanged; ;
    }

    private void OnDestroy()
    {
        HitsCounter.Instance.OnKhukhaHitsChanged -= HitsCounter_OnKhukhaHitsChanged; ;
    }

    private void HitsCounter_OnKhukhaHitsChanged(object sender, HitsCounter.OnKhukhaHitsChangedEventArgs e)
    {
        _counterText.text = e.CurrentKhukhaHits.ToString();
    }   
}
