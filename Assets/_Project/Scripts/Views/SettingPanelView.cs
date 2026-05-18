using UnityEngine;
using UnityEngine.UI;
using System;
using PrimeTween;

public class SettingPanelView : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private RectTransform _panelRoot;

    private Vector3 _panelOriginalScale = Vector3.one;

    [Header("Music Toggle")]
    [SerializeField] private Toggle _musicOnToggle;
    [SerializeField] private Toggle _musicOffToggle;

    [Header("SFX Toggle")]
    [SerializeField] private Toggle _sfxOnToggle;
    [SerializeField] private Toggle _sfxOffToggle;

    [SerializeField] private Button _continueButton;
    [SerializeField] private Button _replayButton;
    [SerializeField] private Button _mapButton;

    public event Action<bool> OnMusicToggled;
    public event Action<bool> OnSfxToggled;
    public event Action OnContinueClicked;
    public event Action OnReplayClicked;
    public event Action OnMapClicked;

    private void Awake()
    {
        if (_panelRoot != null)
            _panelOriginalScale = _panelRoot.localScale;

        var rect = GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;
            rect.anchoredPosition = Vector2.zero;
        }

        var image = GetComponent<Image>();
        if (image == null)
        {
            image = gameObject.AddComponent<Image>();
            image.color = new Color(0, 0, 0, 0.75f);
        }
        image.raycastTarget = true;

        // Music: click ON thì ON bị khóa, OFF mở; click OFF thì OFF bị khóa, ON mở
        _musicOnToggle.onValueChanged.AddListener(isOn =>
        {
            if (!isOn) return; // chỉ xử lý khi vừa được bật
            _musicOnToggle.interactable = false;
            _musicOffToggle.interactable = true;
            _musicOffToggle.SetIsOnWithoutNotify(false);
            OnMusicToggled?.Invoke(true);
        });

        _musicOffToggle.onValueChanged.AddListener(isOn =>
        {
            if (!isOn) return;
            _musicOffToggle.interactable = false;
            _musicOnToggle.interactable = true;
            _musicOnToggle.SetIsOnWithoutNotify(false);
            OnMusicToggled?.Invoke(false);
        });

        // SFX
        _sfxOnToggle.onValueChanged.AddListener(isOn =>
        {
            if (!isOn) return;
            _sfxOnToggle.interactable = false;
            _sfxOffToggle.interactable = true;
            _sfxOffToggle.SetIsOnWithoutNotify(false);
            OnSfxToggled?.Invoke(true);
        });

        _sfxOffToggle.onValueChanged.AddListener(isOn =>
        {
            if (!isOn) return;
            _sfxOffToggle.interactable = false;
            _sfxOnToggle.interactable = true;
            _sfxOnToggle.SetIsOnWithoutNotify(false);
            OnSfxToggled?.Invoke(false);
        });

        _continueButton.onClick.AddListener(() => OnContinueClicked?.Invoke());
        _replayButton.onClick.AddListener(() => OnReplayClicked?.Invoke());
        _mapButton.onClick.AddListener(() => OnMapClicked?.Invoke());
    }

    /// <summary>
    /// Đặt trạng thái ban đầu không kích hoạt event.
    /// musicOn=true  → ON bị khóa, OFF mở
    /// musicOn=false → OFF bị khóa, ON mở
    /// </summary>
    public void SetInitialValues(bool musicOn, bool sfxOn)
    {
        // Music
        _musicOnToggle.SetIsOnWithoutNotify(musicOn);
        _musicOffToggle.SetIsOnWithoutNotify(!musicOn);
        _musicOnToggle.interactable = !musicOn;
        _musicOffToggle.interactable = musicOn;

        // SFX
        _sfxOnToggle.SetIsOnWithoutNotify(sfxOn);
        _sfxOffToggle.SetIsOnWithoutNotify(!sfxOn);
        _sfxOnToggle.interactable = !sfxOn;
        _sfxOffToggle.interactable = sfxOn;
    }

    public void Show()
    {
        gameObject.SetActive(true);
        _canvasGroup.alpha = 0f;
        _panelRoot.localScale = _panelOriginalScale * 0.8f;

        Sequence.Create()
            .Group(Tween.Alpha(_canvasGroup, 1f, 0.2f))
            .Group(Tween.Scale(_panelRoot, _panelOriginalScale, 0.3f, Ease.OutBack));
    }

    public void Hide()
    {
        Sequence.Create()
            .Group(Tween.Alpha(_canvasGroup, 0f, 0.2f))
            .Group(Tween.Scale(_panelRoot, _panelOriginalScale * 0.8f, 0.2f, Ease.InBack))
            .OnComplete(() => gameObject.SetActive(false));
    }
}
