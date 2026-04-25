using UnityEngine;

public class TileModel
{
    public int Id { get; private set; }
    public CardType TileType { get; set; }
    public CardState State { get; set; }
    public Vector2Int GridPosition { get; private set; }
    public int LayerIndex { get; private set; }
    public bool IsSelectable { get; set; }
    public bool IsSpecialTile => TileType >= CardType.Bomb;

    public TileModel(int id, CardType tileType, Vector2Int gridPosition, int layerIndex)
    {
        Id = id;
        TileType = tileType;
        State = CardState.InBoard; // Mặc định khi tạo ra là ở trên bàn cờ
        GridPosition = gridPosition;
        LayerIndex = layerIndex;
        IsSelectable = false; // Phải được BoardModel tính toán lại
    }
}
