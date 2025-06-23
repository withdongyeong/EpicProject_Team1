using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : Singleton<SoundManager>
{
    [Tooltip("오디오 스코어")]
    private AudioSource interactionAudioSource;
    private AudioSource bgmAudioSource;
    private AudioSource BlackHoleAudioSource;

    // 플레이어 사운드 딕셔너리
    private Dictionary<string, AudioClip> playerSoundDictionary = new Dictionary<string, AudioClip>();
    // 플레이어 사운드 볼륨 딕셔너리
    private Dictionary<string, float> playerSoundVolumeDictionary = new Dictionary<string, float>
    {
        { "StickRoll", 1f },
        { "AriaActive", 1f },
        { "PlayerMove", 0.03f },
        { "PlayerDamage", 1f },
        { "PlayerDead", 1f }
    };

    // 타일 사운드 딕셔너리
    private Dictionary<string, AudioClip> tileSoundDictionary = new Dictionary<string, AudioClip>();
    // 타일 볼륨 딕셔너리
    private Dictionary<string, float> tileSoundVolumeDictionary = new Dictionary<string, float>
    {
        { "HealSkillActivate", 0.1f},
        { "FireBallSkillActivate", 0.5f},
        { "FireBoltSkillActivate", 0.3f},
        {"ShieldSkillActivate", 0.3f },
        {"IcicleSkillActivate", 0.2f },
        {"FrostStaffSkillActivate", 0.2f },
        {"TotemSummonSkillActivate", 0.1f },
        {"ManaTurretSkillActivate", 0.3f },
        {"ProjectileSkillActivate", 0.3f}
    };

    //아라크네 사운드 딕셔너리
    private Dictionary<string, AudioClip> ArachneSoundDictionary = new Dictionary<string, AudioClip>();
    //아라크네 볼륨 딕셔너리
    private Dictionary<string, float> ArachneSoundVolumeDictionary = new Dictionary<string, float>
    {
        {"PoisionExplotionActivate", 1f},
        {"PoisonBallActivate", 1f },
        {"SpiderLegActivate", 0.5f },
        {"SpiderSilkActivate", 0.8f },
        { "ArachneDamageActivate", 0.3f},
        { "ArachneDeadActivate", 1f}
    };

    //오크메이지 사운드 딕셔너리
    Dictionary<string, AudioClip> OrcMageSoundDictionary = new Dictionary<string, AudioClip>();
    //오크메이지 사운드볼륨
    private Dictionary<string, float> OrcMageSoundVolumeDictionary = new Dictionary<string, float>
    {
        {"OrcMage_DamageActivate", 0.6f},
        {"OrcMage_DieActivate", 0.6f },
        {"OrcMage_FrogActivate", 1f },
        {"OrcMage_RunActivate", 0.3f },
        {"OrcMage_ScreamActivate", 0.6f },
        {"OrcMage_SpikeActivate", 0.2f }
    };

    //슬라임 사운드 딕셔너리
    private Dictionary<string, AudioClip> SlimeSoundDictionary = new Dictionary<string, AudioClip>();
    //슬라임 볼륨 딕셔너리
    private Dictionary<string, float> SlimeSoundVolumeDictionary = new Dictionary<string, float>
    {
        {"PoisionExplotionActivate", 1f},
        {"PoisonBallActivate", 1f },
        {"SlimeTentacleActivate", 0.5f },
        {"SlimeDamageActivate", 1f},
        {"SlimeDeadActivate", 0.3f}
    };

    //UI 사운드 딕셔너리
    Dictionary<string, AudioClip> UISoundDictionary = new Dictionary<string, AudioClip>();
    //UI 사운드볼륨
    private Dictionary<string, float> UISoundVolumeDictionary = new Dictionary<string, float>
    {
        {"DeploymentActivate", 0.05f},
        {"RerollActivate", 0.5f },
        {"ButtonActivate", 0.3f },
        {"BlackHoleStartActivate", 0.8f }
    };

    //BGM 사운드 딕셔너리
    Dictionary<string, AudioClip> BGMSoundDictionary = new Dictionary<string, AudioClip>();
    //BGM 사운드 볼륨
    private Dictionary<string, float> BGMSoundVolumeDictionary = new Dictionary<string, float>
    {
        {"ShopSceneBGM", 0.1f},
        {"OrcMageBGM", 0.3f },
        {"ArachneBGM", 0.2f },
        {"SlimeBGM", 0.1f }
    };


    protected override void Awake()
    {
        base.Awake();
        EventBus.Init();

        interactionAudioSource = transform.GetChild(0).GetComponent<AudioSource>();
        bgmAudioSource = transform.GetChild(1).GetComponent<AudioSource>();
        BlackHoleAudioSource = transform.GetChild(2).GetComponent<AudioSource>();

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

        AudioClip[] ArachneaudioClips = Resources.LoadAll<AudioClip>("Sounds/Boss/Arachne");


        foreach (AudioClip clip in ArachneaudioClips)
        {
            if (!ArachneSoundDictionary.ContainsKey(clip.name))
            {
                ArachneSoundDictionary.Add(clip.name, clip);
            }
        }

        AudioClip[] OrcMageaudioClips = Resources.LoadAll<AudioClip>("Sounds/Boss/OrcMage");

        foreach (AudioClip clip in OrcMageaudioClips)
        {
            if (!OrcMageSoundDictionary.ContainsKey(clip.name))
            {
                OrcMageSoundDictionary.Add(clip.name, clip);
            }
        }

        AudioClip[] SlimeMageaudioClips = Resources.LoadAll<AudioClip>("Sounds/Boss/Slime");

        foreach (AudioClip clip in SlimeMageaudioClips)
        {
            if (!SlimeSoundDictionary.ContainsKey(clip.name))
            {
                SlimeSoundDictionary.Add(clip.name, clip);
            }
        }

        AudioClip[] UIaudioClips = Resources.LoadAll<AudioClip>("Sounds/UI");

        foreach (AudioClip clip in UIaudioClips)
        {
            if (!UISoundDictionary.ContainsKey(clip.name))
            {
                UISoundDictionary.Add(clip.name, clip);
            }
        }

        AudioClip[] BGMaudioClips = Resources.LoadAll<AudioClip>("Sounds/BGM");

        foreach (AudioClip clip in BGMaudioClips)
        {
            if (!BGMSoundDictionary.ContainsKey(clip.name))
            {
                BGMSoundDictionary.Add(clip.name, clip);
            }
        }

        EventBus.SubscribeSceneLoaded(OnSceneLoaded);
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
    /// 오크메이지 사운드 재생
    /// </summary>
    /// <param name="clip"></param>
    public void OrcMageSoundClip(string clip)
    {
        Debug.Log($"PlayTileSoundClip called with clip: {clip}");

        if (clip != null && interactionAudioSource != null)
        {

            AudioClip tileClip = OrcMageSoundDictionary.ContainsKey(clip) ? OrcMageSoundDictionary[clip] : null;
            if (tileClip != null)
            {
                    float volume = OrcMageSoundVolumeDictionary.ContainsKey(clip) ? OrcMageSoundVolumeDictionary[clip] : 1f;
                    interactionAudioSource.PlayOneShot(tileClip, volume);
            }
        }
    }

    /// <summary>
    /// 슬라임 사운드 재생
    /// </summary>
    /// <param name="clip"></param>
    public void SlimeSoundClip(string clip)
    {
        Debug.Log($"PlayTileSoundClip called with clip: {clip}");

        if (clip != null && interactionAudioSource != null)
        {

            AudioClip tileClip = SlimeSoundDictionary.ContainsKey(clip) ? SlimeSoundDictionary[clip] : null;
            if (tileClip != null)
            {
                    float volume = SlimeSoundVolumeDictionary.ContainsKey(clip) ? SlimeSoundVolumeDictionary[clip] : 1f;
                    interactionAudioSource.PlayOneShot(tileClip, volume);
            }
        }
    }


    /// <summary>
    /// UI 사운드 재생 - UI가 나오면 수정 필요
    /// </summary>
    public void UISoundClip(string clip)
    {
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

    /// <summary>
    /// BGM 사운드 재생 - UI가 나오면 수정 필요
    /// </summary>
    public void BGMSoundClip(string clip)
    {
        if (clip != null && interactionAudioSource != null)
        {
            AudioClip tileClip = BGMSoundDictionary.ContainsKey(clip) ? BGMSoundDictionary[clip] : null;
            if (tileClip != null)
            {
                float volume = BGMSoundVolumeDictionary.ContainsKey(clip) ? BGMSoundVolumeDictionary[clip] : 1f;
                PlayBGMSound(tileClip, volume);
            }
        }
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
    /// 씬 이동에 따른 BGM 선택
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="mode"></param>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    { 
        switch (scene.name)
        {
            case "BuildingScene":
                BGMSoundClip("ShopSceneBGM");
                break;
            case "StageScene":
                break;
            default:
                BGMSoundClip("ShopSceneBGM");
                //bgmAudioSource.Stop(); // 기본은 정지
                break;
        }
    }

    public void BlackHoleReady()
    {
        BlackHoleAudioSource.volume = 0.5f;
    }

    public void BlackHoleNotReady()
    {
        BlackHoleAudioSource.volume = 0f;
    }
}