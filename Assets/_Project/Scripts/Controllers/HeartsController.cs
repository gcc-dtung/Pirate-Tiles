using System;
using UnityEngine;

public class HeartsController : MonoBehaviour
{
    [SerializeField] private HeartsView _heartsView;
    [SerializeField] private OutOfHeartPanelView _outOfHeartPanelView;
    
    [SerializeField] private VoidEventChannelSO _outOfHeartsChannel;
    [SerializeField] private VoidEventChannelSO _gameLostChannel;
    
    [SerializeField] private GameConfigSO _gameConfig;

    private HeartsModel _heartsModel;
    
    private void Start()
    {
        Initialize();
    }

    private void OnEnable()
    {
        if (_outOfHeartsChannel != null) _outOfHeartsChannel.AddListener(OnOutOfHearts);
        if (_gameLostChannel != null) _gameLostChannel.AddListener(OnGameLost);
    }

    private void OnDisable()
    {
        if (_outOfHeartsChannel != null) _outOfHeartsChannel.RemoveListener(OnOutOfHearts);
        if (_gameLostChannel != null) _gameLostChannel.RemoveListener(OnGameLost);
    }

    public void Initialize()
    {
        var save = SaveService.Instance;
        _heartsModel = new HeartsModel();
        _heartsModel.MaxHearts = _gameConfig != null ? _gameConfig.MaxHearts : 5;
        _heartsModel.HealTime = _gameConfig != null ? _gameConfig.HealTimeInSeconds : 10f;
        _heartsModel.CurrentHearts = save != null ? save.GetInt(SaveKeys.Hearts, _heartsModel.MaxHearts) : _heartsModel.MaxHearts;
        
        string lastHealStr = save != null ? save.GetString(SaveKeys.LastHealTime, "") : "";
        if (!string.IsNullOrEmpty(lastHealStr) && long.TryParse(lastHealStr, out long binaryTime))
        {
            _heartsModel.LastHealTime = DateTime.FromBinary(binaryTime);
        }
        else
        {
            _heartsModel.LastHealTime = DateTime.Now;
        }

        if (_heartsModel.CalculateRegeneration(DateTime.Now))
        {
            save?.SetInt(SaveKeys.Hearts, _heartsModel.CurrentHearts);
            save?.SetString(SaveKeys.LastHealTime, _heartsModel.LastHealTime.ToBinary().ToString());
        }

        UpdateView();
    }

    public void ConsumeHeart()
    {
        if (_heartsModel.CurrentHearts > 0)
        {
            _heartsModel.ConsumeHeart();
            SaveService.Instance?.SetInt(SaveKeys.Hearts, _heartsModel.CurrentHearts);
            SaveService.Instance?.SetString(SaveKeys.LastHealTime, _heartsModel.LastHealTime.ToBinary().ToString());
            UpdateView();
        }
        else
        {
            if (_outOfHeartsChannel != null)
            {
                _outOfHeartsChannel.EventRaise();
            }
        }
    }

    private void OnOutOfHearts()
    {
        if (_outOfHeartPanelView != null)
        {
            _outOfHeartPanelView.Show();
        }
    }

    private void OnGameLost()
    {
        ConsumeHeart();
    }

    private void Update()
    {
        if (_heartsModel == null) return;

        if (_heartsModel.CalculateRegeneration(DateTime.Now))
        {
            SaveService.Instance?.SetInt(SaveKeys.Hearts, _heartsModel.CurrentHearts);
            SaveService.Instance?.SetString(SaveKeys.LastHealTime, _heartsModel.LastHealTime.ToBinary().ToString());
            UpdateView();
        }

        if (_heartsModel.IsFull)
        {
            if (_heartsView != null) _heartsView.UpdateCountdown("FULL");
        }
        else
        {
            int secondsLeft = Mathf.CeilToInt(_heartsModel.TimeUntilNextHeal);
            int m = secondsLeft / 60;
            int s = secondsLeft % 60;
            if (_heartsView != null) _heartsView.UpdateCountdown($"{m:00}:{s:00}");
        }
    }

    private void UpdateView()
    {
        if (_heartsView != null && _heartsModel != null)
        {
            _heartsView.UpdateHearts(_heartsModel.CurrentHearts, _heartsModel.MaxHearts);
        }
    }
}
