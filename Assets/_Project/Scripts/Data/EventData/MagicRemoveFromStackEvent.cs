using System.Collections.Generic;

public struct MagicRemoveFromStackEvent : IEvent
{
    public List<int> TileIds;
}
