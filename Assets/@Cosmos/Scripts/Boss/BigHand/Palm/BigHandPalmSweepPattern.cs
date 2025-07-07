using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 손바닥이 파도처럼 쓸어내는 패턴 - 999888777 형태
/// </summary>
public class BigHandPalmSweepPattern : IBossAttackPattern
{
    private GameObject _palmFromLeftPrefab;
    private GameObject _palmFromRightPrefab;
    private GameObject _attackEffectPrefab;
    private GameObject _palmObject;
    private bool _isLeftToRight;
    private int _damage;
    
    public string PatternName => "손바닥_쓸어내기";

    public BigHandPalmSweepPattern(GameObject palmFromLeftPrefab, GameObject palmFromRightPrefab, GameObject attackEffectPrefab, int damage)
    {
        _palmFromLeftPrefab = palmFromLeftPrefab;
        _palmFromRightPrefab = palmFromRightPrefab;
        _attackEffectPrefab = attackEffectPrefab;
        _damage = damage;
    }

    public bool CanExecute(BaseBoss boss)
    {
        return boss != null &&
               _palmFromLeftPrefab != null &&
               _palmFromRightPrefab != null &&
               _attackEffectPrefab != null &&
               GridManager.Instance != null &&
               boss.BombHandler != null &&
               boss.BombHandler.PlayerController != null;
    }

    public IEnumerator Execute(BaseBoss boss)
    {
        boss.SetAnimationTrigger("Attack");
        Debug.Log("손바닥 쓸어내기 패턴 시작");

        // 플레이어 위치에 따라 방향 결정
        _isLeftToRight = boss.BombHandler.PlayerController.CurrentX <= 4;

        // 손바닥 프리팹 생성
        _palmObject = Object.Instantiate(
            _isLeftToRight ? _palmFromLeftPrefab : _palmFromRightPrefab,
            GetStartPosition(),
            Quaternion.identity
        );

        // 손이 완전히 지나갈 때까지 기다림
        Vector3 endPosition = GetEndPosition();
        float handMoveTime = 1f; // 손이 지나가는 시간
        yield return boss.StartCoroutine(SmoothMovePalm(boss,endPosition, handMoveTime));

        // 손바닥 제거
        if (_palmObject != null)
        {
            Object.Destroy(_palmObject);
            _palmObject = null;
        }

        // 손이 지나간 후 각 열을 순차적으로 공격
        for (int step = 0; step < 9; step++)
        {
            int currentColumn = _isLeftToRight ? step : (8 - step);
            yield return boss.StartCoroutine(AttackColumn(boss, currentColumn, step));
        }

        Debug.Log("손바닥 쓸어내기 패턴 완료");
    }

    private IEnumerator AttackColumn(BaseBoss boss, int columnX, int step)
    {
        int attackCount = GetAttackCountForStep(step);

        // 공격할 셀들 계산
        List<Vector3Int> attackCells = GetAttackCells(columnX, step);

        // 각 셀에 대해 FixedBomb으로 공격
        foreach (Vector3Int cell in attackCells)
        {
            boss.StartCoroutine(PlayAttackSound("BigHandAttackActivate", 0.8f));

            boss.BombHandler.ExecuteFixedBomb(
                new List<Vector3Int> { new Vector3Int(0, 0, 0) },
                cell,
                _attackEffectPrefab,
                warningDuration: 0.8f,
                explosionDuration: 0.4f,
                damage: _damage
            );
        }

        yield return new WaitForSeconds(0.2f);
    }

    /// <summary>
    /// 해당 열의 공격 대상 셀들 반환
    /// </summary>
    private List<Vector3Int> GetAttackCells(int columnX, int step)
    {
        List<Vector3Int> cells = new List<Vector3Int>();
        int attackCount = GetAttackCountForStep(step);

        // 아래(Y=0)부터 attackCount만큼 공격
        for (int y = 0; y < attackCount; y++)
        {
            cells.Add(new Vector3Int(columnX, y, 0));
        }

        return cells;
    }

