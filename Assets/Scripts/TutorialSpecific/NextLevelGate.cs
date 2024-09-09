using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevelGate : MonoBehaviour
{
    [SerializeField] private float _fadeOutDuration;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ( collision.TryGetComponent(out Player player))
        {
            player.GoToNextScene();
            FadeTransitionHandler.Instance.FadeOut(_fadeOutDuration, LoadNextLevel);
        }
    }
    private void LoadNextLevel() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
}
