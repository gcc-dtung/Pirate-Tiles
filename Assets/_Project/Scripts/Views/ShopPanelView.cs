using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PrimeTween;
using System;

public class ShopPanelView : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private RectTransform _panelRoot;
    [SerializeField] private Button _closeButton;
    [SerializeField] private Button _backgroundButton;

    [Header("Item Buttons")]
    [SerializeField] private Button _buyUndoButton;
    [SerializeField] private Button _buyMagicButton;
    [SerializeField] private Button _buyShuffleButton;
    [SerializeField] private Button _buyAddCellButton;

    [Header("Cost Texts")]
    [SerializeField] private TextMeshProUGUI _undoCostText;
    [SerializeField] private TextMeshProUGUI _magicCostText;
    [SerializeField] private TextMeshProUGUI _shuffleCostText;
    [SerializeField] private TextMeshProUGUI _addCellCostText;

    public event Action<PowerType> OnBuyClicked;
    public event Action OnCloseClicked;

    private void Awake()
    {
        if (_closeButton != null) _closeButton.onClick.AddListener(() => OnCloseClicked?.Invoke());
        if (_backgroundButton != null) _backgroundButton.onClick.AddListener(() => OnCloseClicked?.Invoke());

        if (_buyUndoButton != null) _buyUndoButton.onClick.AddListener(() => OnBuyClicked?.Invoke(PowerType.Undo));
        if (_buyMagicButton != null) _buyMagicButton.onClick.AddListener(() => OnBuyClicked?.Invoke(PowerType.Magic));
        if (_buyShuffleButton != null) _buyShuffleButton.onClick.AddListener(() => OnBuyClicked?.Invoke(PowerType.Shuffle));
        if (_buyAddCellButton != null) _buyAddCellButton.onClick.AddListener(() => OnBuyClicked?.Invoke(PowerType.AddOneCell));
    }

    public void SetupCosts(int undoCost, int magicCost, int shuffleCost, int addCellCost)
    {
        if (_undoCostText != null) _undoCostText.text = undoCost.ToString();
        if (_magicCostText != null) _magicCostText.text = magicCost.ToString();
        if (_shuffleCostText != null) _shuffleCostText.text = shuffleCost.ToString();
        if (_addCellCostText != null) _addCellCostText.text = addCellCost.ToString();
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
