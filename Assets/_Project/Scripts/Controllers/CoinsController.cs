using UnityEngine;

public class CoinsController : MonoBehaviour
{
    [SerializeField] private CoinsView _coinsView;
    [SerializeField] private PowerUpController _powerUpController;
    
    [SerializeField] private PowerTypeEventChannelSO _spendCoinsRequestChannel;

    private CoinsModel _coinsModel;

    public void Initialize()
    {
        _coinsModel = new CoinsModel();
        _coinsModel.CurrentCoins = 500;
        
        if (_coinsView != null) _coinsView.UpdateCoins(_coinsModel.CurrentCoins);
    }


    public void AddCoins(int amount)
    {
        _coinsModel.AddCoins(amount);
        if (_coinsView != null) _coinsView.UpdateCoins(_coinsModel.CurrentCoins);
    }

    private void OnSpendCoinsRequest(PowerType type)
    {
        int cost = 50;
        if (_coinsModel.ConsumeCoins(cost))
        {
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
