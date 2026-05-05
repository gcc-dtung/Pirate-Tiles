using UnityEngine;
using UnityEngine.EventSystems;
using PrimeTween;
using System;

[RequireComponent(typeof(Collider2D))]
public class CardView : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private SpriteRenderer _iconRenderer;
    [SerializeField] private SpriteRenderer _backgroundRenderer;
    [SerializeField] private CardVFXView _vfxView;

    public int TileId { get; private set; }
    public CardType Type { get; private set; }

    private Color _baseIconColor = Color.white;
    private Color _baseBgColor = Color.white;

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

        if (iconSprite != null)
        {
            // Có art thật → hiện icon, background về trắng
            if (_iconRenderer != null)
            {
                _iconRenderer.sprite = iconSprite;
                _baseIconColor = Color.white;
                _iconRenderer.color = _baseIconColor;
            }
            if (_backgroundRenderer != null)
            {
                _baseBgColor = Color.white;
                _backgroundRenderer.color = _baseBgColor;
            }
        }
        else
        {
            // Chưa có art → tô màu placeholder lên background (vì iconRenderer vô hình khi sprite null)
            if (_iconRenderer != null)
            {
                _iconRenderer.sprite = null;
                _baseIconColor = Color.white;
            }
            if (_backgroundRenderer != null)
            {
                _baseBgColor = GetPlaceholderColor(type);
                _backgroundRenderer.color = _baseBgColor;
            }
        }

        // Reset state visual
        SetSelectable(false);
        if (_vfxView != null)
        {
            _vfxView.SetDissolve(0f);
        }

    }

    /// <summary>Màu placeholder để phân biệt CardType khi chưa có art.</summary>
    private static Color GetPlaceholderColor(CardType type) => type switch
    {
        CardType.Sword    => new Color(0.85f, 0.20f, 0.20f), // đỏ
        CardType.Anchor   => new Color(0.20f, 0.45f, 0.85f), // xanh dương
        CardType.Skull    => new Color(0.15f, 0.15f, 0.15f), // đen
        CardType.Compass  => new Color(0.20f, 0.75f, 0.50f), // xanh lá
        CardType.Rum      => new Color(0.80f, 0.40f, 0.10f), // cam
        CardType.Cannon   => new Color(0.50f, 0.50f, 0.50f), // xám
        CardType.Hook     => new Color(0.60f, 0.30f, 0.70f), // tím
        CardType.Treasure => new Color(1.00f, 0.85f, 0.10f), // vàng
        CardType.Map      => new Color(0.70f, 0.55f, 0.30f), // nâu
        CardType.Wheel    => new Color(0.30f, 0.70f, 0.75f), // cyan
        CardType.Hat      => new Color(0.85f, 0.60f, 0.75f), // hồng
        CardType.Pistol   => new Color(0.40f, 0.25f, 0.15f), // nâu đậm
        CardType.Coin     => new Color(0.95f, 0.75f, 0.20f), // vàng nhạt
        CardType.Bomb     => new Color(0.10f, 0.10f, 0.10f), // đen đậm
        CardType.Lightning=> new Color(0.95f, 0.95f, 0.20f), // vàng sáng
        CardType.Ice      => new Color(0.70f, 0.90f, 1.00f), // xanh nhạt
        CardType.Star     => new Color(1.00f, 0.70f, 0.20f), // cam sáng
        _                 => Color.white,
    };

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"[CardView] Clicked: TileId={TileId}, Type={Type}");
        OnClicked?.Invoke(this);
    }

    public void SetSelectable(bool isSelectable, bool dimWhenNotSelectable = true)
    {
        bool shouldDim = !isSelectable && dimWhenNotSelectable;

        if (_vfxView != null)
        {
            _vfxView.SetBrightness(shouldDim ? 0.5f : 1f);
        }
        
        var col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = isSelectable;
        }

        float multiplier = shouldDim ? 0.6f : 1f;
        if (_iconRenderer != null)
        {
            _iconRenderer.color = new Color(_baseIconColor.r * multiplier, _baseIconColor.g * multiplier, _baseIconColor.b * multiplier, _baseIconColor.a);
        }
        if (_backgroundRenderer != null)
        {
            _backgroundRenderer.color = new Color(_baseBgColor.r * multiplier, _baseBgColor.g * multiplier, _baseBgColor.b * multiplier, _baseBgColor.a);
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
        return Sequence.Create()
            .Group(Tween.Position(transform, targetPosition, duration, Ease.OutQuad))
            .Group(Tween.Scale(transform, Vector3.one * 0.8f, duration, Ease.OutQuad));
    }

    public Tween AnimateMoveToPosition(Vector3 targetPosition, float duration = 0.3f)
    {
        return Tween.Position(transform, targetPosition, duration, Ease.OutQuad);
    }

    public Sequence AnimateUndoMove(Vector3 targetPosition, float duration = 0.3f)
    {
        return Sequence.Create()
            .Group(Tween.Position(transform, targetPosition, duration, Ease.OutCubic))
            .Group(Tween.Scale(transform, Vector3.one, duration, Ease.OutCubic));
    }

    public Sequence AnimateFadeOut(float duration = 0.3f)
    {
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
        return Sequence.Create()
            .Group(Tween.Scale(transform, Vector3.one * 1.5f, duration / 2, Ease.OutBack))
            .Chain(Tween.Scale(transform, Vector3.zero, duration / 2, Ease.InBack));
    }
}
