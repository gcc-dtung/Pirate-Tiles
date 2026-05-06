using UnityEngine;

public class StartScreenController : MonoBehaviour
{
    [Header("View")]
    [SerializeField] private StartScreenView _startScreenView;
    [SerializeField] private SettingPanelView _settingPanelView;

    [Header("Config")]
    [SerializeField] private GameConfigSO _gameConfig;

    private void Awake()
    {
        EnsureDefaultSaveData();
    }

    private void Start()
    {
        EnsureDefaultSaveData();

        if (_startScreenView != null)
        {
            _startScreenView.SetQuitVisible(!Application.isMobilePlatform);
        }

        ApplySettingViewState();
        RefreshStatsView();

        if (_settingPanelView != null)
        {
            _settingPanelView.gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        if (_startScreenView != null)
        {
            _startScreenView.OnPlayClicked += HandlePlayClicked;
            _startScreenView.OnSettingsClicked += HandleSettingsClicked;
            _startScreenView.OnQuitClicked += HandleQuitClicked;
        }

        if (_settingPanelView != null)
        {
            _settingPanelView.OnMusicToggled += HandleMusicToggled;
            _settingPanelView.OnSfxToggled += HandleSfxToggled;
            _settingPanelView.OnContinueClicked += HandleContinueClicked;
            _settingPanelView.OnReplayClicked += HandlePlayClicked;
            _settingPanelView.OnMapClicked += HandleMapClicked;
        }
    }

    private void OnDisable()
    {
        if (_startScreenView != null)
        {
            _startScreenView.OnPlayClicked -= HandlePlayClicked;
            _startScreenView.OnSettingsClicked -= HandleSettingsClicked;
            _startScreenView.OnQuitClicked -= HandleQuitClicked;
        }

        if (_settingPanelView != null)
        {
            _settingPanelView.OnMusicToggled -= HandleMusicToggled;
            _settingPanelView.OnSfxToggled -= HandleSfxToggled;
            _settingPanelView.OnContinueClicked -= HandleContinueClicked;
            _settingPanelView.OnReplayClicked -= HandlePlayClicked;
            _settingPanelView.OnMapClicked -= HandleMapClicked;
        }
    }

    private void EnsureDefaultSaveData()
    {
        var save = SaveService.Instance;
        if (save == null) return;

        int defaultHearts = _gameConfig != null ? _gameConfig.MaxHearts : 5;
        int defaultCoins = _gameConfig != null ? _gameConfig.StartingCoins : 100;

        if (!save.HasKey(SaveKeys.Hearts))
        {
            save.SetInt(SaveKeys.Hearts, defaultHearts);
        }

        if (!save.HasKey(SaveKeys.Coins))
        {
            save.SetInt(SaveKeys.Coins, defaultCoins);
        }

        if (!save.HasKey(SaveKeys.TutorialCompleted))
        {
            save.SetBool(SaveKeys.TutorialCompleted, false);
        }

        if (!save.HasKey(SaveKeys.MusicVolume))
        {
            save.SetBool(SaveKeys.MusicVolume, true);
        }

        if (!save.HasKey(SaveKeys.SoundVolume))
        {
            save.SetBool(SaveKeys.SoundVolume, true);
        }

        if (!save.HasKey(SaveKeys.UnlockLevel))
        {
            save.SetInt(SaveKeys.UnlockLevel, 1);
        }
    }

    private void RefreshStatsView()
    {
        if (_startScreenView == null) return;

        var save = SaveService.Instance;
        int maxHearts = _gameConfig != null ? _gameConfig.MaxHearts : 5;
        int defaultCoins = _gameConfig != null ? _gameConfig.StartingCoins : 100;

        int hearts = save != null ? save.GetInt(SaveKeys.Hearts, maxHearts) : maxHearts;
        int coins = save != null ? save.GetInt(SaveKeys.Coins, defaultCoins) : defaultCoins;

        _startScreenView.SetStats(hearts, maxHearts, coins);
    }

    private void ApplySettingViewState()
    {
        if (_settingPanelView == null) return;

        var save = SaveService.Instance;
        bool musicOn = save == null || save.GetBool(SaveKeys.MusicVolume, true);
        bool sfxOn = save == null || save.GetBool(SaveKeys.SoundVolume, true);
        _settingPanelView.SetInitialValues(musicOn, sfxOn);
    }

    private void HandlePlayClicked()
    {
        var save = SaveService.Instance;
        bool tutorialCompleted = save != null && save.GetBool(SaveKeys.TutorialCompleted, false);
        string nextScene = tutorialCompleted ? SceneNames.Map : SceneNames.Tutorial;
        RequestSceneLoad(nextScene);
    }

    private void HandleSettingsClicked()
    {
        if (_settingPanelView != null) _settingPanelView.Show();
    }

    private void HandleContinueClicked()
    {
        if (_settingPanelView != null) _settingPanelView.Hide();
    }

    private void HandleMapClicked()
    {
        RequestSceneLoad(SceneNames.Map);
    }

    private void HandleMusicToggled(bool isOn)
    {
        SaveService.Instance?.SetBool(SaveKeys.MusicVolume, isOn);
        RaiseAudioSettingChanged();
    }

    private void HandleSfxToggled(bool isOn)
    {
        SaveService.Instance?.SetBool(SaveKeys.SoundVolume, isOn);
        RaiseAudioSettingChanged();
    }

    private void RaiseAudioSettingChanged()
    {
        bool musicEnabled = SaveService.Instance == null || SaveService.Instance.GetBool(SaveKeys.MusicVolume, true);
        bool sfxEnabled = SaveService.Instance == null || SaveService.Instance.GetBool(SaveKeys.SoundVolume, true);

        EventBus<AudioSettingChangedEvent>.Raise(new AudioSettingChangedEvent
        {
            IsMusicEnabled = musicEnabled,
            IsSfxEnabled = sfxEnabled
        });
    }

    private void HandleQuitClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void RequestSceneLoad(string sceneName)
    {
        EventBus<SceneLoadRequestedEvent>.Raise(new SceneLoadRequestedEvent
        {
            SceneName = sceneName,
            UseLoadingScreen = true
        });
    }
}
