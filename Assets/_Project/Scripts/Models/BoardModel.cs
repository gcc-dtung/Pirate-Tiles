using System.Collections.Generic;
using System.Linq;

public class BoardModel
{
    private List<TileModel> _tiles;
    private Dictionary<int, List<int>> _overlapMap; // Key: tileId, Value: danh sách ID của các tile đè lên trên nó

    public IReadOnlyList<TileModel> Tiles => _tiles.AsReadOnly();
    
    // Số lượng tile còn lại trên bàn cờ
    public int RemainingTiles => _tiles.Count(t => t.State == CardState.InBoard);
    
    public bool IsCleared => RemainingTiles <= 0;

    public BoardModel()
    {
        _tiles = new List<TileModel>();
        _overlapMap = new Dictionary<int, List<int>>();
    }

    // 3.2 BoardModel - khởi tạo
    public void Initialize(List<TileModel> tiles, Dictionary<int, List<int>> overlapMap)
    {
        _tiles = tiles;
        _overlapMap = overlapMap;
        UpdateSelectableStatus();
    }

    // 3.3 UpdateSelectableStatus + Raise EventBus
    public void UpdateSelectableStatus()
    {
        int selectableCount = 0;
        foreach (var tile in _tiles)
        {
            if (tile.State != CardState.InBoard)
            {
                tile.IsSelectable = false;
                continue;
            }

            bool isCovered = false;
            // Kiểm tra xem có tile nào đè lên và vẫn đang ở trên bàn cờ không
            if (_overlapMap.TryGetValue(tile.Id, out var coveringTiles))
            {
                foreach (var coverId in coveringTiles)
                {
                    var coverTile = GetTileById(coverId);
                    if (coverTile != null && coverTile.State == CardState.InBoard)
                    {
                        isCovered = true;
                        break;
                    }
                }
            }
            
            tile.IsSelectable = !isCovered;
            if (tile.IsSelectable) 
            {
                selectableCount++;
            }
        }

        // Raise EventBus để thông báo cập nhật
        EventBus<BoardModelUpdatedEvent>.Raise(new BoardModelUpdatedEvent 
        {
            RemainingTiles = RemainingTiles,
            SelectableTiles = selectableCount
        });
    }

    // 3.4 RemoveTile
    public void RemoveTile(int tileId)
    {
        var tile = GetTileById(tileId);
        if (tile != null && tile.State == CardState.InBoard)
        {
            tile.State = CardState.InStack; // Đổi trạng thái (nếu Controller chưa đổi)
            
            // Có thể thêm log hoặc validation ở đây

            // Sau khi remove thì cập nhật lại selectable
            UpdateSelectableStatus();
        }
    }

    // 3.5 ShuffleTileTypes
    public void ShuffleTileTypes()
    {
        // Lấy danh sách các tile còn trên bàn cờ
        var boardTiles = _tiles.Where(t => t.State == CardState.InBoard).ToList();
        if (boardTiles.Count <= 1) return;

        // Trích xuất list Type
        var types = boardTiles.Select(t => t.TileType).ToList();
        
        // Shuffle the types (Fisher-Yates)
        System.Random rng = new System.Random();
        int n = types.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            CardType value = types[k];
            types[k] = types[n];
            types[n] = value;
        }

        // Gán ngược lại
        for (int i = 0; i < boardTiles.Count; i++)
        {
            boardTiles[i].TileType = types[i];
        }
    }

    // 3.6 GetTilesByType
    public List<TileModel> GetTilesByType(CardType type)
    {
        return _tiles.Where(t => t.TileType == type).ToList();
    }

    // Helper
    private TileModel GetTileById(int id)
    {
        return _tiles.FirstOrDefault(t => t.Id == id);
    }
}
