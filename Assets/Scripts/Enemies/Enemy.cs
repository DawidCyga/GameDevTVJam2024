using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour, ITakeDamage, ICanBeStunned
{

    [Header("General Configuration")]
    [SerializeField] protected float _moveSpeed;
    [SerializeField] protected float _attackRange;
    [SerializeField] protected float _sightLength;

    [Header("Layer Masks")]
    [SerializeField] protected LayerMask _whatIsObstacle;
    [SerializeField] protected LayerMask _whatIsPlayer;

    public float TimeTillEndStun { get; protected set; }
    public bool IsStunned { get; protected set; } = false;

    protected Transform _target;

    [Header("For debugging only")]
    [SerializeField] protected bool _isInAttackRange;
    [SerializeField] private bool _isFacingRight;

    private void Start()
    {
        _target = Player.Instance.transform;
        _isFacingRight = true;
    }

    public virtual void TakeDamage()
    {
        Debug.Log("I got killed");
        //TRIGGER DEATH ANIMATION
        Destroy(gameObject);
    }
    public virtual void TryStun(float stunDuration)
    {
        if (IsStunned) { return; }
        TimeTillEndStun = stunDuration;
        IsStunned = true;
    }

    protected virtual bool CanSeePlayer()
    {
        if (Physics2D.Raycast(transform.position, (_target.position - transform.position).normalized, _sightLength, _whatIsObstacle))
        {
            return false;
        }
        return true;
    }

    protected virtual void UpdateInAttackRange()
    {
        Collider2D hitCollider = Physics2D.OverlapCircle(transform.position, _attackRange, _whatIsPlayer);
        _isInAttackRange = (hitCollider != null) ? true : false;

    }

    protected virtual void UpdateFaceDirection()
    {
        if (_target.position.x > transform.position.x && !_isFacingRight)
        {
            SwapFaceDirection();
        }
        else if (_target.position.x < transform.position.x && _isFacingRight)
        {
            SwapFaceDirection();
        }
    }

    private void SwapFaceDirection()
    {
        _isFacingRight = !_isFacingRight;
        transform.Rotate(0, 180, 0);
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
