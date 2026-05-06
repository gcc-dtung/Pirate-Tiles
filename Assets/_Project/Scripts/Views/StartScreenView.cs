using System;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StartScreenView : MonoBehaviour
{
    [Header("Main")]
    [SerializeField] private RectTransform _logoRoot;
    [SerializeField] private CanvasGroup _buttonsGroup;
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _settingsButton;
    [SerializeField] private Button _quitButton;

    [Header("Stats")]
    [SerializeField] private TextMeshProUGUI _heartsText;
    [SerializeField] private TextMeshProUGUI _coinsText;

    public event Action OnPlayClicked;
    public event Action OnSettingsClicked;
    public event Action OnQuitClicked;

    private void OnEnable()
    {
        if (_playButton != null) _playButton.onClick.AddListener(HandlePlayClicked);
        if (_settingsButton != null) _settingsButton.onClick.AddListener(HandleSettingsClicked);
        if (_quitButton != null) _quitButton.onClick.AddListener(HandleQuitClicked);

        PlayIntroAnimation();
    }

    private void OnDisable()
    {
        if (_playButton != null) _playButton.onClick.RemoveListener(HandlePlayClicked);
        if (_settingsButton != null) _settingsButton.onClick.RemoveListener(HandleSettingsClicked);
        if (_quitButton != null) _quitButton.onClick.RemoveListener(HandleQuitClicked);
    }

    public void SetStats(int currentHearts, int maxHearts, int coins)
    {
        if (_heartsText != null) _heartsText.text = $"{currentHearts}/{maxHearts}";
        if (_coinsText != null) _coinsText.text = coins.ToString();
    }

    public void SetQuitVisible(bool visible)
    {
        if (_quitButton != null) _quitButton.gameObject.SetActive(visible);
    }

    private void PlayIntroAnimation()
    {
        if (_logoRoot != null)
        {
            _logoRoot.localScale = Vector3.one * 0.9f;
            Tween.Scale(_logoRoot, Vector3.one, 0.35f, Ease.OutBack);
        }

        if (_buttonsGroup != null)
        {
            _buttonsGroup.alpha = 0f;
            Tween.Alpha(_buttonsGroup, 1f, 0.25f);
        }
    }

    private void HandlePlayClicked() => OnPlayClicked?.Invoke();
    private void HandleSettingsClicked() => OnSettingsClicked?.Invoke();
    private void HandleQuitClicked() => OnQuitClicked?.Invoke();
}
