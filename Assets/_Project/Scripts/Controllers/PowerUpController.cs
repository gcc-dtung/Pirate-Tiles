using UnityEngine;

public class PowerUpController : MonoBehaviour
{
    [SerializeField] private PowerUpBarView _powerUpBarView;
    [SerializeField] private SpendCoinsPanelView _spendCoinsPanelView;
    
    [Header("Event Channels")]
    [SerializeField] private PowerTypeEventChannelSO _spendCoinsRequestChannel;
    [SerializeField] private VoidEventChannelSO _undoRequestChannel;
    [SerializeField] private VoidEventChannelSO _shuffleRequestChannel;
    [SerializeField] private VoidEventChannelSO _magicRequestChannel;
    [SerializeField] private VoidEventChannelSO _addOneCellRequestChannel;

    private PowerUpModel _powerUpModel;
    private PowerType _pendingPurchaseType;
    private GamePhase _currentPhase = GamePhase.None;

    private EventBinding<AddOneCellUsedEvent> _addOneCellBinding;
    private EventBinding<StackUpdatedEvent> _stackUpdatedBinding;
    private EventBinding<GamePhaseChangedEvent> _phaseChangedBinding;

    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        var save = SaveService.Instance;
        _powerUpModel = new PowerUpModel();
        
        _powerUpModel.AddPowerUp(PowerType.Undo, save != null ? save.GetInt(SaveKeys.UndoPowerCount, 1) : 1);
        _powerUpModel.AddPowerUp(PowerType.Magic, save != null ? save.GetInt(SaveKeys.MagicPowerCount, 1) : 1);
        _powerUpModel.AddPowerUp(PowerType.Shuffle, save != null ? save.GetInt(SaveKeys.ShufflePowerCount, 1) : 1);
        _powerUpModel.AddPowerUp(PowerType.AddOneCell, save != null ? save.GetInt(SaveKeys.AddOneStackPowerCount, 1) : 1);

        UpdateAllViews();

