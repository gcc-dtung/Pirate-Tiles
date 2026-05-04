using UnityEngine;
using PrimeTween;
using System;

[RequireComponent(typeof(Collider2D))]
public class CardView : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _iconRenderer;
    [SerializeField] private SpriteRenderer _backgroundRenderer;
    [SerializeField] private CardVFXView _vfxView;

    public int TileId { get; private set; }
    public CardType Type { get; private set; }

    public event Action<CardView> OnClicked;

    private void Awake()
    {
        if (_vfxView == null)
        {
            _vfxView = GetComponent<CardVFXView>();
        }
    }

    public void Initialize(int tileId, CardType type, Sprite iconSprite)
    {
        TileId = tileId;
        Type = type;
        if (_iconRenderer != null)
        {
            _iconRenderer.sprite = iconSprite;
        }
        
        // Reset state visual
        SetSelectable(false);
        if (_vfxView != null)
        {
            _vfxView.SetDissolve(0f);
        }
    }

    private void OnMouseDown()
    {
        OnClicked?.Invoke(this);
    }

    public void SetSelectable(bool isSelectable)
    {
        if (_vfxView != null)
        {
            _vfxView.SetBrightness(isSelectable ? 1f : 0.5f);
        }
        
        var col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = isSelectable;
        }
    }

    public void SetOrderInLayer(int layerIndex)
    {
        int baseOrder = layerIndex * 10;
        if (_backgroundRenderer != null) _backgroundRenderer.sortingOrder = baseOrder;
        if (_iconRenderer != null) _iconRenderer.sortingOrder = baseOrder + 1;
        
        var vfxRenderer = _vfxView != null ? _vfxView.GetComponent<SpriteRenderer>() : null;
        if (vfxRenderer != null && vfxRenderer != _backgroundRenderer && vfxRenderer != _iconRenderer)
        {
            vfxRenderer.sortingOrder = baseOrder + 2;
        }
    }

    /// <summary>
    /// Scale toàn bộ CardView (visual + collider) theo kích thước tile.
    /// VD: size=(2,1) và spacing=1.0 → scale=(2, 1, 1).
    /// </summary>
    /// <param name="tileSize">Kích thước tile theo grid unit, VD: (1,1), (2,1)</param>
    /// <param name="gridSpacing">Khoảng cách giữa 2 tile (= _spacingX của BoardView)</param>
    public void SetSize(Vector2 tileSize, float gridSpacing)
    {
        transform.localScale = new Vector3(tileSize.x * gridSpacing, tileSize.y * gridSpacing, 1f);
    }

    public Sequence AnimateMoveToStack(Vector3 targetPosition, float duration = 0.3f)
    {
        Tween.StopAll(transform);
        return Sequence.Create()
            .Group(Tween.Position(transform, targetPosition, duration, Ease.OutQuad))
            .Group(Tween.Scale(transform, Vector3.one * 0.8f, duration, Ease.OutQuad));
    }

    public Tween AnimateMoveToPosition(Vector3 targetPosition, float duration = 0.3f)
    {
        Tween.StopAll(transform);
        return Tween.Position(transform, targetPosition, duration, Ease.OutQuad);
    }

    public Sequence AnimateUndoMove(Vector3 targetPosition, float duration = 0.3f)
    {
        Tween.StopAll(transform);
        return Sequence.Create()
            .Group(Tween.Position(transform, targetPosition, duration, Ease.OutCubic))
            .Group(Tween.Scale(transform, Vector3.one, duration, Ease.OutCubic));
    }

    public Sequence AnimateFadeOut(float duration = 0.3f)
    {
        Tween.StopAll(transform);
        var seq = Sequence.Create()
            .Group(Tween.Scale(transform, Vector3.one * 1.2f, duration, Ease.OutQuad));
            
        if (_vfxView != null)
        {
            seq.Group(_vfxView.TweenDissolve(1f, duration));
        }
        
        return seq;
    }

    public Sequence AnimateCollectSpecial(float duration = 0.5f)
    {
        Tween.StopAll(transform);
        return Sequence.Create()
            .Group(Tween.Scale(transform, Vector3.one * 1.5f, duration / 2, Ease.OutBack))
            .Chain(Tween.Scale(transform, Vector3.zero, duration / 2, Ease.InBack));
    }
}
