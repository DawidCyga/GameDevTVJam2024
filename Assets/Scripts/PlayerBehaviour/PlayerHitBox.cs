using System;
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
    }

    public void Die() => OnPlayerDeath?.Invoke(this, EventArgs.Empty);
    public bool CanBeKilledByRegularTrail() => false;
}
