using System.Collections;
using System.Collections.Generic;
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
                return 5;
            case SelectedDifficulty.Normal:
                return 3;
            case SelectedDifficulty.Hard:
                return 1;
            default:
                return 0;
        }
    }

    

    public void Select(SelectedDifficulty selection)
    {
        _currentlySelectedDifficulty = selection;
    }

}
