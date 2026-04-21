using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "LevelConfig_01", menuName = "PirateTiles/Data/LevelConfig")]
public class LevelConfigSO : ScriptableObject
{
    [field: Header("Level Rules")]
    [field: SerializeField] public int LevelIndex { get; private set; }
    [field: SerializeField] public float TimeLimit { get; private set; } // Để 0 nếu không giới hạn thời gian
    [field: SerializeField] public int MaxStackSize { get; private set; } = 7; // Khay chứa bài thường là 7 ô

    [field: Header("Board Layout")]
    // Bạn có thể lưu trực tiếp dữ liệu dạng List hoặc sử dụng JSON từ Editor
    [field: SerializeField] public List<LevelTileData> InitialTiles { get; private set; }
    
    // [field: SerializeField] public TextAsset LevelDataJson { get; private set; } // (Tùy chọn nếu bạn muốn load layout từ JSON)
}
