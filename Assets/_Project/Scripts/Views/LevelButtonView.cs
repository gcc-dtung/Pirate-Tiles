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
    
    // Optional: Reference to specific icons for states
    [SerializeField] private Sprite _lockedSprite;
    [SerializeField] private Sprite _unlockedSprite;
    [SerializeField] private Sprite _completedSprite;

    private int _levelIndex;
    public event Action<int> OnLevelClicked;

    private void Awake()
    {
        if (_button != null)
        {
            _button.onClick.AddListener(() => OnLevelClicked?.Invoke(_levelIndex));
        }
    }

    public void Setup(int levelIndex, bool isUnlocked, bool isCompleted)
    {
        _levelIndex = levelIndex;
        if (_levelText != null)
        {
            _levelText.text = levelIndex.ToString();
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
                if (_completedSprite != null) _iconImage.sprite = _completedSprite;
            }
            else if (isUnlocked)
            {
                _iconImage.color = _unlockedColor;
                if (_unlockedSprite != null) _iconImage.sprite = _unlockedSprite;
            }
            else
            {
                _iconImage.color = _lockedColor;
                if (_lockedSprite != null) _iconImage.sprite = _lockedSprite;
            }
        }
    }
}
