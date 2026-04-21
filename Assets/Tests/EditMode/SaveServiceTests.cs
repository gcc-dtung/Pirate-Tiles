using NUnit.Framework;
using UnityEngine;

/// <summary>
/// Unit Tests cho SaveService (Task 2.5)
/// Chạy trong EditMode — không cần Play Mode, không cần Scene.
/// 
/// Vì SaveService là MonoBehaviour Singleton, ta dùng một helper method
/// để tạo instance tạm thời bằng new GameObject().AddComponent<SaveService>().
/// Sau mỗi test, ta dọn dẹp PlayerPrefs và destroy GameObject.
/// </summary>
[TestFixture]
public class SaveServiceTests
{
    private SaveService _saveService;
    private GameObject _gameObject;

    // Prefix riêng cho test để không conflict với dữ liệu thật
    private const string TestPrefix = "__TEST_";

    [SetUp]
    public void SetUp()
    {
        // Tạo GameObject tạm + gắn SaveService
        _gameObject = new GameObject("TestSaveService");
        _saveService = _gameObject.AddComponent<SaveService>();

        // Xóa sạch dữ liệu test cũ
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }

    [TearDown]
    public void TearDown()
    {
        // Dọn dẹp PlayerPrefs sau mỗi test
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        // Destroy GameObject
        if (_gameObject != null)
        {
            Object.DestroyImmediate(_gameObject);
        }
    }

    // ==========================================
    // INT TESTS
    // ==========================================

    [Test]
    public void SetInt_And_GetInt_ReturnsCorrectValue()
    {
        string key = TestPrefix + "IntKey";

        _saveService.SetInt(key, 42);

        Assert.AreEqual(42, _saveService.GetInt(key));
    }

    [Test]
    public void GetInt_WithDefault_ReturnsDefault_WhenKeyNotExist()
    {
        string key = TestPrefix + "NonExistentInt";

        int result = _saveService.GetInt(key, 99);

        Assert.AreEqual(99, result);
    }

    [Test]
    public void SetInt_OverwritesPreviousValue()
    {
        string key = TestPrefix + "OverwriteInt";

        _saveService.SetInt(key, 10);
        _saveService.SetInt(key, 20);

        Assert.AreEqual(20, _saveService.GetInt(key));
    }

    // ==========================================
    // FLOAT TESTS
    // ==========================================

    [Test]
    public void SetFloat_And_GetFloat_ReturnsCorrectValue()
    {
        string key = TestPrefix + "FloatKey";

        _saveService.SetFloat(key, 3.14f);

        Assert.AreEqual(3.14f, _saveService.GetFloat(key), 0.001f);
    }

    [Test]
    public void GetFloat_WithDefault_ReturnsDefault_WhenKeyNotExist()
    {
        string key = TestPrefix + "NonExistentFloat";

        float result = _saveService.GetFloat(key, 1.5f);

        Assert.AreEqual(1.5f, result, 0.001f);
    }

    // ==========================================
    // STRING TESTS
    // ==========================================

    [Test]
    public void SetString_And_GetString_ReturnsCorrectValue()
    {
        string key = TestPrefix + "StringKey";

        _saveService.SetString(key, "Ahoy Captain!");

        Assert.AreEqual("Ahoy Captain!", _saveService.GetString(key));
    }

    [Test]
    public void GetString_WithDefault_ReturnsDefault_WhenKeyNotExist()
    {
        string key = TestPrefix + "NonExistentString";

        string result = _saveService.GetString(key, "default_value");

        Assert.AreEqual("default_value", result);
    }

    [Test]
    public void SetString_WithEmptyString_ReturnsEmpty()
    {
        string key = TestPrefix + "EmptyString";

        _saveService.SetString(key, "");

        Assert.AreEqual("", _saveService.GetString(key, "fallback"));
    }

    // ==========================================
    // BOOL TESTS
    // ==========================================

    [Test]
    public void SetBool_True_And_GetBool_ReturnsTrue()
    {
        string key = TestPrefix + "BoolTrue";

        _saveService.SetBool(key, true);

        Assert.IsTrue(_saveService.GetBool(key));
    }

    [Test]
    public void SetBool_False_And_GetBool_ReturnsFalse()
    {
        string key = TestPrefix + "BoolFalse";

        _saveService.SetBool(key, false);

        Assert.IsFalse(_saveService.GetBool(key));
    }

    [Test]
    public void GetBool_WithDefaultTrue_ReturnsTrue_WhenKeyNotExist()
    {
        string key = TestPrefix + "NonExistentBool";

        bool result = _saveService.GetBool(key, true);

        Assert.IsTrue(result);
    }

    [Test]
    public void GetBool_WithDefaultFalse_ReturnsFalse_WhenKeyNotExist()
    {
        string key = TestPrefix + "NonExistentBoolFalse";

        bool result = _saveService.GetBool(key, false);

        Assert.IsFalse(result);
    }

    [Test]
    public void SetBool_Toggle_OverwritesCorrectly()
    {
        string key = TestPrefix + "BoolToggle";

        _saveService.SetBool(key, true);
        Assert.IsTrue(_saveService.GetBool(key));

        _saveService.SetBool(key, false);
        Assert.IsFalse(_saveService.GetBool(key));
    }

    // ==========================================
    // HASKEY & DELETE TESTS
    // ==========================================

    [Test]
    public void HasKey_ReturnsFalse_WhenKeyNotExist()
    {
        string key = TestPrefix + "GhostKey";

        Assert.IsFalse(_saveService.HasKey(key));
    }

    [Test]
    public void HasKey_ReturnsTrue_AfterSetInt()
    {
        string key = TestPrefix + "ExistingKey";

        _saveService.SetInt(key, 1);

        Assert.IsTrue(_saveService.HasKey(key));
    }

