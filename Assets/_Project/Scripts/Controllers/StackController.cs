using UnityEngine;
using System.Collections.Generic;
using PrimeTween;

public class StackController : MonoBehaviour
{
    [SerializeField] private StackView _stackView;
    
    [Header("Event Channels")]
    [SerializeField] private TileSelectedChannelSO _tileSelectedChannel;
    [SerializeField] private VoidEventChannelSO _tilesMatchedChannel;
    [SerializeField] private VoidEventChannelSO _stackFullChannel;
    [SerializeField] private VoidEventChannelSO _undoRequestChannel;
    [SerializeField] private VoidEventChannelSO _addOneCellRequestChannel;

    private StackModel _stackModel;
    private List<CardView> _cardsInStack = new List<CardView>();
    private Sequence _arrangeSeq; // Track để Complete trước khi tạo mới
    private bool _hasUsedAddOneCell = false;
    private EventBinding<MagicRemoveFromStackEvent> _magicRemoveBinding;

    public void Initialize(StackModel stackModel)
    {
        _stackModel = stackModel;
    }

    public bool IsStackEmpty => _stackModel != null && _stackModel.Count == 0;

    private void OnEnable()
    {
        if (_tileSelectedChannel != null)
        {
            _tileSelectedChannel.AddListener(OnTileSelected);
        }
        if (_undoRequestChannel != null) _undoRequestChannel.AddListener(OnUndoRequest);
        if (_addOneCellRequestChannel != null) _addOneCellRequestChannel.AddListener(OnAddOneCellRequest);

        _magicRemoveBinding = new EventBinding<MagicRemoveFromStackEvent>(OnMagicRemoveFromStack);
        EventBus<MagicRemoveFromStackEvent>.Register(_magicRemoveBinding);
    }

    private void OnDisable()
    {
        if (_tileSelectedChannel != null)
        {
            _tileSelectedChannel.RemoveListener(OnTileSelected);
        }
        if (_undoRequestChannel != null) _undoRequestChannel.RemoveListener(OnUndoRequest);
        if (_addOneCellRequestChannel != null) _addOneCellRequestChannel.RemoveListener(OnAddOneCellRequest);

        EventBus<MagicRemoveFromStackEvent>.Deregister(_magicRemoveBinding);
    }

    private void OnTileSelected(TileSelectedEventData data)
    {
        if (_stackModel == null) return;
        
        // Cho phép nhận thêm tối đa 1 tile khi đã đầy (overflow) để tránh hiện tượng tile bị kẹt trên bàn (ghost tile)
        // và để người chơi thấy rõ tile gây thua cuộc di chuyển vào khay.
        if (_stackModel.Count > _stackModel.MaxSize) return;

        CardView cardView = data.CardView;
        if (cardView == null) return;

        TileModel tileModel = new TileModel(data.TileId, data.TileType, new Vector2(data.GridX, data.GridY), data.LayerIndex, Vector2.one);
        
        int insertIndex = _stackModel.GetInsertIndex(data.TileType);
        _stackModel.InsertTile(insertIndex, tileModel);
        _cardsInStack.Insert(insertIndex, cardView);
        
        EventBus<StackUpdatedEvent>.Raise(new StackUpdatedEvent { Count = _cardsInStack.Count });

        cardView.SetOrderInLayer(100 + insertIndex);

        // Fix other cards' order in layer
        for (int i = 0; i < _cardsInStack.Count; i++)
        {
            if (_cardsInStack[i] != cardView)
            {
                _cardsInStack[i].SetOrderInLayer(100 + i);
            }
        }

        ProcessStackAnimationAsync(cardView, insertIndex);
    }

    private async void ProcessStackAnimationAsync(CardView movedCard, int insertIndex)
    {
        RunArrangeAnimation();

        Vector3 targetPos = _stackView.GetSlotPosition(insertIndex);
        await movedCard.AnimateMoveToStack(targetPos, 0.3f);

        int matchIdx = _stackModel.FindMatch();
        if (matchIdx >= 0)
        {
            await HandleMatchAsync(matchIdx);
        }
        else
        {
            if (_stackModel.IsFull && _stackFullChannel != null)
            {
                _ = _stackView.AnimateShakeFull();
                _stackFullChannel.EventRaise();
            }
        }
    }

    private async System.Threading.Tasks.Task HandleMatchAsync(int startIndex)
    {
        var matchedCards = new List<CardView>();
        for (int i = 0; i < 3; i++)
        {
            matchedCards.Add(_cardsInStack[startIndex + i]);
        }

        _cardsInStack.RemoveRange(startIndex, 3);
        _stackModel.RemoveTiles(startIndex, 3);
        
        EventBus<StackUpdatedEvent>.Raise(new StackUpdatedEvent { Count = _cardsInStack.Count });

        var seq = Sequence.Create();
        foreach (var card in matchedCards)
        {
            _ = seq.Group(card.AnimateFadeOut(0.3f));
        }

        await seq;

        foreach (var card in matchedCards)
        {
            Destroy(card.gameObject);
        }

        if (_tilesMatchedChannel != null)
        {
            _tilesMatchedChannel.EventRaise();
        }

        RunArrangeAnimation();
    }

    /// <summary>
    /// Complete sequence sắp xếp cũ (jump cards về đúng vị trí ngay lập tức)
    /// rồi tạo sequence mới. Tránh tween trên card đã bị Destroy.
    /// </summary>
    private void RunArrangeAnimation()
    {
        if (_arrangeSeq.isAlive) _arrangeSeq.Complete();
        _arrangeSeq = _stackView.AnimateArrange(_cardsInStack);
    }

    private void OnUndoRequest()
    {
        if (_cardsInStack.Count == 0) return;

        var lastTile = _stackModel.RemoveLastTile();
        if (lastTile != null)
        {
            var cardView = _cardsInStack[_cardsInStack.Count - 1];
            _cardsInStack.RemoveAt(_cardsInStack.Count - 1);
            
            EventBus<StackUpdatedEvent>.Raise(new StackUpdatedEvent { Count = _cardsInStack.Count });

            EventBus<UndoPerformedEvent>.Raise(new UndoPerformedEvent
            {
                TileId = lastTile.Id,
                CardView = cardView
            });

            RunArrangeAnimation();
        }
    }

    private void OnAddOneCellRequest()
    {
        if (_hasUsedAddOneCell) return;

        _hasUsedAddOneCell = true;
        _stackModel.ExpandSize(1);
        _stackView.UnlockExtraSlot();

        EventBus<AddOneCellUsedEvent>.Raise(new AddOneCellUsedEvent());
    }

    private void OnMagicRemoveFromStack(MagicRemoveFromStackEvent e)
    {
        bool changed = false;
        
        for (int i = _cardsInStack.Count - 1; i >= 0; i--)
        {
            var cardView = _cardsInStack[i];
            if (e.TileIds.Contains(cardView.TileId))
            {
                _stackModel.RemoveTiles(i, 1);
                _cardsInStack.RemoveAt(i);
                
                cardView.AnimateCollectSpecial().OnComplete(() => {
                    if (cardView != null && cardView.gameObject != null)
                        Destroy(cardView.gameObject);
                });
                changed = true;
            }
        }

        if (changed)
        {
            EventBus<StackUpdatedEvent>.Raise(new StackUpdatedEvent { Count = _cardsInStack.Count });
            RunArrangeAnimation();
        }
    }
}
