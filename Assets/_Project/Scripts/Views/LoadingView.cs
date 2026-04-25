using UnityEngine;
using UnityEngine.UI;
using PrimeTween;

public class LoadingView : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private Slider _progressBar;

    public void Show()
    {
        gameObject.SetActive(true);
        _canvasGroup.alpha = 1f;
        _progressBar.value = 0f;
    }

    public void UpdateProgress(float progress)
    {
        Tween.StopAll(_progressBar);
        Tween.Custom(_progressBar.value, progress, 0.1f, val => _progressBar.value = val);
    }

    public Tween HideAnimate()
    {
        return Tween.Alpha(_canvasGroup, 0f, 0.5f)
            .OnComplete(() => gameObject.SetActive(false));
    }
}
