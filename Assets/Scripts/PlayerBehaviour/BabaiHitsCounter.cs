using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class BabaiHitsCounter : MonoBehaviour
{
    public static BabaiHitsCounter Instance { get; private set; }

    [SerializeField] private int _maximumBabaiHits;

    [Header("For debugging only")]
    [SerializeField] private int _currentBabaiHits;

    public event EventHandler<OnBabaiHitsChangedEventArgs> OnBabaiHitsChanged;
    public class OnBabaiHitsChangedEventArgs { public int CurrentBabaiHits { get; set; } }

    private void Awake()
    {
        Instance = this;

        _currentBabaiHits = 0;
    }

    private void Start()
    {
        WaveSpawner.Instance.OnWaveCleared += WaveSpawner_OnWaveCleared;
    }

    private void WaveSpawner_OnWaveCleared(object sender, WaveSpawner.OnWaveClearedEventArgs e)
    {
        ResetBabaiHits();
    }
    private void ResetBabaiHits()
    {
        _currentBabaiHits = 0;
        OnBabaiHitsChanged?.Invoke(this, new OnBabaiHitsChangedEventArgs { CurrentBabaiHits = _currentBabaiHits });
    }

    public void Hit()
    {
        _currentBabaiHits++;
        OnBabaiHitsChanged?.Invoke(this, new OnBabaiHitsChangedEventArgs { CurrentBabaiHits = _currentBabaiHits });
        if (_currentBabaiHits > _maximumBabaiHits)
        {
            Debug.Log("Player killed by Babai hands");
            //Start death sequence
        }
    }
}
