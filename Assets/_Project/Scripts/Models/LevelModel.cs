public class LevelModel
{
    public int LevelIndex { get; set; }
    public float TimeLimit { get; set; }
    public int MaxStackSize { get; set; }
    
    // Thời gian còn lại khi đang chơi
    public float CurrentTimeRemaining { get; set; }

    public bool IsTimeUp => CurrentTimeRemaining <= 0;
}