    private int GetAttackCountForStep(int step)
    {
        // 양방향 모두 999888777 패턴 (1~3번째:9칸, 4~6번째:8칸, 7~9번째:7칸)
        if (step <= 2) return 9;
        else if (step <= 5) return 8;
        else return 7;
    }

    private Vector3 GetStartPosition()
    {
        if (_isLeftToRight)
        {
            // 왼쪽 화면 밖 위쪽에서 시작
            return GridManager.Instance.GridToWorldPosition(new Vector3Int(-20, 9, 0));
        }
        else
        {
            // 오른쪽 화면 밖 위쪽에서 시작
            return GridManager.Instance.GridToWorldPosition(new Vector3Int(20, 9, 0));
        }
    }

    /// <summary>
    /// 격자 영역 진입 위치 - 첫 번째 공격 열 위쪽
    /// </summary>
    private Vector3 GetGridEntryPosition()
    {
        if (_isLeftToRight)
        {
            // 왼쪽 첫 번째 열(X=0) 위쪽
            return GridManager.Instance.GridToWorldPosition(new Vector3Int(0, 9, 0));
        }
        else
        {
            // 오른쪽 첫 번째 열(X=8) 위쪽
            return GridManager.Instance.GridToWorldPosition(new Vector3Int(8, 9, 0));
        }
    }

    /// <summary>
    /// 손바닥 이동 끝 위치 계산 - 점점 내려가는 파도 모양
    /// </summary>
    private Vector3 GetEndPosition()
    {
        if (_isLeftToRight)
        {
            // 오른쪽 끝 아래쪽으로 (999888777 - 마지막이 7이므로 낮게)
            return GridManager.Instance.GridToWorldPosition(new Vector3Int(8, 3, 0));
        }
        else
        {
            // 왼쪽 끝 아래쪽으로 (999888777 - 마지막이 7이므로 낮게)
            return GridManager.Instance.GridToWorldPosition(new Vector3Int(0, 3, 0));
        }
    }

    /// <summary>
    /// 손바닥을 특정 위치로 이동 (일회성)
    /// </summary>
    private IEnumerator MovePalmTo(Vector3 targetPosition, float duration)
    {
        if (_palmObject == null) yield break;

        Vector3 startPosition = _palmObject.transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration && _palmObject != null)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / duration;
            float smoothProgress = Mathf.SmoothStep(0f, 1f, progress);
            
            _palmObject.transform.position = Vector3.Lerp(startPosition, targetPosition, smoothProgress);
            yield return null;
        }

        if (_palmObject != null)
        {
            _palmObject.transform.position = targetPosition;
        }
    }

    /// <summary>
    /// 손바닥을 부드럽게 이동시키는 코루틴
    /// </summary>
    private IEnumerator SmoothMovePalm(BaseBoss boss, Vector3 endPosition, float duration)
    {
        if (_palmObject == null) yield break;

        Vector3 startPosition = _palmObject.transform.position;
        float elapsedTime = 0f;

        boss.StartCoroutine(PlayAttackSound("BigHandSlideActivate", 0.8f));

        while (elapsedTime < duration && _palmObject != null)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / duration;
            
            // 부드러운 곡선 이동
            float smoothProgress = Mathf.SmoothStep(0f, 1f, progress);
            _palmObject.transform.position = Vector3.Lerp(startPosition, endPosition, smoothProgress);
            
            yield return null;
        }

        if (_palmObject != null)
        {
            _palmObject.transform.position = endPosition;
        }
    }

    public void Cleanup()
    {
        if (_palmObject != null)
        {
            Object.Destroy(_palmObject);
            _palmObject = null;
        }

        Debug.Log("손바닥 쓸어내기 패턴 정리 완료");
    }

    public IEnumerator PlayAttackSound(string SoundName, float BombTime)
    {
        yield return new WaitForSeconds(BombTime); // 예시로 빈 코루틴 반환
        SoundManager.Instance.BigHandSoundClip(SoundName);
    }
}