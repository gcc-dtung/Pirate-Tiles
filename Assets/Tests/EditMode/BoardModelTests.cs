using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class BoardModelTests
{
    private BoardModel _boardModel;

    [SetUp]
    public void SetUp()
    {
        _boardModel = new BoardModel();
    }

    [Test]
    public void Initialize_SetupTilesAndOverlapMap_Correctly()
    {
        // Arrange
        var tiles = new List<TileModel>
        {
            new TileModel(1, CardType.Sword, new Vector2(0, 0), 0, Vector2.one),
            new TileModel(2, CardType.Anchor, new Vector2(0, 0), 1, Vector2.one) // Tile 2 đè lên Tile 1
        };

        var overlapMap = new Dictionary<int, List<int>>
        {
            { 1, new List<int> { 2 } } // Tile 1 bị che bởi Tile 2
        };

        // Act
        _boardModel.Initialize(tiles, overlapMap);

        // Assert
        Assert.AreEqual(2, _boardModel.RemainingTiles);
        Assert.IsFalse(_boardModel.IsCleared);
        Assert.AreEqual(2, _boardModel.Tiles.Count);
        
        // UpdateSelectableStatus được gọi bên trong Initialize
        Assert.IsFalse(tiles[0].IsSelectable, "Tile bị che không được phép selectable");
        Assert.IsTrue(tiles[1].IsSelectable, "Tile nằm trên cùng phải selectable");
    }

    [Test]
    public void UpdateSelectableStatus_WhenCoveringTileRemoved_MakesTileSelectable()
    {
        // Arrange
        var tiles = new List<TileModel>
        {
            new TileModel(1, CardType.Sword, new Vector2(0, 0), 0, Vector2.one),
            new TileModel(2, CardType.Anchor, new Vector2(0, 0), 1, Vector2.one)
        };

        var overlapMap = new Dictionary<int, List<int>>
        {
            { 1, new List<int> { 2 } }
        };

        _boardModel.Initialize(tiles, overlapMap);

        // Act
        _boardModel.RemoveTile(2);

        // Assert
        Assert.AreEqual(CardState.InStack, tiles[1].State);
        Assert.IsTrue(tiles[0].IsSelectable, "Tile bên dưới phải trở thành selectable khi tile bên trên bị xóa");
        Assert.AreEqual(1, _boardModel.RemainingTiles);
    }

    [Test]
    public void UpdateSelectableStatus_RaisesBoardModelUpdatedEvent()
    {
        // Arrange
        var tiles = new List<TileModel>
        {
            new TileModel(1, CardType.Sword, new Vector2(0, 0), 0, Vector2.one)
        };
        _boardModel.Initialize(tiles, new Dictionary<int, List<int>>());

        int remainingTiles = -1;
        int selectableTiles = -1;
        bool eventRaised = false;

        var binding = new EventBinding<BoardModelUpdatedEvent>(e => 
        {
            eventRaised = true;
            remainingTiles = e.RemainingTiles;
            selectableTiles = e.SelectableTiles;
        });

        EventBus<BoardModelUpdatedEvent>.Register(binding);

        // Act
        _boardModel.UpdateSelectableStatus();

        // Assert
        Assert.IsTrue(eventRaised, "EventBus phải được raise");
        Assert.AreEqual(1, remainingTiles);
        Assert.AreEqual(1, selectableTiles);

        EventBus<BoardModelUpdatedEvent>.Deregister(binding);
    }

    [Test]
    public void RemoveTile_ChangesStateToInStack_AndUpdatesSelectable()
    {
        // Arrange
        var tiles = new List<TileModel>
        {
            new TileModel(1, CardType.Sword, new Vector2(0, 0), 0, Vector2.one)
        };
        _boardModel.Initialize(tiles, new Dictionary<int, List<int>>());

        // Act
        _boardModel.RemoveTile(1);

        // Assert
        Assert.AreEqual(CardState.InStack, tiles[0].State);
        Assert.IsTrue(_boardModel.IsCleared);
        Assert.AreEqual(0, _boardModel.RemainingTiles);
    }

    [Test]
    public void ShuffleTileTypes_ChangesTypesButMaintainsTotalCounts()
    {
        // Arrange
        var tiles = new List<TileModel>
        {
            new TileModel(1, CardType.Sword, new Vector2(0, 0), 0, Vector2.one),
            new TileModel(2, CardType.Anchor, new Vector2(1, 0), 0, Vector2.one),
            new TileModel(3, CardType.Skull, new Vector2(2, 0), 0, Vector2.one)
        };
        _boardModel.Initialize(tiles, new Dictionary<int, List<int>>());

        // Act
        _boardModel.ShuffleTileTypes();
        var currentTypes = _boardModel.Tiles.Select(t => t.TileType).ToList();

        // Assert
        Assert.AreEqual(3, currentTypes.Count);
        Assert.IsTrue(currentTypes.Contains(CardType.Sword));
        Assert.IsTrue(currentTypes.Contains(CardType.Anchor));
        Assert.IsTrue(currentTypes.Contains(CardType.Skull));
    }

    [Test]
    public void ShuffleTileTypes_IgnoresTilesNotInBoard()
    {
        // Arrange
        var tiles = new List<TileModel>
        {
            new TileModel(1, CardType.Sword, new Vector2(0, 0), 0, Vector2.one),
            new TileModel(2, CardType.Anchor, new Vector2(1, 0), 0, Vector2.one)
        };
        _boardModel.Initialize(tiles, new Dictionary<int, List<int>>());

        // Tile 1 bị gỡ khỏi bàn
        _boardModel.RemoveTile(1);

        // Act
        _boardModel.ShuffleTileTypes();

        // Assert
        // Tile 1 không được shuffle
        Assert.AreEqual(CardType.Sword, tiles[0].TileType);
        // Tile 2 là tile duy nhất còn trên bàn nên không có gì để shuffle đổi chỗ
        Assert.AreEqual(CardType.Anchor, tiles[1].TileType); 
    }

    [Test]
    public void GetTilesByType_ReturnsCorrectTiles()
    {
        // Arrange
        var tiles = new List<TileModel>
        {
            new TileModel(1, CardType.Sword, new Vector2(0, 0), 0, Vector2.one),
            new TileModel(2, CardType.Sword, new Vector2(1, 0), 0, Vector2.one),
            new TileModel(3, CardType.Anchor, new Vector2(2, 0), 0, Vector2.one)
        };
        _boardModel.Initialize(tiles, new Dictionary<int, List<int>>());

        // Act
        var swordTiles = _boardModel.GetTilesByType(CardType.Sword);
        var anchorTiles = _boardModel.GetTilesByType(CardType.Anchor);
        var skullTiles = _boardModel.GetTilesByType(CardType.Skull);

        // Assert
        Assert.AreEqual(2, swordTiles.Count);
        Assert.AreEqual(1, anchorTiles.Count);
        Assert.AreEqual(0, skullTiles.Count);
    }
}
