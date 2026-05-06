using UnityEngine;
using UnityEngine.SceneManagement;

public class MapController : MonoBehaviour
{
    [Header("Views")]
    [SerializeField] private MapView _mapView;
    [SerializeField] private HeartsView _heartsView;
    [SerializeField] private CoinsView _coinsView;
    [SerializeField] private SettingPanelView _settingPanelView;
    [SerializeField] private ShopPanelView _shopPanelView;
    [SerializeField] private CollectionPanelView _collectionPanelView;

    [Header("Top/Bottom Buttons")]
    [SerializeField] private UnityEngine.UI.Button _settingsButton;
    [SerializeField] private UnityEngine.UI.Button _shopButton;
    [SerializeField] private UnityEngine.UI.Button _collectionButton;

    [Header("Config")]
    [SerializeField] private GameConfigSO _gameConfig;

    private ChapterModel _chapterModel;
    private CoinsModel _coinsModel;
    private HeartsModel _heartsModel;
    private PowerUpModel _powerUpModel;

    private void Start()
    {
        InitializeData();
        InitializeViews();
        LoadChapter(_chapterModel.CurrentChapterIndex);
    }

    private void OnEnable()
    {
        if (_mapView != null)
        {
            _mapView.OnNextChapterClicked += HandleNextChapter;
            _mapView.OnPrevChapterClicked += HandlePrevChapter;
            _mapView.OnLevelSelected += HandleLevelSelected;
        }

        if (_settingsButton != null) _settingsButton.onClick.AddListener(ShowSettings);
        if (_shopButton != null) _shopButton.onClick.AddListener(ShowShop);
        if (_collectionButton != null) _collectionButton.onClick.AddListener(ShowCollection);

        if (_settingPanelView != null)
        {
            _settingPanelView.OnMusicToggled += HandleMusicToggled;
            _settingPanelView.OnSfxToggled += HandleSfxToggled;
            _settingPanelView.OnContinueClicked += HideSettings;
            _settingPanelView.OnMapClicked += HideSettings; // Already on map
            // Note: Replay doesn't make sense on Map, we can ignore or hide it in the setup
        }

        if (_shopPanelView != null)
        {
            _shopPanelView.OnBuyClicked += HandleBuyPowerUp;
            _shopPanelView.OnCloseClicked += HideShop;
        }

        if (_collectionPanelView != null)
        {
            _collectionPanelView.OnCloseClicked += HideCollection;
        }
    }

    private void OnDisable()
    {
        if (_mapView != null)
        {
            _mapView.OnNextChapterClicked -= HandleNextChapter;
            _mapView.OnPrevChapterClicked -= HandlePrevChapter;
            _mapView.OnLevelSelected -= HandleLevelSelected;
        }

        if (_settingsButton != null) _settingsButton.onClick.RemoveListener(ShowSettings);
        if (_shopButton != null) _shopButton.onClick.RemoveListener(ShowShop);
        if (_collectionButton != null) _collectionButton.onClick.RemoveListener(ShowCollection);

        if (_settingPanelView != null)
        {
            _settingPanelView.OnMusicToggled -= HandleMusicToggled;
            _settingPanelView.OnSfxToggled -= HandleSfxToggled;
            _settingPanelView.OnContinueClicked -= HideSettings;
            _settingPanelView.OnMapClicked -= HideSettings;
        }

        if (_shopPanelView != null)
        {
            _shopPanelView.OnBuyClicked -= HandleBuyPowerUp;
            _shopPanelView.OnCloseClicked -= HideShop;
        }

        if (_collectionPanelView != null)
        {
            _collectionPanelView.OnCloseClicked -= HideCollection;
        }
    }

    private void InitializeData()
    {
        var save = SaveService.Instance;
        
        _chapterModel = new ChapterModel();
        _chapterModel.TotalChapters = _gameConfig != null && _gameConfig.Chapters != null ? _gameConfig.Chapters.Count : 0;
        _chapterModel.CurrentChapterIndex = save != null ? save.GetInt(SaveKeys.CurrentChapter, 0) : 0;
        _chapterModel.UnlockedLevel = save != null ? save.GetInt(SaveKeys.UnlockLevel, 1) : 1;

        if (_chapterModel.CurrentChapterIndex >= _chapterModel.TotalChapters && _chapterModel.TotalChapters > 0)
        {
            _chapterModel.CurrentChapterIndex = _chapterModel.TotalChapters - 1;
        }

        _heartsModel = new HeartsModel();
        _heartsModel.MaxHearts = _gameConfig != null ? _gameConfig.MaxHearts : 5;
        _heartsModel.CurrentHearts = save != null ? save.GetInt(SaveKeys.Hearts, _heartsModel.MaxHearts) : _heartsModel.MaxHearts;

        _coinsModel = new CoinsModel();
        _coinsModel.CurrentCoins = save != null ? save.GetInt(SaveKeys.Coins, _gameConfig != null ? _gameConfig.StartingCoins : 100) : 100;

        _powerUpModel = new PowerUpModel();
        if (save != null)
        {
            _powerUpModel.AddPowerUp(PowerType.Undo, save.GetInt(SaveKeys.UndoPowerCount, 1));
            _powerUpModel.AddPowerUp(PowerType.Magic, save.GetInt(SaveKeys.MagicPowerCount, 1));
            _powerUpModel.AddPowerUp(PowerType.Shuffle, save.GetInt(SaveKeys.ShufflePowerCount, 1));
            _powerUpModel.AddPowerUp(PowerType.AddOneCell, save.GetInt(SaveKeys.AddOneStackPowerCount, 1));
        }
    }

