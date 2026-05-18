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
        var boardTiles = _tiles.Where(t => t.State == CardState.InBoard).ToList();
        if (boardTiles.Count <= 1) return;

        var originalTypes = boardTiles.Select(t => t.TileType).ToList();
        System.Random rng = new System.Random();

        int maxRetries = 10;
        bool hasMatch = false;

        for (int retry = 0; retry < maxRetries; retry++)
        {
            var types = new List<CardType>(originalTypes);
            int n = types.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                CardType value = types[k];
                types[k] = types[n];
                types[n] = value;
            }

            for (int i = 0; i < boardTiles.Count; i++)
            {
                boardTiles[i].TileType = types[i];
            }

            UpdateSelectableStatus();

            if (HasSelectableMatch())
            {
                hasMatch = true;
                break;
            }
        }

        if (!hasMatch)
        {
            ForceGuaranteeSelectableMatch(boardTiles, originalTypes, rng);
            UpdateSelectableStatus();
        }
    }

    private bool HasSelectableMatch()
    {
        var selectableTiles = _tiles.Where(t => t.State == CardState.InBoard && t.IsSelectable).ToList();
        var grouped = selectableTiles.GroupBy(t => t.TileType);
        return grouped.Any(g => g.Count() >= 3);
    }

    private void ForceGuaranteeSelectableMatch(List<TileModel> boardTiles, List<CardType> originalTypes, System.Random rng)
    {
        var selectableTiles = boardTiles.Where(t => t.IsSelectable).ToList();
        if (selectableTiles.Count >= 3)
        {
            var matchTiles = selectableTiles.OrderBy(x => rng.Next()).Take(3).ToList();
            
            var typeCounts = originalTypes.GroupBy(t => t).Where(g => g.Count() >= 3).Select(g => g.Key).ToList();
            CardType chosenType = typeCounts.Count > 0 ? typeCounts[rng.Next(typeCounts.Count)] : originalTypes[0];

            foreach (var t in matchTiles)
            {
                t.TileType = chosenType;
                originalTypes.Remove(chosenType);
            }

            var remainingTiles = boardTiles.Except(matchTiles).ToList();
            int n = originalTypes.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                CardType value = originalTypes[k];
                originalTypes[k] = originalTypes[n];
                originalTypes[n] = value;
            }

            for (int i = 0; i < remainingTiles.Count; i++)
            {
                remainingTiles[i].TileType = originalTypes[i];
            }
        }
    }

    // 3.6 GetTilesByType
    public List<TileModel> GetTilesByType(CardType type)
    {
        return _tiles.Where(t => t.TileType == type).ToList();
    }

    public void RestoreTile(int tileId)
    {
        var tile = GetTileById(tileId);
        if (tile != null && tile.State != CardState.InBoard)
        {
            tile.State = CardState.InBoard;
            UpdateSelectableStatus();
        }
    }

    public List<TileModel> GetBestMagicTargets(int count, IReadOnlyList<TileModel> actualStackTiles)
    {
        var boardTiles = _tiles.Where(t => t.State == CardState.InBoard).ToList();

        // 1. Prioritize types that are ALREADY in the stack
        var stackGroups = actualStackTiles.GroupBy(t => t.TileType).OrderByDescending(g => g.Count());
        foreach (var group in stackGroups)
        {
            int neededFromBoard = count - group.Count();
            if (neededFromBoard <= 0) continue;

            var boardTilesOfType = boardTiles.Where(t => t.TileType == group.Key).OrderByDescending(t => t.IsSelectable).ToList();
            if (boardTilesOfType.Count >= neededFromBoard)
            {
                var targets = new List<TileModel>();
                foreach (var st in group)
                {
                    var boardTile = GetTileById(st.Id);
                    if (boardTile != null) targets.Add(boardTile);
                }
                targets.AddRange(boardTilesOfType.Take(neededFromBoard)); // Add needed from board
                return targets;
            }
        }

        // 2. If no combination with stack works, try to find 'count' tiles purely on the board
        var grouped = boardTiles.GroupBy(t => t.TileType).OrderByDescending(g => g.Count());

        foreach (var group in grouped)
        {
            if (group.Count() >= count)
            {
                var sorted = group.OrderByDescending(t => t.IsSelectable).ToList();
                return sorted.Take(count).ToList();
            }
        }
        
        return null;
    }

    // Helper
    private TileModel GetTileById(int id)
    {
        return _tiles.FirstOrDefault(t => t.Id == id);
    }
}
