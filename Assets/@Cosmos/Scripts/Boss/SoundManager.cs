using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Tooltip("오디오 스코어")]
    [SerializeField] private AudioSource interactionAudioSource;

    [SerializeField] private AudioSource bgmAudioSource;

    [Header("BGM")]
    [Tooltip("BGM")]
    [SerializeField] private AudioClip bgm;
    [Tooltip("BGM 볼륨")]
    [SerializeField] private float bgmVolume;

    [Header("플레이어 사운드 정보")]
    [Tooltip("지팡이 회전 사운드")]
    [SerializeField] private AudioClip stickRollSound;
    [Tooltip("지팡이 회전 볼륨")]
    [SerializeField] private float stickRollSoundVolume;
    [Tooltip("영역 전개 사운드")]
    [SerializeField] private AudioClip ariaActiveSound;
    [Tooltip("영역 전개 사운드 볼륨")]
    [SerializeField] private float ariaActiveSoundVolume;
    [Tooltip("이동 사운드")]
    [SerializeField] private AudioClip playerMoveSound;
    [Tooltip("이동 사운드 볼륨")]
    [SerializeField] private float playerMoveSoundVolume;
    [Tooltip("피격 사운드")]
    [SerializeField] private AudioClip playerDamageSound;
    [Tooltip("피격 사운드 볼륨")]
    [SerializeField] private float playerDamageSoundVolume;
    [Tooltip("사망 사운드")]
    [SerializeField] private AudioClip playerDeadSound;
    [Tooltip("사망 사운드 볼륨")]
    [SerializeField] private float playerDeadSoundVolume;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
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
    /// 지팡이 돌리기 사운드
    /// </summary>
    public void StickRollSound()
    {
        PlaySoundClip(stickRollSound, stickRollSoundVolume);
    }

    /// <summary>
    /// 영역 전개 사운드
    /// </summary>
    public void AriaActiveSound()
    {
        PlaySoundClip(ariaActiveSound, ariaActiveSoundVolume);
    }

    /// <summary>
    /// 이동 사운드
    /// </summary>
    public void PlayerMoveActiveSound()
    {
        PlaySoundClip(playerMoveSound, playerMoveSoundVolume);
    }
    /// <summary>
    /// 플레이어 피격
    /// </summary>
    public void PlayerDamageSound()
    {
        PlaySoundClip(playerDamageSound, playerDamageSoundVolume);
    }

    /// <summary>
    /// 플레이어 사망
    /// </summary>
    public void PlayerDeadSound()
    {
        PlaySoundClip(playerDeadSound, playerDeadSoundVolume);
    }



    private void PlaySoundClip(AudioClip clip, float volume)
    {
        if (clip != null && interactionAudioSource != null)
        {
            interactionAudioSource.PlayOneShot(clip, volume);
        }
    }
}
