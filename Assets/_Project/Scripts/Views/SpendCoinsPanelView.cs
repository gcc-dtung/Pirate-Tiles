using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PrimeTween;
using System;

public class SpendCoinsPanelView : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private RectTransform _panelRoot;
    [SerializeField] private TextMeshProUGUI _messageText;
    [SerializeField] private TextMeshProUGUI _costText;
    [SerializeField] private Button _buyButton;
    [SerializeField] private Button _cancelButton;

    public event Action OnConfirmBuy;
    public event Action OnCancel;

    private void Awake()
    {
        _buyButton.onClick.AddListener(() => OnConfirmBuy?.Invoke());
        _cancelButton.onClick.AddListener(() => OnCancel?.Invoke());
    }

    public void Show(PowerType powerType, int cost)
    {
        gameObject.SetActive(true);
        _messageText.text = $"Buy 1 {powerType}?";
        _costText.text = cost.ToString();

        _canvasGroup.alpha = 0f;
        _panelRoot.localScale = Vector3.one * 0.8f;

        Sequence.Create()
            .Group(Tween.Alpha(_canvasGroup, 1f, 0.2f))
            .Group(Tween.Scale(_panelRoot, Vector3.one, 0.4f, Ease.OutBack));
    }

    public void Hide()
    {
        Sequence.Create()
            .Group(Tween.Alpha(_canvasGroup, 0f, 0.2f))
            .Group(Tween.Scale(_panelRoot, Vector3.one * 0.8f, 0.2f, Ease.InBack))
            .OnComplete(() => gameObject.SetActive(false));
    }
}
