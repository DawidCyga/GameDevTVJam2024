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
        if (collision != null && collision.TryGetComponent(out HitsCounter hitCounter))
        {
            hitCounter.Hit(HitsCounter.HitType.Khukha);
        }
        Destroy(gameObject);
    }
}
