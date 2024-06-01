using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PossessedKhukha : PathfinderEnemy
{

    [Header("Khukha Specific Configuration")]
    [SerializeField] private float _timeBetweenAttacks;

    [Header("Khukha: For debugging only")]
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
            FindPathToPlayer();
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
        if (HitsCounter.Instance is not null)
        {
            HitsCounter.Instance.Hit(HitsCounter.HitType.Khukha);
            Debug.Log("I hit player");
        }
    }

    public override void UpdateSlowDown()
    {
        base.UpdateSlowDown();
    }
}
