using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class Enemy : MonoBehaviour, ITakeDamage
{

    [Header("General Configuration")]
    [SerializeField] protected float _moveSpeed;
    [SerializeField] protected float _attackRange;
    [SerializeField] protected float _sightLength;

    [Header("Layer Masks")]
    [SerializeField] protected LayerMask _whatIsObstacle;
    [SerializeField] protected LayerMask _whatIsPlayer;

    protected Transform _target;

    [Header("For debugging only")]
    [SerializeField] protected bool _isInAttackRange;
    [SerializeField] private bool _isDead;

    protected virtual void Start()
    {
        _target = Player.Instance.transform;
    }

    public virtual void TakeDamage()
    {
        //TRIGGER DEATH ANIMATION
        if (!_isDead)
        {
            WaveSpawner.Instance.DecreaseTotalEnemiesSpawnedCurrentWave();
            Destroy(gameObject);
            _isDead = true;
        }
    }


    protected virtual bool CanSeePlayer()
    {
        Vector3 direction = (_target.position - transform.position).normalized;
        float distanceToTarget = Vector3.Distance(transform.position, _target.position);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distanceToTarget, _whatIsObstacle);

        if (hit.collider is null)
        {
            RaycastHit2D hitPlayer = Physics2D.Raycast(transform.position, direction, _sightLength, _whatIsPlayer);
            if (hitPlayer.collider is not null)
            {
                return true;
            }
        }
        return false;
    }

    protected virtual bool CanSeeEntity(Transform entity)
    {
        Vector3 direction = (entity.position - transform.position).normalized;
        float distanceToTarget = Vector3.Distance(transform.position, _target.position);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distanceToTarget, _whatIsObstacle);

        if (hit.collider is null)
        {
            RaycastHit2D hitPlayer = Physics2D.Raycast(transform.position, direction, _sightLength, _whatIsPlayer);
            if (hitPlayer.collider is not null)
            {
                return true;
            }
        }
        return false;
    }

    protected virtual void UpdateInAttackRange()
    {
        Collider2D hitCollider = Physics2D.OverlapCircle(transform.position, _attackRange, _whatIsPlayer);
        _isInAttackRange = (hitCollider != null) ? true : false;
    }

    protected virtual void UpdateFaceDirection()
    {

        float targetHorizontalDirection = Mathf.Sign(_target.position.x - transform.position.x);

        if (targetHorizontalDirection < 0)
        {
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRange);
        Gizmos.color = Color.green;
        if (_target != null)
        {
            Gizmos.DrawLine(transform.position, transform.position + (_target.position - transform.position).normalized * _sightLength);
        }
    }

}
