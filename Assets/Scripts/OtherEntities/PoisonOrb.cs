using System;
using UnityEngine;

public class PoisonOrb : MonoBehaviour
{
    public event EventHandler OnCollected;
    public static event EventHandler OnAnyOrbCollected;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Destroy(gameObject);
            OnCollected?.Invoke(this, EventArgs.Empty);
            OnAnyOrbCollected?.Invoke(this, EventArgs.Empty);
        }
    }
}
