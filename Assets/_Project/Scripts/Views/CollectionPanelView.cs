using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PrimeTween;
using System;

public class CollectionPanelView : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private RectTransform _panelRoot;
    [SerializeField] private Button _closeButton;
    [SerializeField] private Button _backgroundButton;

    public event Action OnCloseClicked;

    private void Awake()
    {
        if (_closeButton != null) _closeButton.onClick.AddListener(() => OnCloseClicked?.Invoke());
        if (_backgroundButton != null) _backgroundButton.onClick.AddListener(() => OnCloseClicked?.Invoke());
    }

    public void Show()
    {
        gameObject.SetActive(true);
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
