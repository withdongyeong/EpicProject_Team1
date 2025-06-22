using UnityEngine.SceneManagement;
using UnityEngine;

public class ArchmageStaffStarSkill : StarBase
{
    private ArchmageStaffSkill skill;

    protected override void Awake()
    {
        base.Awake();
        starBuff.RegisterGameStartAction(GatherAxodia);
        EventBus.SubscribeGameStart(HandleGameStart);
        EventBus.SubscribeSceneLoaded(HandleSceneLoaded);

        // ProjectileSkill 컴포넌트를 가져옵니다.
        skill = transform.parent.GetComponentInChildren<ArchmageStaffSkill>();
    }

    /// <summary>
    /// 전투 시작 시 Axodia를 모으는 함수입니다.
    /// </summary>
    /// <param name="skillBase"></param>
    private void GatherAxodia(SkillBase skillBase)
    {
        if (skillBase.TileObject.name.Contains("Fire") || skillBase.TileObject.name.Contains("Frost"))
        {
            skill.AddAdjacentTile(skillBase.TileObject.name);
        }
    }

    /// <summary>
    /// 게임 시작 시 필요한 초기화 작업을 수행합니다.
    /// </summary>
    private void HandleGameStart()
    {
        // 게임 시작 시 필요한 초기화 작업을 여기에 추가합니다.
        // 예: 타일 오브젝트의 상태를 초기화하거나, 특정 이벤트를 구독하는 등의 작업
    }

    /// <summary>
    /// 빌딩 씬이 로드되었을 때 필요한 작업을 수행합니다.
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="mode"></param>
    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "BuildingScene")
        {
            // 빌딩 씬이 로드되었을 때 필요한 작업을 여기에 추가합니다.
            // 예: 특정 컴포넌트를 초기화하거나, 이벤트를 다시 구독하는 등의 작업
        }
    }

    private void OnDestroy()
    {
        starBuff.UnregisterGameStartAction(GatherAxodia);
        EventBus.UnsubscribeGameStart(HandleGameStart);
        EventBus.UnsubscribeSceneLoaded(HandleSceneLoaded);
    }
}
