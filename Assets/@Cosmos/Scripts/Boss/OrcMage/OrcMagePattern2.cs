using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cosmos.Scripts.Boss.OrcMage
{
    /// <summary>
    /// 오크 메이지 패턴2 - 플레이어 추적하는 빈 3x3 사각형
    /// </summary>
    public class OrcMagePattern2 : IBossAttackPattern
    {
        private GameObject _groundSpikePrefab;

        public string PatternName => "OrcMagePattern2_MovingSquares";

        /// <summary>
        /// 오크 메이지 패턴2 생성자
        /// </summary>
        /// <param name="groundSpikePrefab">그라운드 스파이크 프리팹</param>
        public OrcMagePattern2(GameObject groundSpikePrefab)
        {
            _groundSpikePrefab = groundSpikePrefab;
        }

        /// <summary>
        /// 패턴 실행 가능 여부 확인
        /// </summary>
        /// <param name="boss">보스 객체</param>
        /// <returns>실행 가능 여부</returns>
        public bool CanExecute(BaseBoss boss)
        {
            return boss != null && _groundSpikePrefab != null && boss.BombManager != null;
        }

        /// <summary>
        /// 패턴 실행 - 1개의 빈 사각형이 플레이어 추적 (5번 이동)
        /// </summary>
        /// <param name="boss">보스 객체</param>
        public IEnumerator Execute(BaseBoss boss)
        {
            boss.AttackAnimation();
            
            // 1개의 랜덤 시작 위치 생성
            Vector3Int currentPosition = GenerateRandomStartPosition();
            
            // 5번 이동하면서 연속 공격
            for (int moveCount = 0; moveCount < 5; moveCount++)
            {
                // 현재 플레이어 위치
                Vector3Int playerPos = boss.GridSystem.WorldToGridPosition(boss.BombManager.PlayerController.transform.position);
                
                // 사각형 공격 실행
                yield return ExecuteSingleSquareAttack(boss, currentPosition);
                
                // 다음 이동을 위해 위치 업데이트
                currentPosition = MoveTowardsPlayer(currentPosition, playerPos);
                
                // 다음 공격까지 0.2초 간격
                if (moveCount < 4) // 마지막이 아니면 대기
                {
                    yield return new WaitForSeconds(0.2f);
                }
            }
        }

        /// <summary>
        /// 1개의 랜덤 시작 위치 생성
        /// </summary>
        private Vector3Int GenerateRandomStartPosition()
        {
            return new Vector3Int(
                Random.Range(1, 7), // 1~6 (가장자리 피함)
                Random.Range(1, 7),
                0
            );
        }

        /// <summary>
        /// 단일 사각형 공격 실행
        /// </summary>
        private IEnumerator ExecuteSingleSquareAttack(BaseBoss boss, Vector3Int center)
        {
            List<Vector3Int> hollowSquare = CreateHollowSquare();
            
            // 전조 → 이펙트 공격 (Type1 전조 사용)
            boss.BombManager.ExecuteFixedBomb(hollowSquare, center, _groundSpikePrefab, 
                                              warningDuration: 0.8f, explosionDuration: 0.8f, damage: 20, WarningType.Type1);
            
            yield return new WaitForSeconds(0.5f); // 전조 시간만 대기
        }

        /// <summary>
        /// 가운데가 빈 3x3 사각형 모양 생성
        /// </summary>
        private List<Vector3Int> CreateHollowSquare()
        {
            return new List<Vector3Int>
            {
                new Vector3Int(-1, -1, 0), new Vector3Int(0, -1, 0), new Vector3Int(1, -1, 0), // 아래쪽
                new Vector3Int(-1,  0, 0),                            new Vector3Int(1,  0, 0), // 양옆 (가운데 비움)
                new Vector3Int(-1,  1, 0), new Vector3Int(0,  1, 0), new Vector3Int(1,  1, 0)  // 위쪽
            };
        }

        /// <summary>
        /// 플레이어 방향으로 한 칸 이동
        /// </summary>
        private Vector3Int MoveTowardsPlayer(Vector3Int current, Vector3Int playerPos)
        {
            Vector3Int direction = playerPos - current;
            
            // 한 칸씩만 이동
            int moveX = direction.x > 0 ? 1 : (direction.x < 0 ? -1 : 0);
            int moveY = direction.y > 0 ? 1 : (direction.y < 0 ? -1 : 0);
            
            return current + new Vector3Int(moveX, moveY, 0);
        }

        /// <summary>
        /// 여러 코루틴 동시 실행 (사용하지 않음)
        /// </summary>
        private IEnumerator ExecuteSimultaneous(BaseBoss boss, List<IEnumerator> coroutines)
        {
            // 더 이상 사용하지 않는 메소드
            yield break;
        }
    }
}