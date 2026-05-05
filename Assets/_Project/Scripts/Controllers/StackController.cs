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

    private StackModel _stackModel;
    private List<CardView> _cardsInStack = new List<CardView>();
    private Sequence _arrangeSeq; // Track để Complete trước khi tạo mới

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
    }

    private void OnDisable()
    {
        if (_tileSelectedChannel != null)
        {
            _tileSelectedChannel.RemoveListener(OnTileSelected);
        }
    }

    private void OnTileSelected(TileSelectedEventData data)
    {
        if (_stackModel == null || _stackModel.IsFull) return;

        CardView cardView = data.CardView;
        if (cardView == null) return;

        TileModel tileModel = new TileModel(data.TileId, data.TileType, new Vector2(data.GridX, data.GridY), data.LayerIndex, Vector2.one);
        
        int insertIndex = _stackModel.GetInsertIndex(data.TileType);
        _stackModel.InsertTile(insertIndex, tileModel);
        _cardsInStack.Insert(insertIndex, cardView);

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
        // Thẻ bài đang bay xuống, và các thẻ cũ trong khay dạt ra
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
}
