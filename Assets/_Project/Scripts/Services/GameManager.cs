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
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
