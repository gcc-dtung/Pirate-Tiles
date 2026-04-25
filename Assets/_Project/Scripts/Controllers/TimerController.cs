using UnityEngine;

public class TimerController : MonoBehaviour
{
    [SerializeField] private TimerView _timerView;
    [SerializeField] private VoidEventChannelSO _timerExpiredChannel;

    private LevelModel _levelModel;
    private bool _isRunning = false;
    private EventBinding<GamePhaseChangedEvent> _phaseBinding;

    public void Initialize(LevelModel levelModel)
    {
        _levelModel = levelModel;
        _timerView.UpdateTime(_levelModel.CurrentTimeRemaining);
    }

    private void OnEnable()
    {
        _phaseBinding = new EventBinding<GamePhaseChangedEvent>(OnPhaseChanged);
        EventBus<GamePhaseChangedEvent>.Register(_phaseBinding);
    }

    private void OnDisable()
    {
        EventBus<GamePhaseChangedEvent>.Deregister(_phaseBinding);
    }

    private void Update()
    {
        if (!_isRunning || _levelModel == null) return;

        if (_levelModel.CurrentTimeRemaining > 0)
        {
            _levelModel.CurrentTimeRemaining -= Time.deltaTime;
            _timerView.UpdateTime(_levelModel.CurrentTimeRemaining);

            if (_levelModel.CurrentTimeRemaining <= 0)
            {
                _levelModel.CurrentTimeRemaining = 0;
                _isRunning = false;
                if (_timerExpiredChannel != null)
                {
                    _timerExpiredChannel.RaiseEvent();
                }
            }
        }
    }

    private void OnPhaseChanged(GamePhaseChangedEvent e)
    {
        _isRunning = (e.NewPhase == GamePhase.Playing);
    }
}
