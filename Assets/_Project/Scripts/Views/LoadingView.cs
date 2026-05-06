using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

public class LoadingView : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private Slider _progressBar;

    public void Show()
    {
        gameObject.SetActive(true);

        if (_canvasGroup != null)
        {
            _canvasGroup.alpha = 1f;
        }

        if (_progressBar != null)
        {
            _progressBar.value = 0f;
        }
    }

    public void UpdateProgress(float progress)
    {
        if (_progressBar == null) return;

        Tween.StopAll(_progressBar);
        Tween.Custom(_progressBar.value, progress, 0.1f, val => _progressBar.value = val);
    }

    public Tween HideAnimate()
    {
        if (_canvasGroup == null)
        {
            gameObject.SetActive(false);
            return default;
        }

        return Tween.Alpha(_canvasGroup, 0f, 0.5f)
            .OnComplete(() => gameObject.SetActive(false));
    }
}
