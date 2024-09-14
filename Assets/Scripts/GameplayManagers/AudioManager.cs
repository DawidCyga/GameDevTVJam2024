using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    public enum AudioName
    {
       Dash,
       Jump,
       PlayerDeath,
       EnemyDeath,
       ButtonHover,
       ButtonClick
    }

    [SerializeField] private List<AudioClip> _audioClipList = new List<AudioClip>();

    private Dictionary<AudioName, AudioClip> _enumNameAudioClipDictionary = new Dictionary<AudioName, AudioClip>();

    private AudioSource _audioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            SceneManager.sceneLoaded += SceneManager_sceneLoaded;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        _audioSource = GetComponent<AudioSource>();

        foreach (AudioName audioName in System.Enum.GetValues(typeof(AudioName)))
        {
            _enumNameAudioClipDictionary[audioName] = _audioClipList[(int)audioName];
        }
    }

    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if (Player.Instance is not null)
        {
            Player.Instance.OnStartedJump += Player_OnStartedJump;
            Player.Instance.OnStartedDash += Player_OnStartedDash;
        }
        if (PlayerHitBox.Instance is not null)
        {
            PlayerHitBox.Instance.OnPlayerDying += PlayerHitBox_OnPlayerDying;
        }
        Enemy.OnAnyEnemyDeath += Enemy_OnAnyEnemyDeath;
        ButtonSoundEmitter.OnAnyButtonSelected += ButtonSoundEmitter_OnAnyButtonSelected;
        ButtonSoundEmitter.OnAnyButtonClicked += ButtonSoundEmitter_OnAnyButtonClicked;
    }

    private void Enemy_OnAnyEnemyDeath(object sender, System.EventArgs e) => PlaySound(AudioName.EnemyDeath);
    private void PlayerHitBox_OnPlayerDying(object sender, System.EventArgs e) => PlaySound(AudioName.PlayerDeath);
    private void Player_OnStartedJump(object sender, System.EventArgs e) => PlaySound(AudioName.Jump);
    private void Player_OnStartedDash(object sender, System.EventArgs e) => PlaySound(AudioName.Dash);
    private void ButtonSoundEmitter_OnAnyButtonSelected(object sender, System.EventArgs e) => PlaySound(AudioName.ButtonHover);
    private void ButtonSoundEmitter_OnAnyButtonClicked(object sender, System.EventArgs e) => PlayOnlyOneSound(AudioName.ButtonClick);

    private void PlayOnlyOneSound(AudioName name)
    {
        if (_audioSource.isPlaying)
        {
            _audioSource.Stop();
        }
        PlaySound(name);
    }
    private void PlaySound(AudioName name) => _audioSource.PlayOneShot(_enumNameAudioClipDictionary[name]);

}
