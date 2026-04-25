using UnityEngine;

public class TutorialController : MonoBehaviour
{
    [SerializeField] private TutorialView _tutorialView;
    
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
            _tutorialView.ShowTutorial(new string[] {
                "Welcome to Pirate Tiles!",
                "Match 3 identical tiles in the stack.",
                "Don't let the stack get full!"
            });
            _tutorialView.OnTutorialFinished += HandleTutorialFinished;
        }
    }

    private void HandleTutorialFinished()
    {
        if (_tutorialView != null)
        {
            _tutorialView.OnTutorialFinished -= HandleTutorialFinished;
        }
        
        PlayerPrefs.SetInt("TutorialCompleted", 1);
        PlayerPrefs.Save();
        
        Debug.Log("Tutorial completed.");
    }
}
