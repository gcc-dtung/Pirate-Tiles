public enum SoundEffect
{
    None = 0,
    
    // UI & System
    ButtonClick = 1,
    PanelOpen = 2,
    PanelClose = 3,
    
    // Gameplay Core
    TileClick = 10,
    TileMatch = 11,
    TileFail = 12,
    StackFull = 13,
    ComboMatch = 14,
    
    // Powers
    PowerUpUse = 20,
    UndoUse = 21,
    ShuffleUse = 22,
    MagicUse = 23,
    AddCellUse = 24,
    
    // Economy & Meta
    CoinCollect = 30,
    HeartLost = 31,
    HeartHeal = 32,
    StarEarn = 33,
    
    // Level & Time
    LevelStart = 40,
    LevelComplete = 41,
    GameWin = 42,
    GameLose = 43,
    TimerTick = 44,
    TimerWarning = 45,
    
    // BGM
    BGM_Menu = 100,
    BGM_InGame = 101
}