    private void InitializeViews()
    {
        if (_heartsView != null) _heartsView.UpdateHearts(_heartsModel.CurrentHearts, _heartsModel.MaxHearts);
        if (_coinsView != null) _coinsView.UpdateCoins(_coinsModel.CurrentCoins);
        
        if (_shopPanelView != null && _gameConfig != null)
        {
            _shopPanelView.SetupCosts(_gameConfig.UndoCost, _gameConfig.MagicCost, _gameConfig.ShuffleCost, _gameConfig.AddCellCost);
            _shopPanelView.gameObject.SetActive(false);
        }

        if (_settingPanelView != null)
        {
            var save = SaveService.Instance;
            bool musicOn = save == null || save.GetBool(SaveKeys.MusicVolume, true);
            bool sfxOn = save == null || save.GetBool(SaveKeys.SoundVolume, true);
            _settingPanelView.SetInitialValues(musicOn, sfxOn);
            _settingPanelView.gameObject.SetActive(false);
        }

        if (_collectionPanelView != null) _collectionPanelView.gameObject.SetActive(false);
    }

    private void LoadChapter(int index)
    {
        if (_gameConfig == null || _gameConfig.Chapters == null || index < 0 || index >= _gameConfig.Chapters.Count)
            return;

        var chapterConfig = _gameConfig.Chapters[index];
        _chapterModel.LevelsInCurrentChapter = chapterConfig.LevelNodes != null ? chapterConfig.LevelNodes.Count : 0;

        if (_mapView != null)
        {
            _mapView.SetArrowVisibility(_chapterModel.CanGoPrev, _chapterModel.CanGoNext);
            _mapView.SetupChapter(chapterConfig, _chapterModel.UnlockedLevel);
        }
    }

    private void HandleNextChapter()
    {
        if (_chapterModel.CanGoNext)
        {
            _chapterModel.CurrentChapterIndex++;
            SaveService.Instance?.SetInt(SaveKeys.CurrentChapter, _chapterModel.CurrentChapterIndex);
            LoadChapter(_chapterModel.CurrentChapterIndex);
        }
    }

    private void HandlePrevChapter()
    {
        if (_chapterModel.CanGoPrev)
        {
            _chapterModel.CurrentChapterIndex--;
            SaveService.Instance?.SetInt(SaveKeys.CurrentChapter, _chapterModel.CurrentChapterIndex);
            LoadChapter(_chapterModel.CurrentChapterIndex);
        }
    }

    private void HandleLevelSelected(int levelIndex)
    {
        if (levelIndex <= _chapterModel.UnlockedLevel)
        {
            SaveService.Instance?.SetInt(SaveKeys.SelectedLevelIndex, levelIndex);
            SaveService.Instance?.SetInt(SaveKeys.SelectedChapterIndex, _chapterModel.CurrentChapterIndex);

            EventBus<LevelSelectedEvent>.Raise(new LevelSelectedEvent 
            { 
                LevelIndex = levelIndex,
                ChapterIndex = _chapterModel.CurrentChapterIndex
            });

            EventBus<SceneLoadRequestedEvent>.Raise(new SceneLoadRequestedEvent
            {
                SceneName = SceneNames.InGame,
                UseLoadingScreen = true
            });
        }
    }

    private void ShowSettings() { if (_settingPanelView != null) _settingPanelView.Show(); }
    private void HideSettings() { if (_settingPanelView != null) _settingPanelView.Hide(); }
    
    private void ShowShop() { if (_shopPanelView != null) _shopPanelView.Show(); }
    private void HideShop() { if (_shopPanelView != null) _shopPanelView.Hide(); }
    
    private void ShowCollection() { if (_collectionPanelView != null) _collectionPanelView.Show(); }
    private void HideCollection() { if (_collectionPanelView != null) _collectionPanelView.Hide(); }

    private void HandleBuyPowerUp(PowerType type)
    {
        if (_gameConfig == null) return;

        int cost = 0;
        switch (type)
        {
            case PowerType.Undo: cost = _gameConfig.UndoCost; break;
            case PowerType.Magic: cost = _gameConfig.MagicCost; break;
            case PowerType.Shuffle: cost = _gameConfig.ShuffleCost; break;
            case PowerType.AddOneCell: cost = _gameConfig.AddCellCost; break;
        }

        if (_coinsModel.ConsumeCoins(cost))
        {
            // Tăng số lượng power up
            _powerUpModel.AddPowerUp(type);

            // Cập nhật UI coin
            if (_coinsView != null) _coinsView.UpdateCoins(_coinsModel.CurrentCoins);
            
            // Save state
            var save = SaveService.Instance;
            if (save != null)
            {
                save.SetInt(SaveKeys.Coins, _coinsModel.CurrentCoins);
                switch(type)
                {
                    case PowerType.Undo: save.SetInt(SaveKeys.UndoPowerCount, _powerUpModel.GetCount(type)); break;
                    case PowerType.Magic: save.SetInt(SaveKeys.MagicPowerCount, _powerUpModel.GetCount(type)); break;
                    case PowerType.Shuffle: save.SetInt(SaveKeys.ShufflePowerCount, _powerUpModel.GetCount(type)); break;
                    case PowerType.AddOneCell: save.SetInt(SaveKeys.AddOneStackPowerCount, _powerUpModel.GetCount(type)); break;
                }
            }
        }
        else
        {
            Debug.Log($"Not enough coins to buy {type}. Need {cost}, have {_coinsModel.CurrentCoins}");
        }
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
}
