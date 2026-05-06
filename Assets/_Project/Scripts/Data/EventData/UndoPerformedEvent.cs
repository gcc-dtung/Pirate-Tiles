using UnityEngine;

public struct UndoPerformedEvent : IEvent
{
    public int TileId;
    public CardView CardView;
    public Vector3 OriginalPosition;
}
