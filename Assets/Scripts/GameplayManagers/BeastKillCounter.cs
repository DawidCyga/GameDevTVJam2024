using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BeastKillCounter : MonoBehaviour
{
    public static BeastKillCounter Instance { get; private set; }

    [Header("For debugging only")]

    [SerializeField] private int _babaiCount;
    [SerializeField] private int _khukhaCount;
    [SerializeField] private int _rangedKhukha;
    [SerializeField] private int _fireKhukha;

    [SerializeField] private bool _hasSubscribedToEnemy;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        }
    }

    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if (arg0.buildIndex == 3 && !_hasSubscribedToEnemy)
        {
            Enemy.OnAnyEnemyDeath += Enemy_OnAnyEnemyDeath;
            _hasSubscribedToEnemy = true;
        }
    }

    private void Enemy_OnAnyEnemyDeath(object sender, System.EventArgs e)
    {
        switch (sender)
        {
            case Babai:
                _babaiCount++;
                break;
            case PossessedKhukha:
                _khukhaCount++;
                break;
            case PossessedKhukhaRanged:
                _rangedKhukha++;
                break;
            case PossessedFireKhukha:
                _fireKhukha++;
                break;
            default:
                Debug.Log("I don't know this enemy");
                break;
        }
    }

    public int GetCount(Enemy.EnemyType type)
    {
        switch (type)
        {
            case Enemy.EnemyType.Babai:
                return _babaiCount;
            case Enemy.EnemyType.Khukha:
                return _khukhaCount;
            case Enemy.EnemyType.RangedKhukha:
                return _rangedKhukha;
            case Enemy.EnemyType.FireKhukha:
                return _fireKhukha;
            default:
                Debug.Log("No such enemy type");
                return 0;
        }
    }
}
