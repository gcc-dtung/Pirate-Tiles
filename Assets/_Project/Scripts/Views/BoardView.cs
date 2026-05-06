using System.Collections.Generic;
using UnityEngine;
using PrimeTween;

public class BoardView : MonoBehaviour
{
    [SerializeField] private CardView _cardPrefab;
    [SerializeField] private Transform _boardContainer;
    
    // Configurable offset for tiles
    [SerializeField] private float _spacingX = 1.0f;
    [SerializeField] private float _spacingY = -1.0f; 

    private Dictionary<int, CardView> _cardViews = new Dictionary<int, CardView>();

    // 4.8 SpawnCards
    public void SpawnCards(IEnumerable<TileModel> tiles, TileDatabaseSO database)
    {
        foreach (var tile in tiles)
        {
            if (_cardViews.ContainsKey(tile.Id)) continue;

            var cardObj = Instantiate(_cardPrefab, _boardContainer);
            
            Vector3 targetPos = GetWorldPosition(tile.GridPosition);
            cardObj.transform.localPosition = targetPos;
            cardObj.SetOrderInLayer(tile.LayerIndex);
            cardObj.SetSize(tile.Size, _spacingX); // Scale visual theo kích thước tile

            Sprite icon = database.GetSprite(tile.TileType);
            cardObj.Initialize(tile.Id, tile.TileType, icon);
            cardObj.SetSelectable(tile.IsSelectable);

            _cardViews.Add(tile.Id, cardObj);
        }
    }

    // 4.8 DespawnCard
    public void DespawnCard(int tileId)
    {
        if (_cardViews.TryGetValue(tileId, out var view))
        {
            Destroy(view.gameObject);
            _cardViews.Remove(tileId);
        }
    }
    
    public CardView GetCardView(int tileId)
    {
        _cardViews.TryGetValue(tileId, out var view);
        return view;
    }

    public void SyncSelectable(IEnumerable<TileModel> tiles)
    {
        foreach (var tile in tiles)
        {
            if (tile.State != CardState.InBoard) continue;

            if (_cardViews.TryGetValue(tile.Id, out var view))
            {
                if (view != null)
                {
                    view.SetSelectable(tile.IsSelectable);
                }
            }
        }
    }

    // 4.9 Shuffle animations
    public Sequence AnimateShuffleGather(IEnumerable<TileModel> tilesToShuffle, float duration = 0.5f)
    {
        var seq = Sequence.Create();
        Vector3 centerPos = Vector3.zero;

        foreach (var tile in tilesToShuffle)
        {
            if (_cardViews.TryGetValue(tile.Id, out var view))
            {
                seq.Group(Tween.LocalPosition(view.transform, centerPos, duration, Ease.InBack));
                seq.Group(Tween.LocalRotation(view.transform, Quaternion.Euler(0, 0, 180), duration, Ease.Linear));
            }
        }

        return seq;
    }
    
    public Sequence AnimateShuffleSpread(IEnumerable<TileModel> tilesToShuffle, TileDatabaseSO database, float duration = 0.5f)
    {
        var seq = Sequence.Create();

        foreach (var tile in tilesToShuffle)
        {
            if (_cardViews.TryGetValue(tile.Id, out var view))
            {
                // Cập nhật lại hình ảnh sau khi Model đã shuffle (đổi type)
                Sprite icon = database.GetSprite(tile.TileType);
                view.Initialize(tile.Id, tile.TileType, icon);
                view.SetSelectable(tile.IsSelectable);
                view.SetOrderInLayer(tile.LayerIndex);

                Vector3 targetPos = GetWorldPosition(tile.GridPosition);
                
                seq.Group(Tween.LocalPosition(view.transform, targetPos, duration, Ease.OutBack));
                seq.Group(Tween.LocalRotation(view.transform, Quaternion.identity, duration, Ease.OutBack));
            }
        }
        return seq;
    }

    public Vector3 GetWorldPosition(Vector2 gridPosition)
    {
        return new Vector3(gridPosition.x * _spacingX, gridPosition.y * _spacingY, 0);
    }
}
