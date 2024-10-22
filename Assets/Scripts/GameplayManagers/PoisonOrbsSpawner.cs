using System;
using System.Collections.Generic;
using UnityEngine;

public class PoisonOrbsSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _orbTransform;

    [Header("Spawning Configuration")]
    [SerializeField] private int _maxNumberSpawnedAllowed;
    [SerializeField] private float _minTimeBetweenSpawns;
    [SerializeField] private float _maxTimeBetweenSpawns;
    [SerializeField] private int _numberOrbsSpawnedAtStart;

    [Header("SpawnPoints")]
    [SerializeField] private Transform[] _spawnPoints;
    private Dictionary<Transform, bool> _spawnPointStateDictionary = new Dictionary<Transform, bool>();

    [Header("For Debugging Only")]
    [SerializeField] private int _currentlySpawnedNumber;
    [SerializeField] private float _currentTimeBetweenSpawns;
    [SerializeField] private float _timeSinceLastSpawnedOrb;
    [SerializeField] private bool _isRandomTimeBetweenSpawnsSet;

    private void Awake()
    {
        InitializeMaxNumberSpawnedAllowed();
        InitializeSpawnPointsDictionary();
        SpawnFirstOrbsAtLevelStart();
    }

    private void InitializeMaxNumberSpawnedAllowed() => _maxNumberSpawnedAllowed = DifficultyManager.Instance.GetPoisonousSourcesAmount();

    private void InitializeSpawnPointsDictionary()
    {
        foreach (Transform transform in _spawnPoints)
        {
            _spawnPointStateDictionary[transform] = false;
        }
    }

    private void SpawnFirstOrbsAtLevelStart()
    {
        for (int i = 0; i < _numberOrbsSpawnedAtStart; i++)
        {
            TrySpawnOrb();
        }
    }

    private void Update()
    {
        if (GameStateManager.Instance.GetCurrentGameState() != GameStateManager.GameState.Playing) { return; }

        _timeSinceLastSpawnedOrb += Time.deltaTime;
        ProcessSpawningOrbs();
    }

    private void ProcessSpawningOrbs()
    {
        if (!HasReachedMaxOrbsAtTime())
        {
            TrySetRandomTimeBetweenSpawns();
            if (IsTimeToSpawnNextOrb())
            {
                TrySpawnOrb();
            }
        }
    }

    private void TrySetRandomTimeBetweenSpawns()
    {
        if (!_isRandomTimeBetweenSpawnsSet)
        {
            _currentTimeBetweenSpawns = UnityEngine.Random.Range(_minTimeBetweenSpawns, _maxTimeBetweenSpawns);
            _isRandomTimeBetweenSpawnsSet = true;
        }
    }

    private void TrySpawnOrb()
    {
        List<Transform> availableSpawnPoints = new List<Transform>();

        foreach (var spawnPoint in _spawnPoints)
        {
            if (!_spawnPointStateDictionary[spawnPoint])
            {
                availableSpawnPoints.Add(spawnPoint);
            }
        }

        int randomIndex = UnityEngine.Random.Range(0, availableSpawnPoints.Count);
        Transform selectedSpawnPoint = availableSpawnPoints[randomIndex];

        SpawnOrbAtSpawnPoint(selectedSpawnPoint);
    }

    private void SpawnOrbAtSpawnPoint(Transform spawnPoint)
    {
        Transform orbInstance = Instantiate(_orbTransform, spawnPoint.position, Quaternion.identity, spawnPoint);

        _spawnPointStateDictionary[spawnPoint] = true;
        _currentlySpawnedNumber++;
        _timeSinceLastSpawnedOrb = 0;
        _isRandomTimeBetweenSpawnsSet = false;

        orbInstance.GetComponent<PoisonOrb>().OnCollected += (sender, args) => FreeSpawnPoint(spawnPoint);
    }

    private void FreeSpawnPoint(Transform spawnPoint)
    {
        if (_spawnPointStateDictionary.ContainsKey(spawnPoint))
        {
            _spawnPointStateDictionary[spawnPoint] = false;
            _currentlySpawnedNumber--;
        }
    }

    private bool HasReachedMaxOrbsAtTime() => _currentlySpawnedNumber >= _maxNumberSpawnedAllowed;
    private bool IsTimeToSpawnNextOrb() => _timeSinceLastSpawnedOrb >= _currentTimeBetweenSpawns;
}
