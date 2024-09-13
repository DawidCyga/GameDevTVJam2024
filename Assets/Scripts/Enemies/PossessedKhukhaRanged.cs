using System.Collections;
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

    private Animator _animator;
    private int _animAttackHash = Animator.StringToHash("Attack");

    [Header("Optimization Parameters")]
    [SerializeField] private float _tryAttackRefreshRate;

    private Coroutine _updateCanAttackCoroutine;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        SetDifficultyModifiers();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
       _updateCanAttackCoroutine = StartCoroutine(UpdateCanAttackRoutine());
    }

    private void OnDisable()
    {
        if (_updateCanAttackCoroutine != null)
        {
            StopCoroutine(_updateCanAttackCoroutine);
            _updateCanAttackCoroutine = null;
        }
    }

    private IEnumerator UpdateCanAttackRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(_tryAttackRefreshRate);

            UpdateInAttackRange();
            UpdateFaceDirection();

            if (_isInAttackRange && CanSeePlayer())
            {
                TryShoot();
                _isShooting = true;
            }
            else
            {
                _isShooting = false;
            }

            if (!_isShooting && NeedsPathUpdate())
            {
                _timeSinceLastUpdatedPath = 0;
                FindPathToPlayer();
            }
        }
    }

    private void Update()
    {
        _timeSinceLastUpdatedPath += Time.deltaTime;
        _timeSinceLastShot += Time.deltaTime;
    }

    protected override void SetDifficultyModifiers()
    {
        base.SetDifficultyModifiers();
        if (DifficultyManager.Instance != null)
        {
            _timeBetweenShots = _timeBetweenShots * DifficultyManager.Instance.GetFireRateModifier();
        }
    }

    private void TryShoot()
    {
        UpdateFirePointPosition();
        if (_timeSinceLastShot > _timeBetweenShots)
        {
            Shoot();
            _animator.SetTrigger(_animAttackHash);
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
