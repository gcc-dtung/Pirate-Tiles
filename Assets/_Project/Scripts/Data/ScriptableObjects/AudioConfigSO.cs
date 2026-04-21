using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "AudioConfig", menuName = "PirateTiles/Data/AudioConfig")]
public class AudioConfigSO : ScriptableObject
{
    [field: Header("Background Music")]
    [field: SerializeField] public AudioClip MainMenuBGM { get; private set; }
    [field: SerializeField] public AudioClip InGameBGM { get; private set; }

    [System.Serializable]
    public struct AudioData
    {
        public SoundEffect EffectType;
        public AudioClip Clip;
        [Range(0f, 1f)] public float Volume;
    }

    [field: Header("Sound Effects")]
    [field: SerializeField] public List<AudioData> SFXs { get; private set; }

    public AudioClip GetSfx(SoundEffect effect)
    {
        // Logic tìm AudioClip tương ứng theo Enum
        var data = SFXs.Find(x => x.EffectType == effect);
        return data.Clip;
    }
}
