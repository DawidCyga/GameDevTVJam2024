using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WaveSpawnerUI : MonoBehaviour
{
    [SerializeField] private GameObject _waveSpawnerUIContainer;
    [SerializeField] private TextMeshProUGUI _waveUIText;
    [SerializeField] private TextMeshProUGUI _enemiesUIText;

    [Header("For debugging only")]
    [SerializeField] private int _totalWaveCount;
    [SerializeField] private int _currentWave;
    [SerializeField] private int _enemiesLeftThisWave;
    [SerializeField] private int _totalEnemiesThisWave;


    private void Awake()
    {
        UpdateUIVisibility(false);
    }

    private void Start()
    {
        WaveSpawner.Instance.OnStartedNewWave += WaveSpawner_OnStartedNewWave;
        WaveSpawner.Instance.OnTotalEnemyCountThisWaveDecreased += WaveSpawner_OnTotalEnemyCountThisWaveDecreased;
        WaveSpawner.Instance.OnWaveCleared += WaveSpawner_OnWaveCleared;

        _totalWaveCount = WaveSpawner.Instance.GetTotalWaveCount();
    }

    private void OnDestroy()
    {
        WaveSpawner.Instance.OnStartedNewWave -= WaveSpawner_OnStartedNewWave;
        WaveSpawner.Instance.OnTotalEnemyCountThisWaveDecreased -= WaveSpawner_OnTotalEnemyCountThisWaveDecreased;
        WaveSpawner.Instance.OnWaveCleared -= WaveSpawner_OnWaveCleared;
    }

    private void WaveSpawner_OnStartedNewWave(object sender, System.EventArgs e)
    {
        _currentWave = WaveSpawner.Instance.GetCurrentWaveIndex() + 1;
        _totalEnemiesThisWave = WaveSpawner.Instance.GetTotalEnemyCountCurrentWave();
        _enemiesLeftThisWave = _totalEnemiesThisWave;
        UpdateWaveUI(_currentWave.ToString());
        UpdateEnemiesUI(_enemiesLeftThisWave.ToString(), _totalEnemiesThisWave.ToString());
        UpdateUIVisibility(true);
        Debug.Log("UI on started new wave");
    }

    private void WaveSpawner_OnTotalEnemyCountThisWaveDecreased(object sender, System.EventArgs e)
    {
        _enemiesLeftThisWave--;
        UpdateEnemiesUI(_enemiesLeftThisWave.ToString(), _totalEnemiesThisWave.ToString());
        Debug.Log("On total enemy count decreased");
    }

    private void WaveSpawner_OnWaveCleared(object sender, WaveSpawner.OnWaveClearedEventArgs e)
    {
        UpdateUIVisibility(false);
    }
    private void UpdateWaveUI(string currentWave) => _waveUIText.text = $"{currentWave} / {_totalWaveCount}";
    private void UpdateEnemiesUI(string firstValue, string secondValue) => _enemiesUIText.text = $"{firstValue} / {secondValue}";
    private void UpdateUIVisibility(bool state) => _waveSpawnerUIContainer.SetActive(state);
}
