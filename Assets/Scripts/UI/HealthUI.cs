using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private Transform _healthIconsContainer;
    [SerializeField] private RectTransform _healthIconsContainerRect;

    [SerializeField] private Transform _healthIcon;

    [SerializeField] private List<Transform> _healthIconsList = new List<Transform>();

    private void Start()
    {
        InstantiateHealthIcons();
        HitsCounter.Instance.OnHealthDecreased += HitsCounter_OnHealthDecreased;
        HitsCounter.Instance.OnHealthRestored += HitsCounter_OnHealthRestored;
    }

    private void OnDestroy()
    {
        HitsCounter.Instance.OnHealthDecreased -= HitsCounter_OnHealthDecreased;
        HitsCounter.Instance.OnHealthRestored -= HitsCounter_OnHealthRestored;
    }

    private void InstantiateHealthIcons()
    {
        int iconNumber = HitsCounter.Instance.GetMaxHealth();

        switch (iconNumber)
        {
            case 5:
                _healthIconsContainerRect.sizeDelta = new Vector2(660, 158);
                break;
            case 3:
                _healthIconsContainerRect.sizeDelta = new Vector2(420, 158);
                break;
            case 2:
                _healthIconsContainerRect.sizeDelta = new Vector2(300, 158);
                break;
        }

        Vector3 startPosition = new Vector3(130, 0, 0);
        float spacing = 120f;

        for (int i = 0; i < iconNumber; i++)
        {
            Vector3 localPosition = startPosition + new Vector3(i * spacing, 0, 0);
            Transform healthIconTransform = Instantiate(_healthIcon, _healthIconsContainer);
            healthIconTransform.localPosition = localPosition;

            _healthIconsList.Add(healthIconTransform);
        }
    }

    private void HitsCounter_OnHealthDecreased(object sender, EventArgs e)
    {
        int healtLeft = HitsCounter.Instance.GetCurrentHealth();
        if (healtLeft < 0)
        {
            healtLeft = 0;
        }

        for (int i = healtLeft; i < _healthIconsList.Count; i++)
        {
            Image image = _healthIconsList[i].GetComponent<Image>();
            image.color = Color.black;
        }
    }

    private void HitsCounter_OnHealthRestored(object sender, EventArgs e)
    {
        foreach (Transform healthIcon in _healthIconsList)
        {
            Image image = healthIcon.GetComponent<Image>();
            image.color = Color.white;
        }
    }
}
