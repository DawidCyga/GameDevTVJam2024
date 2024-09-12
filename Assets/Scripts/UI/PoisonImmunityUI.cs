using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PoisonImmunityUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _counterContiner;
    [SerializeField] private TextMeshProUGUI _counterText;

    [Header("Counter Setup - for debugging only")]
    [SerializeField] private float _immunityCounter;

    [Header("State")]
    private bool _isShowing;

    private void Start()
    {
        DashAbility.Instance.OnPerformedPoisonDash += DashAbility_OnPerformedPoisonDash;
    }

    private void DashAbility_OnPerformedPoisonDash(object sender, DashAbility.OnPerformedPoisonEventArgs e)
    {
        _immunityCounter = e.TimePlayerSafeFromPoison;

        ToggleShow(true);
    }

    private void Update()
    {
        if (_isShowing)
        {
            if (GameStateManager.Instance.GetCurrentGameState() == GameStateManager.GameState.Dialogue) return;
            _immunityCounter -= Time.deltaTime;

            UpdateCounterDisplay();

            if (_immunityCounter < 0)
            {
                ToggleShow(false);
            }
        }
    }

    private void ToggleShow(bool state)
    {
        switch (state)
        {
            case true:
                _counterContiner.gameObject.SetActive(true);
                _isShowing = true;
                return;
            case false:
                _counterContiner.gameObject.SetActive(false);
                _isShowing = false;
                return;
        }
    }

    private void UpdateCounterDisplay() => _counterText.text = string.Format("{0:0.00}", _immunityCounter);

}
