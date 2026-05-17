using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class LevelButtonView : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private Image _iconImage;
    [SerializeField] private Color _lockedColor = Color.gray;
    [SerializeField] private Color _unlockedColor = Color.white;
    [SerializeField] private Color _completedColor = Color.yellow;
    [SerializeField] private bool _showLevelNumber = false;
    
    // Optional: Reference to specific icons for states
    [SerializeField] private Sprite _lockedSprite;
    [SerializeField] private Sprite _unlockedSprite;
    [SerializeField] private Sprite _completedSprite;

    [SerializeField] private Material _grayscaleMaterial; // Thêm biến nhận Material đen trắng
    private Material _defaultMaterial;

    private int _levelIndex;
    public event Action<int> OnLevelClicked;

    private void Awake()
    {
        if (_button != null)
        {
            _button.onClick.AddListener(() => OnLevelClicked?.Invoke(_levelIndex));
        }

        if (_iconImage != null)
        {
            _defaultMaterial = _iconImage.material; // Lưu lại material gốc
        }
    }

    public void Setup(int levelIndex, bool isUnlocked, bool isCompleted, Sprite customIcon = null)
    {
        _levelIndex = levelIndex;
        if (_levelText != null)
        {
            _levelText.text = levelIndex.ToString();
            _levelText.gameObject.SetActive(_showLevelNumber);
        }

        if (_button != null)
        {
            _button.interactable = isUnlocked;
        }

        if (_iconImage != null)
        {
            if (isCompleted)
            {
                _iconImage.color = _completedColor;
                _iconImage.material = _defaultMaterial; // Dùng material gốc
                if (customIcon != null) _iconImage.sprite = customIcon;
                else if (_completedSprite != null) _iconImage.sprite = _completedSprite;
            }
            else if (isUnlocked)
            {
                _iconImage.color = _unlockedColor;
                _iconImage.material = _defaultMaterial; // Dùng material gốc
                if (customIcon != null) _iconImage.sprite = customIcon;
                else if (_unlockedSprite != null) _iconImage.sprite = _unlockedSprite;
            }
            else
            {
                // Thay vì dùng Color.gray làm tối ảnh, ta áp dụng Grayscale Material và giữ màu trắng
                _iconImage.color = _unlockedColor; // Giữ màu trắng (tránh bị tối mờ)
                
                if (_grayscaleMaterial != null) 
                    _iconImage.material = _grayscaleMaterial; // Đổi sang trắng đen
                else 
                    _iconImage.color = _lockedColor; // Nếu chưa có material thì mới dùng màu xám dự phòng

                if (customIcon != null) _iconImage.sprite = customIcon;
                else if (_lockedSprite != null) _iconImage.sprite = _lockedSprite;
            }
        }
    }
}
