using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;

public class BabaiHand : MonoBehaviour
{

    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _timeBetweenAttacks;

    [Header("For debugging only")]
    [SerializeField] private float _timeSinceLastAttack;
    [SerializeField] private bool _isDead;


    private Transform _target;

    private void Start()
    {
        _target = Player.Instance.transform;
    }

    private void Update()
    {
        TryAttackPlayer();

        _timeSinceLastAttack += Time.deltaTime;
    }

    public void AssignBabai(Transform babaiTransform)
    {
        Babai babai = babaiTransform.GetComponent<Babai>();
        babai.OnDeath += BabaiSummoner_OnDeath;
    }

    private void BabaiSummoner_OnDeath(object sender, EventArgs e)
    {
        // ANIMATION?
        if (!_isDead)
        {
            Destroy(gameObject);
            _isDead = true;
        }
    }

    private void TryAttackPlayer()
    {
        if (_timeSinceLastAttack > _timeBetweenAttacks)
        {
            MoveTowardsPlayer();
        }
    }

    private void MoveTowardsPlayer()
    {
        Vector3 directionToPlayer = (_target.position - transform.position).normalized;
        transform.position += directionToPlayer * _moveSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_timeSinceLastAttack < _timeBetweenAttacks) { return; }
        if (collision != null && collision.TryGetComponent(out BabaiHitsCounter hitCounter))
        {
            hitCounter.Hit();
            _timeSinceLastAttack = 0;
        }
    }


}
