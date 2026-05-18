using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneService : MonoBehaviour
{
    public static SceneService Instance { get; private set; }
    
    public static string TargetSceneToLoad { get; private set; }

    private EventBinding<SceneLoadRequestedEvent> _sceneLoadBinding;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        _sceneLoadBinding = new EventBinding<SceneLoadRequestedEvent>(OnSceneLoadRequested);
        EventBus<SceneLoadRequestedEvent>.Register(_sceneLoadBinding);
    }

    private void OnDisable()
    {
        EventBus<SceneLoadRequestedEvent>.Deregister(_sceneLoadBinding);
    }

    private void OnSceneLoadRequested(SceneLoadRequestedEvent data)
    {
        if (data.UseLoadingScreen)
        {
            TargetSceneToLoad = data.SceneName;
            SceneManager.LoadScene(SceneNames.Loading);
            return;
        }

        SceneManager.LoadScene(data.SceneName);
    }
}
