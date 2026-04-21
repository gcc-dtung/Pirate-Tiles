using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneService : MonoBehaviour
{
    public static SceneService Instance { get; private set; }

    [Header("Loading UI")]
    // Kéo một Loading Canvas (nằm dưới GameManager) vào đây, mặc định tắt đi
    [SerializeField] private GameObject _loadingScreen; 
    
    private EventBinding<SceneLoadRequestedEvent> _sceneLoadBinding;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
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
            _ = LoadSceneAsyncAwaitable(data.SceneName); // Lệnh _ = để báo cho compiler biết ta chủ động bỏ qua việc await
        }
        else
        {
            SceneManager.LoadScene(data.SceneName);
        }
    }

    private async Awaitable LoadSceneAsyncAwaitable(string sceneName)
    {
        // 1. Hiện màn hình Loading
        if (_loadingScreen != null) _loadingScreen.SetActive(true);

        // 2. Bắt đầu load scene ngầm và đợi nó hoàn thành
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        
        // Chờ cho đến khi tiến trình load trả về kết quả xong
        while (!asyncLoad.isDone)
        {
            // Ở đây bạn có thể update thanh Progress Bar nếu có:
            // float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            
            // Tương đương với yield return null trong Coroutine
            await Awaitable.NextFrameAsync();
        }

        // 3. Tắt màn hình Loading sau khi load xong
        if (_loadingScreen != null) _loadingScreen.SetActive(false);
    }
}
