using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] private float _timeTillSelfDestruct;

    private Rigidbody2D _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    public void Launch(Vector3 direction, float fireForce)
    {
        _rigidbody.AddForce(direction * fireForce, ForceMode2D.Impulse);
    }

    private void Update()
    {
        _timeTillSelfDestruct -= Time.deltaTime;

        if (_timeTillSelfDestruct < 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out PlayerHitBox playerHitBox))
        {
            playerHitBox.TakeDamage();
        }
        Destroy(gameObject);
    }
}
