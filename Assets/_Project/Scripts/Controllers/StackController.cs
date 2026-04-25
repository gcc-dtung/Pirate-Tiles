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

    public void Initialize(StackModel stackModel)
    {
        _stackModel = stackModel;
    }

    private void OnEnable()
    {
        if (_tileSelectedChannel != null) _tileSelectedChannel.Subscribe(OnTileSelected);
    }

    private void OnDisable()
    {
        if (_tileSelectedChannel != null) _tileSelectedChannel.Unsubscribe(OnTileSelected);
    }

    private void OnTileSelected(TileSelectedEventData data)
    {
        if (_stackModel == null || _stackModel.IsFull) return;

        CardView cardView = data.CardView;
        if (cardView == null) return;

        TileModel tileModel = new TileModel(data.TileId, data.TileType, new Vector2Int(data.GridX, data.GridY), data.LayerIndex);
        
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
        _stackView.AnimateArrange(_cardsInStack);

        Vector3 targetPos = _stackView.GetSlotPosition(insertIndex);
        await movedCard.AnimateMoveToStack(targetPos, 0.3f).ToYieldInstruction();

        int matchIdx = _stackModel.FindMatch();
        if (matchIdx >= 0)
        {
            await HandleMatchAsync(matchIdx);
        }
        else
        {
            if (_stackModel.IsFull && _stackFullChannel != null)
            {
                _stackView.AnimateShakeFull();
                _stackFullChannel.RaiseEvent();
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
            seq.Group(card.AnimateFadeOut(0.3f));
        }

        await seq.ToYieldInstruction();

        foreach (var card in matchedCards)
        {
            Destroy(card.gameObject);
        }

        if (_tilesMatchedChannel != null)
        {
            _tilesMatchedChannel.RaiseEvent();
        }

        _stackView.AnimateArrange(_cardsInStack);
    }
}
