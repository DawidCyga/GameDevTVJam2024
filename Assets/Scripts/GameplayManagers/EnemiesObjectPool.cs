using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesObjectPool : MonoBehaviour
{

    public static EnemiesObjectPool Instance {  get; private set; }

    [Header("Enemy Types to pool")]
    [SerializeField] private Transform _possessedKhukhaTransform;
    [SerializeField] private Transform _possessedKhukhaRangedTransform;
    [SerializeField] private Transform _babaiTransform;
    [SerializeField] private Transform _possessedFireKhukhaTransform;

    [Header("Pool Size Configuration")]
    [SerializeField] private int _possessedKhukhaPoolSize;
    [SerializeField] private int _possessedKhukhaRangedPoolSize;
    [SerializeField] private int _babaiPoolSize;
    [SerializeField] private int _possessedFireKhukhaPoolSize;

    [Header("Pools")]
    private Transform[] _possessedKhukhaPool;
    private Transform[] _possessedKhukhaRangedPool;
    private Transform[] _babaiPool;
    private Transform[] _possessedFireKhukhaPool;

    private Dictionary<Transform, Transform[]> _typePoolDictionary = new Dictionary<Transform, Transform[]>();

    private void Awake()
    {
        Instance = this;

        InitializeAndPopulatePools();
    }

    private void InitializeAndPopulatePools()
    {
        _possessedKhukhaPool = InitializeAndPopulateSinglePool(_possessedKhukhaPoolSize, _possessedKhukhaTransform);
        _possessedKhukhaRangedPool = InitializeAndPopulateSinglePool(_possessedKhukhaRangedPoolSize, _possessedKhukhaRangedTransform);
        _babaiPool = InitializeAndPopulateSinglePool(_babaiPoolSize, _babaiTransform);
        _possessedFireKhukhaPool = InitializeAndPopulateSinglePool(_possessedFireKhukhaPoolSize, _possessedFireKhukhaTransform);
    }

    private Transform[] InitializeAndPopulateSinglePool(int size, Transform objectTransform)
    {
        Transform[] pool = new Transform[size];

        for (int i = 0; i < size; i++)
        {
            Transform objectInstance = Instantiate(objectTransform, transform.position, Quaternion.identity, transform);
            objectInstance.gameObject.SetActive(false);
            pool[i] = objectInstance;
        }

        _typePoolDictionary[objectTransform] = pool;
        return pool;
    }

    public Transform PoolEnemyObject(Transform enemyToPool)
    {
        if (_typePoolDictionary.TryGetValue(enemyToPool, out Transform[] pool))
        {
            foreach (Transform enemy in pool)
            {
                if (!enemy.gameObject.activeInHierarchy)
                {
                    enemy.gameObject.SetActive(true);
                    return enemy;
                }
            }
        }

        Debug.LogWarning("No inactive enemy found in the pool for: " + enemyToPool.name);
        return null;
    }
}
