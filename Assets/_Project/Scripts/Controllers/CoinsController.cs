using UnityEngine;

public class CoinsController : MonoBehaviour
{
    [SerializeField] private CoinsView _coinsView;
    [SerializeField] private PowerUpController _powerUpController;
    
    [SerializeField] private PowerTypeEventChannelSO _spendCoinsRequestChannel;

    private CoinsModel _coinsModel;

    private void Start()
    {
        Initialize();
    }

    private void OnEnable()
    {
        if (_spendCoinsRequestChannel != null) _spendCoinsRequestChannel.AddListener(OnSpendCoinsRequest);
    }

    private void OnDisable()
    {
        if (_spendCoinsRequestChannel != null) _spendCoinsRequestChannel.RemoveListener(OnSpendCoinsRequest);
    }

    public void Initialize()
    {
        var save = SaveService.Instance;
        _coinsModel = new CoinsModel();
        _coinsModel.CurrentCoins = save != null ? save.GetInt(SaveKeys.Coins, 100) : 100;
        
        if (_coinsView != null) _coinsView.UpdateCoins(_coinsModel.CurrentCoins);
    }


    public void AddCoins(int amount)
    {
        _coinsModel.AddCoins(amount);
        SaveService.Instance?.SetInt(SaveKeys.Coins, _coinsModel.CurrentCoins);
        if (_coinsView != null) _coinsView.UpdateCoins(_coinsModel.CurrentCoins);
    }

    private void OnSpendCoinsRequest(PowerType type)
    {
        int cost = 50;
        if (_coinsModel.ConsumeCoins(cost))
        {
            SaveService.Instance?.SetInt(SaveKeys.Coins, _coinsModel.CurrentCoins);
            if (_coinsView != null) _coinsView.UpdateCoins(_coinsModel.CurrentCoins);
            if (_powerUpController != null)
            {
                _powerUpController.OnPurchaseSuccessful(type);
            }
        }
        else
        {
            Debug.Log("Not enough coins!");
        }
    }
}
