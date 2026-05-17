using UnityEngine;
using System.Linq;
using PrimeTween;

public class BoardController : MonoBehaviour
{
    [SerializeField] private BoardView _boardView;
    [SerializeField] private TileSelectedChannelSO _tileSelectedChannel;
    [SerializeField] private VoidEventChannelSO _boardClearedChannel;
    [SerializeField] private VoidEventChannelSO _undoRequestChannel;
    [SerializeField] private VoidEventChannelSO _shuffleRequestChannel;
    [SerializeField] private VoidEventChannelSO _magicRequestChannel;
    [SerializeField] private VoidEventChannelSO _tilesMatchedChannel;

    private BoardModel _boardModel;
    private StackModel _stackModel;
    private TileDatabaseSO _tileDatabase;
    private GamePhase _currentPhase = GamePhase.None;

    private EventBinding<BoardModelUpdatedEvent> _boardModelUpdatedBinding;
    private EventBinding<UndoPerformedEvent> _undoBinding;
    private EventBinding<GamePhaseChangedEvent> _phaseChangedBinding;

    public void Initialize(BoardModel boardModel, StackModel stackModel, TileDatabaseSO database)
    {
        _boardModel = boardModel;
        _stackModel = stackModel;
        _tileDatabase = database;
        
        // 1. Spawn cards trước để có SpriteRenderer thực tế
        _boardView.SpawnCards(_boardModel.Tiles, _tileDatabase);
        
        // 2. Tính overlap map dựa trên bounds thực tế của SpriteRenderer
        var overlapMap = _boardView.BuildOverlapMapFromBounds(_boardModel.Tiles);
        
        // 3. Re-initialize BoardModel với overlap map chính xác
        _boardModel.Initialize(new System.Collections.Generic.List<TileModel>(_boardModel.Tiles), overlapMap);
        
        // 4. Sync lại visual selectable sau khi đã có overlap map đúng
        _boardView.SyncSelectable(_boardModel.Tiles);
        
        foreach (var tile in _boardModel.Tiles)
        {
            var view = _boardView.GetCardView(tile.Id);
            if (view != null)
            {
                view.OnClicked += HandleCardClicked;
            }
        }
    }

    private void OnEnable()
    {
        _boardModelUpdatedBinding = new EventBinding<BoardModelUpdatedEvent>(OnBoardUpdated);
        EventBus<BoardModelUpdatedEvent>.Register(_boardModelUpdatedBinding);

        _undoBinding = new EventBinding<UndoPerformedEvent>(OnUndoPerformed);
        EventBus<UndoPerformedEvent>.Register(_undoBinding);

        if (_undoRequestChannel != null) _undoRequestChannel.AddListener(OnUndoRequest);
        if (_shuffleRequestChannel != null) _shuffleRequestChannel.AddListener(OnShuffleRequest);
        if (_magicRequestChannel != null) _magicRequestChannel.AddListener(OnMagicRequest);

        _phaseChangedBinding = new EventBinding<GamePhaseChangedEvent>(OnPhaseChanged);
        EventBus<GamePhaseChangedEvent>.Register(_phaseChangedBinding);
    }

    private void OnDisable()
    {
        EventBus<BoardModelUpdatedEvent>.Deregister(_boardModelUpdatedBinding);
        EventBus<UndoPerformedEvent>.Deregister(_undoBinding);
        
        if (_undoRequestChannel != null) _undoRequestChannel.RemoveListener(OnUndoRequest);
        if (_shuffleRequestChannel != null) _shuffleRequestChannel.RemoveListener(OnShuffleRequest);
        if (_magicRequestChannel != null) _magicRequestChannel.RemoveListener(OnMagicRequest);

        EventBus<GamePhaseChangedEvent>.Deregister(_phaseChangedBinding);
        if (_boardModel != null)
        {
            foreach (var tile in _boardModel.Tiles)
            {
                var view = _boardView.GetCardView(tile.Id);
                if (view != null)
                {
                    view.OnClicked -= HandleCardClicked;
                }
            }
        }
    }

    private void HandleCardClicked(CardView cardView)
    {
        if (cardView == null) return;
        if (_currentPhase != GamePhase.Playing) return;
        
        // Nếu stack đã quá giới hạn (đang chờ xử lý game over), không cho click thêm nữa
        if (_stackModel != null && _stackModel.Count > _stackModel.MaxSize) return;
        
        cardView.SetSelectable(false, false);
        cardView.OnClicked -= HandleCardClicked;

        int tileId = cardView.TileId;
        _boardModel.RemoveTile(tileId);

        _tileSelectedChannel.EventRaise(new TileSelectedEventData
        {
            TileId = tileId,
            TileType = cardView.Type,
            CardView = cardView
        });

        if (_boardModel.IsCleared && _boardClearedChannel != null)
        {
            _boardClearedChannel.EventRaise();
        }
    }

    private void OnPhaseChanged(GamePhaseChangedEvent e)
    {
        _currentPhase = e.NewPhase;
    }

    private void OnBoardUpdated(BoardModelUpdatedEvent e)
    {
        if (_boardModel != null && _boardView != null)
        {
            _boardView.SyncSelectable(_boardModel.Tiles);
        }
    }

    private void OnUndoRequest()
    {
        // StackController handles this and raises UndoPerformedEvent
    }

    private void OnUndoPerformed(UndoPerformedEvent e)
    {
        _boardModel.RestoreTile(e.TileId);
        var tile = _boardModel.Tiles.FirstOrDefault(t => t.Id == e.TileId);
        if (tile != null)
        {
            Vector3 originalPos = _boardView.GetWorldPosition(tile.GridPosition);
            e.CardView.AnimateUndoMove(originalPos);
            e.CardView.SetOrderInLayer(tile.LayerIndex);
            e.CardView.SetSelectable(true);
            e.CardView.OnClicked += HandleCardClicked;
        }
    }

    private async void OnShuffleRequest()
    {
        var boardTiles = _boardModel.Tiles.Where(t => t.State == CardState.InBoard).ToList();
        await _boardView.AnimateShuffleGather(boardTiles);
        _boardModel.ShuffleTileTypes();
        await _boardView.AnimateShuffleSpread(boardTiles, _tileDatabase);
    }

    private async void OnMagicRequest()
    {
        var targets = _boardModel.GetBestMagicTargets(3);
        if (targets == null || targets.Count < 3) return;

        var seq = Sequence.Create();
        var stackTileIds = new System.Collections.Generic.List<int>();

        foreach (var tile in targets)
        {
            if (tile.State == CardState.InBoard)
            {
                _boardModel.RemoveTile(tile.Id);
                var cardView = _boardView.GetCardView(tile.Id);
                if (cardView != null)
                {
                    cardView.OnClicked -= HandleCardClicked;
                    _ = seq.Group(cardView.AnimateCollectSpecial());
                }
            }
            else if (tile.State == CardState.InStack)
            {
                stackTileIds.Add(tile.Id);
            }
        }

        if (stackTileIds.Count > 0)
        {
            EventBus<MagicRemoveFromStackEvent>.Raise(new MagicRemoveFromStackEvent { TileIds = stackTileIds });
        }

        await seq;

        foreach (var tile in targets)
        {
            if (!stackTileIds.Contains(tile.Id))
            {
                _boardView.DespawnCard(tile.Id);
            }
        }

        if (_tilesMatchedChannel != null)
        {
            _tilesMatchedChannel.EventRaise();
        }

        if (_boardModel.IsCleared && _boardClearedChannel != null)
        {
            _boardClearedChannel.EventRaise();
        }
    }
}
