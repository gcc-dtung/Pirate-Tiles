using UnityEngine;

public class SaveService : MonoBehaviour
{
    // Singleton pattern vì Service được gắn trên GameManager (DontDestroyOnLoad)
    public static SaveService Instance { get; private set; }

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

    // ==========================================
    // CORE WRAPPERS
    // ==========================================

    public void SetInt(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
        PlayerPrefs.Save();
        NotifyDataChanged(key);
    }

    public int GetInt(string key, int defaultValue = 0)
    {
        return PlayerPrefs.GetInt(key, defaultValue);
    }

    public void SetFloat(string key, float value)
    {
        PlayerPrefs.SetFloat(key, value);
        PlayerPrefs.Save();
        NotifyDataChanged(key);
    }

    public float GetFloat(string key, float defaultValue = 0f)
    {
        return PlayerPrefs.GetFloat(key, defaultValue);
    }

    public void SetString(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
        PlayerPrefs.Save();
        NotifyDataChanged(key);
    }

    public string GetString(string key, string defaultValue = "")
    {
        return PlayerPrefs.GetString(key, defaultValue);
    }

    // PlayerPrefs không hỗ trợ bool, nên ta quy định 1 = true, 0 = false
    public void SetBool(string key, bool value)
    {
        PlayerPrefs.SetInt(key, value ? 1 : 0);
        PlayerPrefs.Save();
        NotifyDataChanged(key);
    }

    public bool GetBool(string key, bool defaultValue = false)
    {
        int defaultInt = defaultValue ? 1 : 0;
        return PlayerPrefs.GetInt(key, defaultInt) == 1;
    }

    public bool HasKey(string key)
    {
        return PlayerPrefs.HasKey(key);
    }

    public void DeleteKey(string key)
    {
        PlayerPrefs.DeleteKey(key);
        PlayerPrefs.Save();
        NotifyDataChanged(key); // Notify để UI/Model biết dữ liệu đã bị xóa (reset về default)
    }

    public void DeleteAll()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }

    // ==========================================
    // EVENTBUS NOTIFICATION
    // ==========================================

    private void NotifyDataChanged(string key)
    {
        // Bắn sự kiện nội bộ ra toàn cục, bất kỳ Controller nào subscribe
        // SaveDataChangedEvent đều sẽ biết khi dữ liệu mang tên "key" thay đổi
        EventBus<SaveDataChangedEvent>.Raise(new SaveDataChangedEvent 
        { 
            Key = key 
        });
    }

    // ==========================================
    // DOMAIN-SPECIFIC METHODS (Tuỳ chọn)
    // ==========================================
    // Nếu bạn không muốn viết "SaveService.Instance.SetInt(SaveKeys.Hearts, 5)" ở mọi nơi,
    // Bạn có thể wrap luôn domain method vào đây cho code gọn gàng, ví dụ:
    
    // public void SaveHearts(int heartsCount) => SetInt(SaveKeys.Hearts, heartsCount);
    // public int LoadHearts() => GetInt(SaveKeys.Hearts, 5); // 5 là mặc định
}
