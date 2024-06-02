using System;
using UnityEngine;

public class BabaiHand : MonoBehaviour
{

    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _timeBetweenAttacks;
    [SerializeField] private float _timeBeforeCanAttack;

    [Header("For debugging only")]
    [SerializeField] private float _timeSinceLastAttack;
    [SerializeField] private float _timeSinceSpawned;
    [SerializeField] private bool _isDead;


    private Transform _target;
    private Babai _babai;

    private void Start()
    {
        _target = Player.Instance.transform;
    }

    private void OnDestroy()
    {
        _babai.OnDeath += BabaiSummoner_OnDeath;
    }

    private void Update()
    {
        if (_timeSinceSpawned > _timeBeforeCanAttack)
        {
            TryAttackPlayer();
        }

        _timeSinceLastAttack += Time.deltaTime;
        _timeSinceSpawned += Time.deltaTime;
    }

    public void AssignBabai(Transform babaiTransform)
    {
        _babai = babaiTransform.GetComponent<Babai>();
        _babai.OnDeath += BabaiSummoner_OnDeath;
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
        if (collision != null && collision.TryGetComponent(out HitsCounter hitCounter))
        {
            hitCounter.Hit(Enemy.EnemyType.BabaiHand);
            _timeSinceLastAttack = 0;
        }
    }


}
