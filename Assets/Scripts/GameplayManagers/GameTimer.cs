using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    public static GameTimer Instance { get; private set; }

    [Header("For debugging only")]
    [SerializeField] private float _timeSinceStartedPlaying;
    [SerializeField] private bool _isTimeCounting;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        GameStateManager.Instance.OnGameStateChanged += GameStateManager_OnGameStateChanged;
    }

    private void GameStateManager_OnGameStateChanged(object sender, GameStateManager.OnGameStateChangedEventArgs e)
    {
        _isTimeCounting = (e.GameState == GameStateManager.GameState.Playing) ? true : false;
    }

    private void Update()
    {
        if (_isTimeCounting)
        {
            _timeSinceStartedPlaying += Time.deltaTime;
        }
    }

    public float GetTimeSinceStartedPlaying() => _timeSinceStartedPlaying;

}
