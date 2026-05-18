using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

public class LoadingView : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private Slider _progressBar;

    [Header("Boat Animation")]
    [SerializeField] private RectTransform _boatImage; // ảnh con thuyền
    [SerializeField] private float _boatYOffset = 40f; // khoảng cách thuyền trên thanh bar (pixel)

    private RectTransform _sliderRect;

    private void Awake()
    {
        if (_progressBar != null)
            _sliderRect = _progressBar.GetComponent<RectTransform>();
    }

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

        // Đặt thuyền về vị trí đầu
        UpdateBoatPosition(0f);
    }

    public void UpdateProgress(float progress)
    {
        if (_progressBar == null) return;

        Tween.StopAll(_progressBar);
        Tween.Custom(_progressBar.value, progress, 0.1f, val =>
        {
            _progressBar.value = val;
            UpdateBoatPosition(val);
        });
    }

    /// <summary>
    /// Cập nhật vị trí X của thuyền theo tiến trình (0–1),
    /// thuyền nằm ngay trên đầu thanh fill.
    /// </summary>
    private void UpdateBoatPosition(float progress)
    {
        if (_boatImage == null || _sliderRect == null) return;

        // Tính chiều rộng thực tế của slider trong world-space
        float sliderWidth = _sliderRect.rect.width;

        // Vị trí X: từ cạnh trái (-width/2) đến cạnh phải (+width/2) của slider
        float xOffset = Mathf.Lerp(-sliderWidth * 0.5f, sliderWidth * 0.5f, progress);

        // Giữ parent của thuyền là cùng canvas, nên lấy tọa độ từ slider rect
        Vector2 sliderCenter = _sliderRect.anchoredPosition;

        _boatImage.anchoredPosition = new Vector2(
            sliderCenter.x + xOffset,
            sliderCenter.y + _boatYOffset
        );
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
