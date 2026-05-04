using UnityEngine;
using System.Collections.Generic;

public class LevelController : MonoBehaviour
{
    [SerializeField] private LevelConfigSO _currentLevelConfig;
    [SerializeField] private TileDatabaseSO _tileDatabase;
    
    [Header("Controllers to Initialize")]
    [SerializeField] private BoardController _boardController;
    [SerializeField] private StackController _stackController;
    [SerializeField] private TimerController _timerController;

    private void Start()
    {
        InitializeLevel();
    }

    private void InitializeLevel()
    {
        if (_currentLevelConfig == null) return;

        var levelModel = new LevelModel();
        levelModel.LevelIndex = _currentLevelConfig.LevelIndex;
        levelModel.TimeLimit = _currentLevelConfig.TimeLimit;
        levelModel.CurrentTimeRemaining = _currentLevelConfig.TimeLimit;
        levelModel.MaxStackSize = _currentLevelConfig.MaxStackSize;

        var boardModel = new BoardModel();
        var stackModel = new StackModel(levelModel.MaxStackSize);

        List<TileModel> tiles = new List<TileModel>();
        Dictionary<int, List<int>> overlapMap = new Dictionary<int, List<int>>();
        
        if (_currentLevelConfig.InitialTiles != null && _currentLevelConfig.InitialTiles.Count > 0)
        {
            int idCounter = 1;
            foreach (var tileData in _currentLevelConfig.InitialTiles)
            {
                // Đảm bảo Size tối thiểu (1,1) nếu chưa set trong Inspector
                var size = (tileData.Size.x > 0 && tileData.Size.y > 0) ? tileData.Size : Vector2.one;
                var tile = new TileModel(idCounter++, tileData.Type, tileData.GridPosition, tileData.LayerIndex, size);
                tiles.Add(tile);
            }
            
            for (int i = 0; i < tiles.Count; i++)
            {
                var t1 = tiles[i];
                List<int> covers = new List<int>();
                for (int j = 0; j < tiles.Count; j++)
                {
                    var t2 = tiles[j];
                    if (t1.Id != t2.Id && t2.LayerIndex > t1.LayerIndex)
                    {
                        // AABB overlap: t2 (layer cao hơn) có đè lên t1 không?
                        // Hai hình chữ nhật overlap khi khoảng cách tâm < tổng nửa kích thước mỗi chiều
                        float overlapX = (t1.Size.x + t2.Size.x) / 2f;
                        float overlapY = (t1.Size.y + t2.Size.y) / 2f;
                        bool overlaps = Mathf.Abs(t1.GridPosition.x - t2.GridPosition.x) < overlapX
                                     && Mathf.Abs(t1.GridPosition.y - t2.GridPosition.y) < overlapY;
                        if (overlaps)
                        {
                            covers.Add(t2.Id);
                        }
                    }
                }
                overlapMap[t1.Id] = covers;
            }
        }
        else 
        {
            int id = 1;
            tiles.Add(new TileModel(id++, CardType.Sword, new Vector2(0, 0), 0, Vector2.one));
            tiles.Add(new TileModel(id++, CardType.Anchor, new Vector2(1, 0), 0, Vector2.one));
            tiles.Add(new TileModel(id++, CardType.Skull, new Vector2(2, 0), 0, Vector2.one));
        }

        boardModel.Initialize(tiles, overlapMap);

        if (_boardController != null) _boardController.Initialize(boardModel, _tileDatabase);
        if (_stackController != null) _stackController.Initialize(stackModel);
        if (_timerController != null) _timerController.Initialize(levelModel);
    }
}
