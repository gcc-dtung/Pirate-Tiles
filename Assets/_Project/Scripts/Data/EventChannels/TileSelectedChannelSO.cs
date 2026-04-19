using UnityEngine;

[CreateAssetMenu(menuName = "PirateTiles/Events/Tile Selected Channel")]
public class TileSelectedChannelSO : EventChannelSO<TileSelectedEventData> { }

public class TileSelectedEventListener : EventListener<TileSelectedEventData> { }
