using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitBox : MonoBehaviour, ITakeDamage
{
    public static PlayerHitBox Instance { get; private set; }

    public event EventHandler OnPlayerDeath;

    [SerializeField] private float _maxInvincibilityTime;

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


    public void TakeDamage()
    {
        //die animations
        //sound effects
       // Debug.Log("Player killed");
        OnPlayerDeath?.Invoke(this, EventArgs.Empty);
    }
}
