using UnityEngine;
using UnityEngine.UI;
using System;
using PrimeTween;

public class SettingPanelView : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private RectTransform _panelRoot;

    [SerializeField] private Toggle _musicToggle;
    [SerializeField] private Toggle _sfxToggle;
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
        _musicToggle.onValueChanged.AddListener(isOn => OnMusicToggled?.Invoke(isOn));
        _sfxToggle.onValueChanged.AddListener(isOn => OnSfxToggled?.Invoke(isOn));
        
        _continueButton.onClick.AddListener(() => OnContinueClicked?.Invoke());
        _replayButton.onClick.AddListener(() => OnReplayClicked?.Invoke());
        _mapButton.onClick.AddListener(() => OnMapClicked?.Invoke());
    }

    public void SetInitialValues(bool musicOn, bool sfxOn)
    {
        _musicToggle.SetIsOnWithoutNotify(musicOn);
        _sfxToggle.SetIsOnWithoutNotify(sfxOn);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        _canvasGroup.alpha = 0f;
        _panelRoot.localScale = Vector3.one * 0.8f;

        Sequence.Create()
            .Group(Tween.Alpha(_canvasGroup, 1f, 0.2f))
            .Group(Tween.Scale(_panelRoot, Vector3.one, 0.3f, Ease.OutBack));
    }

    public void Hide()
    {
        Sequence.Create()
            .Group(Tween.Alpha(_canvasGroup, 0f, 0.2f))
            .Group(Tween.Scale(_panelRoot, Vector3.one * 0.8f, 0.2f, Ease.InBack))
            .OnComplete(() => gameObject.SetActive(false));
    }
}
