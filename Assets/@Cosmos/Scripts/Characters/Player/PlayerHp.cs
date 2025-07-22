using UnityEngine;
using System.Collections;

/// <summary>
/// 플레이어 체력 관리 시스템 (무적 시간 및 깜박임 포함)
/// 보스 패턴 로그 시스템과 연동
/// </summary>
public class PlayerHp : MonoBehaviour
{
    private PlayerShield _playerShield;
    private PlayerProtection _playerProtection;
    private SpriteRenderer _spriteRenderer;
    private CameraShakeTrigger _shakeTrigger;
    private DamageScreenEffect _damageScreenEffect; 

    
    private int _maxHealth = 100;
    [SerializeField]
    private int _currentHealth;
    
    [Header("무적 시간 설정")]
    public float invincibilityDuration = 0.5f;
    public int blinkCount = 5; // 총 깜박임 횟수
    
    private bool _isInvincible = false;
    private Coroutine _blinkCoroutine;

    private int _healedAmount = 0; //도전과제용 이번 라운드에 얼마나 힐했는지
    
    // 보스 패턴 로그용 - 마지막 피격 패턴 추적
    private string _lastHitPattern = "";

    /// <summary>
    /// 최대 체력 프로퍼티
    /// </summary>
    public int MaxHealth { get => _maxHealth; set => _maxHealth = value; }
    
    /// <summary>
    /// 현재 체력 프로퍼티
    /// </summary>
    public int CurrentHealth { get => _currentHealth; set => _currentHealth = value; }
    
    /// <summary>
    /// 무적 상태 여부 프로퍼티
    /// </summary>
    public bool IsInvincible { get => _isInvincible; }

    public int HealedAmount { get => _healedAmount; }

    private void Awake()
    {
        _playerShield = GetComponent<PlayerShield>();
        _playerProtection = GetComponent<PlayerProtection>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _shakeTrigger = FindAnyObjectByType<CameraShakeTrigger>();
        _damageScreenEffect = FindAnyObjectByType<DamageScreenEffect>();
        
        if (_spriteRenderer == null)
        {
            Debug.LogError("PlayerHealth: SpriteRenderer component not found!");
        }
        
        if (_damageScreenEffect == null)
        {
            Debug.LogWarning("PlayerHealth: DamageScreenEffect component not found!");
        }
    }

    private void Start()
    {
        _currentHealth = _maxHealth;
        // 매우 어려움
        if (GameManager.Instance.DifficultyLevel == 3)
        {
            _currentHealth = 1;
        }
    }
    
    /// <summary>
    /// 데미지 처리 (무적 시간 포함) - 패턴명 필수
    /// </summary>
    /// <param name="damage">받을 데미지</param>
    /// <param name="patternName">공격한 패턴명 (예: "1_1")</param>
    public void TakeDamage(int damage, string patternName)
    {
        // 무적 상태면 데미지 무시
        if (_isInvincible)
            return;

        // 보스 패턴 피격 로그 기록
        BossPatternLogger.Instance.LogPlayerHit(patternName);
        
        // 마지막 피격 패턴 저장 (사망시 킬러 패턴 기록용)
        _lastHitPattern = patternName;

        //피격 애니메이션을 재생하고 소리도 틉니다
        FindAnyObjectByType<StageHandler>().Player.Animator.SetTrigger("Damaged");
        SoundManager.Instance.PlayPlayerSound("PlayerDamage");

        // 피격감 강화(카메라 진동)
        _shakeTrigger.Shake(0.5f);
        //무적 부여
        StartInvincibility();

        // 보호 상태면 보호막량 감소
        damage = _playerProtection.TryProtectionBlock(damage);
        
        //이건 막았다는 뜻이므로 return 합니다.
        if (damage <= 0)
        {
            return;
        }

        // 방어 상태면 방어막량 감소
        if (_playerShield.TryShieldBlock(damage))
        {
            SoundManager.Instance.PlayTileSoundClip("ShieldSkillRemove");
            return;
        }

        _currentHealth -= damage;
        _currentHealth = Mathf.Max(0, _currentHealth);
        
        if (_currentHealth <= 0)
        {
            if (GameManager.Instance.IsInTutorial)
            {
                _currentHealth = 2;
                return;
            }
            // 죽었으면 죽음 처리
            Die();
            SoundManager.Instance.PlayPlayerSound("PlayerDead");
        }
        else if(_damageScreenEffect != null)
        {
            // 피격감 강화(화면 효과) - 추가된 부분
            _damageScreenEffect.ShowDamageEffect();
        }
    }
    
