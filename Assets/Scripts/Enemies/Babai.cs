using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Babai : Enemy
{
    [Header("Babai Specific configuration")]
    [SerializeField] private float _followPathSpeed;

    private enum State
    {
        Offence,
        Defence
    }

    [SerializeField] private State _state;

    private FollowPath _followPath;

    private void Awake()
    {
        _followPath = GetComponent<FollowPath>();
    }

    protected override void Start()
    {
        base.Start();
    }

    private void Update()
    {
        switch (_state)
        {
            case State.Offence:

                FollowPath();

                TrySwitchToDefence();
                break;
            case State.Defence:


                TrySwitchToOffence();
                break;
        }

        UpdateFaceDirection();
    }

    private void FollowPath()
    {
        _followPath.Follow(_followPathSpeed);
    }

    private void TrySwitchToDefence()
    {
        if (CanSeePlayer())
        {
            _state = State.Defence;
        }
    }
    private void TrySwitchToOffence()
    {
        if (!CanSeePlayer())
        {
            _state = State.Offence;
        }
    }

    public override void TakeDamage()
    {
        base.TakeDamage();
    }

    protected override bool CanSeePlayer()
    {
        return base.CanSeePlayer();
    }

    protected override void UpdateInAttackRange()
    {
        base.UpdateInAttackRange();
    }

    protected override void UpdateFaceDirection()
    {
        base.UpdateFaceDirection();
    }
}
