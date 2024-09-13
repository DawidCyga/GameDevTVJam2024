using UnityEngine;
using UnityEngine.SceneManagement;

public class GameTimer : MonoBehaviour
{
    public static GameTimer Instance { get; private set; }

    [Header("For debugging only")]
    [SerializeField] private float _timeSinceStartedPlaying;
    [SerializeField] private bool _isTimeCounting;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if (arg0.buildIndex == 0 || arg0.buildIndex == 1)
        {
            Destroy(gameObject);
            return;
        }
        if (arg0.buildIndex != 3)
        {
            _isTimeCounting = false;
        }

        if (GameStateManager.Instance is not null)
        {
            GameStateManager.Instance.OnGameStateChanged += GameStateManager_OnGameStateChanged;
        }
    }
    private void OnDestroy()
    {
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.OnGameStateChanged -= GameStateManager_OnGameStateChanged;
        }
        SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
    }

    private void GameStateManager_OnGameStateChanged(object sender, GameStateManager.OnGameStateChangedEventArgs e) 
        => _isTimeCounting = (e.GameState == GameStateManager.GameState.Playing) ? true : false;

    private void Update()
    {
        if (_isTimeCounting)
        {
            _timeSinceStartedPlaying += Time.deltaTime;
        }
    }
    public float GetTimeSinceStartedPlaying() => _timeSinceStartedPlaying;
}
