using UnityEngine;

[System.Serializable]
public struct LevelTileData 
{
    public Vector2Int GridPosition;
    public int LayerIndex; // Lớp (cao độ) của lá bài để xác định bài đè lên nhau
    public CardType Type; // Loại bài (hoặc None nếu muốn random tự động)
}
