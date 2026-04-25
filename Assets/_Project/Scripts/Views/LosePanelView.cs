using UnityEngine;
using PrimeTween;
using UnityEngine.UI;

public class LosePanelView : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private RectTransform _panelRoot;
    [SerializeField] private Button _replayButton;
    [SerializeField] private Button _homeButton;
    [SerializeField] private Button _reviveButton;

    public void Show()
    {
        gameObject.SetActive(true);
        _canvasGroup.alpha = 0f;
        _panelRoot.localScale = Vector3.zero;

        Sequence.Create()
            .Group(Tween.Alpha(_canvasGroup, 1f, 0.3f))
            .Group(Tween.Scale(_panelRoot, Vector3.one, 0.5f, Ease.OutBack));
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
