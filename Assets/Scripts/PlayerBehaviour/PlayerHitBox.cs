using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitBox : MonoBehaviour, ITakeDamage
{
    public static PlayerHitBox Instance { get; private set; }

    public event EventHandler OnPlayerDying;
    public event EventHandler OnPlayerDeath;

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
        OnPlayerDying?.Invoke(this, EventArgs.Empty);
        Player.Instance.enabled = false;
        Debug.Log("Player hit box: I take damage");
    }

    public void Die()
    {
        OnPlayerDeath?.Invoke(this, EventArgs.Empty);
        Debug.Log("Player hit box, I die - triggered from animation");
    }
}
