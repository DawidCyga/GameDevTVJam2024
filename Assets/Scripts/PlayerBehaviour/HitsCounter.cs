using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class HitsCounter : MonoBehaviour
{
    public enum HitType
    {
        Babai,
        Khukha
    }

    public static HitsCounter Instance { get; private set; }

    [SerializeField] private int _maximumBabaiHits;
    [SerializeField] private int _maximumKhukhaHits;

    [Header("For debugging only")]
    [SerializeField] private int _currentBabaiHits;
    [SerializeField] private int _currentKhukhaHits;

    public event EventHandler<OnBabaiHitsChangedEventArgs> OnBabaiHitsChanged;
    public class OnBabaiHitsChangedEventArgs { public int CurrentBabaiHits { get; set; } }

    public event EventHandler<OnKhukhaHitsChangedEventArgs> OnKhukhaHitsChanged;
    public class OnKhukhaHitsChangedEventArgs { public int CurrentKhukhaHits { get; set; } }

    private void Awake()
    {
        Instance = this;

        _currentBabaiHits = 0;
        _currentKhukhaHits = 0;
    }

    private void Start()
    {
        WaveSpawner.Instance.OnWaveCleared += WaveSpawner_OnWaveCleared;
    }

    private void OnDestroy()
    {
        WaveSpawner.Instance.OnWaveCleared -= WaveSpawner_OnWaveCleared;
    }

    private void WaveSpawner_OnWaveCleared(object sender, WaveSpawner.OnWaveClearedEventArgs e)
    {
        ResetHits();
    }
    private void ResetHits()
    {
        _currentBabaiHits = 0;
        _currentKhukhaHits = 0;
        OnBabaiHitsChanged?.Invoke(this, new OnBabaiHitsChangedEventArgs { CurrentBabaiHits = _currentBabaiHits });
        OnKhukhaHitsChanged?.Invoke(this, new OnKhukhaHitsChangedEventArgs { CurrentKhukhaHits = _currentKhukhaHits });
    }

    public void Hit(HitType hitType)
    {
        if (hitType == HitType.Babai)
        {
            _currentBabaiHits++;
            OnBabaiHitsChanged?.Invoke(this, new OnBabaiHitsChangedEventArgs { CurrentBabaiHits = _currentBabaiHits });
        }
        else if (hitType == HitType.Khukha)
        {
            _currentKhukhaHits++;
            OnKhukhaHitsChanged?.Invoke(this, new OnKhukhaHitsChangedEventArgs { CurrentKhukhaHits = _currentKhukhaHits });
        }

        if (_currentBabaiHits > _maximumBabaiHits || _currentKhukhaHits > _maximumKhukhaHits)
        {
            Debug.Log("Killed from counter");
            PlayerHitBox.Instance.TakeDamage();
        }       
    }
}
