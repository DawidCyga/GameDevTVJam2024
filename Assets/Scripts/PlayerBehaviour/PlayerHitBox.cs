using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitBox : MonoBehaviour, ITakeDamage
{
    public static PlayerHitBox Instance { get; private set; }

    public event EventHandler OnPlayerDeath;

    [SerializeField] private float _maxInvincibilityTime;

    [Header("For debugging only")]
    [SerializeField] private bool _isInvincible;
    [SerializeField] private float _timeSinceTurnedInvincible;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
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

    public void SetInvincible()
    {
        _timeSinceTurnedInvincible = 0;
        _isInvincible = true;
    }

    public void TakeDamage()
    {
        if (_isInvincible) { return; }
        //die animations
        //sound effects
        Debug.Log("Player killed");
        //OnPlayerDeath?.Invoke(this, EventArgs.Empty);
    }
}
