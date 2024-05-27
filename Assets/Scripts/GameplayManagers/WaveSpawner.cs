using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WaveSet
{
    [SerializeField] private WaveToSpawn[] _waves;

    [SerializeField] private Transform[] _spawnPoints;

    [SerializeField] private float _breakBetweenWaves;

    public WaveToSpawn GetWave(int index) => _waves[index];
    public int GetTotalWavesNumber() => _waves.Length;
    public Transform[] GetSpawnPoints() => _spawnPoints;
    public float GetBreakBetweenWaves() => _breakBetweenWaves;

}

[Serializable]
public class WaveToSpawn
{
    [field: SerializeField] public List<EnemyType> EnemyTypeList { get; private set; } = new List<EnemyType>();

    [field: SerializeField] public float TimeBetweenEnemySpawns;

    public int GetTotalEnemyCount()
    {
        int totalEnemyCount = 0;
        foreach (EnemyType enemy in EnemyTypeList) { totalEnemyCount += enemy.Count; }
        return totalEnemyCount;
    }
}

[Serializable]
public class EnemyType
{
    [field: SerializeField] public Transform EnemyPrefab { get; private set; }
    [field: SerializeField] public int Count { get; private set; }
}

public class WaveSpawner : MonoBehaviour
{

    public static WaveSpawner Instance { get; private set; }

    public event EventHandler OnWaveCleared;
    public event EventHandler OnAllWavesCleared;
    public event EventHandler OnAliveSpawnedEnemiesCountChanged;
    public event EventHandler OnNewWaveSet;

    public enum WaveState
    {
        Break,
        Spawning,
        Fighting,
        Finish
    }

    [SerializeField] private WaveState _currentWaveState;

    [SerializeField] private WaveSet[] _waveSets;

    [Header("For debugging only")]
    [SerializeField] private int _currentWaveSetIndex;
    [SerializeField] private int _currentWaveIndex;

    [SerializeField] private float _breakCountdown;

    [SerializeField] private int _aliveSpawnedEnemies;

    [SerializeField] private bool _HasFinishedFighting = false;

    private Coroutine _activeSpawnRoutine;

    private void Awake()
    {
        Instance = this;

        _currentWaveSetIndex = 0;
        ResetCurrentWaveIndex();
        _aliveSpawnedEnemies = 0;
        ResetBreakCountdown();
        _currentWaveState = WaveState.Break;
    }

    private void ResetCurrentWaveIndex() => _currentWaveIndex = 0;
    private void ResetBreakCountdown() => _breakCountdown = _waveSets[_currentWaveSetIndex].GetBreakBetweenWaves();

    private void Update()
    {
        switch (_currentWaveState)
        {
            case WaveState.Break:
                CountdownBreakTime();
                if (_breakCountdown <= 0)
                {
                    _breakCountdown = _waveSets[_currentWaveSetIndex].GetBreakBetweenWaves();
                    _currentWaveState = WaveState.Spawning;
                }
                break;
            case WaveState.Spawning:
                SpawnEnemies();
                break;
            case WaveState.Fighting:
                WaitForEndOfFight();
                break;
            case WaveState.Finish:
                _HasFinishedFighting = true;
                break;
        }
    }

    private void CountdownBreakTime() => _breakCountdown -= Time.deltaTime;

    private void SpawnEnemies()
    {
        WaveToSpawn currentWave = _waveSets[_currentWaveSetIndex].GetWave(_currentWaveIndex);
        if (_activeSpawnRoutine == null)
        {
            _activeSpawnRoutine = StartCoroutine(SpawningRoutine(currentWave));
        }
        OnAliveSpawnedEnemiesCountChanged?.Invoke(this, EventArgs.Empty);
    }

    private IEnumerator SpawningRoutine(WaveToSpawn currentWave)
    {
        int totalNumberOfEnemiesSpawnedThisWave = 0;

        while (totalNumberOfEnemiesSpawnedThisWave < currentWave.GetTotalEnemyCount())
        {
            foreach (EnemyType enemyType in currentWave.EnemyTypeList)
            {
                for (int i = 0; i < enemyType.Count; i++)
                {
                    Transform enemyPrefab = Instantiate(enemyType.EnemyPrefab);
                    enemyPrefab.position = _waveSets[_currentWaveSetIndex].GetSpawnPoints()[UnityEngine.Random.Range(0, _waveSets[_currentWaveSetIndex].GetSpawnPoints().Length)].position;

                    totalNumberOfEnemiesSpawnedThisWave++;
                    _aliveSpawnedEnemies++;
                    yield return new WaitForSeconds(currentWave.TimeBetweenEnemySpawns);
                }
            }
        }
        _activeSpawnRoutine = null;
        _currentWaveState = WaveState.Fighting;

    }

    private void WaitForEndOfFight()
    {
        if (_aliveSpawnedEnemies > 0) return;

        _currentWaveIndex++;

        if (_currentWaveIndex < _waveSets[_currentWaveSetIndex].GetTotalWavesNumber())
        {
            OnWaveCleared?.Invoke(this, EventArgs.Empty);
            _currentWaveState = WaveState.Break;

        }
        else
        {
            _currentWaveIndex = _waveSets[_currentWaveSetIndex].GetTotalWavesNumber() - 1;
            _currentWaveState = WaveState.Finish;
            OnAllWavesCleared?.Invoke(this, EventArgs.Empty);
        }
    }

    public void DecreaseAliveSpawnedEnemies()
    {
        _aliveSpawnedEnemies--;
        OnAliveSpawnedEnemiesCountChanged?.Invoke(this, EventArgs.Empty);
    }

    public void SetNextWaveSet()
    {
        _currentWaveSetIndex++;
        if (_currentWaveSetIndex < _waveSets.Length)
        {
            ResetCurrentWaveIndex();
            ResetBreakCountdown();
            _HasFinishedFighting = false;
            _currentWaveState = WaveState.Break;
            OnNewWaveSet?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            // Trigger win condition
            Debug.Log("It's all done, you cleared all the waves");
        }
    }

    public int GetAliveSpawnedEnemiesCount() => _aliveSpawnedEnemies;
    public int GetTotalEnemyCountThisWave() => _waveSets[_currentWaveSetIndex].GetWave(_currentWaveIndex).GetTotalEnemyCount();
    public int GetClearedWavesNumber() => _currentWaveIndex;
    public int GetTotalWaveNumberThisSet() => _waveSets[_currentWaveSetIndex].GetTotalWavesNumber();
    public float GetTimeTillNextWave() => _breakCountdown;
    public bool HasFinishedFighting() => _HasFinishedFighting;

}
