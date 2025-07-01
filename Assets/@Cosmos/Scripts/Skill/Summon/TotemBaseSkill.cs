using UnityEngine;
using UnityEngine.SceneManagement;

public class TotemBaseSkill : SkillBase
{
    protected override void InitClearStarList(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "StageScene")
        {
            // 전투씬 시작시 인접 효과를 초기화합니다.
            ClearStarBuff();
            //토템 핸들러를 소환합니다.
            GameObject totemHandler = Resources.Load<GameObject>("Prefabs/Summons/Totem/TotemHandler");
            TotemHandler totemHandlerScript = Instantiate(totemHandler, transform.position, Quaternion.identity).GetComponent<TotemHandler>();
            //FindAnyObjectByType<PlayerSummons>().AddToList(totemHandlerScript);
        }
    }

    protected override void Activate()
    {
        
    }
}
