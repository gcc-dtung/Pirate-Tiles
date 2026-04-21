using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "TileDatabase", menuName = "PirateTiles/Data/TileDatabase")]
public class TileDatabaseSO : ScriptableObject
{
    [System.Serializable]
    public struct TileVisualData
    {
        public CardType Type;
        public Sprite Sprite;
    }

    [field: Header("Pirate Theme Assets")]
    [field: SerializeField] public List<TileVisualData> TileVisuals { get; private set; }

    public Sprite GetSprite(CardType type)
    {
        var data = TileVisuals.Find(x => x.Type == type);
        return data.Sprite;
    }
}
