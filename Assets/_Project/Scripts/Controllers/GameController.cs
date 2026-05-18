using UnityEngine;

public class GameController : MonoBehaviour
{
    [Header("Controllers")]
    [SerializeField] private BoardController _boardController;
    [SerializeField] private StackController _stackController;
    [SerializeField] private PowerUpController _powerUpController;

    [Header("Event Channels (Cross-Layer)")]
    [SerializeField] private VoidEventChannelSO _tilesMatchedChannel;
    [SerializeField] private VoidEventChannelSO _stackFullChannel;
    [SerializeField] private VoidEventChannelSO _gameWonChannel;
    [SerializeField] private VoidEventChannelSO _gameLostChannel;
    [SerializeField] private VoidEventChannelSO _boardClearedChannel;

    [Header("Views")]
    [SerializeField] private WinPanelView _winPanelView;
    [SerializeField] private LosePanelView _losePanelView;
    [SerializeField] private SettingPanelView _settingPanelView;

    private GameStateModel _gameState;
    private bool _isBoardCleared = false;

    private void Awake()
    {
        _gameState = new GameStateModel();
        _gameState.Phase = GamePhase.Init;
        _isBoardCleared = false;
    }

    private void Start()
    {
        SetPhase(GamePhase.Playing);
    }

    private void OnEnable()
    {
        if (_tilesMatchedChannel != null) _tilesMatchedChannel.AddListener(OnTilesMatched);
        if (_stackFullChannel != null) _stackFullChannel.AddListener(OnStackFull);
        if (_gameWonChannel != null) _gameWonChannel.AddListener(OnGameWonEventRaised);
        if (_gameLostChannel != null) _gameLostChannel.AddListener(OnGameLostEventRaised);
        if (_boardClearedChannel != null) _boardClearedChannel.AddListener(OnBoardCleared);

        if (_settingPanelView != null)
        {
            _settingPanelView.OnContinueClicked += HandleSettingContinue;
            _settingPanelView.OnMapClicked    += HandleGoToMap;
            _settingPanelView.OnMusicToggled  += HandleMusicToggled;
            _settingPanelView.OnSfxToggled    += HandleSfxToggled;
        }
    }

    private void OnDisable()
    {
        if (_tilesMatchedChannel != null) _tilesMatchedChannel.RemoveListener(OnTilesMatched);
        if (_stackFullChannel != null) _stackFullChannel.RemoveListener(OnStackFull);
        if (_gameWonChannel != null) _gameWonChannel.RemoveListener(OnGameWonEventRaised);
        if (_gameLostChannel != null) _gameLostChannel.RemoveListener(OnGameLostEventRaised);
        if (_boardClearedChannel != null) _boardClearedChannel.RemoveListener(OnBoardCleared);

        if (_settingPanelView != null)
        {
            _settingPanelView.OnContinueClicked -= HandleSettingContinue;
            _settingPanelView.OnMapClicked    -= HandleGoToMap;
            _settingPanelView.OnMusicToggled  -= HandleMusicToggled;
            _settingPanelView.OnSfxToggled    -= HandleSfxToggled;
        }
    }

    private void SetPhase(GamePhase newPhase)
    {
        var prevPhase = _gameState.Phase;
        _gameState.Phase = newPhase;

        EventBus<GamePhaseChangedEvent>.Raise(new GamePhaseChangedEvent
        {
            PreviousPhase = prevPhase,
            NewPhase = newPhase
        });
    }

    private void OnBoardCleared()
    {
        _isBoardCleared = true;
        CheckWinCondition();
    }

    private void CheckWinCondition()
    {
        if (_gameState.Phase != GamePhase.Playing) return;
        
        if (_isBoardCleared && (_stackController == null || _stackController.IsStackEmpty))
        {
            SetPhase(GamePhase.Won);
            if (_gameWonChannel != null) _gameWonChannel.EventRaise();
        }
    }

    private void HandleWin()
    {
        if (_gameState.Phase != GamePhase.Playing) return;
        SetPhase(GamePhase.Won);
        if (_gameWonChannel != null) _gameWonChannel.EventRaise();
    }

    private void HandleLose()
    {
        if (_gameState.Phase != GamePhase.Playing) return;
        SetPhase(GamePhase.Lost);
        if (_gameLostChannel != null) _gameLostChannel.EventRaise();
    }

    private void OnTilesMatched()
    {
        // Add score or any overall game state logic on match
        CheckWinCondition();
    }

    private void OnStackFull()
    {
        HandleLose();
    }

    private void OnGameWonEventRaised()
    {
        int coinsReward = 100;
        if (_winPanelView != null) _winPanelView.Show(coinsReward); 

        var save = SaveService.Instance;
        if (save != null)
        {
            // Update Coins
            int currentCoins = save.GetInt(SaveKeys.Coins, 100); // Mặc định 100 theo GameConfig
            save.SetInt(SaveKeys.Coins, currentCoins + coinsReward);

            // Update Unlocked Level
            int currentLevelIndex = save.GetInt(SaveKeys.SelectedLevelIndex, 1);
            int highestUnlocked = save.GetInt(SaveKeys.UnlockLevel, 1);

            if (currentLevelIndex >= highestUnlocked)
            {
                save.SetInt(SaveKeys.UnlockLevel, currentLevelIndex + 1);
            }
        }
    }

    private void OnGameLostEventRaised()
    {
        if (_losePanelView != null) _losePanelView.Show();
    }

    // ── Setting Panel ──────────────────────────────────────
    public void OpenSettingPanel()
    {
        if (_settingPanelView == null) return;

        var save = SaveService.Instance;
        bool musicOn = save == null || save.GetBool(SaveKeys.MusicVolume, true);
        bool sfxOn   = save == null || save.GetBool(SaveKeys.SoundVolume, true);
        _settingPanelView.SetInitialValues(musicOn, sfxOn);
        _settingPanelView.Show();
    }

    private void HandleSettingContinue()
    {
        _settingPanelView.Hide();
    }

    private void HandleGoToMap()
    {
        EventBus<SceneLoadRequestedEvent>.Raise(new SceneLoadRequestedEvent
        {
            SceneName = SceneNames.Map,
            UseLoadingScreen = true
        });
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
        bool sfxEnabled   = SaveService.Instance == null || SaveService.Instance.GetBool(SaveKeys.SoundVolume, true);
        EventBus<AudioSettingChangedEvent>.Raise(new AudioSettingChangedEvent
        {
            IsMusicEnabled = musicEnabled,
            IsSfxEnabled   = sfxEnabled
        });
    }
}
