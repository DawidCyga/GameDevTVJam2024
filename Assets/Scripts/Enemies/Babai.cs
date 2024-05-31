using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Babai : Enemy
{
    private enum State
    {
        Offence,
        Defence
    }

    private State _state;

    [Header("For debugging only")]
    [SerializeField] private bool _isInDefence;
    [SerializeField] private bool _isInOffence;

    protected override void Start()
    {
        base.Start();
    }

    private void Update()
    {
        switch (_state)
        {
            case State.Offence:
                
                TrySwitchToDefence();
                break;
            case State.Defence:
                

                TrySwitchToOffence();
                break;
        }

        UpdateFaceDirection();
    }

    private void TrySwitchToDefence()
    {
        if (CanSeePlayer())
        {
            _state = State.Defence;
            _isInDefence = true;
            _isInOffence = false;
            Debug.Log("Switched to defence");
        }
    }
    private void TrySwitchToOffence()
    {
        if (!CanSeePlayer())
        {
            _state = State.Offence;
            _isInOffence = true;
            _isInDefence = false;
            Debug.Log("Switched to offence");
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
