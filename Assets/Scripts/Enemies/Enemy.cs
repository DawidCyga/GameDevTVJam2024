using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class Enemy : MonoBehaviour, ITakeDamage
{
    public enum EnemyType
    {
        Babai,
        BabaiHand,
        RangedKhukha,
        Khukha,
        RangedFireKhukha,
        FireKhukha,
        Story,
        PoisonousTrail
    }

    [Header("General Configuration")]
    [SerializeField] protected float _moveSpeed;
    [SerializeField] protected float _attackRange;
    [SerializeField] protected float _sightLength;
    [SerializeField] protected EnemyType _enemyType;

    [Header("Layer Masks")]
    [SerializeField] protected LayerMask _whatIsObstacle;
    [SerializeField] protected LayerMask _whatIsPlayer;

    protected Transform _target;

    [Header("For debugging only")]
    [SerializeField] protected bool _isInAttackRange;
    [SerializeField] private bool _isDead;

    public static event EventHandler OnAnyEnemyDeath;

    protected virtual void Start()
    {
        _target = Player.Instance.transform;
    }

    protected virtual void OnEnable()
    {
        _isDead = false;
    }

    public virtual void TakeDamage()
    {
        if (!_isDead)
        {
            WaveSpawner.Instance.DecreaseTotalEnemiesSpawnedCurrentWave();
            OnAnyEnemyDeath?.Invoke(this, EventArgs.Empty);

            gameObject.SetActive(false);

            _isDead = true;
        }
    }


    protected virtual bool CanSeePlayer()
    {
        //Vector3 direction = (_target.position - transform.position).normalized;
        //float distanceToTarget = Vector3.Distance(transform.position, _target.position);

        //RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distanceToTarget, _whatIsObstacle);

        //if (hit.collider is null)
        //{
        //    RaycastHit2D hitPlayer = Physics2D.Raycast(transform.position, direction, _sightLength, _whatIsPlayer);
        //    if (hitPlayer.collider is not null)
        //    {
        //        return true;
        //    }
        //}
        //return false;

       return CanSeeEntity(_target);
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

    // testing difficulty system
    protected virtual void SetDifficultyModifiers()     
    {
        if (DifficultyManager.Instance != null)
        {
            _moveSpeed = _moveSpeed * DifficultyManager.Instance.GetMoveSpeedModifier();
        }
    }

    public EnemyType GetEnemyType() => _enemyType;

    public virtual bool CanBeKilledByRegularTrail() => false;
    
   
    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(transform.position, _attackRange);
    //    Gizmos.color = Color.green;
    //    if (_target != null)
    //    {
    //        Gizmos.DrawLine(transform.position, transform.position + (_target.position - transform.position).normalized * _sightLength);
    //    }
    //}
}
