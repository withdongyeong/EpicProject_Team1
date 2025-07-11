using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

/// <summary>
/// 모든 보스의 기본 클래스 (새로운 패턴 시스템 적용)
/// </summary>
public abstract class BaseBoss : MonoBehaviour
{
    [Header("기본 스탯")]
    private int _maxHealth = 100;
    private int _weakDamage = 0;
    private int _strongDamage = 0;
    [SerializeField]
    private int _currentHealth;
    private bool _isDead = false;
    
    [Header("공통 리듬 설정")]
    public float bpm = 80f;
    
    [Header("상태 이상 클래스")]
    private BossDebuffs _bossDebuff;
    private bool _isStopped = false; // 공격 중지 여부
    private bool _unstoppable = false; // 공격 중지 가능 여부
    private bool _isHandBoss = false; // 손 보스 여부 (손 보스는 공격 중지 불가능)

    // 컴포넌트 참조
    private GridManager _gridSystem;
    private BombAvoidanceHandler _bombHandler;
    
    // 새로운 패턴 시스템
    private List<ExecutableUnit> _executableUnits;

    private Animator _animator;
    private DamageTextHandler _damageTextHandler;

    // 피격음 코루틴
    private Coroutine hitSoundCoroutine;
    
    private TotalDamageManager _totalDamageManager;

    // Properties
    
    /// <summary>
    /// BPM 기반의 박자 간격 반환 (초 단위)
    /// </summary>
    public float Beat => 60f / bpm;
    public float HalfBeat => Beat / 2f;
    
    /// <summary>
    /// 최대 체력 프로퍼티
    /// </summary>
    public int MaxHealth { get => _maxHealth; protected set => _maxHealth = value; }
    
    /// <summary>
    /// 약 데미지
    /// </summary>
    public int WeakDamage { get => _weakDamage; protected set => _weakDamage = value; }
    
    /// <summary>
    /// 강데미지
    /// </summary>
    public int StrongDamage { get => _strongDamage; protected set => _strongDamage = value; }
    
    /// <summary>
    /// 현재 체력 프로퍼티
    /// </summary>
    public int CurrentHealth { get => _currentHealth; private set => _currentHealth = value; }
    
    /// <summary>
    /// 사망 여부 프로퍼티
    /// </summary>
    public bool IsDead { get => _isDead; private set => _isDead = value; }

    /// <summary>
    /// 공격 중지 여부 프로퍼티
    /// </summary>
    public bool IsStopped { get => _isStopped; set => _isStopped = value; }

    /// <summary>
    /// 공격 중지 가능 여부 프로퍼티
    /// </summary>
    public bool Unstoppable { get => _unstoppable; set => _unstoppable = value; }

    /// <summary>
    /// 손 보스 여부 프로퍼티
    /// </summary>
    public bool IsHandBoss { get => _isHandBoss; set => _isHandBoss = value; } // 손 보스 여부 프로퍼티

    /// <summary>
    /// 그리드 시스템 프로퍼티
    /// </summary>
    public GridManager GridSystem { get => _gridSystem; }

    /// <summary>
    /// 폭탄 회피 매니저 프로퍼티
    /// </summary>
    public BombAvoidanceHandler BombHandler { get => _bombHandler; }

    /// <summary>
    /// 애니메이터 프로퍼티
    /// </summary>
    public Animator Animator { get => _animator; }


    protected virtual void Awake()
    {
        EventBus.SubscribeGameStart(Init);
        _damageTextHandler = FindAnyObjectByType<DamageTextHandler>();
    }
    /// <summary>
    /// 보스 초기화
    /// </summary>
    private void Init()
    {
        _currentHealth = _maxHealth;
        
        // 컴포넌트 참조 설정
        _gridSystem = GridManager.Instance;
        _bossDebuff = GetComponent<BossDebuffs>();
        _animator = GetComponentInChildren<Animator>();
        _bombHandler = FindAnyObjectByType<BombAvoidanceHandler>();
        _totalDamageManager = TotalDamageManager.Instance;
        
        // 패턴 리스트 초기화
        _executableUnits = new List<ExecutableUnit>();
        
        // 공격 패턴 초기화
        InitializeAttackPatterns();
        
        // 등록된 패턴이 있는지 확인
        if (_executableUnits.Count == 0)
        {
            Debug.LogWarning($"{GetType().Name}: No attack patterns registered!");
        }
        
        // 공격 루틴 시작 - 수정된 버전
        StartCoroutine(AttackRoutine());

        // 상태이상 적용 루틴 시작  
        StartCoroutine(ApplyDebuffsRoutine());
        
        // BombHandler 할당 확인
        if (_bombHandler == null)
        {
            Debug.LogWarning($"{GetType().Name}: BombAvoidanceHandler not found in scene!");
        }
    }

 
  
