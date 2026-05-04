using UnityEngine;

public class TileModel
{
    public int Id { get; private set; }
    public CardType TileType { get; set; }
    public CardState State { get; set; }
    public Vector2 GridPosition { get; private set; } // Tọa độ tâm tile, hỗ trợ thập phân
    public Vector2 Size { get; private set; }          // Kích thước tile: (1,1), (2,1), (1,2)...
    public int LayerIndex { get; private set; }
    public bool IsSelectable { get; set; }
    public bool IsSpecialTile => TileType >= CardType.Bomb;

    public TileModel(int id, CardType tileType, Vector2 gridPosition, int layerIndex, Vector2 size)
    {
        Id = id;
        TileType = tileType;
        State = CardState.InBoard;
        GridPosition = gridPosition;
        LayerIndex = layerIndex;
        Size = size.x > 0 && size.y > 0 ? size : Vector2.one; // Default (1,1) nếu size không hợp lệ
        IsSelectable = false;
    }
}
