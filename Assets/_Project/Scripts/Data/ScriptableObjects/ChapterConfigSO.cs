using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ChapterConfig", menuName = "PirateTiles/Data/ChapterConfig")]
public class ChapterConfigSO : ScriptableObject
{
    [field: SerializeField] public int ChapterIndex { get; private set; }
    [field: SerializeField] public string ChapterName { get; private set; }
    [field: SerializeField] public Sprite MapBackground { get; private set; }

    [System.Serializable]
    public struct LevelNodeData 
    {
        public LevelConfigSO LevelConfig;
        public Vector2 MapPosition; // Vị trí trên map background
        public Sprite LevelIcon; // Icon riêng cho từng level
    }
    
    [field: SerializeField] public List<LevelNodeData> LevelNodes { get; private set; }
}
