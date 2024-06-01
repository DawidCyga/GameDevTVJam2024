using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class HitsCounter : MonoBehaviour
{
    public static HitsCounter Instance { get; private set; }

    public enum HitType
    {
        Babai,
        RangedKhukha,
        Khukha
    }

    [SerializeField] private int _maxHealth;

    [Header("Damage Configuration")]
    [SerializeField] private int _babaiDamage;
    [SerializeField] private int _rangedKhukhaDamage;
    [SerializeField] private int _khukhaDamage;

    [Header("For debugging only")]
    [SerializeField] private int _currentHealth;

    private Dictionary<HitType, int> _hitTypeDamageDictionary = new Dictionary<HitType, int>();

    public event EventHandler OnHealthDecreased;
    public event EventHandler OnHealthRestored;
    
    private void Awake()
    {
        Instance = this;

        _hitTypeDamageDictionary[HitType.Babai] = _babaiDamage;
        _hitTypeDamageDictionary[HitType.RangedKhukha] = _rangedKhukhaDamage;
        _hitTypeDamageDictionary[HitType.Khukha] = _khukhaDamage;
    }

    private void Start()
    {
        WaveSpawner.Instance.OnWaveCleared += WaveSpawner_OnWaveCleared;
        RestoreLife();
    }

    private void OnDestroy()
    {
        WaveSpawner.Instance.OnWaveCleared -= WaveSpawner_OnWaveCleared;
    }

    private void WaveSpawner_OnWaveCleared(object sender, WaveSpawner.OnWaveClearedEventArgs e)
    {
        RestoreLife();
    }
    private void RestoreLife()
    {
        _currentHealth = _maxHealth;
        OnHealthRestored?.Invoke(this, EventArgs.Empty);
    }

    public void Hit(HitType hitType)
    {
        _currentHealth -= _hitTypeDamageDictionary[hitType];
        OnHealthDecreased?.Invoke(this, EventArgs.Empty);

        if (_currentHealth <= 0)
        {
            PlayerHitBox.Instance.TakeDamage();
        }
    }

    public int GetMaxHealth() => _maxHealth;
    public int GetCurrentHealth() => _currentHealth;
}
