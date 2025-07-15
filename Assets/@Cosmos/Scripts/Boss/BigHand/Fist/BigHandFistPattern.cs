using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 주먹이 위에서 내려와서 가로7칸 공격하는 패턴
/// </summary>
public class BigHandFistPattern : IBossAttackPattern
{
    private GameObject _fistPrefab;
    private GameObject _attackEffectPrefab;
    private GameObject _fistObject;
    private List<Vector3Int> _blockedPositions;
    private int _damage;

    public string PatternName => "주먹_공격";
    
    public BigHandFistPattern(GameObject fistPrefab, GameObject attackEffectPrefab, int damage)
    {
        _fistPrefab = fistPrefab;
        _attackEffectPrefab = attackEffectPrefab;
        _blockedPositions = new List<Vector3Int>();
        _damage = damage;
    }

    public bool CanExecute(BaseBoss boss)
    {
        return boss != null && 
               _fistPrefab != null && 
               _attackEffectPrefab != null &&
               GridManager.Instance != null &&
               boss.BombHandler != null;
    }

    /// <summary>
    /// 주먹 공격 패턴 실행 - 위에서 내려와서 가로7칸 타격
    /// </summary>
    public IEnumerator Execute(BaseBoss boss)
    {
        boss.SetAnimationTrigger("Attack");
        
        Debug.Log("주먹 공격 패턴 시작");
        
        yield return boss.StartCoroutine(LaunchFistAttack(boss));
        
        Debug.Log("주먹 공격 패턴 완료");
    }

    /// <summary>
    /// 주먹 공격 실행
    /// </summary>
    private IEnumerator LaunchFistAttack(BaseBoss boss)
    {
        var bigHand = boss as BigHand;
        if (bigHand?.PlannedFistArea == null || bigHand.PlannedFistArea.Count == 0)
        {
            Debug.LogError("주먹 영역 정보가 없습니다!");
            yield break;
        }

        Vector3Int centerColumn = bigHand.PlannedFistCenterColumn;
        List<Vector3Int> fistArea = bigHand.PlannedFistArea;
        
        Debug.Log($"주먹 공격 실행: 중심={centerColumn}, 영역 개수={fistArea.Count}");
        
        // 주먹 시작 위치와 목표 위치 계산
        Vector3 targetWorldPos = GridManager.Instance.GridToWorldPosition(centerColumn);
        Vector3 startPos = targetWorldPos + Vector3.up * 25f; // 화면 위쪽에서 시작
        
        // 주먹 오브젝트 생성
        _fistObject = Object.Instantiate(_fistPrefab, startPos, Quaternion.identity);
        
        // 주먹 영역 전조 + 공격 (벽 생성 없이)
        List<Vector3Int> singlePointShape = new List<Vector3Int> { new Vector3Int(0, 0, 0) };

        boss.StartCoroutine(PlayAttackSound("BigHandFistActivate", 1f));

        foreach (Vector3Int pos in fistArea)
        {
            boss.BombHandler.ExecuteWarningThenDamage(
                singlePointShape, 
                pos, 
                warningDuration: 1f, 
                damage: _damage, 
                warningType: WarningType.Type2);
        }
        
        float duration = 1f;
        float elapsedTime = 0f;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.SmoothStep(0f, 1f, elapsedTime / duration);
            
            if (_fistObject != null)
                _fistObject.transform.position = Vector3.Lerp(startPos, targetWorldPos, progress);
            
            yield return null;
        }
        
        if (_fistObject != null)
            _fistObject.transform.position = targetWorldPos;
        
        // 이동차단 설정 제거 - 단순히 시각적 효과만
        
        // 보스에 주먹 정보 저장 (복귀용)
        if (bigHand != null)
        {
            bigHand.FistCenterPosition = centerColumn;
            bigHand.FistObject = _fistObject;
        }
        
        Debug.Log($"주먹 꽂기 완료 (이동차단 없음)");
        
