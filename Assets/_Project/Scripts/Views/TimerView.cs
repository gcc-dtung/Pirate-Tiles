using UnityEngine;
using TMPro;
using PrimeTween;

public class TimerView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _timerText;
    [SerializeField] private Color _normalColor = Color.white;
    [SerializeField] private Color _warningColor = Color.red;

    private Tween _pulseTween;

    public void UpdateTime(float secondsRemaining)
    {
        if (secondsRemaining < 0) secondsRemaining = 0;

        int minutes = Mathf.FloorToInt(secondsRemaining / 60f);
        int seconds = Mathf.FloorToInt(secondsRemaining % 60f);
        _timerText.text = $"{minutes:00}:{seconds:00}";

        if (secondsRemaining <= 10f && secondsRemaining > 0)
        {
            _timerText.color = _warningColor;
            if (!_pulseTween.isAlive)
            {
                _pulseTween = Tween.Scale(_timerText.transform, 1.2f, 0.5f, cycles: -1, cycleMode: CycleMode.Yoyo);
            }
        }
        else
        {
            _timerText.color = _normalColor;
            _pulseTween.Stop();
            _timerText.transform.localScale = Vector3.one;
        }
    }
}
