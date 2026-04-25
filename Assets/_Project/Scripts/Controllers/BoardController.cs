using UnityEngine;

public class BoardController : MonoBehaviour
{
    [SerializeField] private BoardView _boardView;
    [SerializeField] private TileSelectedChannelSO _tileSelectedChannel;
    [SerializeField] private VoidEventChannelSO _boardClearedChannel;
    [SerializeField] private VoidEventChannelSO _undoRequestChannel;

    private BoardModel _boardModel;
    private EventBinding<BoardModelUpdatedEvent> _boardModelUpdatedBinding;

    public void Initialize(BoardModel boardModel, TileDatabaseSO database)
    {
        _boardModel = boardModel;
        
        _boardView.SpawnCards(_boardModel.Tiles, database);
        
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

    }

    private void OnDisable()
    {
        EventBus<BoardModelUpdatedEvent>.Deregister(_boardModelUpdatedBinding);
        

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
        
        int tileId = cardView.TileId;
        
        _boardModel.RemoveTile(tileId);

        _tileSelectedChannel.EventRaise(new TileSelectedEventData
        {
            TileId = tileId,
            TileType = cardView.Type,
            CardView = cardView
        });

        cardView.SetSelectable(false);
        cardView.OnClicked -= HandleCardClicked;

        if (_boardModel.IsCleared && _boardClearedChannel != null)
        {
            _boardClearedChannel.EventRaise();
        }
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
        // Xử lý khi nhận sự kiện undo. BoardModel sẽ thêm lại thẻ bài từ Stack.
        // Điều này sẽ được gọi từ PowerUpController sau.
    }
}
