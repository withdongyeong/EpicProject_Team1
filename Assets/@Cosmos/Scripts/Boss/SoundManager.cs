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
    private SoundVolumeSettings soundVolumeSettings;

    // 플레이어 사운드 딕셔너리
    private Dictionary<string, AudioClip> playerSoundDictionary = new Dictionary<string, AudioClip>();

    // 타일 사운드 딕셔너리
    private Dictionary<string, AudioClip> tileSoundDictionary = new Dictionary<string, AudioClip>();

    //아라크네 사운드 딕셔너리
    private Dictionary<string, AudioClip> ArachneSoundDictionary = new Dictionary<string, AudioClip>();

    //오크메이지 사운드 딕셔너리
    Dictionary<string, AudioClip> OrcMageSoundDictionary = new Dictionary<string, AudioClip>();

    //슬라임 사운드 딕셔너리
    private Dictionary<string, AudioClip> SlimeSoundDictionary = new Dictionary<string, AudioClip>();
    //폭탄 사운드 딕셔너리
    private Dictionary<string, AudioClip> BomberSoundDictionary = new Dictionary<string, AudioClip>();
    //골렘 사운드 딕셔너리
    private Dictionary<string, AudioClip> GolemSoundDictionary = new Dictionary<string, AudioClip>();
    //나무거북 사운드 딕셔너리
    private Dictionary<string, AudioClip> TurtreeSoundDictionary = new Dictionary<string, AudioClip>();
    //리퍼 사운드 딕셔너리
    private Dictionary<string, AudioClip> ReaperSoundDictionary = new Dictionary<string, AudioClip>();
    //기사 사운드 딕셔너리
    private Dictionary<string, AudioClip> KnightSoundDictionary = new Dictionary<string, AudioClip>();

    //손 사운드 딕셔너리
    private Dictionary<string, AudioClip> BigHandSoundDictionary = new Dictionary<string, AudioClip>();
    //최종보스 사운드 딕셔너리
    private Dictionary<string, AudioClip> LastBossSoundDictionary = new Dictionary<string, AudioClip>();

    //UI 사운드 딕셔너리
    Dictionary<string, AudioClip> UISoundDictionary = new Dictionary<string, AudioClip>();

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
        soundVolumeSettings = GetComponent<SoundVolumeSettings>();

        interactionAudioSource = transform.GetChild(0).GetComponent<AudioSource>();
        bgmAudioSource = transform.GetChild(1).GetComponent<AudioSource>();

        /// 배경음악은 타임 스케일 영향 안 받도록(일시정지 영향 X)
        if (bgmAudioSource != null)
        {
            bgmAudioSource.ignoreListenerPause = true;
        }
        
        // 효과음 초기 설정
        UpdateInteractionAudioSettings();
        
        // 게임 상태 변경 이벤트 구독
        EventBus.SubscribeGameStateChanged(OnGameStateChanged);
        
        LoadAllSoundClips();
        SubscribeEvents();
    }

    /// <summary>
    /// 게임 상태 변경 시 효과음 AudioSource 설정 업데이트
    /// </summary>
    private void OnGameStateChanged(GameState newState)
    {
        UpdateInteractionAudioSettings();
    }

    /// <summary>
    /// 게임 상태에 따른 효과음 AudioSource ignoreListenerPause 설정
    /// Count, Victory, Defeat 상태에서는 효과음 일시정지 안함
    /// </summary>
    private void UpdateInteractionAudioSettings()
    {
        if (interactionAudioSource == null) return;
        
        GameState currentState = GameStateManager.Instance.CurrentState;
        
        // Count(카운트다운), Victory, Defeat 상태에서는 효과음이 일시정지되지 않도록 설정
        bool shouldIgnorePause = (currentState == GameState.Count || 
                                 currentState == GameState.Victory || 
                                 currentState == GameState.Defeat);
        
        interactionAudioSource.ignoreListenerPause = shouldIgnorePause;
    }

    /// <summary>
    /// 모든 사운드 클립 로딩
    /// </summary>
    private void LoadAllSoundClips()
    {
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
    }

    /// <summary>
    /// 이벤트 구독
    /// </summary>
    private void SubscribeEvents()
    {
        EventBus.SubscribeSceneLoaded(OnSceneLoaded);
        EventBus.SubscribePlayerDeath(PlayerDeadSound);
        EventBus.SubscribeBossDeath(BossDeadSound);
    }

    private void PlayerDeadSound()
    {
        StopBGMSound();
        UISoundClip("LoseActivate");
    }

    private void BossDeadSound()
    {
        UISoundClip("GameClearActivate");
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
                float volume = GetVolumeFromSettings(clip);
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
        if (clip != null && interactionAudioSource != null)
        {
            AudioClip tileClip = tileSoundDictionary.ContainsKey(clip) ? tileSoundDictionary[clip] : null;
            if (tileClip != null)
            {
                float volume = GetVolumeFromSettings(clip);
                interactionAudioSource.PlayOneShot(tileClip, volume);
            }
        }
    }

    /// <summary>
    /// 아라크네 사운드 재생
    /// </summary>
    public void ArachneSoundClip(string clip)
    {
        if (clip != null && interactionAudioSource != null)
        {
            AudioClip tileClip = ArachneSoundDictionary.ContainsKey(clip) ? ArachneSoundDictionary[clip] : null;
            if (tileClip != null)
            {
                float volume = GetVolumeFromSettings(clip);
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
        if (clip != null && interactionAudioSource != null)
        {
            AudioClip tileClip = OrcMageSoundDictionary.ContainsKey(clip) ? OrcMageSoundDictionary[clip] : null;
            if (tileClip != null)
            {
                float volume = GetVolumeFromSettings(clip);
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
        if (clip != null && interactionAudioSource != null)
        {
            AudioClip tileClip = SlimeSoundDictionary.ContainsKey(clip) ? SlimeSoundDictionary[clip] : null;
            if (tileClip != null)
            {
                float volume = GetVolumeFromSettings(clip);
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
        if (clip != null && interactionAudioSource != null)
        {
            AudioClip tileClip = BomberSoundDictionary.ContainsKey(clip) ? BomberSoundDictionary[clip] : null;
            if (tileClip != null)
            {
                float volume = GetVolumeFromSettings(clip);
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
        if (clip != null && interactionAudioSource != null)
        {
            AudioClip tileClip = GolemSoundDictionary.ContainsKey(clip) ? GolemSoundDictionary[clip] : null;
            if (tileClip != null)
            {
                float volume = GetVolumeFromSettings(clip);
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
        if (clip != null && interactionAudioSource != null)
        {
            AudioClip tileClip = TurtreeSoundDictionary.ContainsKey(clip) ? TurtreeSoundDictionary[clip] : null;
            if (tileClip != null)
            {
                float volume = GetVolumeFromSettings(clip);
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
        if (clip != null && interactionAudioSource != null)
        {
            AudioClip tileClip = ReaperSoundDictionary.ContainsKey(clip) ? ReaperSoundDictionary[clip] : null;
            if (tileClip != null)
            {
                float volume = GetVolumeFromSettings(clip);
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
        if (clip != null && interactionAudioSource != null)
        {
            AudioClip tileClip = KnightSoundDictionary.ContainsKey(clip) ? KnightSoundDictionary[clip] : null;
            if (tileClip != null)
            {
                float volume = GetVolumeFromSettings(clip);
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
        if (clip != null && interactionAudioSource != null)
        {
            AudioClip tileClip = BigHandSoundDictionary.ContainsKey(clip) ? BigHandSoundDictionary[clip] : null;
            if (tileClip != null)
            {
                float volume = GetVolumeFromSettings(clip);
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
        if (clip != null && interactionAudioSource != null)
        {
            AudioClip tileClip = LastBossSoundDictionary.ContainsKey(clip) ? LastBossSoundDictionary[clip] : null;
            if (tileClip != null)
            {
                float volume = GetVolumeFromSettings(clip);
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
                float volume = GetVolumeFromSettings(clip);
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

    public void StopBGMSound()
    {
        if (bgmAudioSource != null && bgmAudioSource.isPlaying)
        {
            bgmAudioSource.Stop();
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
        EventBus.UnsubscribePlayerDeath(PlayerDeadSound);
        EventBus.UnsubscribeBossDeath(BossDeadSound);
        EventBus.UnsubscribeGameStateChanged(OnGameStateChanged);
    }

    private float GetVolumeFromSettings(string clipName)
    {
        var field = soundVolumeSettings.GetType().GetField(clipName);
        if (field != null && field.FieldType == typeof(float))
        {
            return (float)field.GetValue(soundVolumeSettings);
        }
        else
        {
            Debug.LogWarning($"[SoundManager] Volume field not found for: {clipName}");
            return 1f;
        }
    }
}