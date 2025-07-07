using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
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
        {"HealSkillActivate", 0.1f},
        {"FireBallSkillActivate", 0.5f},
        {"FireBoltSkillActivate", 0.2f},
        {"ShieldSkillActivate", 0.3f },
        {"IcicleSkillActivate", 0.2f },
        {"FlamingSwordSkillActivate", 0.5f},
        {"FrostStaffSkillActivate", 0.2f },
        {"TotemSummonSkillActivate", 0.1f },
        {"ManaTurretSkillActivate", 0.3f },
        {"ProjectileSkillActivate", 0.3f},
        {"ArchmageStaffSkillActivate", 0.7f},
        {"RainbowSkillActivate", 0.1f},
        {"StaffSkillActivate", 0.3f},
        {"ProtectionSkillActivate", 1f },
        {"ShieldSkillRemove", 0.3f },
        { "SwordSkillActivate", 1f},
        { "WarFlagSkillActivate", 1f },
        {"FrostHammerSkillActivate", 1f },
        {"HauntedDollSkillActivate", 0.3f},
        {"CloudSkillActivate", 0.5f },
        {"PhantomSkillActivate", 0.5f },
        {"NecronomiconSkillActivate", 0.3f }
    };

    //아라크네 사운드 딕셔너리
    private Dictionary<string, AudioClip> ArachneSoundDictionary = new Dictionary<string, AudioClip>();
    //아라크네 볼륨 딕셔너리
    private Dictionary<string, float> ArachneSoundVolumeDictionary = new Dictionary<string, float>
    {
        {"PoisionExplotionActivate", 0.7f},
        {"PoisonBallActivate", 0.7f },
        {"SpiderLegActivate", 0.3f },
        {"SpiderSilkActivate", 0.8f },
        {"ArachneDamageActivate", 0.3f},
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

    private Dictionary<string, AudioClip> BomberSoundDictionary = new Dictionary<string, AudioClip>();
    //복탄 볼륨 딕셔너리
    private Dictionary<string, float> BomberSoundVolumeDictionary = new Dictionary<string, float>
    {
        {"BomberAttackActivate", 0.05f},
        {"BomberDamageActivate", 0.3f },
        {"BomberDeadActivate", 0.7f },
    };

    private Dictionary<string, AudioClip> GolemSoundDictionary = new Dictionary<string, AudioClip>();
    //골렘 볼륨 딕셔너리
    private Dictionary<string, float> GolemSoundVolumeDictionary = new Dictionary<string, float>
    {
        {"GolemAttackActivate", 0.2f},
        {"GolemDamageActivate", 1f },
        {"GolemDeadActivate", 1f },
    };

    private Dictionary<string, AudioClip> TurtreeSoundDictionary = new Dictionary<string, AudioClip>();
    //나무거북 볼륨 딕셔너리
    private Dictionary<string, float> TurtreeSoundVolumeDictionary = new Dictionary<string, float>
    {
        {"TurtreeAttackActivate", 0.05f},
        {"TurtreeDamageActivate", 0.3f },
        {"TurtreeDeadActivate", 0.3f },
    };

    private Dictionary<string, AudioClip> ReaperSoundDictionary = new Dictionary<string, AudioClip>();
    //리퍼 볼륨 딕셔너리
    private Dictionary<string, float> ReaperSoundVolumeDictionary = new Dictionary<string, float>
    {
        {"ReaperAttackActivate", 0.3f},
        {"ReaperDamageActivate", 1f },
        {"ReaperDeadActivate", 1f },
    };

    private Dictionary<string, AudioClip> KnightSoundDictionary = new Dictionary<string, AudioClip>();
    //기사 볼륨 딕셔너리
    private Dictionary<string, float> KnightSoundVolumeDictionary = new Dictionary<string, float>
    {
        {"KnightAttackActivate", 0.1f},
        {"KnightDashActivate", 1f},
        {"KnightDamageActivate", 1f },
        {"KnightDeadActivate", 1f },
    };

    private Dictionary<string, AudioClip> BigHandSoundDictionary = new Dictionary<string, AudioClip>();
    //손 볼륨 딕셔너리
    private Dictionary<string, float> BigHandSoundVolumeDictionary = new Dictionary<string, float>
    {
        {"BigHandAttackActivate", 0.03f},
        {"BigHandFistActivate", 0.01f},
        {"BigHandFingerActivate", 0.7f},
        {"BigHandDamageActivate", 0.3f },
        {"BigHandDeadActivate", 0.3f },
    };

    private Dictionary<string, AudioClip> LastBossSoundDictionary = new Dictionary<string, AudioClip>();
    //최종보스 볼륨 딕셔너리
    private Dictionary<string, float> LastBossSoundVolumeDictionary = new Dictionary<string, float>
    {
        {"LastBossDamageActivate", 1f },
        {"LastBossDeadActivate", 1f },
        {"LastBossFlameAttackActivate", 0.1f },
        {"LastBossFrostAttackActivate", 0.1f },
        {"LastBossStaffAttackActivate", 0.1f },
        {"LastBossSwordAttackActivate", 0.1f },
        {"LastBossFlameModeActivate", 0.1f },
        {"LastBossFrostModeActivate", 0.1f },
        {"LastBossStaffModeActivate", 0.1f },
        {"LastBossSwordModeActivate", 1f }
    };

    //UI 사운드 딕셔너리
    Dictionary<string, AudioClip> UISoundDictionary = new Dictionary<string, AudioClip>();
    //UI 사운드볼륨
    private Dictionary<string, float> UISoundVolumeDictionary = new Dictionary<string, float>
    {
        {"DeploymentActivate", 0.05f},
        {"RerollActivate", 0.5f },
        {"ButtonActivate", 0.1f },
        {"BlackHoleStartActivate", 0.8f }
    };

    //BGM 사운드 딕셔너리
    Dictionary<string, AudioClip> BGMSoundDictionary = new Dictionary<string, AudioClip>();
    //BGM 사운드 볼륨
    private Dictionary<string, float> BGMSoundVolumeDictionary = new Dictionary<string, float>
    {
        {"ShopBGM", 0.1f},
        {"OrcMageBGM", 0.15f },
        {"ArachneBGM", 0.1f },
        {"SlimeBGM", 0.05f },
        {"TitleBGM", 0.1f }
    };


    protected override void Awake()
    {
        base.Awake();
        EventBus.Init();

        interactionAudioSource = transform.GetChild(0).GetComponent<AudioSource>();
        bgmAudioSource = transform.GetChild(1).GetComponent<AudioSource>();

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

        AudioClip[] BomberMageaudioClips = Resources.LoadAll<AudioClip>("Sounds/Boss/Bomber");

        foreach (AudioClip clip in BomberMageaudioClips)
        {
            if (!BomberSoundDictionary.ContainsKey(clip.name))
            {
                BomberSoundDictionary.Add(clip.name, clip);
            }
        }

        AudioClip[] GolemMageaudioClips = Resources.LoadAll<AudioClip>("Sounds/Boss/Golem");

        foreach (AudioClip clip in GolemMageaudioClips)
        {
            if (!GolemSoundDictionary.ContainsKey(clip.name))
            {
                GolemSoundDictionary.Add(clip.name, clip);
            }
        }

        AudioClip[] TurtreeMageaudioClips = Resources.LoadAll<AudioClip>("Sounds/Boss/Turtree");

        foreach (AudioClip clip in TurtreeMageaudioClips)
        {
            if (!TurtreeSoundDictionary.ContainsKey(clip.name))
            {
                TurtreeSoundDictionary.Add(clip.name, clip);
            }
        }

        AudioClip[] ReaperMageaudioClips = Resources.LoadAll<AudioClip>("Sounds/Boss/Reaper");

        foreach (AudioClip clip in ReaperMageaudioClips)
        {
            if (!ReaperSoundDictionary.ContainsKey(clip.name))
            {
                ReaperSoundDictionary.Add(clip.name, clip);
            }
        }

        AudioClip[] KnightMageaudioClips = Resources.LoadAll<AudioClip>("Sounds/Boss/Knight");

        foreach (AudioClip clip in KnightMageaudioClips)
        {
            if (!KnightSoundDictionary.ContainsKey(clip.name))
            {
                KnightSoundDictionary.Add(clip.name, clip);
            }
        }

        AudioClip[] BigHandMageaudioClips = Resources.LoadAll<AudioClip>("Sounds/Boss/BigHand");

        foreach (AudioClip clip in BigHandMageaudioClips)
        {
            if (!BigHandSoundDictionary.ContainsKey(clip.name))
            {
                BigHandSoundDictionary.Add(clip.name, clip);
            }
        }

        AudioClip[] LastBossMageaudioClips = Resources.LoadAll<AudioClip>("Sounds/Boss/LastBoss");

        foreach (AudioClip clip in LastBossMageaudioClips)
        {
            if (!LastBossSoundDictionary.ContainsKey(clip.name))
            {
                LastBossSoundDictionary.Add(clip.name, clip);
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
        //Debug.Log($"PlayTileSoundClip called with clip: {clip}");

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
    /// 폭폭탄 사운드 재생
    /// </summary>
    /// <param name="clip"></param>
    public void BomberSoundClip(string clip)
    {
        Debug.Log($"PlayTileSoundClip called with clip: {clip}");

        if (clip != null && interactionAudioSource != null)
        {

            AudioClip tileClip = BomberSoundDictionary.ContainsKey(clip) ? BomberSoundDictionary[clip] : null;
            if (tileClip != null)
            {
                float volume = BomberSoundVolumeDictionary.ContainsKey(clip) ? BomberSoundVolumeDictionary[clip] : 1f;
                interactionAudioSource.PlayOneShot(tileClip, volume);
            }
        }
    }

    /// <summary>
    /// 골렘 사운드 재생
    /// </summary>
    /// <param name="clip"></param>
    public void GolemSoundClip(string clip)
    {
        Debug.Log($"PlayTileSoundClip called with clip: {clip}");

        if (clip != null && interactionAudioSource != null)
        {

            AudioClip tileClip = GolemSoundDictionary.ContainsKey(clip) ? GolemSoundDictionary[clip] : null;
            if (tileClip != null)
            {
                float volume = GolemSoundVolumeDictionary.ContainsKey(clip) ? GolemSoundVolumeDictionary[clip] : 1f;
                interactionAudioSource.PlayOneShot(tileClip, volume);
            }
        }
    }

    /// <summary>
    /// 나무거북 사운드 재생
    /// </summary>
    /// <param name="clip"></param>
    public void TurtreeSoundClip(string clip)
    {
        Debug.Log($"PlayTileSoundClip called with clip: {clip}");

        if (clip != null && interactionAudioSource != null)
        {

            AudioClip tileClip = TurtreeSoundDictionary.ContainsKey(clip) ? TurtreeSoundDictionary[clip] : null;
            if (tileClip != null)
            {
                float volume = TurtreeSoundVolumeDictionary.ContainsKey(clip) ? TurtreeSoundVolumeDictionary[clip] : 1f;
                interactionAudioSource.PlayOneShot(tileClip, volume);
            }
        }
    }

    /// <summary>
    /// 리퍼 사운드 재생
    /// </summary>
    /// <param name="clip"></param>
    public void ReaperSoundClip(string clip)
    {
        Debug.Log($"PlayTileSoundClip called with clip: {clip}");

        if (clip != null && interactionAudioSource != null)
        {

            AudioClip tileClip = ReaperSoundDictionary.ContainsKey(clip) ? ReaperSoundDictionary[clip] : null;
            if (tileClip != null)
            {
                float volume = ReaperSoundVolumeDictionary.ContainsKey(clip) ? ReaperSoundVolumeDictionary[clip] : 1f;
                interactionAudioSource.PlayOneShot(tileClip, volume);
            }
        }
    }

    /// <summary>
    /// 기사 사운드 재생
    /// </summary>
    /// <param name="clip"></param>
    public void KnightSoundClip(string clip)
    {
        Debug.Log($"PlayTileSoundClip called with clip: {clip}");

        if (clip != null && interactionAudioSource != null)
        {

            AudioClip tileClip = KnightSoundDictionary.ContainsKey(clip) ? KnightSoundDictionary[clip] : null;
            if (tileClip != null)
            {
                float volume = KnightSoundVolumeDictionary.ContainsKey(clip) ? KnightSoundVolumeDictionary[clip] : 1f;
                interactionAudioSource.PlayOneShot(tileClip, volume);
            }
        }
    }

    /// <summary>
    /// 손 사운드 재생
    /// </summary>
    /// <param name="clip"></param>
    public void BigHandSoundClip(string clip)
    {
        Debug.Log($"PlayTileSoundClip called with clip: {clip}");

        if (clip != null && interactionAudioSource != null)
        {

            AudioClip tileClip = BigHandSoundDictionary.ContainsKey(clip) ? BigHandSoundDictionary[clip] : null;
            if (tileClip != null)
            {
                float volume = BigHandSoundVolumeDictionary.ContainsKey(clip) ? BigHandSoundVolumeDictionary[clip] : 1f;
                interactionAudioSource.PlayOneShot(tileClip, volume);
            }
        }
    }

    /// <summary>
    /// 최종보스 사운드 재생
    /// </summary>
    /// <param name="clip"></param>
    public void LastBossSoundClip(string clip)
    {
        Debug.Log($"PlayTileSoundClip called with clip: {clip}");

        if (clip != null && interactionAudioSource != null)
        {

            AudioClip tileClip = LastBossSoundDictionary.ContainsKey(clip) ? LastBossSoundDictionary[clip] : null;
            if (tileClip != null)
            {
                float volume = LastBossSoundVolumeDictionary.ContainsKey(clip) ? LastBossSoundVolumeDictionary[clip] : 1f;
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
                BGMSoundClip("ShopBGM");
                break;
            case "StageScene":
                break;
            case "LogoScene":
                break;
            case "TitleScene":
                BGMSoundClip("TitleBGM");
                break;
            default:
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

    public void DamageEffectStart(AudioClip audioClip)
    {
        if (audioClip != null)
        {
            interactionAudioSource.PlayOneShot(audioClip, 0.5f);
        }
    }
    
    private void OnDestroy()
    {
        EventBus.UnsubscribeSceneLoaded(OnSceneLoaded);
    }
}