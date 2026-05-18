using UnityEngine;
using PrimeTween;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            // Tắt warning khi tween endValue = currentValue (harmless, xảy ra khi card đã ở đúng vị trí)
            PrimeTweenConfig.warnEndValueEqualsCurrent = false;

            // Cấu hình FPS tối ưu cho thiết bị di động (Unlocks ~60 FPS or matching screen refresh rate)
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
