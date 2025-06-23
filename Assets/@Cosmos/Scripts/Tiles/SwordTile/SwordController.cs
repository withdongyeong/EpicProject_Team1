using UnityEngine;
using System;

public class SwordController : MonoBehaviour
{
    private GameObject _hitEffect;

    /// <summary>
    /// 스킬 타입
    /// </summary>
    private enum SkillType
    {
        Dash,
        BladeStorm
    }

    private SkillType skillType = SkillType.Dash;
    
    /// <summary>
    /// 검의 현재 상태
    /// </summary>
    private SwordState currentState = SwordState.Flying;
    
    /// <summary>
    /// 검의 데미지
    /// </summary>
    private int _damage = 5;
    
    /// <summary>
    /// 비행 속도
    /// </summary>
    private float flySpeed = 8f;
    
    /// <summary>
    /// 방향 전환 속도 (각속도)
    /// </summary>
    private float turnSpeed = 270f;
    
    /// <summary>
    /// 현재 비행 방향 (각도, degree)
    /// </summary>
    private float currentDirection = 0f;
    
    /// <summary>
    /// 스킬 돌진 속도
    /// </summary>
    private float skillDashSpeed = 30f;
    
    /// <summary>
    /// 스킬 돌진 지속 시간
    /// </summary>
    private float skillDashDuration = 1f;
    
    /// <summary>
    /// 스킬 돌진 타이머
    /// </summary>
    private float skillDashTimer = 0f;
    
    /// <summary>
    /// 플레이어 참조
    /// </summary>
    private Transform player;
    
    /// <summary>
    /// 리지드바디 컴포넌트
    /// </summary>
    private Rigidbody2D rb;
    
    /// <summary>
    /// 스프라이트 렌더러
    /// </summary>
    private SpriteRenderer spriteRenderer;
    
    /// <summary>
    /// 콜라이더
    /// </summary>
    private Collider2D swordCollider;
    
    // Properties
    /// <summary>
    /// 검의 현재 상태 프로퍼티
    /// </summary>
    public SwordState CurrentState { get => currentState; private set => currentState = value; }
    
    /// <summary>
    /// 검의 데미지 프로퍼티
    /// </summary>
    public int Damage { get => _damage; set => _damage = value; }
    
    /// <summary>
    /// 비행 속도 프로퍼티
    /// </summary>
    public float FlySpeed { get => flySpeed; set => flySpeed = value; }
    
    /// <summary>
    /// 방향 전환 속도 프로퍼티
    /// </summary>
    public float TurnSpeed { get => turnSpeed; set => turnSpeed = value; }
    
    /// <summary>
    /// 현재 비행 방향 프로퍼티
    /// </summary>
    public float CurrentDirection { get => currentDirection; set => currentDirection = value; }

    /// <summary>
    /// 칼날폭풍 목표 위치
    /// </summary>
    private Vector3 bladeStormPosition;

    /// <summary>
    /// 칼날폭풍 단계 (0: 이동, 1: 회전, 2: 흩어짐)
    /// </summary>
    private int bladeStormPhase = 0;

    /// <summary>
    /// 칼날폭풍 타이머
    /// </summary>
    private float bladeStormTimer = 0f;

    /// <summary>
    /// 칼날폭풍 회전 각도
    /// </summary>
    private float bladeStormAngle = 0f;

    /// <summary>
    /// 칼날폭풍 회전 반지름
    /// </summary>
    private float bladeStormRadius = 2f;

    /// <summary>
    /// 각 검의 고유 오프셋 각도
    /// </summary>
    private float bladeStormOffsetAngle;
    
    /// <summary>
    /// 검 상태 열거형
    /// </summary>
    public enum SwordState
    {
        Flying,     // 비행 상태
        Skill       // 스킬 상태
    }

    /// <summary>
    /// 컴포넌트 초기화
    /// </summary>
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        swordCollider = GetComponent<Collider2D>();
        
        // 랜덤한 초기 방향 설정
        currentDirection = UnityEngine.Random.Range(0f, 360f);
        
