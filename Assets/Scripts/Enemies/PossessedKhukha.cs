using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PossessedKhukha : PathfinderEnemy
{




    private void Update()
    {
        _timeSinceLastUpdatedPath += Time.deltaTime;

        UpdateInAttackRange();

        if (NeedsPathUpdate())
        {
            Debug.Log("Needs path update");
            _timeSinceLastUpdatedPath = 0;
            FindPathToPlayer();
        }

        if (_isInAttackRange && CanSeePlayer())
        {
            // Attack();
            Debug.Log("I attack player");
        }

        UpdateFaceDirection();
    }

    protected override void UpdateInAttackRange()
    {
        base.UpdateInAttackRange();
    }

    protected override bool NeedsPathUpdate()
    {
        return base.NeedsPathUpdate();
    }

    protected override void FindPathToPlayer()
    {
        base.FindPathToPlayer();
    }

    protected override bool CanSeePlayer()
    {
        return base.CanSeePlayer();
    }

    

    protected override void UpdateFaceDirection()
    {
        base.UpdateFaceDirection();
    }

    
}
