using UnityEngine;
using UnityEngine.SceneManagement;

public class RegularTrail : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private float _timeToSelfDestruct;

    [Header("For Debugging Only")]
    [SerializeField] private float _timeSinceAlive;

    private void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex == 3 || SceneManager.GetActiveScene().buildIndex == 4)
        {
            if (GameStateManager.Instance.GetCurrentGameState() == GameStateManager.GameState.Dialogue) return;
        }

        _timeSinceAlive += Time.deltaTime;

        DestroyAfterTime();
    }

    private void DestroyAfterTime()
    {
        if (_timeSinceAlive > _timeToSelfDestruct)
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
                    return;
                }
                else
                {
                    Debug.Log("On trigger enter else");
                    if (takeDamage.CanBeKilledByRegularTrail())
                    {
                        takeDamage.TakeDamage();
                        Debug.Log("Take damage should be called");
                    }
                }
            }
        }
    }
}