    /// <summary>
    /// 공격 패턴 초기화 - 상속받는 클래스에서 반드시 구현
    /// </summary>
    protected abstract void InitializeAttackPatterns();

    /// <summary>
    /// 개별 패턴 추가
    /// </summary>
    /// <param name="pattern">추가할 패턴</param>
    /// <param name="intervalAfterExecution">패턴 실행 후 대기시간</param>
    protected void AddIndividualPattern(IBossAttackPattern pattern, float intervalAfterExecution)
    {
        if (pattern == null)
        {
            Debug.LogError("Cannot add null pattern!");
            throw new ArgumentNullException(nameof(pattern));
        }

        PatternElement patternElement = new PatternElement(pattern, intervalAfterExecution);
        ExecutableUnit executableUnit = new ExecutableUnit(patternElement);
        _executableUnits.Add(executableUnit);
    }
    
    /// <summary>
    /// 패턴 그룹 빌더 생성
    /// </summary>
    /// <returns>패턴 그룹 빌더</returns>
    protected PatternGroupBuilder AddGroup()
    {
        return new PatternGroupBuilder(OnGroupBuilt);
    }

    /// <summary>
    /// 그룹 빌드 완료 시 호출되는 콜백
    /// </summary>
    /// <param name="executableUnit">빌드된 실행 단위</param>
    private void OnGroupBuilt(ExecutableUnit executableUnit)
    {
        try
        {
            if (executableUnit == null)
            {
                Debug.LogError($"{GetType().Name}.OnGroupBuilt: ExecutableUnit is null!");
                return;
            }

            if (!executableUnit.IsGroup)
            {
                Debug.LogError($"{GetType().Name}.OnGroupBuilt: ExecutableUnit is not a group!");
                return;
            }

            _executableUnits.Add(executableUnit);
            int patternCount = executableUnit.PatternGroup.Patterns.Count;
        }
        catch (Exception ex)
        {
            Debug.LogError($"{GetType().Name}.OnGroupBuilt: Error adding group - {ex.Message}\nStackTrace: {ex.StackTrace}");
        }
    }

    /// <summary>
    /// 데미지 처리
    /// </summary>
    /// <param name="damage">받을 데미지</param>
    public virtual void TakeDamage(int damage, GameObject hitObject = null)
    {
        if (_isDead) return;
        if (hitObject == null)
        {
            hitObject = Resources.Load("Prefabs/HitEffect/HitEffectBasic") as GameObject;
        }
        float randomX = UnityEngine.Random.Range(-0.5f, 0.5f);
        float randomY = UnityEngine.Random.Range(-0.5f, 0.5f);
        Vector3 hitPosition = transform.position + new Vector3(randomX, randomY, 0);
        if (hitSoundCoroutine == null)
        {
            hitSoundCoroutine = StartCoroutine(HitSoundCoroutine(hitObject));
        }
        damage = _bossDebuff.ApplyMarkEffect(damage);
        damage = _bossDebuff.ApplyPainEffect(damage);
        if (_isStopped)
        {
            damage = Mathf.CeilToInt(damage * 1.5f); // 공격 중지 상태에서는 데미지 1.5배 증가
        }
        _currentHealth -= damage;
        _currentHealth = Mathf.Max(0, _currentHealth);
        _damageTextHandler.SpawnDamageText(damage);
        _totalDamageManager.AddDamage(damage);
        
        if (_currentHealth <= 0)
        {
            Die();
        }
        else
        {
            DamageFeedback();
        }
    }

