using UnityEngine.SceneManagement;

public class NonActivateSkill : SkillBase
{
    protected override void Activate()
    {

    }
    protected override void InitClearStarList(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "StageScene")
        {
            // 전투씬 시작시 인접 효과를 초기화합니다.
            ClearStarBuff();
            _coolTimeMaterial.SetFloat("_FillAmount", 0);
        }
    }

    protected override void ResetCoolDown(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "BuildingScene")
        {
            _coolTimeMaterial.SetFloat("_FillAmount", 1);
        }
    }


    protected override void LateUpdate()
    {

    }
}
