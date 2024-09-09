using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonousTrail : MonoBehaviour
{
    [SerializeField] private float _timePlayerSafe;
    [SerializeField] private float _timeToSelfDestruct;

    [Header("For debugging only")]
    [SerializeField] private float _counterPlayerSafe;
    [SerializeField] private bool _canKillPlayer;
    [SerializeField] private float _timeSinceSpawned;

    private void Awake()
    {
        _canKillPlayer = false;
        _counterPlayerSafe = _timePlayerSafe;
        _timeSinceSpawned = 0;
    }

    private void Update()
    {
        if (_counterPlayerSafe > 0 && !_canKillPlayer)
        {
            _counterPlayerSafe -= Time.deltaTime;
        }
        else
        {
            _canKillPlayer = true;
        }

        _timeSinceSpawned += Time.deltaTime;

        TrySelfDestruct();
    }

    private void TrySelfDestruct()
    {
        if (_timeSinceSpawned > _timeToSelfDestruct)
        {
            Destroy(gameObject);
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
                    if (!_canKillPlayer) { return; }
                    if (HitsCounter.Instance is not null)
                    { 
                        HitsCounter.Instance.Hit(Enemy.EnemyType.PoisonousTrail);
                    }
                }
                else
                {
                    takeDamage.TakeDamage();
                }
            }
        }
    }
}