        // 각 검마다 다른 속도 설정
        flySpeed = UnityEngine.Random.Range(6f, 12f);
        turnSpeed = UnityEngine.Random.Range(120f, 540f);
    }

    /// <summary>
    /// 게임 시작 시 초기화
    /// </summary>
    private void Start()
    {
        // 플레이어 찾기
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        _hitEffect = Resources.Load("Prefabs/HitEffect/HitEffect_Sword") as GameObject;

        // 5초 후에 오브젝트 파괴
        Destroy(gameObject, 7f);
    }

    ///// <summary>
    ///// 자식 스프라이트 렌더러들에 랜덤 색상 적용
    ///// </summary>
    //private void ApplyRandomColor()
    //{
    //    // 랜덤 색상 생성
    //    Color randomColor = new Color(
    //        UnityEngine.Random.Range(0.3f, 1f), // R
    //        UnityEngine.Random.Range(0.3f, 1f), // G
    //        UnityEngine.Random.Range(0.3f, 1f), // B
    //        1f // A (완전 불투명)
    //    );
    
    //    // 모든 자식의 SpriteRenderer 찾아서 같은 색 적용
    //    SpriteRenderer[] childRenderers = GetComponentsInChildren<SpriteRenderer>();
    
    //    foreach (SpriteRenderer renderer in childRenderers)
    //    {
    //        renderer.color = randomColor;
    //    }
    //}

    /// <summary>
    /// 매 프레임 업데이트
    /// </summary>
    private void Update()
    {
        HandleCurrentState();
    }

    /// <summary>
    /// 현재 상태에 따른 행동 처리
    /// </summary>
    private void HandleCurrentState()
    {
        switch (currentState)
        {
            case SwordState.Flying:
                HandleFlyingState();
                break;
            case SwordState.Skill:
                HandleSkillState();
                break;
        }
    }

    /// <summary>
    /// 비행 상태 처리
    /// </summary>
    private void HandleFlyingState()

    {
        Vector3 centerPosition = GridManager.Instance.GridToWorldPosition(new Vector3Int(4, 4, 0));

        // 플레이어를 향해 방향 조정
        Vector3 toPlayer = (centerPosition - transform.position);
        if (toPlayer.magnitude > 0.1f)
        {
            float targetDirection = Mathf.Atan2(toPlayer.y, toPlayer.x) * Mathf.Rad2Deg;
            
            // 부드럽게 방향 전환
            currentDirection = Mathf.LerpAngle(currentDirection, targetDirection, turnSpeed * Time.deltaTime / 250f);
        }
        
        // 현재 방향으로 이동
        Vector2 moveDirection = new Vector2(
            Mathf.Cos(currentDirection * Mathf.Deg2Rad),
            Mathf.Sin(currentDirection * Mathf.Deg2Rad)
        );
        
        rb.linearVelocity = moveDirection * flySpeed;
         
        // 검의 회전을 이동 방향에 맞춤
        transform.rotation = Quaternion.Euler(0, 0, currentDirection);
    }

    /// <summary>
    /// 스킬 상태 처리
    /// </summary>
    private void HandleSkillState()
    {
        if (skillType == SkillType.Dash)
        {
            HandleDashSkill();
        }
        else if (skillType == SkillType.BladeStorm)
        {
            HandleBladeStormSkill();
        }
    }

    /// <summary>
    /// 대시 스킬 처리 (기존 코드)
    /// </summary>
    private void HandleDashSkill()
    {
        skillDashTimer -= Time.deltaTime;
      
        Vector2 moveDirection = new Vector2(
            Mathf.Cos(currentDirection * Mathf.Deg2Rad),
            Mathf.Sin(currentDirection * Mathf.Deg2Rad)
        );
    
        rb.linearVelocity = moveDirection * skillDashSpeed;
    
        if (skillDashTimer <= 0f)
        {
            ReturnToFlying();
        }
    }

    /// <summary>
    /// 칼날폭풍 스킬 처리
    /// </summary>
    private void HandleBladeStormSkill()
    {
        switch (bladeStormPhase)
        {
            case 0: // 목표 위치로 이동
                HandleBladeStormMovement();
                break;
            case 1: // 회전
                HandleBladeStormRotation();
                break;
            case 2: // 흩어짐
                ReturnToFlying();
                break;
        }
    }

    /// <summary>
    /// 칼날폭풍 이동 단계
    /// </summary>
    private void HandleBladeStormMovement()
    {
        Vector3 direction = (bladeStormPosition - transform.position);
        if (direction.magnitude < 0.5f)
        {
            // 도착했으면 회전 단계로
            bladeStormPhase = 1;
            bladeStormTimer = 2f; // 회전 지속시간
            return;
        }
    
        // 기존 flySpeed 사용 (또는 더 빠른 속도)
        rb.linearVelocity = direction.normalized * (skillDashSpeed > 0 ? skillDashSpeed : flySpeed);
    }

    /// <summary>
    /// 칼날폭풍 회전 단계
    /// </summary>
    private void HandleBladeStormRotation()
    {
        bladeStormTimer -= Time.deltaTime;
        bladeStormAngle += 720f * Time.deltaTime; // 초당 720도 회전
    
        // 원형으로 회전하면서 중심점 추적
        Vector3 offset = new Vector3(
            Mathf.Cos((bladeStormAngle + bladeStormOffsetAngle) * Mathf.Deg2Rad) * bladeStormRadius,
            Mathf.Sin((bladeStormAngle + bladeStormOffsetAngle) * Mathf.Deg2Rad) * bladeStormRadius,
            0f
        );
    
        Vector3 targetPos = bladeStormPosition + offset;
        Vector3 direction = (targetPos - transform.position);
    
        // 부드러운 이동
        rb.linearVelocity = direction * 10f;
    
        // 회전 방향 설정 (이동 방향 + 90도 = 접선 방향)
        Vector3 tangentDirection = new Vector3(-offset.y, offset.x, 0f).normalized;
        float angle = Mathf.Atan2(tangentDirection.y, tangentDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
        currentDirection = angle;
    
        if (bladeStormTimer <= 0f)
        {
            bladeStormPhase = 2;
        }
    }

    /// <summary>
    /// 칼날폭풍 스킬 활성화
    /// </summary>
    /// <param name="stormPosition">폭풍 중심 위치</param>
    public void ActivateBladeStorm(Vector3 stormPosition)
    {
        if (currentState == SwordState.Flying)
        {
            skillType = SkillType.BladeStorm;
            bladeStormPosition = stormPosition;
            bladeStormPhase = 0;
            bladeStormOffsetAngle = UnityEngine.Random.Range(0f, 360f); // 각 검마다 다른 시작 각도
        
            ChangeState(SwordState.Skill);
        }
    }
    /// <summary>
    /// 상태 변경
    /// </summary>
    /// <param name="newState">새로운 상태</param>
    public void ChangeState(SwordState newState)
    {
        if (currentState == newState) return;
        
        SwordState oldState = currentState;
        currentState = newState;
        
        // 상태 변경 시 처리
        OnStateEnter(newState, oldState);
    }

    /// <summary>
    /// 상태 진입 시 처리
    /// </summary>
    /// <param name="newState">새로운 상태</param>
    /// <param name="oldState">이전 상태</param>
    private void OnStateEnter(SwordState newState, SwordState oldState)
    {
        switch (newState)
        {
            case SwordState.Flying:
                rb.gravityScale = 0f;
                swordCollider.isTrigger = true;
                break;
            case SwordState.Skill:
                // 스킬 상태 진입 처리
                break;
        }
    }

    /// <summary>
    /// 스킬 상태로 전환
    /// </summary>
    /// <param name="targetPosition">스킬 목표 위치</param>
    public void ActivateSkill(Vector3 targetPosition)
    {
        if (currentState == SwordState.Flying)
        {
            // 즉시 방향 전환
            Vector3 direction = (targetPosition - transform.position).normalized;
            currentDirection = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        
            // 검 회전도 즉시 맞춤
            transform.rotation = Quaternion.Euler(0, 0, currentDirection);
        
            skillDashTimer = skillDashDuration;
            ChangeState(SwordState.Skill);
        }
    }

    /// <summary>
    /// 스킬에서 비행 상태로 복귀
    /// </summary>
    public void ReturnToFlying()
    {
        if (currentState == SwordState.Skill)
        {
            // 상태 초기화
            bladeStormPhase = 0;
            bladeStormTimer = 0f;
            skillType = SkillType.Dash; // 기본값으로 리셋
        
            ChangeState(SwordState.Flying);
        }
    }

    /// <summary>
    /// 충돌 처리
    /// </summary>
    /// <param name="other">충돌한 오브젝트</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 비행 상태에서만 몬스터 공격
        if (currentState == SwordState.Skill)
        {
            BaseBoss monster = other.GetComponent<BaseBoss>();
            if (monster != null)
            {
                monster.TakeDamage(_damage, _hitEffect);
                skillDashTimer = 0.02f;
            }
        }
    }

    /// <summary>
    /// 기즈모 그리기 (Scene 뷰에서 디버깅용)
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (currentState == SwordState.Flying)
        {
            // 현재 이동 방향 표시
            Vector3 directionVector = new Vector3(
                Mathf.Cos(currentDirection * Mathf.Deg2Rad),
                Mathf.Sin(currentDirection * Mathf.Deg2Rad),
                0f
            ) * 2f;
            
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + directionVector);
            
            // 마우스 위치 표시
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0f;
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(mousePos, 0.2f);
            
            // 속도 정보 표시
            #if UNITY_EDITOR
            UnityEditor.Handles.Label(transform.position + Vector3.up * 0.5f, $"Speed: {flySpeed:F1}");
            #endif
        }
    }
}