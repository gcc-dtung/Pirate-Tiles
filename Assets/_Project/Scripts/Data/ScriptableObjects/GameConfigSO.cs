using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "PirateTiles/Data/GameConfig")]
public class GameConfigSO : ScriptableObject
{
    [field: Header("Hearts System (Energy)")]
    [field: SerializeField] public int MaxHearts { get; private set; } = 5;
    [field: SerializeField] public float HealTimeInSeconds { get; private set; } = 1800f; // 30 phút hồi 1 tim

    [field: Header("Economy System")]
    [field: SerializeField] public int StartingCoins { get; private set; } = 100;
    
    [field: Header("Power-Ups Cost")]
    [field: SerializeField] public int UndoCost { get; private set; } = 10;
    [field: SerializeField] public int MagicCost { get; private set; } = 15;
    [field: SerializeField] public int ShuffleCost { get; private set; } = 20;
    [field: SerializeField] public int AddCellCost { get; private set; } = 30;
}
