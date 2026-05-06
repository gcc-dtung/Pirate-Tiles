using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinPanelView : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private RectTransform _panelRoot;
    [SerializeField] private TextMeshProUGUI _coinsText;
    [SerializeField] private Button _nextButton;
    [SerializeField] private Button _homeButton;

    private void OnEnable()
    {
        if (_homeButton != null)
        {
            _homeButton.onClick.AddListener(OnHomeClicked);
        }
    }

    private void OnDisable()
    {
        if (_homeButton != null)
        {
            _homeButton.onClick.RemoveListener(OnHomeClicked);
        }
    }

    public void Show(int coinsEarned)
    {
        gameObject.SetActive(true);
        _canvasGroup.alpha = 0f;
        _panelRoot.anchoredPosition = new Vector2(0f, 1000f);

        Sequence.Create()
            .Group(Tween.Alpha(_canvasGroup, 1f, 0.3f))
            .Group(Tween.UIAnchoredPosition(_panelRoot, Vector2.zero, 0.6f, Ease.OutBounce))
            .Chain(AnimateCoinsCountUp(coinsEarned));
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private Tween AnimateCoinsCountUp(int targetCoins)
    {
        return Tween.Custom(this, 0f, targetCoins, 1f, (target, val) =>
        {
            target._coinsText.text = $"+{Mathf.RoundToInt(val)}";
        });
    }

    private void OnHomeClicked()
    {
        EventBus<SceneLoadRequestedEvent>.Raise(new SceneLoadRequestedEvent
        {
            SceneName = SceneNames.Map,
            UseLoadingScreen = true
        });
    }
}
