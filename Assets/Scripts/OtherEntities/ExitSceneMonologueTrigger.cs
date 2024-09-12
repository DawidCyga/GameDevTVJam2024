using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitSceneMonologueTrigger : MonoBehaviour
{
    public static ExitSceneMonologueTrigger Instance { get; private set; }

    public event EventHandler OnPlayerAttemptsLeavingScene;

    private void Awake()
    {
        Instance = this;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player attempts leaving triggered");
            OnPlayerAttemptsLeavingScene?.Invoke(this, EventArgs.Empty);
        }
    }
}
