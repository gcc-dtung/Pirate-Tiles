using PrimeTween;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingSceneController : MonoBehaviour
{
    [SerializeField] private LoadingView _loadingView;
    [SerializeField] private float _minimumLoadingTime = 1.5f; // Thời gian load tối thiểu (giây)

    private void Start()
    {
        // Nếu không có scene đích (tức là khởi động game lần đầu), mặc định tải vào Map
        string targetScene = string.IsNullOrEmpty(SceneService.TargetSceneToLoad)
            ? SceneNames.Map
            : SceneService.TargetSceneToLoad;

        if (_loadingView != null)
        {
            _loadingView.Show();
        }

        _ = LoadTargetSceneAsync(targetScene);
    }

    private async Awaitable LoadTargetSceneAsync(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;
        
        float timeElapsed = 0f;
        
        while (asyncLoad.progress < 0.9f || timeElapsed < _minimumLoadingTime)
        {
            timeElapsed += Time.deltaTime;
            
            float loadProgress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            float timeProgress = Mathf.Clamp01(timeElapsed / _minimumLoadingTime);
            
            // Lấy giá trị nhỏ hơn giữa tiến trình tải thật và tiến trình thời gian ảo
            float displayProgress = Mathf.Min(loadProgress, timeProgress);

            if (_loadingView != null)
            {
                _loadingView.UpdateProgress(displayProgress);
            }
            
            await Awaitable.NextFrameAsync();
        }

        if (_loadingView != null)
        {
            _loadingView.UpdateProgress(1f);
            await _loadingView.HideAnimate();
        }

        asyncLoad.allowSceneActivation = true;
    }
}
