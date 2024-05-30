using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PossessedKhukhaRanged : PathfinderEnemy
{
    [Header("Ranged Khukha Firing Setup")]
    [SerializeField] private Transform _firePoint;
    [SerializeField] private Transform _firePointPivot;
    [SerializeField] private Transform _projectilePrefab;
    [SerializeField] private Transform _projectilesParent;
    [Space]
    [SerializeField] private float _timeBetweenShots;
    [Space]
    [SerializeField] private float _fireForce;

    [Header("Ranged Khukha: For debugging only")]
    [SerializeField] private float _timeSinceLastShot;
    [SerializeField] private bool _isShooting;


    private void Update()
    {
        _timeSinceLastUpdatedPath += Time.deltaTime;
        _timeSinceLastShot += Time.deltaTime;

        UpdateInAttackRange();
        UpdateFaceDirection();

        if (!_isShooting && NeedsPathUpdate())
        {
            _timeSinceLastUpdatedPath = 0;
            FindPathToPlayer();
        }

        if (_isInAttackRange && CanSeePlayer())
        {
            TryShoot();
            _isShooting = true;
        }
        else
        {
            _isShooting = false;
        }

    }

    protected override void UpdateInAttackRange() => base.UpdateInAttackRange();

    protected override void UpdateFaceDirection() => base.UpdateFaceDirection();

    protected override bool NeedsPathUpdate() => base.NeedsPathUpdate();

    protected override void FindPathToPlayer() => base.FindPathToPlayer();

    protected override bool CanSeePlayer() => base.CanSeePlayer();

    private void TryShoot()
    {
        UpdateFirePointPosition();

        if (_timeSinceLastShot > _timeBetweenShots)
        {
            Shoot();
            _timeSinceLastShot = 0;
        }
    }

    private void Shoot()
    {
        Vector3 direction = _target.position - _firePoint.position;

        Transform enemyProjectilePrefab = Instantiate(_projectilePrefab, _firePoint.position, _firePoint.localRotation, _projectilesParent);

        float targetZRotation = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        enemyProjectilePrefab.rotation = Quaternion.Euler(0, 0, targetZRotation - 90);

        EnemyProjectile enemyProjectile = enemyProjectilePrefab.GetComponent<EnemyProjectile>();

        enemyProjectile.Launch(direction, _fireForce);
    }

    private void UpdateFirePointPosition()
    {
        Vector3 direction = _target.position - _firePointPivot.position;

        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        _firePointPivot.rotation = Quaternion.Euler(0, 0, targetAngle);
    }
}
