using UnityEngine;
using UnityEngine.SceneManagement;

public class TotemBaseSkill : NonActivateSkill
{
    protected override void InitClearStarList(Scene scene, LoadSceneMode mode)
    {
        if (SceneLoader.IsInStage() && tileObject.IsPlaced)
        {
            // 전투씬 시작시 인접 효과를 초기화합니다.
            ClearStarBuff();
            _coolTimeMaterial.SetFloat("_FillAmount", 0);
            //토템 핸들러를 소환합니다.
            GameObject totemHandler = Resources.Load<GameObject>("Prefabs/Summons/Totem/TotemHandler");
            Vector3 spawnPos = transform.TransformPoint((Vector3.right + Vector3.up)/2);
            TotemHandler totemHandlerScript = Instantiate(totemHandler, spawnPos, Quaternion.identity).GetComponent<TotemHandler>();
            //FindAnyObjectByType<PlayerSummons>().AddToList(totemHandlerScript);
        }
    }


}
