using UnityEngine;

public class VolumeSettingsHolder : MonoBehaviour
{
    public static VolumeSettingsHolder Instance { get; private set; }

    [field: SerializeField] public float MasterVolume { get; set; }
    [field: SerializeField] public float MusicVolume { get; set; }
    [field: SerializeField] public float SFXVolume { get; set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
