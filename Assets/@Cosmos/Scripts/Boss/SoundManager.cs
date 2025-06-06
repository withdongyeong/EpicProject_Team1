using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SoundManager : Singleton<SoundManager>
{
    [Tooltip("오디오 스코어")]
    [SerializeField] private AudioSource interactionAudioSource;

    [SerializeField] private AudioSource bgmAudioSource;

    [Header("BGM")]
    [Tooltip("BGM")]
    [SerializeField] private AudioClip bgm;
    [Tooltip("BGM 볼륨")]
    [SerializeField] private float bgmVolume;

    // 플레이어 사운드 딕셔너리
    private Dictionary<string, AudioClip> playerSoundDictionary = new Dictionary<string, AudioClip>();
    // 플레이어 사운드 볼륨 딕셔너리
    private Dictionary<string, float> playerSoundVolumeDictionary = new Dictionary<string, float>
    {
        { "StickRoll", 1f },
        { "AriaActive", 1f },
        { "PlayerMove", 0.1f },
        { "PlayerDamage", 0.3f },
        { "PlayerDead", 0.3f }
    };

    // 타일 사운드 딕셔너리
    private Dictionary<string, AudioClip> tileSoundDictionary = new Dictionary<string, AudioClip>();
    // 타일 볼륨 딕셔너리
    private Dictionary<string, float> tileSoundVolumeDictionary = new Dictionary<string, float>
    {
        { "HealSkillActivate", 0.3f},
    };

    //아라크네 사운드 딕셔너리
    private Dictionary<string, AudioClip> ArachneSoundDictionary = new Dictionary<string, AudioClip>();
    //아라크네 볼륨 딕셔너리
    private Dictionary<string, float> ArachneSoundVolumeDictionary = new Dictionary<string, float>();

    Dictionary<string, AudioClip> UISoundDictionary = new Dictionary<string, AudioClip>();
    //아라크네 볼륨 딕셔너리
    private Dictionary<string, float> UISoundVolumeDictionary = new Dictionary<string, float>();

    protected override void Awake()
    {
        base.Awake();

        // 플레이어 사운드 초기화
        AudioClip[] playeraudioClips = Resources.LoadAll<AudioClip>("Sounds/Player");
        foreach (AudioClip clip in playeraudioClips)
        {
            if (!playerSoundDictionary.ContainsKey(clip.name))
            {
                playerSoundDictionary.Add(clip.name, clip);
            }
        }

        // 타일 사운드 초기화
        AudioClip[] tileaudioClips = Resources.LoadAll<AudioClip>("Sounds/Tile");
        foreach (AudioClip clip in tileaudioClips)
        {
            if (!tileSoundDictionary.ContainsKey(clip.name))
            {
                tileSoundDictionary.Add(clip.name, clip);
            }
        }

        AudioClip[] ArachneaudioClips = Resources.LoadAll<AudioClip>("Sound/Boss/Arachne");


        foreach (AudioClip clip in ArachneaudioClips)
        {
            if (!ArachneSoundDictionary.ContainsKey(clip.name))
            {
                ArachneSoundDictionary.Add(clip.name, clip);
            }
        }

        AudioClip[]UIaudioClips = Resources.LoadAll<AudioClip>("Sound/UI");

        foreach (AudioClip clip in UIaudioClips)
        {
            if (!UISoundDictionary.ContainsKey(clip.name))
            {
               UISoundDictionary.Add(clip.name, clip);
            }
        }

    }

    private void Start()
    {
        PlayBGMSound(bgm, bgmVolume);
    }

    public void PlayBGMSound(AudioClip clip, float volume = 0.3f)
    {
        if (clip != null && bgmAudioSource != null)
        {
            bgmAudioSource.clip = clip;
            bgmAudioSource.volume = volume;
            bgmAudioSource.Play();
        }
    }

    /// <summary>
    /// 플레이어 사운드 재생
    /// </summary>
    /// <param name="clip"></param>
    public void PlayPlayerSound(string clip)
    {
        if (clip != null && interactionAudioSource != null)
        {
            // 플레이어 사운드 딕셔너리에서 클립을 찾습니다.
            AudioClip playerClip = playerSoundDictionary.ContainsKey(clip) ? playerSoundDictionary[clip] : null;
            if (playerClip != null)
            {
                // 플레이어 사운드 볼륨 딕셔너리에서 볼륨을 찾습니다.
                float volume = playerSoundVolumeDictionary.ContainsKey(clip) ? playerSoundVolumeDictionary[clip] : 1f;
                interactionAudioSource.PlayOneShot(playerClip, volume);
            }
        }
    }

    /// <summary>
    /// 타일 사운드 재생
    /// </summary>
    /// <param name="clip"></param>
    public void PlayTileSoundClip(string clip)
    {
        Debug.Log($"PlayTileSoundClip called with clip: {clip}");
        if (clip != null && interactionAudioSource != null)
        {
            AudioClip tileClip = tileSoundDictionary.ContainsKey(clip) ? tileSoundDictionary[clip] : null;
            if (tileClip != null)
            {
                float volume = tileSoundVolumeDictionary.ContainsKey(clip) ? tileSoundVolumeDictionary[clip] : 1f;
                interactionAudioSource.PlayOneShot(tileClip, volume);
            }
        }
    }


    /// <summary>
    /// 아라크네 사운드 재생
    /// </summary>
    public void ArachneSoundClip(string clip)
    {
        Debug.Log($"PlayTileSoundClip called with clip: {clip}");
        if (clip != null && interactionAudioSource != null)
        {
            AudioClip tileClip = ArachneSoundDictionary.ContainsKey(clip) ? ArachneSoundDictionary[clip] : null;
            if (tileClip != null)
            {
                float volume = ArachneSoundVolumeDictionary.ContainsKey(clip) ? ArachneSoundVolumeDictionary[clip] : 1f;
                interactionAudioSource.PlayOneShot(tileClip, volume);
            }
        }
    }

    /// <summary>
    /// UI 사운드 재생 - UI가 나오면 수정 필요
    /// </summary>
    public void UISoundClip(string clip)
    {
        Debug.Log($"PlayTileSoundClip called with clip: {clip}");
        if (clip != null && interactionAudioSource != null)
        {
            AudioClip tileClip = UISoundDictionary.ContainsKey(clip) ? UISoundDictionary[clip] : null;
            if (tileClip != null)
            {
                float volume = UISoundVolumeDictionary.ContainsKey(clip) ? UISoundVolumeDictionary[clip] : 1f;
                interactionAudioSource.PlayOneShot(tileClip, volume);
            }
        }
    }
}
