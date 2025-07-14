using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ManaTurretStarSkill : StarBase
{
    [SerializeField] private int damage = 10;
    [SerializeField] private GameObject projectilePrefab;
    private ManaTurretSkill skill;

    public int Damage { get => damage; set => damage = value; }

    protected override void Awake()
    {
        base.Awake();
        starBuff.RegisterActivateAction(ActivateManaTurret);
        EventBus.SubscribeGameStart(HandleGameStart);
        EventBus.SubscribeSceneLoaded(HandleSceneLoaded);
        skill = transform.parent.GetComponentInChildren<ManaTurretSkill>();
    }

    /// <summary>
    /// 전투 시작 시 타겟 적을 설정하고 그리드에 이동 불가 위치를 추가합니다.
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

    private void ActivateManaTurret(SkillBase skillBase)
    {
        if(CheckCondition(skillBase))
        {
            skill.ActivateManaTurret();
        }
    }

    public override bool CheckCondition(SkillBase skillBase)
    {
        if (skillBase.TileObject.GetTileData().TileCategory == TileCategory.Weapon && !skillBase.TileObject.name.Contains("ManaAI"))
        {
            return true;
        }
        return false;
    }

    private void OnDestroy()
    {
        EventBus.UnsubscribeGameStart(HandleGameStart);
        EventBus.UnsubscribeSceneLoaded(HandleSceneLoaded);
        GridManager.Instance.RemoveUnmovableGridPosition(GridManager.Instance.WorldToGridPosition(transform.position));
    }
}
