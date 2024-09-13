using System.Collections;
using UnityEngine;

public class PossessedFireKhukha : PathfinderEnemy
{
    [Header("Fire Khukha Specific Configuration")]
    [SerializeField] private float _aggroDistance;
    [SerializeField] private float _timeBetweenAttacks;

    [Header("Fire Khukha: For debugging only")]
    [SerializeField] private float _timeSinceLastAttacked;

    [Header("Optimization Parameters")]
    [SerializeField] private float _tryAttackRefreshRate;

    private Coroutine _updateCanAttackCoroutine;

    private void Awake() => SetDifficultyModifiers();

    protected override void OnEnable()
    {
        base.OnEnable();

        _timeSinceLastAttacked = Mathf.Infinity;
        _updateCanAttackCoroutine = StartCoroutine(UpdateAttackRoutine());
    }

    private void OnDisable()
    {
        if (_updateCanAttackCoroutine != null)
        {
            StopCoroutine(_updateCanAttackCoroutine);
            _updateCanAttackCoroutine = null;
        }
    }

    private IEnumerator UpdateAttackRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(_tryAttackRefreshRate);

            UpdateInAttackRange();
            UpdateFaceDirection();

            if (_isInAttackRange && CanSeePlayer())
            {
                TryAttack();
            }

            if (NeedsPathUpdate())
            {
                _timeSinceLastUpdatedPath = 0;

                if (!aggroPlayer() && FindNearestTree() != null)
                {
                    FindPathToEntity(FindNearestTree());
                }
                else
                {
                    FindPathToPlayer();
                }
            }
        }
    }

    private void Update()
    {
        _timeSinceLastUpdatedPath += Time.deltaTime;
        _timeSinceLastAttacked += Time.deltaTime;
    }

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
            HitsCounter.Instance.Hit(Enemy.EnemyType.FireKhukha);
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

    public override bool CanBeKilledByRegularTrail() => true;
}
