using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 모든 보스의 기본 클래스 (새로운 패턴 시스템 적용)
/// </summary>
public abstract class BaseBoss : MonoBehaviour
{
    [Header("기본 스탯")]
    private int _maxHealth = 100;
    private int _currentHealth;
    private bool _isDead = false;
    
    [Header("상태 이상 클래스")]
    private BossDebuffs _bossDebuff;
    private bool _isStopped = false; // 공격 중지 여부

    // 컴포넌트 참조
    private GridManager _gridSystem;
    private BombAvoidanceManager _bombManager;
    
    // 새로운 패턴 시스템
    private List<ExecutableUnit> _executableUnits;
    private Coroutine _attackCoroutine;

    private Animator _animator;

    // Properties
    /// <summary>
    /// 최대 체력 프로퍼티
    /// </summary>
    public int MaxHealth { get => _maxHealth; protected set => _maxHealth = value; }
    
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
    /// 그리드 시스템 프로퍼티
    /// </summary>
    public GridManager GridSystem { get => _gridSystem; }

    /// <summary>
    /// 폭탄 회피 매니저 프로퍼티
    /// </summary>
    public BombAvoidanceManager BombManager { get => _bombManager; }

    /// <summary>
    /// 애니메이터 프로퍼티
    /// </summary>
    public Animator Animator { get => _animator; }
    

    /// <summary>
    /// 보스 초기화
    /// </summary>
    protected virtual void Start()
    {
        _currentHealth = _maxHealth;
        
        // 컴포넌트 참조 설정
        _gridSystem = GridManager.Instance;
        _bossDebuff = GetComponent<BossDebuffs>();
        _animator = GetComponent<Animator>();
        _bombManager = FindAnyObjectByType<BombAvoidanceManager>();
        
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
        _attackCoroutine = StartCoroutine(AttackRoutine());

        // 상태이상 적용 루틴 시작  
        StartCoroutine(ApplyDebuffsRoutine());

        Debug.Log($"{GetType().Name} spawned with {_maxHealth} HP and {_executableUnits.Count} executable units!");
        
        // BombManager 할당 확인
        if (_bombManager == null)
        {
            Debug.LogWarning($"{GetType().Name}: BombAvoidanceManager not found in scene!");
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
        
        Debug.Log($"Individual pattern added with {intervalAfterExecution}s interval");
    }
    
    /// <summary>
    /// 패턴 그룹 빌더 생성
    /// </summary>
    /// <returns>패턴 그룹 빌더</returns>
    protected PatternGroupBuilder AddGroup()
    {
        Debug.Log($"{GetType().Name}: Creating new PatternGroupBuilder");
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
            Debug.Log($"{GetType().Name}: Pattern group added successfully with {patternCount} patterns. Total executable units: {_executableUnits.Count}");
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
    public virtual void TakeDamage(int damage)
    {
        if (_isDead) return;

        _currentHealth -= damage;
        _currentHealth = Mathf.Max(0, _currentHealth);
        Debug.Log("Boss 가 데미지 받음 : " + damage);
        
        if (_currentHealth <= 0)
        {
            Die();
        }
        else
        {
            _animator.SetTrigger("DamageTrigger");
        }
    }

    /// <summary>
    /// 사망 처리
    /// </summary>
    protected virtual void Die()
    {
        _isDead = true;
        Debug.Log($"{GetType().Name} DEFEATED!");
        
        // 사망 이벤트 발생
        EventBus.PublishBossDeath();
        _animator.SetTrigger("DeadTrigger");

        // GameManager에 보스 사망 알림
        StageManager stageManager = FindAnyObjectByType<StageManager>();
        if (stageManager != null)
        {
            StartCoroutine(BossDeath(stageManager));
        }
    }

    /// <summary>
    /// 보스 사망 처리 코루틴
    /// </summary>
    /// <param name="stageManager">게임 매니저</param>
    private IEnumerator BossDeath(StageManager stageManager)
    {
        yield return new WaitForSeconds(0.1f);
        stageManager.HandleBossDeath();
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
        StartCoroutine(StopAttackRoutine(time));
    }

    /// <summary>
    /// 공격 중지 루틴
    /// </summary>
    /// <param name="time">중지 시간</param>
    public IEnumerator StopAttackRoutine(float time)
    {
        _isStopped = true;
        if (_attackCoroutine != null)
        {
            StopCoroutine(_attackCoroutine);
            _attackCoroutine = null;
        }
        yield return new WaitForSeconds(time);
        _isStopped = false;
        _attackCoroutine = StartCoroutine(AttackRoutine());
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
        Debug.Log($"{GetType().Name}: AttackRoutine started");
        yield return new WaitForSeconds(1f); // 초반 딜레이
    
        while (!_isDead && !_isStopped)
        {
            if (_executableUnits.Count > 0)
            {
                Debug.Log($"{GetType().Name}: Executing random unit from {_executableUnits.Count} available units");
                yield return StartCoroutine(ExecuteRandomUnit()); // 패턴 완료까지 대기 (인터벌 포함)
            }
            else
            {
                Debug.LogWarning($"{GetType().Name}: No executable units available, waiting...");
                yield return new WaitForSeconds(1f);
            }
        }
    
        Debug.Log($"{GetType().Name}: AttackRoutine ended (isDead: {_isDead}, isStopped: {_isStopped})");
    }


    /// <summary>
    /// 랜덤 실행 단위 실행
    /// </summary>
    private IEnumerator ExecuteRandomUnit()
    {
        int randomIndex = UnityEngine.Random.Range(0, _executableUnits.Count);
        ExecutableUnit selectedUnit = _executableUnits[randomIndex];
    
        Debug.Log($"{GetType().Name}: Selected unit {randomIndex} (IsGroup: {selectedUnit.IsGroup})");
        
        if (selectedUnit.IsIndividualPattern)
        {
            Debug.Log($"{GetType().Name}: Executing individual pattern: {selectedUnit.IndividualPattern.Pattern.PatternName}");
            yield return StartCoroutine(ExecuteIndividualPattern(selectedUnit.IndividualPattern));
        }
        else if (selectedUnit.IsGroup)
        {
            Debug.Log($"{GetType().Name}: Executing pattern group with {selectedUnit.PatternGroup.Patterns.Count} patterns");
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

    /// <summary>
    /// 공격 애니메이션 트리거
    /// </summary>
    public void AttackAnimation()
    {
        _animator.SetTrigger("AttackTrigger");
    }
}