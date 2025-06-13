using System.Collections;
using UnityEngine;

namespace Cosmos.Scripts.Boss.OrcMage
{
    /// <summary>
    /// 오크 메이지 패턴1 - 개구리 소환
    /// </summary>
    public class OrcMagePattern1 : IBossAttackPattern
    {
        private GameObject _frogPrefab;

        public string PatternName => "OrcMagePattern1_FrogSummon";

        /// <summary>
        /// 오크 메이지 패턴1 생성자
        /// </summary>
        /// <param name="frogPrefab">소환할 개구리 프리팹</param>
        public OrcMagePattern1(GameObject frogPrefab)
        {
            _frogPrefab = frogPrefab;
            Debug.Log($"OrcMagePattern1 created with frogPrefab: {(_frogPrefab != null ? "Valid" : "NULL")}");
        }

        /// <summary>
        /// 패턴 실행 가능 여부 확인
        /// </summary>
        /// <param name="boss">보스 객체</param>
        /// <returns>실행 가능 여부</returns>
        public bool CanExecute(BaseBoss boss)
        {
            return boss != null && _frogPrefab != null;
        }

        /// <summary>
        /// 패턴 실행 - 개구리 한 마리 소환 (보스 위치에)
        /// </summary>
        /// <param name="boss">보스 객체</param>
        public IEnumerator Execute(BaseBoss boss)
        {
            Debug.Log("OrcMagePattern1.Execute: Starting frog summoning");
            
            // 공격 애니메이션 트리거
            boss.SetAnimationTrigger("Attack1");
            
            // 보스 위치에서 개구리 소환
            GameObject summonedFrog = Object.Instantiate(_frogPrefab, boss.transform.position, Quaternion.identity);
            Debug.Log($"OrcMagePattern1: Frog summoned at boss position {boss.transform.position}");
            
            // 패턴 완료 대기
            yield return new WaitForSeconds(0.5f);
            
            Debug.Log("OrcMagePattern1.Execute: Frog summoning completed");
        }
    }
}