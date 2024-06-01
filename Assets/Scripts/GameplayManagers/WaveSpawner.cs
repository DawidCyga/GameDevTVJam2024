using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Wave
{
    [SerializeField] private string _waveName;

    [SerializeField] private List<SpawnPoint> _spawnPointList;
    [SerializeField] private float _timeTillStartWave;

    public List<SpawnPoint> GetSpawnPointList() => _spawnPointList;
    public float GetTimeTillStartWave() => _timeTillStartWave;
}


[Serializable]
public class SpawnPoint
{
    [SerializeField] private string _pointName;

    [SerializeField] private Transform _positionTransform;

    [SerializeField] private List<EnemyToSpawn> _enemyToSpawnList;

    public Vector3 GetPosition() => _positionTransform.position;
    public int GetEnemiesCount() => _enemyToSpawnList.Count;
    public EnemyToSpawn GetEnemyToSpawnAtIndex(int index) => _enemyToSpawnList[index];
}

[Serializable]
public class EnemyToSpawn
{
    [SerializeField] private string _enemyName;

    [SerializeField] private Transform _enemyPrefab;
    [SerializeField] private float _timeToSpawn;

    public Transform GetEnemyPrefab() => _enemyPrefab;
    public float GetTimeToSpawn() => _timeToSpawn;
}

public class WaveSpawner : MonoBehaviour
{
    public static WaveSpawner Instance { get; private set; }

    public enum WaveState
    {
        Dialogue,
        Break,
        Spawning,
        Fighting,
    }

    public WaveState _currentWaveState;


    [SerializeField] private Wave[] _waves;

    [SerializeField] private Transform _spawnedEnemiesParentTransform;


    [Header("For debugging only")]

    [SerializeField] private int _currentWaveIndex;

    [SerializeField] private int _totalEnemiesSpawnedCurrentWave;

    [SerializeField] private float _timeTillEndBreak;

    [SerializeField] private int _totalEnemyCountCurrentWave;

    [SerializeField] private bool _waveIndexIncreasedThisWave;

    private List<Coroutine> _activeSpawningRoutines = new List<Coroutine>();
    private int _completedSpawningRoutinesNumber;

    public event EventHandler OnStartedNewWave;
    public event EventHandler OnTotalEnemyCountThisWaveDecreased;
    public event EventHandler<OnWaveClearedEventArgs> OnWaveCleared;
    public class OnWaveClearedEventArgs { public int CurrentWaveIndex { get; set; } }

    private void Awake()
    {
        Instance = this;

        _currentWaveIndex = 0;
        ResetCurrentWaveIndex();
        _totalEnemiesSpawnedCurrentWave = 0;
        _timeTillEndBreak = _waves[_currentWaveIndex].GetTimeTillStartWave();
        _currentWaveState = WaveState.Dialogue;
        _completedSpawningRoutinesNumber = 0;
    }

    private void ResetCurrentWaveIndex() => _currentWaveIndex = 0;
    private void ResetTimeTillEndBreak() => _timeTillEndBreak = _waves[_currentWaveIndex + 1].GetTimeTillStartWave();

    private void Update()
    {
        switch (_currentWaveState)
        {
            case WaveState.Dialogue:
                break;
            case WaveState.Break:
                if (_currentWaveIndex == _waves.Length)
                {
                    return;
                }

                OnStartedNewWave?.Invoke(this, EventArgs.Empty);

                CountdownBeforeEndBreak();
                _waveIndexIncreasedThisWave = false;

                if (_timeTillEndBreak <= 0)
                {
                    if (_currentWaveIndex < _waves.Length - 1)
                    {
                        ResetTimeTillEndBreak();
                    }
                    _currentWaveState = WaveState.Spawning;
                }
                break;
            case WaveState.Spawning:
                SpawnEnemies();
                break;
            case WaveState.Fighting:
                WaitForEndOfFight();
                break;
        }

    }

    private void CountdownBeforeEndBreak() => _timeTillEndBreak -= Time.deltaTime;

    private void SpawnEnemies()
    {
        Wave currentWave = _waves[_currentWaveIndex];
        List<SpawnPoint> currentSpawnPointList = currentWave.GetSpawnPointList();

        if (_activeSpawningRoutines.Count == 0)
        {
            foreach (SpawnPoint spawnPoint in currentSpawnPointList)
            {
                Coroutine spawnRoutine = StartCoroutine(SpawnAtPointRoutine(spawnPoint));
                _activeSpawningRoutines.Add(spawnRoutine);
            }
        }

        if (_completedSpawningRoutinesNumber == currentSpawnPointList.Count)
        {
            Debug.Log("Changed to fighting");
            ChangeStateToFighting();
        }

    }

    private IEnumerator SpawnAtPointRoutine(SpawnPoint spawnPoint)
    {
        int totalEnemiesSpawnedAtSpawnPoint = 0;
        int enemiesToSpawnCount = spawnPoint.GetEnemiesCount();

        Vector3 spawnPosition = spawnPoint.GetPosition();

        while (totalEnemiesSpawnedAtSpawnPoint < enemiesToSpawnCount)
        {
            for (int i = 0; i < enemiesToSpawnCount; i++)
            {
                EnemyToSpawn enemyToSpawn = spawnPoint.GetEnemyToSpawnAtIndex(i);
                Transform enemyPrefab = enemyToSpawn.GetEnemyPrefab();
                float timeToSpawn = enemyToSpawn.GetTimeToSpawn();

                yield return new WaitForSeconds(timeToSpawn);

                Transform enemyInstance = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity, _spawnedEnemiesParentTransform);
                totalEnemiesSpawnedAtSpawnPoint++;
                _totalEnemiesSpawnedCurrentWave++;
                yield return null;
            }
        }
        _completedSpawningRoutinesNumber++;

    }

    private void ChangeStateToFighting()
    {
        foreach (Coroutine coroutine in _activeSpawningRoutines)
        {
            StopCoroutine(coroutine);
        }
        _activeSpawningRoutines.Clear();
        _completedSpawningRoutinesNumber = 0;
        _currentWaveState = WaveState.Fighting;
    }

    private void WaitForEndOfFight()
    {
        if (_totalEnemiesSpawnedCurrentWave > 0) return;

        if (!_waveIndexIncreasedThisWave)
        {
            _currentWaveIndex++;
            _waveIndexIncreasedThisWave = true;
        }

        if (_currentWaveIndex <= _waves.Length)
        {
            Debug.Log("On wave cleared");
            OnWaveCleared?.Invoke(this, new OnWaveClearedEventArgs { CurrentWaveIndex = _currentWaveIndex });
            _currentWaveState = WaveState.Dialogue;
        }
        else
        {
            Debug.Log("Something went wrong");

        }
    }

    public void StartBreakBeforeNextWave() => _currentWaveState = WaveState.Break;

    public void DecreaseTotalEnemiesSpawnedCurrentWave()
    {
        _totalEnemiesSpawnedCurrentWave--;
        OnTotalEnemyCountThisWaveDecreased?.Invoke(this, EventArgs.Empty);
    }

    public int GetTotalWaveCount() => _waves.Length;

    public int GetCurrentWaveIndex() => _currentWaveIndex;

    public int GetTotalEnemyCountCurrentWave()
    {
        int totalEnemyCountCurrentWave = 0;
        SpawnPoint[] spawnPoints = _waves[_currentWaveIndex].GetSpawnPointList().ToArray();

        foreach (SpawnPoint spawnPoint in spawnPoints)
        {
            totalEnemyCountCurrentWave += spawnPoint.GetEnemiesCount();
        }
        return totalEnemyCountCurrentWave;
    }

}