        // 충격파와 복귀를 동시에 시작
        boss.StartCoroutine(ExecuteShockwaveFromCenter(boss, centerColumn));
        yield return boss.StartCoroutine(ReturnFistImmediately(bigHand));
    }

    /// <summary>
    /// 주먹을 즉시 복귀시키는 메서드
    /// </summary>
    private IEnumerator ReturnFistImmediately(BigHand bigHand)
    {
        if (bigHand?.FistObject == null)
        {
            Debug.LogWarning("복귀할 주먹이 없습니다!");
            yield break;
        }

        Vector3 currentPos = bigHand.FistObject.transform.position;
        Vector3 exitPos = currentPos + Vector3.up * 25f; // 위쪽으로 복귀
        
        Debug.Log("주먹 즉시 복귀 시작");
        
        // 복귀 애니메이션 (빠르게)
        float duration = 1.0f;
        float elapsedTime = 0f;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.SmoothStep(0f, 1f, elapsedTime / duration);
            
            if (bigHand.FistObject != null)
                bigHand.FistObject.transform.position = Vector3.Lerp(currentPos, exitPos, progress);
            
            yield return null;
        }
        
        // 주먹 오브젝트 제거
        if (bigHand.FistObject != null)
        {
            Object.Destroy(bigHand.FistObject);
            bigHand.FistObject = null;
            Debug.Log("주먹 즉시 복귀 완료");
        }
        
        // 정보 정리
        if (bigHand.PlannedFistArea != null)
        {
            bigHand.PlannedFistArea.Clear();
            Debug.Log("계획된 주먹 영역 정보 정리 완료");
        }
        bigHand.FistCenterPosition = Vector3Int.zero;
    }

    /// <summary>
    /// 주먹 중심에서 퍼져나가는 충격파 웨이브
    /// </summary>
    private IEnumerator ExecuteShockwaveFromCenter(BaseBoss boss, Vector3Int fistCenter)
    {
        Debug.Log($"충격파 시작: 중심 {fistCenter}");
        
        // 거리별로 웨이브 확산
        for (int distance = 1; distance <= 6; distance++)
        {
            List<Vector3Int> wavePositions = GetPositionsAtDistance(fistCenter, distance);
            
            if (wavePositions.Count > 0)
            {
                Debug.Log($"충격파 거리 {distance}: {wavePositions.Count}칸");
                boss.StartCoroutine(PlayAttackSound("BigHandAttackActivate", 1f));

                // 해당 거리의 모든 위치에 동시 공격
                foreach (Vector3Int pos in wavePositions)
                {
                    boss.BombHandler.ExecuteFixedBomb(
                        new List<Vector3Int> { new Vector3Int(0, 0, 0) },
                        pos,
                        _attackEffectPrefab,
                        warningDuration: 1f,
                        explosionDuration: 0.5f,
                        damage: _damage
                    );
                }
                
                yield return new WaitForSeconds(boss.Beat/4); // 웨이브 간격
            }
        }
        
        Debug.Log("충격파 완료");
    }
    
    /// <summary>
    /// 중심점에서 특정 거리에 있는 모든 위치 계산
    /// </summary>
    private List<Vector3Int> GetPositionsAtDistance(Vector3Int center, int distance)
    {
        List<Vector3Int> positions = new List<Vector3Int>();
        
        // 맨하탄 거리 기준으로 계산
        for (int x = 0; x < 9; x++)
        {
            for (int y = 0; y < 9; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                int manhattanDistance = Mathf.Abs(pos.x - center.x) + Mathf.Abs(pos.y - center.y);
                
                if (manhattanDistance == distance)
                {
                    positions.Add(pos);
                }
            }
        }
        
        return positions;
    }

    public void Cleanup()
    {
        // 이동차단 해제 코드 제거 (더 이상 사용하지 않음)
        _blockedPositions.Clear();
        
        if (_fistObject != null)
        {
            Object.Destroy(_fistObject);
            _fistObject = null;
        }
        
        Debug.Log("주먹 공격 패턴 정리 완료");
    }

    public IEnumerator PlayAttackSound(string SoundName, float BombTime)
    {
        yield return new WaitForSeconds(BombTime); // 예시로 빈 코루틴 반환
        SoundManager.Instance.BigHandSoundClip(SoundName);
    }

}