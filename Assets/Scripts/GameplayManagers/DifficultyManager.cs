using UnityEngine;
using UnityEngine.SceneManagement;

public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager Instance { get; private set; }

    public enum SelectedDifficulty
    {
        Easy,
        Normal,
        Hard
    }

    [Header("Health Amount")]
    [SerializeField] private int _easyHealthAmount;
    [SerializeField] private int _normalHealthAmount;
    [SerializeField] private int _hardHealthAmount;

    [Header("Monsters Movement Speed Modifier")]
    [SerializeField] private float _easyMovementModifier;
    [SerializeField] private float _normalMovementModifier;
    [SerializeField] private float _hardMovementModifier;

    [Header("Monsters Fire Rate Modifier")]
    [SerializeField] private float _easyFireRateModifier;
    [SerializeField] private float _normalFireRateModifier;
    [SerializeField] private float _hardFireRateModifier;

    [Header("Available Poisonous Sources Amount")]
    [SerializeField] private int _easyPoisonousSources;
    [SerializeField] private int _normalPoisonousSources;
    [SerializeField] private int _hardPoisonousSources;

    [Header("Tree Health Amount")]
    [SerializeField] private float _easyTreeHealth;
    [SerializeField] private float _normalTreeHealth;
    [SerializeField] private float _hardTreeHealth;

    [Header("For debugging only")]
    [SerializeField] private SelectedDifficulty _currentlySelectedDifficulty;

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
        if (HitsCounter.Instance is not null)
        {
            HitsCounter.Instance.SetMaxHealth(GetMaxHealthValue());
        }
        if (DifficultyUI.Instance is not null)
        {
            DifficultyUI.Instance.DisplayDifficulty(_currentlySelectedDifficulty.ToString());
        }
    }
    private int GetMaxHealthValue()
    {
        switch (_currentlySelectedDifficulty)
        {
            case SelectedDifficulty.Easy:
                return _easyHealthAmount;
            case SelectedDifficulty.Normal:
                return _normalHealthAmount;
            case SelectedDifficulty.Hard:
                return _hardHealthAmount;
            default:
                return 0;
        }
    }

    public float GetMoveSpeedModifier()
    {
        switch (_currentlySelectedDifficulty)
        {
            case SelectedDifficulty.Easy:
                return _easyMovementModifier;
            case SelectedDifficulty.Normal:
                return _normalMovementModifier;
            case SelectedDifficulty.Hard:
                return _hardMovementModifier;
            default:
                return 0;
        }
    }

    public float GetFireRateModifier()
    {
        switch (_currentlySelectedDifficulty)
        {
            case SelectedDifficulty.Easy:
                return _easyFireRateModifier;
            case SelectedDifficulty.Normal:
                return _normalFireRateModifier;
            case SelectedDifficulty.Hard:
                return _hardFireRateModifier;
            default:
                return 0;
        }
    }

    public int GetPoisonousSourcesAmount()
    {
        switch (_currentlySelectedDifficulty)
        {
            case SelectedDifficulty.Easy:
                return _easyPoisonousSources;
            case SelectedDifficulty.Normal:
                return _normalPoisonousSources;
            case SelectedDifficulty.Hard:
                return _hardPoisonousSources;
            default:
                return 0;
        }
    }

    public float GetTreeHealth()
    {
        switch (_currentlySelectedDifficulty)
        {
            case SelectedDifficulty.Easy:
                return _easyTreeHealth;
            case SelectedDifficulty.Normal:
                return _normalTreeHealth;
            case SelectedDifficulty.Hard:
                return _hardTreeHealth;
            default:
                return 0;
        }
    }

    public void Select(SelectedDifficulty selection) => _currentlySelectedDifficulty = selection;
}
