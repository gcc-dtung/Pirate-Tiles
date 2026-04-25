using UnityEngine;
using TMPro;
using PrimeTween;

public class CoinsView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _coinsText;
    
    private Tween _countTween;

    public void UpdateCoins(int newAmount)
    {
        int startValue = 0;
        if (_coinsText.text != null && int.TryParse(_coinsText.text, out int current))
        {
            startValue = current;
        }

        _countTween.Stop();
        _countTween = Tween.Custom(this, startValue, newAmount, 0.5f, (target, val) => 
        {
            target._coinsText.text = Mathf.RoundToInt(val).ToString();
        });
    }
}
