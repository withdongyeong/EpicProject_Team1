using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ManaAISkill : SkillBase
{
    private bool isInGameScene = false;

    protected override void Awake()
    {
        base.Awake();
        // 스킬이 활성화될 때마다 타일 오브젝트를 설정합니다.
        EventBus.SubscribeSceneLoaded(HandleSceneLoaded);
    }

    public void ActivateManaTurret(SkillBase skillBase)
    {
        StartCoroutine(ActivateManaTurretCoroutine(skillBase));
    }


    private IEnumerator ActivateManaTurretCoroutine(SkillBase skillBase)
    {
        // 1초마다 발사 시도 -> 쿨타임 간격으로 발사
        while (isInGameScene)
        {
            yield return new WaitForSeconds(1f);
            skillBase.TryActivate();
        }
    }

    /// <summary>
    /// 씬이 이동할 때 공격을 중단합니다.
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="mode"></param>
    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.name == "BuildingScene")
        {
            StopAllCoroutines();
            isInGameScene = false;
        }
        else
        {
            isInGameScene = true;
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        EventBus.UnsubscribeSceneLoaded(HandleSceneLoaded);
    }
}
