using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;

public class MapView : MonoBehaviour
{
    [Header("Chapter Info")]
    [SerializeField] private TextMeshProUGUI _chapterTitleText;
    [SerializeField] private Image _mapBackgroundImage;
    
    [Header("Navigation")]
    [SerializeField] private Button _leftArrowButton;
    [SerializeField] private Button _rightArrowButton;

    [Header("Levels")]
    [SerializeField] private LevelButtonView _levelButtonPrefab;
    [SerializeField] private Transform _levelContainer;

    public event Action OnNextChapterClicked;
    public event Action OnPrevChapterClicked;
    public event Action<int> OnLevelSelected;

    private List<LevelButtonView> _spawnedButtons = new List<LevelButtonView>();

    private void Awake()
    {
        if (_leftArrowButton != null) _leftArrowButton.onClick.AddListener(() => OnPrevChapterClicked?.Invoke());
        if (_rightArrowButton != null) _rightArrowButton.onClick.AddListener(() => OnNextChapterClicked?.Invoke());
    }

    public void SetArrowVisibility(bool canGoPrev, bool canGoNext)
    {
        if (_leftArrowButton != null) _leftArrowButton.gameObject.SetActive(canGoPrev);
        if (_rightArrowButton != null) _rightArrowButton.gameObject.SetActive(canGoNext);
    }

    public void SetupChapter(ChapterConfigSO chapterConfig, int globalUnlockedLevel)
    {
        if (_chapterTitleText != null)
        {
            _chapterTitleText.text = chapterConfig.ChapterName;
        }

        if (_mapBackgroundImage != null && chapterConfig.MapBackground != null)
        {
            _mapBackgroundImage.sprite = chapterConfig.MapBackground;
        }

        // Clear old buttons
        foreach(var b in _spawnedButtons) 
        {
            if (b != null) Destroy(b.gameObject);
        }
        _spawnedButtons.Clear();

        if (chapterConfig.LevelNodes != null)
        {
            for (int i = 0; i < chapterConfig.LevelNodes.Count; i++)
            {
                var nodeData = chapterConfig.LevelNodes[i];
                var btn = Instantiate(_levelButtonPrefab, _levelContainer);
                
                // Position
                var rect = btn.GetComponent<RectTransform>();
                if (rect != null)
                {
                    rect.anchoredPosition = nodeData.MapPosition;
                }

                int levelIndex = nodeData.LevelConfig != null ? nodeData.LevelConfig.LevelIndex : (chapterConfig.ChapterIndex * 12 + i + 1); // Fallback
                
                bool isUnlocked = levelIndex <= globalUnlockedLevel;
                bool isCompleted = levelIndex < globalUnlockedLevel;

                btn.Setup(levelIndex, isUnlocked, isCompleted);
                
                btn.OnLevelClicked += (lvl) => OnLevelSelected?.Invoke(lvl);
                
                _spawnedButtons.Add(btn);
            }
        }
    }
}
