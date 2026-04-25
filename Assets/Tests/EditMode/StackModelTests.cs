using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class StackModelTests
{
    private StackModel _stackModel;

    [SetUp]
    public void SetUp()
    {
        _stackModel = new StackModel(7);
    }

    [Test]
    public void Initialize_SetsMaxSizeAndClearsTiles()
    {
        _stackModel.InsertTile(0, new TileModel(1, CardType.Sword, Vector2Int.zero, 0));
        
        _stackModel.Initialize(8);
        
        Assert.AreEqual(8, _stackModel.MaxSize);
        Assert.AreEqual(0, _stackModel.Count);
    }

    [Test]
    public void GetInsertIndex_ReturnsCorrectIndexForSameType()
    {
        _stackModel.InsertTile(0, new TileModel(1, CardType.Sword, Vector2Int.zero, 0));
        _stackModel.InsertTile(1, new TileModel(2, CardType.Anchor, Vector2Int.zero, 0));
        
        // Cố gắng chèn thêm một Sword -> phải chèn ngay sau Sword đầu tiên (index 1)
        int index = _stackModel.GetInsertIndex(CardType.Sword);
        
        Assert.AreEqual(1, index);
    }

    [Test]
    public void GetInsertIndex_ReturnsLastIndexForNewType()
    {
        _stackModel.InsertTile(0, new TileModel(1, CardType.Sword, Vector2Int.zero, 0));
        
        // Chèn loại hoàn toàn mới -> phải chèn vào cuối
        int index = _stackModel.GetInsertIndex(CardType.Anchor);
        
        Assert.AreEqual(1, index);
    }

    [Test]
    public void InsertTile_ChangesStateToInStack()
    {
        var tile = new TileModel(1, CardType.Sword, Vector2Int.zero, 0);
        
        _stackModel.InsertTile(0, tile);
        
        Assert.AreEqual(1, _stackModel.Count);
        Assert.AreEqual(CardState.InStack, tile.State);
    }

    [Test]
    public void FindMatch_ReturnsIndexWhenThreeIdenticalTiles()
    {
        _stackModel.InsertTile(0, new TileModel(1, CardType.Sword, Vector2Int.zero, 0));
        _stackModel.InsertTile(1, new TileModel(2, CardType.Anchor, Vector2Int.zero, 0));
        _stackModel.InsertTile(2, new TileModel(3, CardType.Anchor, Vector2Int.zero, 0));
        _stackModel.InsertTile(3, new TileModel(4, CardType.Anchor, Vector2Int.zero, 0));
        
        // Match 3 thẻ Anchor (index 1, 2, 3) -> Trả về 1 (index bắt đầu)
        int matchIndex = _stackModel.FindMatch();
        
        Assert.AreEqual(1, matchIndex);
    }

    [Test]
    public void FindMatch_ReturnsMinusOneWhenNoMatch()
    {
        _stackModel.InsertTile(0, new TileModel(1, CardType.Sword, Vector2Int.zero, 0));
        _stackModel.InsertTile(1, new TileModel(2, CardType.Anchor, Vector2Int.zero, 0));
        _stackModel.InsertTile(2, new TileModel(3, CardType.Anchor, Vector2Int.zero, 0));
        
        // Chỉ có 2 Anchor, không đủ 3
        int matchIndex = _stackModel.FindMatch();
        
        Assert.AreEqual(-1, matchIndex);
    }

    [Test]
    public void RemoveTiles_RemovesCorrectAmountOfTiles()
    {
        _stackModel.InsertTile(0, new TileModel(1, CardType.Sword, Vector2Int.zero, 0));
        _stackModel.InsertTile(1, new TileModel(2, CardType.Anchor, Vector2Int.zero, 0));
        _stackModel.InsertTile(2, new TileModel(3, CardType.Anchor, Vector2Int.zero, 0));
        _stackModel.InsertTile(3, new TileModel(4, CardType.Anchor, Vector2Int.zero, 0));

        _stackModel.RemoveTiles(1, 3);
        
        Assert.AreEqual(1, _stackModel.Count);
        Assert.AreEqual(CardType.Sword, _stackModel.Tiles[0].TileType);
    }

    [Test]
    public void GetMostFrequentType_ReturnsMostFrequentCardType()
    {
        _stackModel.InsertTile(0, new TileModel(1, CardType.Sword, Vector2Int.zero, 0));
        _stackModel.InsertTile(1, new TileModel(2, CardType.Anchor, Vector2Int.zero, 0));
        _stackModel.InsertTile(2, new TileModel(3, CardType.Anchor, Vector2Int.zero, 0));

        CardType frequent = _stackModel.GetMostFrequentType();

        Assert.AreEqual(CardType.Anchor, frequent);
    }

    [Test]
    public void ExpandSize_IncreasesMaxSize()
    {
        int initialSize = _stackModel.MaxSize;
        
        _stackModel.ExpandSize(1);
        
        Assert.AreEqual(initialSize + 1, _stackModel.MaxSize);
    }

    [Test]
    public void IsFull_ReturnsTrueWhenCountEqualsMaxSize()
    {
        var model = new StackModel(2); // Max size là 2
        
        model.InsertTile(0, new TileModel(1, CardType.Sword, Vector2Int.zero, 0));
        Assert.IsFalse(model.IsFull);
        
        model.InsertTile(1, new TileModel(2, CardType.Anchor, Vector2Int.zero, 0));
        Assert.IsTrue(model.IsFull);
    }
}
