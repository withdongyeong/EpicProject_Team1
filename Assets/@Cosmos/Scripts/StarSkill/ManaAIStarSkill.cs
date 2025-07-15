using UnityEngine.SceneManagement;

public class ManaAIStarSkill : StarBase
{
    private ManaAISkill skill;
    private int _manaTurretCount = 0;

    protected override void Awake()
    {
        base.Awake();
        starBuff.RegisterGameStartAction(ActivateManaTurret);
        EventBus.SubscribeGameStart(HandleGameStart);
        EventBus.SubscribeSceneLoaded(HandleSceneLoaded);
        skill = transform.parent.GetComponentInChildren<ManaAISkill>();
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
            _manaTurretCount = 0; // 빌딩 씬에서는 마나 터렛 카운트를 초기화합니다.
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
            _manaTurretCount++;
            if (_manaTurretCount >= 4)
            {
                // 4개 이상의 마나 터렛이 활성화되면 업적 달성
                SteamAchievement.Achieve("ACH_CON_TURRET");
            }
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
