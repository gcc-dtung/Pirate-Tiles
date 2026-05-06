using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneService : MonoBehaviour
{
    public static SceneService Instance { get; private set; }

    [Header("Loading UI")]
    [SerializeField] private GameObject _loadingScreen;

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
            TryResolveLoadingScreen();
            _ = LoadSceneAsyncAwaitable(data.SceneName);
            return;
        }

        SceneManager.LoadScene(data.SceneName);
    }

    private async Awaitable LoadSceneAsyncAwaitable(string sceneName)
    {
        LoadingView loadingView = null;

        if (_loadingScreen != null)
        {
            _loadingScreen.SetActive(true);
            loadingView = _loadingScreen.GetComponent<LoadingView>();
            if (loadingView != null)
            {
                loadingView.Show();
            }
        }

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            if (loadingView != null)
            {
                loadingView.UpdateProgress(asyncLoad.progress);
            }
            await Awaitable.NextFrameAsync();
        }

        if (loadingView != null)
        {
            loadingView.UpdateProgress(1f);
            await loadingView.HideAnimate();
        }
        else if (_loadingScreen != null)
        {
            _loadingScreen.SetActive(false);
        }
    }

    private void TryResolveLoadingScreen()
    {
        if (_loadingScreen != null) return;

        var roots = SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (var root in roots)
        {
            var children = root.GetComponentsInChildren<Transform>(true);
            foreach (var child in children)
            {
                if (child.name == "LoadingScreen")
                {
                    _loadingScreen = child.gameObject;
                    return;
                }
            }
        }
    }
}
