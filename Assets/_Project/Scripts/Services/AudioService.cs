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
        if (Instance == null)
        {
            Instance = this;
            EnsureAudioSources();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        bool isMusicEnabled = true;
        bool isSfxEnabled = true;

        if (SaveService.Instance != null)
        {
            isMusicEnabled = SaveService.Instance.GetBool(SaveKeys.MusicVolume, true);
            isSfxEnabled = SaveService.Instance.GetBool(SaveKeys.SoundVolume, true);
        }

        if (_bgmSource != null) _bgmSource.mute = !isMusicEnabled;
        if (_sfxSource != null) _sfxSource.mute = !isSfxEnabled;
    }

    private void OnEnable()
    {
        _audioSettingBinding = new EventBinding<AudioSettingChangedEvent>(OnAudioSettingChanged);
        EventBus<AudioSettingChangedEvent>.Register(_audioSettingBinding);
    }

    private void OnDisable()
    {
        EventBus<AudioSettingChangedEvent>.Deregister(_audioSettingBinding);
    }

    public void PlayBGM(AudioClip clip, bool loop = true)
    {
        if (_bgmSource == null)
        {
            Debug.LogWarning("[AudioService] BGM AudioSource is null.");
            return;
        }

        if (clip == null)
        {
            Debug.LogWarning("[AudioService] BGM clip is null.");
            return;
        }

        _bgmSource.clip = clip;
        _bgmSource.loop = loop;
        _bgmSource.Play();
    }

    public void StopBGM()
    {
        if (_bgmSource != null) _bgmSource.Stop();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (_sfxSource == null)
        {
            Debug.LogWarning("[AudioService] SFX AudioSource is null.");
            return;
        }

        if (clip == null) return;
        _sfxSource.PlayOneShot(clip);
    }

    private void OnAudioSettingChanged(AudioSettingChangedEvent data)
    {
        if (_bgmSource != null) _bgmSource.mute = !data.IsMusicEnabled;
        if (_sfxSource != null) _sfxSource.mute = !data.IsSfxEnabled;
    }

    private void EnsureAudioSources()
    {
        if (_bgmSource == null)
        {
            _bgmSource = FindOrCreateSource("BGMSource", true);
        }

        if (_sfxSource == null)
        {
            _sfxSource = FindOrCreateSource("SFXSource", false);
        }
    }

    private AudioSource FindOrCreateSource(string childName, bool loop)
    {
        Transform child = transform.Find(childName);
        GameObject go = child != null ? child.gameObject : new GameObject(childName);
        if (go.transform.parent != transform)
        {
            go.transform.SetParent(transform);
        }

        AudioSource source = go.GetComponent<AudioSource>();
        if (source == null)
        {
            source = go.AddComponent<AudioSource>();
        }

        source.loop = loop;
        return source;
    }
}