        if (_powerUpBarView != null)
        {
            _powerUpBarView.SetButtonInteractable(PowerType.Undo, false);
        }
    }

    private void OnEnable()
    {
        _addOneCellBinding = new EventBinding<AddOneCellUsedEvent>(OnAddOneCellUsed);
        EventBus<AddOneCellUsedEvent>.Register(_addOneCellBinding);

        _stackUpdatedBinding = new EventBinding<StackUpdatedEvent>(OnStackUpdated);
        EventBus<StackUpdatedEvent>.Register(_stackUpdatedBinding);

        _phaseChangedBinding = new EventBinding<GamePhaseChangedEvent>(OnPhaseChanged);
        EventBus<GamePhaseChangedEvent>.Register(_phaseChangedBinding);

        if (_powerUpBarView != null)
        {
            _powerUpBarView.OnPowerUpClicked += HandlePowerUpClicked;
        }

        if (_spendCoinsPanelView != null)
        {
            _spendCoinsPanelView.OnConfirmBuy += HandleConfirmBuy;
            _spendCoinsPanelView.OnCancel += HandleCancelBuy;
        }
    }

    private void OnDisable()
    {
        EventBus<AddOneCellUsedEvent>.Deregister(_addOneCellBinding);
        EventBus<StackUpdatedEvent>.Deregister(_stackUpdatedBinding);
        EventBus<GamePhaseChangedEvent>.Deregister(_phaseChangedBinding);

        if (_powerUpBarView != null)
        {
            _powerUpBarView.OnPowerUpClicked -= HandlePowerUpClicked;
        }

        if (_spendCoinsPanelView != null)
        {
            _spendCoinsPanelView.OnConfirmBuy -= HandleConfirmBuy;
            _spendCoinsPanelView.OnCancel -= HandleCancelBuy;
        }
    }

    private void HandlePowerUpClicked(PowerType type)
    {
        if (_currentPhase != GamePhase.Playing) return;

        int currentCount = _powerUpModel.GetCount(type);
        if (currentCount > 0)
        {
            UsePowerUp(type);
        }
        else
        {
            _pendingPurchaseType = type;
            int cost = 50; 
            if (_spendCoinsPanelView != null)
            {
                _spendCoinsPanelView.Show(type, cost);
            }
        }
    }

    private void HandleConfirmBuy()
    {
        if (_spendCoinsRequestChannel != null)
        {
            _spendCoinsRequestChannel.EventRaise(_pendingPurchaseType);
        }
        
        if (_spendCoinsPanelView != null)
        {
            _spendCoinsPanelView.Hide();
        }
    }

    private void HandleCancelBuy()
    {
        if (_spendCoinsPanelView != null)
        {
            _spendCoinsPanelView.Hide();
        }
    }

    public void OnPurchaseSuccessful(PowerType type)
    {
        AddPowerUp(type, 1);
        UsePowerUp(type);
    }

    public void AddPowerUp(PowerType type, int amount)
    {
        _powerUpModel.AddPowerUp(type, amount);
        SavePowerUpCount(type);
        if (_powerUpBarView != null)
        {
            _powerUpBarView.UpdatePowerUpCount(type, _powerUpModel.GetCount(type));
        }
    }

    private void UsePowerUp(PowerType type)
    {
        _powerUpModel.UsePowerUp(type);
        SavePowerUpCount(type);
        if (_powerUpBarView != null)
        {
            _powerUpBarView.UpdatePowerUpCount(type, _powerUpModel.GetCount(type));
        }

        switch (type)
        {
            case PowerType.Undo:
                if (_undoRequestChannel != null) _undoRequestChannel.EventRaise();
                break;
            case PowerType.Magic:
                if (_magicRequestChannel != null) _magicRequestChannel.EventRaise();
                break;
            case PowerType.Shuffle:
                if (_shuffleRequestChannel != null) _shuffleRequestChannel.EventRaise();
                break;
            case PowerType.AddOneCell:
                if (_addOneCellRequestChannel != null) _addOneCellRequestChannel.EventRaise();
                break;
        }
    }

    private void UpdateAllViews()
    {
        if (_powerUpBarView == null) return;
        _powerUpBarView.UpdatePowerUpCount(PowerType.Undo, _powerUpModel.GetCount(PowerType.Undo));
        _powerUpBarView.UpdatePowerUpCount(PowerType.Magic, _powerUpModel.GetCount(PowerType.Magic));
        _powerUpBarView.UpdatePowerUpCount(PowerType.Shuffle, _powerUpModel.GetCount(PowerType.Shuffle));
        _powerUpBarView.UpdatePowerUpCount(PowerType.AddOneCell, _powerUpModel.GetCount(PowerType.AddOneCell));
    }

    private void OnPhaseChanged(GamePhaseChangedEvent e)
    {
        _currentPhase = e.NewPhase;
    }

    private void OnAddOneCellUsed(AddOneCellUsedEvent e)
    {
        if (_powerUpBarView != null)
        {
            _powerUpBarView.SetButtonInteractable(PowerType.AddOneCell, false);
        }
    }

    private void OnStackUpdated(StackUpdatedEvent e)
    {
        if (_powerUpBarView != null)
        {
            _powerUpBarView.SetButtonInteractable(PowerType.Undo, e.Count > 0);
        }
    }

    private void SavePowerUpCount(PowerType type)
    {
        var save = SaveService.Instance;
        if (save == null) return;
        
        int count = _powerUpModel.GetCount(type);
        switch (type)
        {
            case PowerType.Undo: save.SetInt(SaveKeys.UndoPowerCount, count); break;
            case PowerType.Magic: save.SetInt(SaveKeys.MagicPowerCount, count); break;
            case PowerType.Shuffle: save.SetInt(SaveKeys.ShufflePowerCount, count); break;
            case PowerType.AddOneCell: save.SetInt(SaveKeys.AddOneStackPowerCount, count); break;
        }
    }
}
