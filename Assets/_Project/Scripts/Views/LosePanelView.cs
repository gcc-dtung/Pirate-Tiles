using PrimeTween;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    private void OnEnable()
    {
        if (_replayButton != null) _replayButton.onClick.AddListener(OnReplayClicked);
        if (_homeButton != null) _homeButton.onClick.AddListener(OnHomeClicked);
    }

    private void OnDisable()
    {
        if (_replayButton != null) _replayButton.onClick.RemoveListener(OnReplayClicked);
        if (_homeButton != null) _homeButton.onClick.RemoveListener(OnHomeClicked);
    }

    private void OnReplayClicked()
    {
        EventBus<SceneLoadRequestedEvent>.Raise(new SceneLoadRequestedEvent
        {
            SceneName = SceneManager.GetActiveScene().name,
            UseLoadingScreen = true
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
