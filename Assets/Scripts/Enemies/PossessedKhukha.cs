using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PossessedKhukha : PathfinderEnemy
{

    private void Update()
    {
        _timeSinceLastUpdatedPath += Time.deltaTime;

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
            Attack();
            //Debug.Log("I attack player");
        }
       
    }

    protected override void UpdateInAttackRange() => base.UpdateInAttackRange();

    protected override void UpdateFaceDirection() => base.UpdateFaceDirection();

    protected override bool NeedsPathUpdate() =>    base.NeedsPathUpdate();

    protected override void FindPathToPlayer() => base.FindPathToPlayer();
    
    protected override bool CanSeePlayer() => base.CanSeePlayer();
    
    private void Attack()
    {
        if (PlayerHitBox.Instance is not null)
        {
            PlayerHitBox.Instance.TakeDamage();            
        }
    }

    public override void UpdateSlowDown()
    {
        base.UpdateSlowDown();
    }
}
