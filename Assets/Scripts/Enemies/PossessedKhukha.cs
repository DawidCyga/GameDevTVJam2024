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
  //  private Animator _animator;
  //  private int _animAttackHash = Animator.StringToHash("Attack");

    [Header("Optimization Parameters")]
    [SerializeField] private float _tryAttackRefreshRate;

    private Coroutine _updateCanAttackCoroutine;

    private void Awake()
    {
      //  _animator = GetComponent<Animator>();
        SetDifficultyModifiers();

    }

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
                FindPathToPlayer();
            }
        }
    }

    private void Update()
    {
        _timeSinceLastUpdatedPath += Time.deltaTime;
        _timeSinceLastAttacked += Time.deltaTime;

        //if (NeedsPathUpdate())
        //{
        //    _timeSinceLastUpdatedPath = 0;
        //    FindPathToPlayer();
        //}  
    }

  //  protected override void UpdateInAttackRange() => base.UpdateInAttackRange();
  //  protected override void UpdateFaceDirection() => base.UpdateFaceDirection();
   // protected override bool NeedsPathUpdate() =>    base.NeedsPathUpdate();
   // protected override void FindPathToPlayer() => base.FindPathToPlayer();
  //  protected override bool CanSeePlayer() => base.CanSeePlayer();
   // public override void UpdateSlowDown() => base.UpdateSlowDown();

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
            HitsCounter.Instance.Hit(Enemy.EnemyType.Khukha);
          //  _animator.SetTrigger(_animAttackHash);
            Debug.Log("Possessed Khukha attack");
        }
    }

}
