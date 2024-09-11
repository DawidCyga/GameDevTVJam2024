using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class HitsCounter : MonoBehaviour
{
    public static HitsCounter Instance { get; private set; }

    [SerializeField] private int _maxHealth;
    [SerializeField] private float _maxInvincibilityTime;

    [Header("Damage Configuration")]
    [SerializeField] private int _babaiDamage;
    [SerializeField] private int _rangedKhukhaDamage;
    [SerializeField] private int _khukhaDamage;
    [SerializeField] private int _rangedFireKhukhaDamage;
    [SerializeField] private int _firekhukhaDamage;
    [SerializeField] private int _poisonousTrail;

    [Header("For debugging only")]
    [SerializeField] private int _currentHealth;
    [SerializeField] private float _timeSinceTurnedInvincible;
    [SerializeField] private bool _isInvincible;
    [SerializeField] private bool _isKilled;

    private Dictionary<Enemy.EnemyType, int> _hitTypeDamageDictionary = new Dictionary<Enemy.EnemyType, int>();

    public event EventHandler OnHealthDecreased;
    public event EventHandler OnHealthRestored;

    private void Awake()
    {
        Instance = this;

        _hitTypeDamageDictionary[Enemy.EnemyType.BabaiHand] = _babaiDamage;
        _hitTypeDamageDictionary[Enemy.EnemyType.RangedKhukha] = _rangedKhukhaDamage;
        _hitTypeDamageDictionary[Enemy.EnemyType.Khukha] = _khukhaDamage;
        _hitTypeDamageDictionary[Enemy.EnemyType.RangedFireKhukha] = _rangedFireKhukhaDamage;
        _hitTypeDamageDictionary[Enemy.EnemyType.FireKhukha] = _firekhukhaDamage;
        _hitTypeDamageDictionary[Enemy.EnemyType.PoisonousTrail] = _poisonousTrail;
    }

    private void Start()
    {
        if (WaveSpawner.Instance is not null)
        {
            WaveSpawner.Instance.OnWaveCleared += WaveSpawner_OnWaveCleared;
        }
        RestoreLife();
    }

    private void Update()
    {
        if (_isInvincible)
        {
            _timeSinceTurnedInvincible += Time.deltaTime;
            if (_timeSinceTurnedInvincible > _maxInvincibilityTime)
            {
                _isInvincible = false;
            }
        }
    }

    private void OnDestroy()
    {
        if (WaveSpawner.Instance is not null)
        {
            WaveSpawner.Instance.OnWaveCleared -= WaveSpawner_OnWaveCleared;
        }
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

    public void Hit(Enemy.EnemyType hitType)
    {
        if (_isInvincible) { return; }
        _currentHealth -= _hitTypeDamageDictionary[hitType];
        OnHealthDecreased?.Invoke(this, EventArgs.Empty);

        Debug.Log("Got hit");

        if (_currentHealth <= 0)
        {
            if (!_isKilled)
            {
                PlayerHitBox.Instance.TakeDamage();
                _isKilled = true;
            }
        }
    }

    public void SetInvincible()
    {
        _isInvincible = true;
        _timeSinceTurnedInvincible = 0;
        Debug.Log("Invincibility set: " + _timeSinceTurnedInvincible + " max: " + _maxInvincibilityTime);
    }

    public int GetMaxHealth() => _maxHealth;
    public void SetMaxHealth(int value) => _maxHealth = value;
    public int GetCurrentHealth() => _currentHealth;
}
