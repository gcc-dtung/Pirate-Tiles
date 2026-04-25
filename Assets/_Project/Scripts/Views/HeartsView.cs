using UnityEngine;
using TMPro;

public class HeartsView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _heartsCountText;
    [SerializeField] private TextMeshProUGUI _healCountdownText;

    public void UpdateHearts(int currentHearts, int maxHearts)
    {
        _heartsCountText.text = $"{currentHearts}/{maxHearts}";
    }

    public void UpdateCountdown(string timeString)
    {
        _healCountdownText.text = timeString;
        _healCountdownText.gameObject.SetActive(!string.IsNullOrEmpty(timeString));
    }
}
