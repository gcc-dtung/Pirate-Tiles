using UnityEngine;

public class AudioService : MonoBehaviour
{
    public static AudioService Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource _bgmSource;
    [SerializeField] private AudioSource _sfxSource;

    private EventBinding<AudioSettingChangedEvent> _audioSettingBinding;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // Restore settings from SaveService
        bool isMusicEnabled = SaveService.Instance.GetBool(SaveKeys.MusicVolume, true);
        bool isSfxEnabled = SaveService.Instance.GetBool(SaveKeys.SoundVolume, true);
        
        _bgmSource.mute = !isMusicEnabled;
        _sfxSource.mute = !isSfxEnabled;
    }

    private void OnEnable()
    {
        // Lắng nghe sự kiện người dùng thay đổi Setting để tắt/mở tiếng
        _audioSettingBinding = new EventBinding<AudioSettingChangedEvent>(OnAudioSettingChanged);
        EventBus<AudioSettingChangedEvent>.Register(_audioSettingBinding);
    }

    private void OnDisable()
    {
        EventBus<AudioSettingChangedEvent>.Deregister(_audioSettingBinding);
    }

    // === PUBLIC API CHO CÁC CONTROLLER GỌI ===

    public void PlayBGM(AudioClip clip, bool loop = true)
    {
        if (clip == null) return;
        _bgmSource.clip = clip;
        _bgmSource.loop = loop;
        _bgmSource.Play();
    }

    public void StopBGM() => _bgmSource.Stop();

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        // Dùng PlayOneShot để các âm thanh SFX có thể đè lên nhau (tiếng click, tiếng match bài)
        _sfxSource.PlayOneShot(clip); 
    }

    // === XỬ LÝ EVENT ===

    private void OnAudioSettingChanged(AudioSettingChangedEvent data)
    {
        _bgmSource.mute = !data.IsMusicEnabled;
        _sfxSource.mute = !data.IsSfxEnabled;
    }
}
