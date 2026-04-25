using UnityEngine;

public class TutorialController : MonoBehaviour
{
    [SerializeField] private TutorialView _tutorialView;
    
    private string[] _steps = new string[] {
        "Welcome to Pirate Tiles!",
        "Match 3 identical tiles in the stack.",
        "Don't let the stack get full!"
    };
    private int _currentStepIndex = 0;

    private void Start()
    {
        // Kiểm tra PlayerPrefs xem đã chơi qua chưa
        bool hasPlayedTutorial = PlayerPrefs.GetInt("TutorialCompleted", 0) == 1;
        if (!hasPlayedTutorial)
        {
            StartTutorial();
        }
    }

    public void StartTutorial()
    {
        if (_tutorialView != null)
        {
            _currentStepIndex = 0;
            _tutorialView.OnNext += HandleNextStep;
            _tutorialView.OnSkip += EndTutorial;
            
            ShowCurrentStep();
        }
    }

    private void ShowCurrentStep()
    {
        if (_currentStepIndex < _steps.Length)
        {
            _tutorialView.ShowStep(_steps[_currentStepIndex]);
        }
        else
        {
            EndTutorial();
        }
    }

    private void HandleNextStep()
    {
        _currentStepIndex++;
        ShowCurrentStep();
    }

    private void EndTutorial()
    {
        if (_tutorialView != null)
        {
            _tutorialView.OnNext -= HandleNextStep;
            _tutorialView.OnSkip -= EndTutorial;
            _tutorialView.Hide();
        }
        
        PlayerPrefs.SetInt("TutorialCompleted", 1);
        PlayerPrefs.Save();
        
        Debug.Log("Tutorial completed.");
    }
}
