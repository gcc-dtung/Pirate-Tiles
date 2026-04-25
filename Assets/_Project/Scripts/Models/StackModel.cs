using System.Collections.Generic;
using System.Linq;

public class StackModel
{
    private List<TileModel> _tiles;

    public int MaxSize { get; private set; }
    public IReadOnlyList<TileModel> Tiles => _tiles.AsReadOnly();
    public int Count => _tiles.Count;
    public bool IsFull => Count >= MaxSize;

    public StackModel(int initialMaxSize = 7)
    {
        MaxSize = initialMaxSize;
        _tiles = new List<TileModel>();
    }

    public void Initialize(int maxStackSize)
    {
        MaxSize = maxStackSize;
        _tiles.Clear();
    }

    // 3.9 GetInsertIndex
    public int GetInsertIndex(CardType type)
    {
        // Tìm vị trí của tile cùng loại cuối cùng trong stack
        for (int i = _tiles.Count - 1; i >= 0; i--)
        {
            if (_tiles[i].TileType == type)
            {
                return i + 1; // Chèn ngay sau tile cùng loại
            }
        }
        // Nếu không có, chèn vào cuối
        return _tiles.Count;
    }

    public void InsertTile(int index, TileModel tile)
    {
        if (IsFull) return; // Tuỳ cách xử lý, có thể throw exception
        
        // Đảm bảo index hợp lệ
        if (index < 0) index = 0;
        if (index > _tiles.Count) index = _tiles.Count;

        _tiles.Insert(index, tile);
        tile.State = CardState.InStack;
    }

    // 3.10 FindMatch
    public int FindMatch()
    {
        // Duyệt tìm 3 tile liên tiếp giống nhau
        for (int i = 0; i <= _tiles.Count - 3; i++)
        {
            if (_tiles[i].TileType == _tiles[i + 1].TileType && 
                _tiles[i].TileType == _tiles[i + 2].TileType)
            {
                return i; // Trả về index bắt đầu của chuỗi match
            }
        }
        return -1; // Không tìm thấy
    }

    public void RemoveTiles(int startIndex, int count = 3)
    {
        if (startIndex >= 0 && startIndex + count <= _tiles.Count)
        {
            // Trong game thực tế, ta có thể đổi trạng thái các thẻ này thành None hoặc Remove hoàn toàn
            _tiles.RemoveRange(startIndex, count);
        }
    }

    // 3.11 GetMostFrequentType
    public CardType GetMostFrequentType()
    {
        if (_tiles.Count == 0) return CardType.None;

        var mostFrequent = _tiles
            .GroupBy(t => t.TileType)
            .OrderByDescending(g => g.Count())
            .FirstOrDefault();

        return mostFrequent?.Key ?? CardType.None;
    }

    // 3.12 ExpandSize
    public void ExpandSize(int amount = 1)
    {
        MaxSize += amount;
    }
}
