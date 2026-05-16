using UnityEditor;
using UnityEngine;

public class SaveDataTools
{
    [MenuItem("PirateTiles/Tools/Reset Unlocked Levels")]
    public static void ResetUnlockedLevels()
    {
        PlayerPrefs.SetInt(SaveKeys.UnlockLevel, 1);
        PlayerPrefs.Save();
        Debug.Log("Unlocked levels have been reset to 1.");
    }

    [MenuItem("PirateTiles/Tools/Reset All Data")]
    public static void ResetAllData()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("All PlayerPrefs data has been deleted.");
    }
    
    [MenuItem("PirateTiles/Tools/Unlock All Levels")]
    public static void UnlockAllLevels()
    {
        PlayerPrefs.SetInt(SaveKeys.UnlockLevel, 999);
        PlayerPrefs.Save();
        Debug.Log("All levels have been unlocked (set to 999).");
    }
}
