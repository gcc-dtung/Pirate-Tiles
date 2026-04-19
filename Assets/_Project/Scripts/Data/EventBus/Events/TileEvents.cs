using UnityEngine;

public struct TileStateChangedEvent : IEvent {
    public int TileId;

    public CardState PreviousState;

    public CardState NewState;
}

public struct BoardModelUpdatedEvent : IEvent {
    public int RemainingTiles;

    public int SelectableTiles;
}
