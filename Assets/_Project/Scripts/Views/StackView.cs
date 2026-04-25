using UnityEngine;
using PrimeTween;
using System.Collections.Generic;

public class StackView : MonoBehaviour
{
    [SerializeField] private Transform[] _slotPositions;
    [SerializeField] private Transform _stackContainer; // Object chứa background của khay, dùng để rung

    public int MaxSlots => _slotPositions != null ? _slotPositions.Length : 0;

    // 4.10 Setup StackView
    public Vector3 GetSlotPosition(int index)
    {
        if (_slotPositions != null && index >= 0 && index < _slotPositions.Length)
        {
            return _slotPositions[index].position;
        }
        return Vector3.zero;
    }

    // 4.11 AnimateArrange
    public Sequence AnimateArrange(IReadOnlyList<CardView> cardsInStack, float duration = 0.2f)
    {
        var seq = Sequence.Create();
        for (int i = 0; i < cardsInStack.Count; i++)
        {
            var card = cardsInStack[i];
            if (card == null) continue;
            
            Vector3 targetPos = GetSlotPosition(i);
            
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
