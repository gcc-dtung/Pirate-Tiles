using UnityEngine;

public class AudioController : MonoBehaviour
{
    [SerializeField] private VoidEventChannelSO _tilesMatchedChannel;
    [SerializeField] private VoidEventChannelSO _gameWonChannel;
    [SerializeField] private VoidEventChannelSO _gameLostChannel;

    private EventBinding<AudioSettingChangedEvent> _audioSettingBinding;

    private void OnEnable()
    {
        if (_tilesMatchedChannel != null) _tilesMatchedChannel.Subscribe(OnTilesMatched);
        if (_gameWonChannel != null) _gameWonChannel.Subscribe(OnGameWon);
        if (_gameLostChannel != null) _gameLostChannel.Subscribe(OnGameLost);

        _audioSettingBinding = new EventBinding<AudioSettingChangedEvent>(OnAudioSettingChanged);
        EventBus<AudioSettingChangedEvent>.Register(_audioSettingBinding);
    }

    private void OnDisable()
    {
        if (_tilesMatchedChannel != null) _tilesMatchedChannel.Unsubscribe(OnTilesMatched);
        if (_gameWonChannel != null) _gameWonChannel.Unsubscribe(OnGameWon);
        if (_gameLostChannel != null) _gameLostChannel.Unsubscribe(OnGameLost);

        EventBus<AudioSettingChangedEvent>.Deregister(_audioSettingBinding);
    }

    private void OnTilesMatched()
    {
        // TODO: Play match sound logic
        Debug.Log("Audio: Match sound played.");
    }

    private void OnGameWon()
    {
        // TODO: Play win sound logic
        Debug.Log("Audio: Win sound played.");
    }

    private void OnGameLost()
    {
        // TODO: Play lose sound logic
        Debug.Log("Audio: Lose sound played.");
    }

    private void OnAudioSettingChanged(AudioSettingChangedEvent e)
    {
        // TODO: Handle audio setting
        Debug.Log($"Audio: Settings changed - Music: {e.IsMusicEnabled}, SFX: {e.IsSfxEnabled}");
    }
}
