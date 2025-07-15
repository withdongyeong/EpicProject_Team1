using UnityEngine;
using UnityEngine.SceneManagement;

public class MagicCircleStarSkill : StarBase
{
    private bool isActivated = false; // Magic Circle 인접 효과가 활성화되었는지 여부를 나타내는 변수
    private int magicCircleCount = 0; // Magic Circle의 개수를 세는 변수
    private SkillBase[] skills; // 스킬 목록을 저장할 배열

    protected override void Awake()
    {
        base.Awake();
        conditionCount = 3;
        starBuff.RegisterGameStartAction(CheckMagicCircle);
        EventBus.SubscribeSceneLoaded(InitClear);
        // 자식 오브젝트에서 모든 SkillBase 컴포넌트를 가져옵니다.
        skills = transform.parent.GetComponentsInChildren<SkillBase>();
    }

    /// <summary>
    /// 전투 시작 시 인접한 마법진을 확인하는 함수입니다.
    /// </summary>
    /// <param name="skillBase"></param>
    private void CheckMagicCircle(SkillBase skillBase)
    {
        if (skillBase.TileObject.GetTileData().TileCategory == TileCategory.MagicCircle)
        {
            AddMagicCircleCount();
        }
    }

    public override bool CheckCondition(SkillBase skillBase)
    {
        if (skillBase.TileObject.GetTileData().TileCategory == TileCategory.MagicCircle)
        {
            return true;
        }
        return false;
    }

    private void AddMagicCircleCount()
    {
        if (!isActivated)
        {
            magicCircleCount++;
            if (magicCircleCount >= conditionCount)
            {
                // 모든 스킬에 Magic Circle 버프를 적용합니다.
                foreach (var skill in skills)
                {
                    skill.ApplyStatBuff(new TileBuffData(BuffableTileStat.MagicCircle, 1f));
                }
                isActivated = true; // Magic Circle 인접 효과가 활성화됨
                // 업적 달성
                SteamAchievement.Achieve("ACH_CON_MAGIC_CIRCLE");
            }
        }
    }

    private void InitClear(Scene scene, LoadSceneMode mode)
    {
        if (SceneLoader.IsInStage())
        {
            isActivated = false; // 씬이 변경될 때마다 초기화
            magicCircleCount = 0; // Magic Circle 개수 초기화
        }
    }

    private void OnDestroy()
    {
        starBuff.UnregisterGameStartAction(CheckMagicCircle);
        EventBus.UnsubscribeSceneLoaded(InitClear); // 씬 로드 이벤트 구독 해제
    }
}