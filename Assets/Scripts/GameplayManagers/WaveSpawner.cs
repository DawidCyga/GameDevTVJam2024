using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Wave
{
    [SerializeField] private List<SpawnPoint> _spawnPointList;
    [SerializeField] private float _timeTillStartWave;

    public List<SpawnPoint> GetSpawnPointList() => _spawnPointList;
    public float GetTimeTillStartWave() => _timeTillStartWave;
}


[Serializable]
public class SpawnPoint
{
    [SerializeField] private Transform _positionTransform;

    [SerializeField] private List<EnemyToSpawn> _enemyToSpawnList;

    public Vector3 GetPosition() => _positionTransform.position;
    public int GetEnemiesCount() => _enemyToSpawnList.Count;
    public EnemyToSpawn GetEnemyToSpawnAtIndex(int index) => _enemyToSpawnList[index];
}

[Serializable]
public class EnemyToSpawn
{
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
        Start,
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

    private List<Coroutine> _activeSpawningRoutines = new List<Coroutine>();


    public event EventHandler<OnWaveClearedEventArgs> OnWaveCleared;
    public class OnWaveClearedEventArgs { public int CurrentWaveIndex { get; set; } }

    private void Awake()
    {
        Instance = this;

        _currentWaveIndex = 0;
        ResetCurrentWaveIndex();
        _totalEnemiesSpawnedCurrentWave = 0;
       // ResetTimeTillEndBreak();
        _currentWaveState = WaveState.Start;
    }

    private void ResetCurrentWaveIndex() => _currentWaveIndex = 0;
    private void ResetTimeTillEndBreak() => _timeTillEndBreak = _waves[_currentWaveIndex].GetTimeTillStartWave();

    private void Update()
    {
        switch (_currentWaveState)
        {
            case WaveState.Start:
                Debug.Log("Just started level");
                break;
            case WaveState.Break:
                CountdownBeforeEndBreak();
                if (_timeTillEndBreak <= 0)
                {
                    ResetTimeTillEndBreak();
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

        ResetSpawnAtPointRoutines();

        if (_activeSpawningRoutines.Count == 0)
        {
            foreach (SpawnPoint spawnPoint in currentWave.GetSpawnPointList())
            {
                Coroutine spawnRoutine = StartCoroutine(SpawnAtPointRoutine(spawnPoint));
                _activeSpawningRoutines.Add(spawnRoutine);
            }
        }

        //TO CHECK
        //OnAliveSpawnedEnemiesCountChanged?.Invoke(this, EventArgs.Empty);
    }

    private void ResetSpawnAtPointRoutines()
    {
        foreach (Coroutine coroutine in _activeSpawningRoutines)
        {
            StopCoroutine(coroutine);
        }
        _activeSpawningRoutines.Clear();
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

        _currentWaveState = WaveState.Fighting;

    }

    private void WaitForEndOfFight()
    {
        if (_totalEnemiesSpawnedCurrentWave > 0) return;

        _currentWaveIndex++;

        if (_currentWaveIndex < _waves.Length)
        {
            OnWaveCleared?.Invoke(this, new OnWaveClearedEventArgs { CurrentWaveIndex = _currentWaveIndex });
        }
        else
        {
            // _currentWaveIndex = _waves.Length - 1;
            Debug.Log("Something went wrong. Current Wave index is is of waves length");
        }
    }

    public void StartBreakBeforeNextWave() => _currentWaveState = WaveState.Break;
}
