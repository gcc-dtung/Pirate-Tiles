using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class TutorialView : MonoBehaviour
{
    [SerializeField] private GameObject _tutorialPanel;
    [SerializeField] private TextMeshProUGUI _instructionText;
    [SerializeField] private Button _skipButton;
    [SerializeField] private Button _nextButton;

    public event Action OnSkip;
    public event Action OnNext;

    private void Awake()
    {
        _skipButton.onClick.AddListener(() => OnSkip?.Invoke());
        _nextButton.onClick.AddListener(() => OnNext?.Invoke());
    }

    public void ShowStep(string text)
    {
        _tutorialPanel.SetActive(true);
        _instructionText.text = text;
    }

    public void Hide()
    {
        _tutorialPanel.SetActive(false);
    }
}
