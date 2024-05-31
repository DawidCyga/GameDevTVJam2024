using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WaveSpawnerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _waveSpawnerUIText;
    [SerializeField] private GameObject _waveSpawnerUIContainer;

    [Header("For debugging only")]
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
    }

    private void OnDestroy()
    {
        WaveSpawner.Instance.OnStartedNewWave -= WaveSpawner_OnStartedNewWave;
        WaveSpawner.Instance.OnTotalEnemyCountThisWaveDecreased -= WaveSpawner_OnTotalEnemyCountThisWaveDecreased;
    }

    private void WaveSpawner_OnStartedNewWave(object sender, System.EventArgs e)
    {
        _totalEnemiesThisWave = WaveSpawner.Instance.GetTotalEnemyCountCurrentWave();
        _enemiesLeftThisWave = _totalEnemiesThisWave;
        UpdateWaveSpawnerUI(_enemiesLeftThisWave.ToString(), _totalEnemiesThisWave.ToString());
        UpdateUIVisibility(true);
    }

    private void WaveSpawner_OnTotalEnemyCountThisWaveDecreased(object sender, System.EventArgs e)
    {
        _enemiesLeftThisWave--;
        UpdateWaveSpawnerUI(_enemiesLeftThisWave.ToString(), _totalEnemiesThisWave.ToString());
    }

    private void WaveSpawner_OnWaveCleared(object sender, WaveSpawner.OnWaveClearedEventArgs e)
    {
        UpdateUIVisibility(false);
    }

    private void UpdateWaveSpawnerUI(string firstValue, string secondValue) => _waveSpawnerUIText.text = $"{firstValue} / {secondValue}";

    private void UpdateUIVisibility(bool state) => _waveSpawnerUIContainer.SetActive(state);

}
