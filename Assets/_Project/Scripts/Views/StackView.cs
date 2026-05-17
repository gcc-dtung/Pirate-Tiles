using UnityEngine;
using PrimeTween;
using System.Collections.Generic;

public class StackView : MonoBehaviour
{
    public bool IsExtraSlotUnlocked { get; private set; }

    [System.Serializable]
    public struct StackSlot
    {
        public Transform SlotTransform;        // vị trí slot
        public SpriteRenderer Background;      // sprite nền ô trống
        public SpriteRenderer LockIcon;        // icon khóa (chỉ ô 8 mới cần)
    }

    [Header("Slot Setup")]
    [SerializeField] private StackSlot[] _slots;
    [SerializeField] private Sprite _normalSlotSprite;
    [SerializeField] private Sprite _lockedSlotSprite;
    [SerializeField] private Color _normalSlotColor = Color.white;
    [SerializeField] private Color _lockedSlotColor = Color.gray;

    [SerializeField] private Transform _stackContainer; // Object chứa background của khay, dùng để rung

    public int MaxSlots => _slots != null ? _slots.Length : 0;

    private void Awake()
    {
        InitializeSlots();
    }

    public void InitializeSlots()
    {
        IsExtraSlotUnlocked = false;
        if (_slots == null) return;

        for (int i = 0; i < _slots.Length; i++)
        {
            if (i < 7)
            {
                if (_slots[i].Background != null)
                {
                    if (_normalSlotSprite != null)
                    {
                        _slots[i].Background.sprite = _normalSlotSprite;
                    }
                    _slots[i].Background.color = _normalSlotColor;
                }
                if (_slots[i].LockIcon != null)
                {
                    _slots[i].LockIcon.gameObject.SetActive(false);
                }
            }
            else // Slot 8 (index 7)
            {
                if (_slots[i].Background != null)
                {
                    if (_lockedSlotSprite != null)
                    {
                        _slots[i].Background.sprite = _lockedSlotSprite;
                    }
                    _slots[i].Background.color = _lockedSlotColor;
                }
                if (_slots[i].LockIcon != null)
                {
                    _slots[i].LockIcon.gameObject.SetActive(true);
                }
            }
        }
    }

    public void UnlockExtraSlot()
    {
        if (_slots != null && _slots.Length > 7)
        {
            IsExtraSlotUnlocked = true;
            var extraSlot = _slots[7];
            if (extraSlot.Background != null)
            {
                if (_normalSlotSprite != null)
                {
                    extraSlot.Background.sprite = _normalSlotSprite;
                }
                extraSlot.Background.color = _normalSlotColor;
            }
            if (extraSlot.LockIcon != null)
            {
                extraSlot.LockIcon.gameObject.SetActive(false);
            }
        }
    }

    // 4.10 Setup StackView
    public Vector3 GetSlotPosition(int index)
    {
        if (_slots != null && index >= 0 && index < _slots.Length)
        {
            var slotData = _slots[index];
            Transform slot = slotData.SlotTransform;
            
            // Dùng bounds.center của SpriteRenderer để lấy tâm chính xác (tránh lỗi do Pivot của Sprite)
            Vector3 visualCenter = slot.position;
            if (slotData.Background != null && slotData.Background.sprite != null)
            {
                visualCenter = slotData.Background.bounds.center;
            }

            Canvas canvas = slot.GetComponentInParent<Canvas>();
            if (canvas != null && Camera.main != null)
            {
                // Unity xem ScreenSpaceCamera chưa gán camera giống hệt Overlay
                bool isOverlay = canvas.renderMode == RenderMode.ScreenSpaceOverlay || 
                                (canvas.renderMode == RenderMode.ScreenSpaceCamera && canvas.worldCamera == null);

                if (isOverlay)
                {
                    // Với Overlay, bounds.center trả về tọa độ pixel trên màn hình
                    Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(visualCenter.x, visualCenter.y, Mathf.Abs(Camera.main.transform.position.z)));
                    worldPos.z = 0;
                    return worldPos;
                }
                else
                {
                    // Với WorldSpace/ScreenSpaceCamera, bounds.center ĐÃ LÀ tọa độ World Space
                    return new Vector3(visualCenter.x, visualCenter.y, 0f);
                }
            }
            
            return new Vector3(visualCenter.x, visualCenter.y, 0f);
        }
        return Vector3.zero;
    }

    // 4.11 AnimateArrange
    public Sequence AnimateArrange(IReadOnlyList<CardView> cardsInStack, float duration = 0.2f, CardView excludeCard = null)
    {
        var seq = Sequence.Create();
        // Số slot hiển thị tối đa: 6 (index) nếu chưa unlock, 7 nếu đã unlock
        int maxVisualIndex = IsExtraSlotUnlocked ? 7 : 6;

        for (int i = 0; i < cardsInStack.Count; i++)
        {
            var card = cardsInStack[i];
            if (card == null) continue;
            // Bỏ qua card đang được animate riêng bằng AnimateMoveToStack
            if (card == excludeCard) continue;
            
            // Clamp để tile không bao giờ hiện ở ô bị khóa
            int visualIndex = Mathf.Min(i, maxVisualIndex);
            
            Vector3 targetPos = GetSlotPosition(visualIndex);
            
            // Di chuyển đồng loạt các thẻ bài đến đúng vị trí slot của nó trong khay
            seq.Group(card.AnimateMoveToPosition(targetPos, duration));
        }
        return seq;
    }



    // 4.12 AnimateShakeFull
    public Sequence AnimateShakeFull(float duration = 0.4f)
    {
        if (_stackContainer == null) return Sequence.Create();
        
        Tween.StopAll(_stackContainer);
        
        // Tạo hiệu ứng rung lắc nhẹ báo lỗi khi khay đầy
        return Sequence.Create()
            .Group(Tween.ShakeLocalPosition(_stackContainer, strength: new Vector3(0.2f, 0, 0), duration: duration, frequency: 10));
    }
}
