using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Babai : Enemy
{
    [Header("Babai Specific configuration")]
    [SerializeField] private float _playerEvadeSpeed;
    [SerializeField] private float _followPathSpeed;

    [SerializeField] private float _distanceToWallBehind;
    [SerializeField] private LayerMask _whatIsWall;

    private enum State
    {
        Offence,
        Defence
    }

    [SerializeField] private State _state;

    private PathFollower _pathFollower;
    private TargetEvader _targetEvader;

    private void Awake()
    {
        _pathFollower = GetComponent<PathFollower>();
        _targetEvader = GetComponent<TargetEvader>();
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

                TryEvadePlayer();

                UpdateFaceDirection();
                TrySwitchToOffence();
                break;
        }

        
    }

    private void FollowPath()
    {
        _pathFollower.Follow(_followPathSpeed);
    }

    private void TrySwitchToDefence()
    {
        if (CanSeePlayer())
        {
            _state = State.Defence;
        }
    }

    private void TryEvadePlayer()
    {
        if (!IsDetectingWallBehind())
        {
            _targetEvader.MoveAwayHorizontally(_target.position, _playerEvadeSpeed);
        }
    }

    private bool IsDetectingWallBehind() => Physics2D.Raycast(transform.position, -transform.right, _distanceToWallBehind, _whatIsWall);

    private void TrySwitchToOffence()
    {
        if (!CanSeePlayer())
        {
            _state = State.Offence;
        }
    }

    protected override bool CanSeePlayer() => base.CanSeePlayer();

    protected override void UpdateFaceDirection() => base.UpdateFaceDirection();

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x - _distanceToWallBehind, transform.position.y));
    }

}