    [Test]
    public void DeleteKey_RemovesKey()
    {
        string key = TestPrefix + "DeleteMe";

        _saveService.SetInt(key, 100);
        Assert.IsTrue(_saveService.HasKey(key));

        _saveService.DeleteKey(key);
        Assert.IsFalse(_saveService.HasKey(key));
    }

    [Test]
    public void DeleteKey_GetInt_ReturnsDefault_AfterDelete()
    {
        string key = TestPrefix + "DeleteAndGet";

        _saveService.SetInt(key, 50);
        _saveService.DeleteKey(key);

        Assert.AreEqual(0, _saveService.GetInt(key)); // Default là 0
    }

    [Test]
    public void DeleteAll_RemovesAllKeys()
    {
        string key1 = TestPrefix + "Key1";
        string key2 = TestPrefix + "Key2";

        _saveService.SetInt(key1, 1);
        _saveService.SetString(key2, "test");

        _saveService.DeleteAll();

        Assert.IsFalse(_saveService.HasKey(key1));
        Assert.IsFalse(_saveService.HasKey(key2));
    }

    // ==========================================
    // EVENTBUS NOTIFICATION TESTS
    // ==========================================

    [Test]
    public void SetInt_RaisesEvent_WithCorrectKey()
    {
        string key = TestPrefix + "EventIntKey";
        string receivedKey = null;

        var binding = new EventBinding<SaveDataChangedEvent>(e => receivedKey = e.Key);
        EventBus<SaveDataChangedEvent>.Register(binding);

        _saveService.SetInt(key, 10);

        Assert.AreEqual(key, receivedKey);

        EventBus<SaveDataChangedEvent>.Deregister(binding);
    }

    [Test]
    public void SetFloat_RaisesEvent_WithCorrectKey()
    {
        string key = TestPrefix + "EventFloatKey";
        string receivedKey = null;

        var binding = new EventBinding<SaveDataChangedEvent>(e => receivedKey = e.Key);
        EventBus<SaveDataChangedEvent>.Register(binding);

        _saveService.SetFloat(key, 1.0f);

        Assert.AreEqual(key, receivedKey);

        EventBus<SaveDataChangedEvent>.Deregister(binding);
    }

    [Test]
    public void SetString_RaisesEvent_WithCorrectKey()
    {
        string key = TestPrefix + "EventStringKey";
        string receivedKey = null;

        var binding = new EventBinding<SaveDataChangedEvent>(e => receivedKey = e.Key);
        EventBus<SaveDataChangedEvent>.Register(binding);

        _saveService.SetString(key, "test");

        Assert.AreEqual(key, receivedKey);

        EventBus<SaveDataChangedEvent>.Deregister(binding);
    }

    [Test]
    public void SetBool_RaisesEvent_WithCorrectKey()
    {
        string key = TestPrefix + "EventBoolKey";
        string receivedKey = null;

        var binding = new EventBinding<SaveDataChangedEvent>(e => receivedKey = e.Key);
        EventBus<SaveDataChangedEvent>.Register(binding);

        _saveService.SetBool(key, true);

        Assert.AreEqual(key, receivedKey);

        EventBus<SaveDataChangedEvent>.Deregister(binding);
    }

    [Test]
    public void DeleteKey_RaisesEvent_WithCorrectKey()
    {
        string key = TestPrefix + "EventDeleteKey";
        string receivedKey = null;

        _saveService.SetInt(key, 1);

        var binding = new EventBinding<SaveDataChangedEvent>(e => receivedKey = e.Key);
        EventBus<SaveDataChangedEvent>.Register(binding);

        _saveService.DeleteKey(key);

        Assert.AreEqual(key, receivedKey);

        EventBus<SaveDataChangedEvent>.Deregister(binding);
    }

    [Test]
    public void SetInt_MultipleListeners_AllReceiveEvent()
    {
        string key = TestPrefix + "MultiListener";
        int callCount = 0;

        var binding1 = new EventBinding<SaveDataChangedEvent>(e => callCount++);
        var binding2 = new EventBinding<SaveDataChangedEvent>(e => callCount++);
        EventBus<SaveDataChangedEvent>.Register(binding1);
        EventBus<SaveDataChangedEvent>.Register(binding2);

        _saveService.SetInt(key, 5);

        Assert.AreEqual(2, callCount);

        EventBus<SaveDataChangedEvent>.Deregister(binding1);
        EventBus<SaveDataChangedEvent>.Deregister(binding2);
    }

    // ==========================================
    // DOMAIN-SPECIFIC (SaveKeys) TESTS
    // ==========================================

    [Test]
    public void SaveKeys_Hearts_PersistsCorrectly()
    {
        _saveService.SetInt(SaveKeys.Hearts, 5);

        Assert.AreEqual(5, _saveService.GetInt(SaveKeys.Hearts));
    }

    [Test]
    public void SaveKeys_MusicVolume_BoolPersistsCorrectly()
    {
        _saveService.SetBool(SaveKeys.MusicVolume, false);

        Assert.IsFalse(_saveService.GetBool(SaveKeys.MusicVolume, true));
    }

    [Test]
    public void SaveKeys_UnlockLevel_IncrementsCorrectly()
    {
        _saveService.SetInt(SaveKeys.UnlockLevel, 1);
        Assert.AreEqual(1, _saveService.GetInt(SaveKeys.UnlockLevel));

        int currentLevel = _saveService.GetInt(SaveKeys.UnlockLevel);
        _saveService.SetInt(SaveKeys.UnlockLevel, currentLevel + 1);
        Assert.AreEqual(2, _saveService.GetInt(SaveKeys.UnlockLevel));
    }
}
