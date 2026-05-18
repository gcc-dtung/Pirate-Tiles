using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class LevelController : MonoBehaviour
{
    [SerializeField] private LevelConfigSO _currentLevelConfig;
    [SerializeField] private GameConfigSO _gameConfig;
    [SerializeField] private TileDatabaseSO _tileDatabase;
    
    [Header("Controllers to Initialize")]
    [SerializeField] private BoardController _boardController;
    [SerializeField] private StackController _stackController;

    private void Start()
    {
        InitializeLevel();
    }

    private void InitializeLevel()
    {
        if (_gameConfig != null)
        {
            int chapterIdx = SaveService.Instance != null ? SaveService.Instance.GetInt(SaveKeys.SelectedChapterIndex, 0) : 0;
            int levelIdx = SaveService.Instance != null ? SaveService.Instance.GetInt(SaveKeys.SelectedLevelIndex, 1) : 1;

            if (chapterIdx >= 0 && chapterIdx < _gameConfig.Chapters.Count)
            {
                var chapter = _gameConfig.Chapters[chapterIdx];
                // Tìm trong chapter hiện tại trước
                _currentLevelConfig = chapter.LevelNodes
                    .Select(n => n.LevelConfig)
                    .FirstOrDefault(c => c != null && c.LevelIndex == levelIdx);
            }

            // Nếu không tìm thấy trong chapter hiện tại, tìm trong toàn bộ các chapter khác
            if (_currentLevelConfig == null)
            {
                for (int i = 0; i < _gameConfig.Chapters.Count; i++)
                {
                    var chapter = _gameConfig.Chapters[i];
                    var config = chapter.LevelNodes
                        .Select(n => n.LevelConfig)
                        .FirstOrDefault(c => c != null && c.LevelIndex == levelIdx);
                    
                    if (config != null)
                    {
                        _currentLevelConfig = config;
                        // Cập nhật lại ChapterIndex để đồng bộ dữ liệu
                        SaveService.Instance?.SetInt(SaveKeys.SelectedChapterIndex, i);
                        break;
                    }
                }
            }
        }

        if (_currentLevelConfig == null) return;

        var levelModel = new LevelModel();
        levelModel.LevelIndex = _currentLevelConfig.LevelIndex;
        levelModel.TimeLimit = _currentLevelConfig.TimeLimit;
        levelModel.CurrentTimeRemaining = _currentLevelConfig.TimeLimit;
        levelModel.MaxStackSize = _currentLevelConfig.MaxStackSize;

        var boardModel = new BoardModel();
        var stackModel = new StackModel(levelModel.MaxStackSize);

        List<TileModel> tiles = new List<TileModel>();
        // Overlap map sẽ được tính chính xác trong BoardController 
        // sau khi spawn CardView, dựa trên SpriteRenderer.bounds thực tế
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
        }
        else 
        {
            int id = 1;
            tiles.Add(new TileModel(id++, CardType.Sword, new Vector2(0, 0), 0, Vector2.one));
            tiles.Add(new TileModel(id++, CardType.Anchor, new Vector2(1, 0), 0, Vector2.one));
            tiles.Add(new TileModel(id++, CardType.Skull, new Vector2(2, 0), 0, Vector2.one));
        }

        boardModel.Initialize(tiles, overlapMap);

        if (_boardController != null) _boardController.Initialize(boardModel, stackModel, _tileDatabase);
        if (_stackController != null) _stackController.Initialize(stackModel);
    }
}