    /// <summary>
    /// 무적 시간 시작
    /// </summary>
    private void StartInvincibility()
    {
        if (_isInvincible) return; // 이미 무적 상태면 무시
        
        _isInvincible = true;
        
        // 깜박임 효과 시작
        if (_blinkCoroutine != null)
        {
            StopCoroutine(_blinkCoroutine);
        }
        _blinkCoroutine = StartCoroutine(BlinkEffect());
        
        // 무적 시간 종료 예약
        StartCoroutine(EndInvincibilityAfterTime());
    }
    
    /// <summary>
    /// 깜박임 효과 코루틴 - 정확히 5번 깜박깜박
    /// </summary>
    private IEnumerator BlinkEffect()
    {
        if (_spriteRenderer == null) yield break;
        
        float blinkInterval = invincibilityDuration / (blinkCount * 2); // 깜박깜박을 위해 2배
        
        for (int i = 0; i < blinkCount; i++)
        {
            // 사라짐
            _spriteRenderer.enabled = false;
            yield return new WaitForSeconds(blinkInterval);
            
            // 나타남
            _spriteRenderer.enabled = true;
            yield return new WaitForSeconds(blinkInterval);
        }
        
        // 확실히 보이게 설정
        _spriteRenderer.enabled = true;
    }
    
    /// <summary>
    /// 무적 시간 종료 코루틴
    /// </summary>
    private IEnumerator EndInvincibilityAfterTime()
    {
        yield return new WaitForSeconds(invincibilityDuration);
        EndInvincibility();
    }
    
    /// <summary>
    /// 무적 시간 종료
    /// </summary>
    private void EndInvincibility()
    {
        _isInvincible = false;
        
        // 깜박임 효과 중지
        if (_blinkCoroutine != null)
        {
            StopCoroutine(_blinkCoroutine);
            _blinkCoroutine = null;
        }
        
        // 스프라이트 확실히 보이게 설정
        if (_spriteRenderer != null)
        {
            _spriteRenderer.enabled = true;
        }

        EventBus.PublishPlayerHpChanged(_currentHealth);
    }
    
    /// <summary>
    /// 회복 처리
    /// </summary>
    public void Heal(int amount)
    {
        _currentHealth += amount;
        _currentHealth = Mathf.Min(_maxHealth, _currentHealth);
        _healedAmount += amount;
        if(_healedAmount >= 100)
        {
            SteamAchievement.Achieve("ACH_BTL_HEAL");
        }
        EventBus.PublishPlayerHpChanged(_currentHealth);
    }

    /// <summary>
    /// 사망 처리
    /// </summary>
    private void Die()
    {
        Debug.Log("플레이어 사망");
        
        // 마지막 피격 패턴을 킬러 패턴으로 기록
        if (!string.IsNullOrEmpty(_lastHitPattern))
        {
            BossPatternLogger.Instance.LogPlayerDeath(_lastHitPattern);
        }
        
        FindAnyObjectByType<StageHandler>().Player.Animator.SetTrigger("Death");
        
        // 무적 시간 강제 종료 (죽었으니까)
        EndInvincibility();
        
        // 격자에 남아있는 이동불가 제거
        GridManager.Instance.ClearAllUnmovableGridPositions();
        
        FindAnyObjectByType<StageHandler>().Player.Animator.SetTrigger("Death");
        EventBus.PublishPlayerDeath();
    }
    
    /// <summary>
    /// 무적 시간 강제 종료 (외부에서 호출 가능)
    /// </summary>
    public void ForceEndInvincibility()
    {
        EndInvincibility();
    }

    /// <summary>
    /// 테스트용 체력 설정
    /// </summary>
    /// <param name="hp">설정할 체력</param>
    public void TestHpSetting(int hp)
    {
        _currentHealth = hp;
    }
}