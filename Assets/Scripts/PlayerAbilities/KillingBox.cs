using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class KillingBox : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private float _totalLifetime;

    [Header("Stun Configuration")]
    [SerializeField] private float _stunDuration;
    [SerializeField] private float _stunExplosionArea;
    [SerializeField] private LayerMask _whatCanBeStunned;


    [Header("For debugging only")]
    
    [SerializeField] private float _lifetimeLeft;
    [SerializeField] private bool _hasFallen;
    [SerializeField] private bool _canExplode;

    private Rigidbody2D _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _hasFallen = false;
        _canExplode = false;
        _lifetimeLeft = _totalLifetime;
    }

    public void Drop(float dropForce)
    {
        _rigidbody.AddForce(Vector2.down * dropForce, ForceMode2D.Impulse);
    }

    private void Update()
    {
       

        if (_hasFallen)
        {
            _canExplode = true;
            SelfDestructAfterLifetime();
        }

        if (_canExplode)
        {
            Explode();
            //_canExplode = false;
        }
    }

    private void SelfDestructAfterLifetime()
    {
        _lifetimeLeft -= Time.deltaTime;

        if (_lifetimeLeft < 0)
        {
            //TODO: Start self destruct sequence
            Destroy(gameObject);
        }
    }

    private void Explode()
    {
        // animation plays
        Collider2D[] HitColliders = Physics2D.OverlapCircleAll(transform.position, _stunExplosionArea, _whatCanBeStunned);

        foreach (Collider2D collider in HitColliders)
        {
            // Need enemies first in order to call method on them for receiving stun.
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        _hasFallen = true;

        if (collision.gameObject.GetComponent<PlayerHitBox>() is not null)
        {
            return;
        }

        if (collision.gameObject.TryGetComponent(out ITakeDamage takeDamage))
        {
            takeDamage.TakeDamage();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _stunExplosionArea);
    }
}
