using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonousTrail : MonoBehaviour
{
    [SerializeField] private float _timePlayerSafe;

    [Header("For debugging only")]
    [SerializeField] private float _counterPlayerSafe;
    [SerializeField] private bool _isUnableToKillPlayer;

    private void Awake()
    {
        _isUnableToKillPlayer = true;
        _counterPlayerSafe = _timePlayerSafe;
    }

    private void Update()
    {
        if (_counterPlayerSafe > 0 && _isUnableToKillPlayer)
        {
            _counterPlayerSafe -= Time.deltaTime;
        }
        else
        {
            _isUnableToKillPlayer = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null)
        {
            if (collision.TryGetComponent(out ITakeDamage takeDamage))
            {
                if (collision.TryGetComponent(out PlayerHitBox hitbox))
                {
                    if (_isUnableToKillPlayer) { return; }
                }
                takeDamage.TakeDamage();
            }
        }
    }
}