    protected virtual void Update()
    {
        if(Input.GetKeyDown(KeyCode.F7))
        {
            _currentHealth = 1;
        }
    }

    /// <summary>
    /// 피격음 코루틴(소리 중첩 방지)
    /// </summary>
    /// <param name="hitObject"></param>
    /// <returns></returns>
    private IEnumerator HitSoundCoroutine(GameObject hitObject)
    {
        Instantiate(hitObject, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(0.05f);
        hitSoundCoroutine = null;
    }

    protected virtual void DamageFeedback()
    {
        _animator.SetTrigger("DamageTrigger");
    }

    /// <summary>
    /// 사망 처리
    /// </summary>
    protected virtual void Die()
    {
        _isDead = true;
        // 애니메이터 사망처리
        _animator.SetBool("IsAlive", false);
        Debug.Log($"{GetType().Name} DEFEATED!");
        
        // 사망 이벤트 발생
        EventBus.PublishBossDeath();
        _animator.SetTrigger("DeadTrigger");

        // GameManager에 보스 사망 알림
        StageHandler stageHandler = FindAnyObjectByType<StageHandler>();
        if (stageHandler != null)
        {
            StartCoroutine(BossDeath(stageHandler));
        }
    }

    /// <summary>
    /// 보스 사망 처리 코루틴
    /// </summary>
    /// <param name="stageHandler">게임 매니저</param>
    private IEnumerator BossDeath(StageHandler stageHandler)
    {
        yield return new WaitForSeconds(0.1f);
        stageHandler.HandleBossDeath();
    }

    /// <summary>
    /// 데미지 효과 생성
    /// </summary>
    /// <param name="position">효과 생성 위치</param>
    /// <param name="effectPrefab">효과 프리팹</param>
    /// <param name="second">효과 지속시간</param>
    public void CreateDamageEffect(Vector3 position, GameObject effectPrefab, float second)
    {
        if (effectPrefab != null)
        {
            GameObject effect = Instantiate(effectPrefab, position, Quaternion.identity);
            effect.transform.GetChild(0).GetComponent<SpriteRenderer>().flipY = false;
            Destroy(effect, second);
        }
    }

    /// <summary>
    /// 반전된 데미지 효과 생성
    /// </summary>
    /// <param name="position">효과 생성 위치</param>
    /// <param name="effectPrefab">효과 프리팹</param>
    /// <param name="second">효과 지속시간</param>
    public void CreateDamageEffect_Inversion(Vector3 position, GameObject effectPrefab, float second)
    {
        if (effectPrefab != null)
        {
            GameObject effect = Instantiate(effectPrefab, position, Quaternion.identity);
            effect.transform.GetChild(0).GetComponent<SpriteRenderer>().flipY = true;
            Destroy(effect, second);
        }
    }
    

    /// <summary>
    /// 상태이상 적용 루틴
    /// </summary>
    private IEnumerator ApplyDebuffsRoutine()
    {
        yield return new WaitForSeconds(1f); // 초반 딜레이

        while (!_isDead && !_isStopped)
        {
            _bossDebuff.ApplyAllDebuffs();
            yield return new WaitForSeconds(1f);
        }
    }

    /// <summary>
    /// 공격 중지 메소드
    /// </summary>
    /// <param name="time">중지 시간</param>
    public void StopAttack(float time)
    {
        _isStopped = true;
        StopAllCoroutines();
        StartCoroutine(StopAttackRoutine(time));
    }

    /// <summary>
    /// 공격 중지 루틴
    /// </summary>
    /// <param name="time">중지 시간</param>
    public IEnumerator StopAttackRoutine(float time)
    {
        yield return new WaitForSeconds(time);
        _isStopped = false;
        StartCoroutine(AttackRoutine());
        StartCoroutine(ApplyDebuffsRoutine());
    }

    /// <summary>
    /// 상태이상 추가
    /// </summary>
    /// <param name="debuff">추가할 상태이상</param>
    public void AddDebuff(BossDebuff debuff)
    {
        _bossDebuff.AddDebuff(debuff);
    }

    /// <summary>
    /// 상태이상 개수 조회
    /// </summary>
    /// <param name="debuff">조회할 상태이상</param>
    /// <returns>상태이상 개수</returns>
    public int GetDebuffCount(BossDebuff debuff)
    {
        return _bossDebuff.Debuffs[(int)debuff];
    }

    /// <summary>
    /// 상태이상 제거
    /// </summary>
    /// <param name="debuff">제거할 디버프</param>
    public void RemoveDebuff(BossDebuff debuff)
    {
        _bossDebuff.RemoveDebuff(debuff);
    }

    /// <summary>
    /// 공격 패턴 실행 루틴
    /// </summary>
    private IEnumerator AttackRoutine()
    {
        yield return new WaitForSeconds(1f); // 초반 딜레이
    
        while (!_isDead && !_isStopped)
        {
            if (_executableUnits.Count > 0)
            {
                yield return StartCoroutine(ExecuteRandomUnit()); // 패턴 완료까지 대기 (인터벌 포함)
            }
            else
            {
                Debug.LogWarning($"{GetType().Name}: No executable units available, waiting...");
                yield return new WaitForSeconds(1f);
            }
        }
    }


    /// <summary>
    /// 랜덤 실행 단위 실행
    /// </summary>
    private IEnumerator ExecuteRandomUnit()
    {
        int randomIndex = UnityEngine.Random.Range(0, _executableUnits.Count);
        ExecutableUnit selectedUnit = _executableUnits[randomIndex];
        
        if (selectedUnit.IsIndividualPattern)
        {
            yield return StartCoroutine(ExecuteIndividualPattern(selectedUnit.IndividualPattern));
        }
        else if (selectedUnit.IsGroup)
        {
            yield return StartCoroutine(ExecutePatternGroup(selectedUnit.PatternGroup));
        }
        else
        {
            Debug.LogError($"{GetType().Name}: ExecutableUnit is neither individual nor group!");
        }
    }

    /// <summary>
    /// 개별 패턴 실행 코루틴
    /// </summary>
    /// <param name="patternElement">실행할 패턴 요소</param>
    private IEnumerator ExecuteIndividualPattern(PatternElement patternElement)
    {
        bool canExecute = patternElement.Pattern.CanExecute(this);
        
        if (canExecute)
        {
            // 패턴 실행 완료까지 대기
            yield return StartCoroutine(patternElement.Pattern.Execute(this));
        }
        
        // 패턴 완료 후 인터벌 대기
        yield return new WaitForSeconds(patternElement.IntervalAfterExecution);
    }

    /// <summary>
    /// 패턴 그룹 실행 코루틴 (순차적)
    /// </summary>
    /// <param name="patternGroup">실행할 패턴 그룹</param>
    private IEnumerator ExecutePatternGroup(PatternGroup patternGroup)
    {
        // 그룹 내 모든 패턴을 순차적으로 실행
        foreach (PatternElement patternElement in patternGroup.Patterns)
        {
            if (_isDead || _isStopped)
                break;
                
            if (patternElement.Pattern.CanExecute(this))
            {
                // 패턴 실행 완료까지 대기
                yield return StartCoroutine(patternElement.Pattern.Execute(this));
            }
            
            // 패턴 완료 후 개별 인터벌 대기
            yield return new WaitForSeconds(patternElement.IntervalAfterExecution);
        }
        
        // 그룹 완료 후 그룹 인터벌 대기
        yield return new WaitForSeconds(patternGroup.IntervalAfterGroup);
    }

    public void boolAnimation(string boolName, bool value)
    {
        _animator.SetBool(boolName, value);
    }

    /// <summary>
    /// 공격 애니메이션 트리거
    /// </summary>
    public void AttackAnimation()
    {
        _animator.SetTrigger("AttackTrigger");
    }

    /// <summary>
    /// 애니메이션 트리거 설정
    /// </summary>
    /// <param name="trigger">트리거 이름 </param>
    public void SetAnimationTrigger(string trigger)
    {
        _animator.SetTrigger(trigger);
    }
    
    public IEnumerator PlayOrcExplosionSoundDelayed(string clipName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SoundManager.Instance.OrcMageSoundClip(clipName);
    }


    private void OnDestroy()
    {
        EventBus.UnsubscribeGameStart(Init);
    }
}