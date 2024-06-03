using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class PossessedFireKhukha : PathfinderEnemy
{
    [Header("Fire Khukha Specific Configuration")]
    [SerializeField] private float _aggroDistance;
    [SerializeField] private float _timeBetweenAttacks;

    [Header("Fire Khukha: For debugging only")]
    [SerializeField] private float _timeSinceLastAttacked;

    private void Update()
    {
        _timeSinceLastUpdatedPath += Time.deltaTime;
        _timeSinceLastAttacked += Time.deltaTime;

        UpdateInAttackRange();
        UpdateFaceDirection();
        UpdateSlowDown();

        if (NeedsPathUpdate())
        {
            _timeSinceLastUpdatedPath = 0;
            
            if (!aggroPlayer() && FindNearestTree()!= null)
            {
                FindPathToEntity(FindNearestTree());
            }
            else
            {
                FindPathToPlayer();
            }
        }

        if (_isInAttackRange && CanSeePlayer())
        {
            TryAttack();
        }
    }

    protected override void UpdateInAttackRange() => base.UpdateInAttackRange();
    protected override void UpdateFaceDirection() => base.UpdateFaceDirection();
    protected override bool NeedsPathUpdate() =>    base.NeedsPathUpdate();
    protected override void FindPathToPlayer() => base.FindPathToPlayer();
    protected override bool CanSeePlayer() => base.CanSeePlayer();
    public override void UpdateSlowDown() => base.UpdateSlowDown();


    private void TryAttack()
    {
        if (_timeSinceLastAttacked > _timeBetweenAttacks)
        {
            Attack();
            _timeSinceLastAttacked = 0;
        }
    }

    private void Attack()
    {
        Debug.Log("I attack you");
        if (HitsCounter.Instance is not null)
        {
            HitsCounter.Instance.Hit(Enemy.EnemyType.FireKhukha);
            Debug.Log("I hit player");
        }
    }
      
    private bool aggroPlayer()
    {
        float distanceFromPlayer = Vector3.Distance(_target.position, transform.position);

        return (distanceFromPlayer < _aggroDistance);
    }

    Transform FindNearestTree()
    {
        GameObject[] trees = GameObject.FindGameObjectsWithTag("Tree");
        Transform nearestTree = null;
        float shortestDistance = Mathf.Infinity;

        foreach (GameObject tree in trees)
        {
            TreeScript treeScript = tree.GetComponentInChildren<TreeScript>();

            if (!treeScript._isIgnited)
            {
                float distanceToTree = Vector3.Distance(transform.position, tree.transform.position);
                if (distanceToTree < shortestDistance)
                {
                    shortestDistance = distanceToTree;
                    nearestTree = tree.transform;
                }
            }
        }

        return nearestTree;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _aggroDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRange);
        Gizmos.color = Color.green;
        if (_target != null)
        {
            Gizmos.DrawLine(transform.position, transform.position + (_target.position - transform.position).normalized * _sightLength);
        }
    }
}
