using UnityEngine;
using System.Collections;

/// <summary>
/// 플레이어 체력 관리 시스템 (무적 시간 및 깜박임 포함)
/// </summary>
public class PlayerHp : MonoBehaviour
{
    private PlayerShield _playerShield;
    private PlayerProtection _playerProtection;
    private SpriteRenderer _spriteRenderer;

    
    private int _maxHealth = 100;
    [SerializeField]
    private int _currentHealth;
    
    [Header("무적 시간 설정")]
    public float invincibilityDuration = 0.5f;
    public int blinkCount = 5; // 총 깜박임 횟수
    
    private bool _isInvincible = false;
    private Coroutine _blinkCoroutine;

    public int MaxHealth { get => _maxHealth; set => _maxHealth = value; }
    public int CurrentHealth { get => _currentHealth; set => _currentHealth = value; }
    public bool IsInvincible { get => _isInvincible; }

   

    private void Awake()
    {
        _playerShield = GetComponent<PlayerShield>();
        _playerProtection = GetComponent<PlayerProtection>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (_spriteRenderer == null)
        {
            Debug.LogError("PlayerHealth: SpriteRenderer component not found!");
        }
    }

    private void Start()
    {
        _currentHealth = _maxHealth;
    }
    
    /// <summary>
    /// 데미지 처리 (무적 시간 포함)
    /// </summary>
    public void TakeDamage(int damage)
    {
        // 무적 상태면 데미지 무시
        if (_isInvincible)
            return;

        // 보호 상태면 보호막량 감소
        if (_playerProtection.TryProtectionBlock(damage))
            return;

        // 방어 상태면 방어막량 감소
        if (_playerShield.TryShieldBlock(damage)) 
            return;

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
        else
        {
            // 살아있으면 피격 처리 및 무적 시간 시작
            FindAnyObjectByType<StageHandler>().Player.Animator.SetTrigger("Damaged");
            SoundManager.Instance.PlayPlayerSound("PlayerDamage");
            
            StartInvincibility();
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
        EventBus.PublishPlayerHpChanged(_currentHealth);
        
    }

    /// <summary>
    /// 사망 처리
    /// </summary>
    private void Die()
    {
        Debug.Log("플레이어 사망");
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
}