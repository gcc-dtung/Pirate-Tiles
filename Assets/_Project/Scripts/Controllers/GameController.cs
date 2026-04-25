using UnityEngine;

public class GameController : MonoBehaviour
{
    [Header("Controllers")]
    [SerializeField] private BoardController _boardController;
    [SerializeField] private StackController _stackController;
    [SerializeField] private PowerUpController _powerUpController;
    [SerializeField] private TimerController _timerController;

    [Header("Event Channels (Cross-Layer)")]
    [SerializeField] private VoidEventChannelSO _tilesMatchedChannel;
    [SerializeField] private VoidEventChannelSO _stackFullChannel;
    [SerializeField] private VoidEventChannelSO _timerExpiredChannel;
    [SerializeField] private VoidEventChannelSO _gameWonChannel;
    [SerializeField] private VoidEventChannelSO _gameLostChannel;
    [SerializeField] private VoidEventChannelSO _boardClearedChannel;

    [Header("Views")]
    [SerializeField] private WinPanelView _winPanelView;
    [SerializeField] private LosePanelView _losePanelView;

    private GameStateModel _gameState;

    private void Awake()
    {
        _gameState = new GameStateModel();
        _gameState.Phase = GamePhase.Init;
    }

    private void Start()
    {
        SetPhase(GamePhase.Playing);
    }

    private void SetPhase(GamePhase newPhase)
    {
        var prevPhase = _gameState.Phase;
        _gameState.Phase = newPhase;

        EventBus<GamePhaseChangedEvent>.Raise(new GamePhaseChangedEvent
        {
            PreviousPhase = prevPhase,
            NewPhase = newPhase
        });
    }

    private void HandleWin()
    {
        if (_gameState.Phase != GamePhase.Playing) return;
        SetPhase(GamePhase.Won);
        if (_gameWonChannel != null) _gameWonChannel.EventRaise();
    }

    private void HandleLose()
    {
        if (_gameState.Phase != GamePhase.Playing) return;
        SetPhase(GamePhase.Lost);
        if (_gameLostChannel != null) _gameLostChannel.EventRaise();
    }

    private void OnTilesMatched()
    {
        // Add score or any overall game state logic on match
    }

    private void OnStackFull()
    {
        HandleLose();
    }

    private void OnTimerExpired()
    {
        HandleLose();
    }

    private void OnGameWonEventRaised()
    {
        if (_winPanelView != null) _winPanelView.Show(100); 
    }

    private void OnGameLostEventRaised()
    {
        if (_losePanelView != null) _losePanelView.Show();
    }
}
