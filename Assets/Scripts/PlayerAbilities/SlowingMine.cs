using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class SlowingMine : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private float _totalLifetime;

    [Header("SlowDown Configuration")]
    [SerializeField] private float _slowDownDuration;
    [SerializeField] private float _slowDownMultiplier;
    [SerializeField] private float _explosionArea;
    [SerializeField] private LayerMask _whatCanBeStunned;


    [Header("For debugging only")]
    
    [SerializeField] private float _lifetimeLeft;
    [SerializeField] private bool _hasFallen;
    [SerializeField] private bool _canExplode;
    [SerializeField] private bool _hasExploded;

    private Rigidbody2D _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _hasFallen = false;
        _canExplode = false;
        _lifetimeLeft = _totalLifetime;
    }

    public void Drop(float dropForce) => _rigidbody.AddForce(Vector2.down * dropForce, ForceMode2D.Impulse);

    private void Update()
    {
        if (_hasFallen && !_hasExploded)
        {
            _canExplode = true;
            SelfDestructAfterLifetime();
        }

        if (_canExplode)
        {
            Explode();
        }
    }

    private void SelfDestructAfterLifetime()
    {
        _lifetimeLeft -= Time.deltaTime;

        if (_lifetimeLeft < 0)
        {
            Destroy(gameObject);
        }
    }

    private void Explode()
    {
        Collider2D[] HitColliders = Physics2D.OverlapCircleAll(transform.position, _explosionArea, _whatCanBeStunned);

        foreach (Collider2D collider in HitColliders)
        {
            if (collider.TryGetComponent(out ICanBeSlowedDown canBeSlowedDown))
            {
                canBeSlowedDown.TrySlowDown(_slowDownDuration, _slowDownMultiplier);
            }
        }
        _canExplode = false;
        _hasExploded = true;
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
        Gizmos.DrawWireSphere(transform.position, _explosionArea);
    }
}
