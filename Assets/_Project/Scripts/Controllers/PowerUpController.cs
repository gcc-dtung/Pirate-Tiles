using UnityEngine;

public class PowerUpController : MonoBehaviour
{
    [SerializeField] private PowerUpBarView _powerUpBarView;
    [SerializeField] private SpendCoinsPanelView _spendCoinsPanelView;
    
    [Header("Event Channels")]
    [SerializeField] private PowerTypeEventChannelSO _spendCoinsRequestChannel;
    [SerializeField] private VoidEventChannelSO _undoRequestChannel;
    [SerializeField] private VoidEventChannelSO _shuffleRequestChannel;

    private PowerUpModel _powerUpModel;
    private PowerType _pendingPurchaseType;

    public void Initialize()
    {
        _powerUpModel = new PowerUpModel();
        
        _powerUpModel.AddPowerUp(PowerType.Undo, 1);
        _powerUpModel.AddPowerUp(PowerType.Magic, 1);
        _powerUpModel.AddPowerUp(PowerType.Shuffle, 1);
        _powerUpModel.AddPowerUp(PowerType.AddCell, 1);

        UpdateAllViews();
    }

    private void OnEnable()
    {
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
        if (_powerUpBarView != null)
        {
            _powerUpBarView.UpdatePowerUpCount(type, _powerUpModel.GetCount(type));
        }
    }

    private void UsePowerUp(PowerType type)
    {
        _powerUpModel.UsePowerUp(type);
        if (_powerUpBarView != null)
        {
            _powerUpBarView.UpdatePowerUpCount(type, _powerUpModel.GetCount(type));
        }

        switch (type)
        {
            case PowerType.Undo:
                if (_undoRequestChannel != null) _undoRequestChannel.RaiseEvent();
                break;
            case PowerType.Magic:
                // TODO
                break;
            case PowerType.Shuffle:
                if (_shuffleRequestChannel != null) _shuffleRequestChannel.RaiseEvent();
                break;
            case PowerType.AddCell:
                // TODO
                break;
        }
    }

    private void UpdateAllViews()
    {
        if (_powerUpBarView == null) return;
        _powerUpBarView.UpdatePowerUpCount(PowerType.Undo, _powerUpModel.GetCount(PowerType.Undo));
        _powerUpBarView.UpdatePowerUpCount(PowerType.Magic, _powerUpModel.GetCount(PowerType.Magic));
        _powerUpBarView.UpdatePowerUpCount(PowerType.Shuffle, _powerUpModel.GetCount(PowerType.Shuffle));
        _powerUpBarView.UpdatePowerUpCount(PowerType.AddCell, _powerUpModel.GetCount(PowerType.AddCell));
    }
}
