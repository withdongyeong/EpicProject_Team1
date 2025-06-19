using System.Collections;
using UnityEngine;

namespace Cosmos.Scripts.Boss.OrcMage
{
    /// <summary>
    /// 오크 메이지 패턴1 - 행별 개구리 소환 (1행→9행→9행→1행)
    /// </summary>
    public class OrcMagePattern1 : IBossAttackPattern
    {
        private GameObject _frogPrefab;
        private float _summonInterval;

        public string PatternName => "OrcMagePattern1_RowSummon";

        /// <summary>
        /// 오크 메이지 패턴1 생성자
        /// </summary>
        /// <param name="frogPrefab">소환할 개구리 프리팹</param>
        /// <param name="summonInterval">각 소환 사이 간격 (기본값: 0.3초)</param>
        public OrcMagePattern1(GameObject frogPrefab, float summonInterval = 0.3f)
        {
            _frogPrefab = frogPrefab;
            _summonInterval = summonInterval;
            Debug.Log($"OrcMagePattern1 created with interval: {_summonInterval}s");
        }

        /// <summary>
        /// 패턴 실행 가능 여부 확인
        /// </summary>
        /// <param name="boss">보스 객체</param>
        /// <returns>실행 가능 여부</returns>
        public bool CanExecute(BaseBoss boss)
        {
            return boss != null && _frogPrefab != null && boss.BombHandler != null;
        }

        /// <summary>
        /// 패턴 실행 - 1행→9행, 9행→1행 순차 소환
        /// </summary>
        /// <param name="boss">보스 객체</param>
        public IEnumerator Execute(BaseBoss boss)
        {
            Debug.Log("OrcMagePattern1.Execute: Starting row-by-row frog summoning");
            
            // 공격 애니메이션 트리거
            boss.SetAnimationTrigger("Attack2");
            
            // 1단계: 1행 → 9행 순차 소환
            yield return SummonFromTopToBottom(boss);
            
            // 2단계: 9행 → 1행 순차 소환  
            yield return SummonFromBottomToTop(boss);
            
            Debug.Log("OrcMagePattern1.Execute: All frogs summoned, pattern completed");
        }

        /// <summary>
        /// 1행 → 9행 순차 소환
        /// </summary>
        /// <param name="boss">보스 객체</param>
        private IEnumerator SummonFromTopToBottom(BaseBoss boss)
        {
            Debug.Log("OrcMagePattern1: Starting 1행→9행 summoning");
            
            for (int row = 0; row < 9; row++)
            {
                // 해당 행의 오른쪽 끝에 전조 표시
                yield return ShowWarningForRow(boss, row, WarningType.Type2);
                
                // 보스 위치 기준으로 상대적 위치 계산 (보스 앞부터 뒤까지)
                Vector3 summonPos = boss.transform.position + new Vector3(0, row - 4, 0); // -4~+4 범위
                
                // 개구리 소환
                GameObject frog = Object.Instantiate(_frogPrefab, summonPos, Quaternion.identity);
                Debug.Log($"OrcMagePattern1: Frog summoned at relative row {row - 4} (world: {summonPos})");
                
                // 다음 소환까지 대기
                if (row < 8) // 마지막이 아니면 대기
                {
                    yield return new WaitForSeconds(_summonInterval);
                }
            }
            
            Debug.Log("OrcMagePattern1: 1행→9행 summoning completed");
        }

        /// <summary>
        /// 9행 → 1행 순차 소환
        /// </summary>
        /// <param name="boss">보스 객체</param>
        private IEnumerator SummonFromBottomToTop(BaseBoss boss)
        {
            Debug.Log("OrcMagePattern1: Starting 9행→1행 summoning");
            
            for (int row = 8; row >= 0; row--)
            {
                // 해당 행의 오른쪽 끝에 전조 표시
                yield return ShowWarningForRow(boss, row, WarningType.Type2);
                
                // 보스 위치 기준으로 상대적 위치 계산 (보스 뒤부터 앞까지)
                Vector3 summonPos = boss.transform.position + new Vector3(0, row - 4, 0); // +4~-4 범위
                
                // 개구리 소환
                GameObject frog = Object.Instantiate(_frogPrefab, summonPos, Quaternion.identity);
                Debug.Log($"OrcMagePattern1: Frog summoned at relative row {row - 4} (world: {summonPos})");
                
                // 다음 소환까지 대기
                if (row > 0) // 마지막이 아니면 대기
                {
                    yield return new WaitForSeconds(_summonInterval);
                }
            }
            
            Debug.Log("OrcMagePattern1: 9행→1행 summoning completed");
        }

        /// <summary>
        /// 특정 행의 오른쪽 끝에 전조 표시
        /// </summary>
        /// <param name="boss">보스 객체</param>
        /// <param name="row">행 번호 (0~8)</param>
        private IEnumerator ShowWarningForRow(BaseBoss boss, int row, WarningType type)
        {
            // 해당 행의 오른쪽 끝 위치 (8열)
            Vector3Int warningPos = new Vector3Int(8, row, 0);
            
            // Type3 전조 0.2초 동안 표시
            boss.BombHandler.ShowWarningOnly(warningPos, 0.2f, type);
            
            // 전조 표시 시간 대기
            yield return new WaitForSeconds(0.2f);
            
            Debug.Log($"OrcMagePattern1: Warning shown for row {row + 1} at column 9");
        }
    }
}