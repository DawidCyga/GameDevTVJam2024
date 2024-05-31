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

    [SerializeField] private State _state;

    [Header("Babai Specific Configuration")]
    [SerializeField] private float _playerEvadeSpeed;
    [SerializeField] private float _followPathSpeed;

    [Header("Babai Hands Spawning Configuration")]
    [SerializeField] private float _maxSpawnedHandsNumber;
    [SerializeField] private float _timeBetweenSpawningHands;

    [Header("Babai Wall Detection Configuration")]
    [SerializeField] private float _distanceToWallBehind;
    [SerializeField] private LayerMask _whatIsWall;

    [Header("For debugging only")]
    [SerializeField] private float _currentSpawnedHandsNumber;
    [SerializeField] private float _timeSinceLastSpawnedHand;

    private PathFollower _pathFollower;
    private HandsSpawner _handsSpawner;
    private TargetEvader _targetEvader;

    public event EventHandler OnDeath;

    private void Awake()
    {
        _pathFollower = GetComponent<PathFollower>();
        _handsSpawner = GetComponent<HandsSpawner>();
        _targetEvader = GetComponent<TargetEvader>();

        _currentSpawnedHandsNumber = 0;
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
                TrySpawnHands();

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

    private void TrySpawnHands()
    {
        if (_timeSinceLastSpawnedHand > _timeBetweenSpawningHands && _currentSpawnedHandsNumber < _maxSpawnedHandsNumber)
        {
            _handsSpawner.Spawn();
            _currentSpawnedHandsNumber++;
            _timeSinceLastSpawnedHand = 0;
        }

        _timeSinceLastSpawnedHand += Time.deltaTime;
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

    public override void TakeDamage()
    {
        OnDeath?.Invoke(this, EventArgs.Empty);
        base.TakeDamage();
    }
}
