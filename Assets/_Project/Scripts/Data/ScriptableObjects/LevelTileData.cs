using UnityEngine;

[System.Serializable]
public struct LevelTileData 
{
    public Vector2 GridPosition; // Tọa độ tâm tile, hỗ trợ thập phân (VD: 0.5, 0.5)
    public Vector2 Size;         // Kích thước tile theo grid unit, VD: (1,1), (2,1), (1,2)
    public int LayerIndex;       // Lớp (cao độ) của lá bài để xác định bài đè lên nhau
    public CardType Type;        // Loại bài (hoặc None nếu muốn random tự động)
}
