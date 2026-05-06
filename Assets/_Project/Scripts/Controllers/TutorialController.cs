using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialController : MonoBehaviour
{
    [SerializeField] private TutorialView _tutorialView;

    private readonly string[] _steps =
    {
        "Welcome to Pirate Tiles!",
        "Match 3 identical tiles in the stack.",
        "Don't let the stack get full!"
    };

    private int _currentStepIndex;

    private void Start()
    {
        bool hasPlayedTutorial = SaveService.Instance != null
            && SaveService.Instance.GetBool(SaveKeys.TutorialCompleted, false);

        if (!hasPlayedTutorial)
        {
            StartTutorial();
        }
    }

    public void StartTutorial()
    {
        if (_tutorialView == null) return;

        _currentStepIndex = 0;
        _tutorialView.OnNext += HandleNextStep;
        _tutorialView.OnSkip += EndTutorial;
        ShowCurrentStep();
    }

    private void ShowCurrentStep()
    {
        if (_tutorialView == null) return;

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

        SaveService.Instance?.SetBool(SaveKeys.TutorialCompleted, true);
        Debug.Log("Tutorial completed.");

        if (SceneManager.GetActiveScene().name == SceneNames.Tutorial)
        {
            EventBus<SceneLoadRequestedEvent>.Raise(new SceneLoadRequestedEvent
            {
                SceneName = SceneNames.Map,
                UseLoadingScreen = true
            });
        }
    }
}
