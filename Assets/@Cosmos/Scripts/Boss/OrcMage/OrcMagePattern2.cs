using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cosmos.Scripts.Boss.OrcMage
{
    /// <summary>
    /// 오크 메이지 웨이브 패턴 - 위에서 아래로 스윽 지나가는 파동
    /// </summary>
    public class OrcMagePatternWave : IBossAttackPattern
    {
        private GameObject _groundSpikePrefab;

        public string PatternName => "6_1";

        public OrcMagePatternWave(GameObject groundSpikePrefab)
        {
            _groundSpikePrefab = groundSpikePrefab;
        }

        public bool CanExecute(BaseBoss boss)
        {
            return boss != null && _groundSpikePrefab != null && boss.BombHandler != null;
        }

        /// <summary>
        /// 웨이브 패턴 실행 - 위에서 아래로 3번의 파동
        /// </summary>
        public IEnumerator Execute(BaseBoss boss)
        {
            boss.SetAnimationTrigger("Attack1");

            // 3번의 웨이브 실행
            for (int wave = 0; wave < 3; wave++)
            {
                yield return ExecuteWave(boss);
                if (wave < 2) yield return new WaitForSeconds(0.5f);
            }
        }

        /// <summary>
        /// 단일 웨이브 실행
        /// </summary>
        private IEnumerator ExecuteWave(BaseBoss boss)
        {
            // 9줄을 순차적으로 실행 (위→아래)
            for (int row = 0; row < 9; row++)
            {
                List<Vector3Int> lineShape = CreateHorizontalLine();
                Vector3Int center = new Vector3Int(4, 8 - row, 0); // 위부터 시작

                boss.StartCoroutine(boss.PlayOrcExplosionSoundDelayed("OrcMage_SpikeActivate", 0.8f));
                boss.BombHandler.ExecuteFixedBomb(lineShape, center, _groundSpikePrefab,
                                                  warningDuration: 0.3f, explosionDuration: 0.5f, damage: 25, warningType:WarningType.Type1, patternName:PatternName);

                yield return new WaitForSeconds(0.2f); // 빠른 연속 실행
            }
        }

        /// <summary>
        /// 가로 한 줄 모양 생성
        /// </summary>
        private List<Vector3Int> CreateHorizontalLine()
        {
            List<Vector3Int> line = new List<Vector3Int>();
            for (int x = -4; x <= 4; x++)
            {
                line.Add(new Vector3Int(x, 0, 0));
            }
            return line;
        }
    }
}