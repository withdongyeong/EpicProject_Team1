using UnityEngine.SceneManagement;

public class ManaAIStarSkill : StarBase
{
    private ManaAISkill skill;

    protected override void Awake()
    {
        base.Awake();
        starBuff.RegisterGameStartAction(ActivateManaTurret);
        EventBus.SubscribeGameStart(HandleGameStart);
        EventBus.SubscribeSceneLoaded(HandleSceneLoaded);
        skill = transform.parent.GetComponentInChildren<ManaAISkill>();
        conditionCount = 1;
    }

    /// <summary>
    /// 전투 시작 시 그리드에 이동 불가 위치를 추가합니다.
    /// </summary>
    public void HandleGameStart()
    {
        GridManager.Instance.AddUnmovableGridPosition(GridManager.Instance.WorldToGridPosition(transform.position));
    }

    /// <summary>
    /// 빌딩 씬이 시작될 때 이동 불가 위치를 제거합니다.
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="mode"></param>
    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (SceneLoader.IsInBuilding())
        {
            GridManager.Instance.RemoveUnmovableGridPosition(GridManager.Instance.WorldToGridPosition(transform.position));
        }
    }

    public override bool CheckCondition(SkillBase skillBase)
    {
        if (skillBase.TileObject.name.Contains("ManaTurret"))
        {
            return true;
        }
        return false;
    }

    private void ActivateManaTurret(SkillBase skillBase)
    {
        if(CheckCondition(skillBase))
        {
            skill.ActivateManaTurret(skillBase);
        }
    }

    private void OnDestroy()
    {
        starBuff.UnregisterGameStartAction(ActivateManaTurret);
        EventBus.UnsubscribeGameStart(HandleGameStart);
        EventBus.UnsubscribeSceneLoaded(HandleSceneLoaded);
        GridManager.Instance.RemoveUnmovableGridPosition(GridManager.Instance.WorldToGridPosition(transform.position));
    }
}
