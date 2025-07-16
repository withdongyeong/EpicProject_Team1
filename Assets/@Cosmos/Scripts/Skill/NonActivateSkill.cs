using UnityEngine;
using UnityEngine.SceneManagement;

public class NonActivateSkill : SkillBase
{
    protected SpriteRenderer spriteRenderer;

    protected Color originalColor;



    protected override void Awake()
    {
        base.Awake();
        spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    protected override void Activate()
    {

    }
    
    //전투씬 돌입시 호출되는 함수입니다
    protected override void InitClearStarList(Scene scene, LoadSceneMode mode)
    {
        if (SceneLoader.IsInStage())
        {   
            // 전투씬 시작시 인접 효과를 초기화합니다.
            ClearStarBuff();
            Color invisibleColor = originalColor;
            invisibleColor.a = 0.15f;
            spriteRenderer.color = invisibleColor;
            foreach (Transform child in transform.GetChild(0))
            {
                child.gameObject.SetActive(false);
            }
        }
    }

    //상점씬 돌입시 호출되는 함수입니다.
    protected override void ResetCoolDown(Scene scene, LoadSceneMode mode)
    {
        if (SceneLoader.IsInBuilding())
        {
            spriteRenderer.color = originalColor;
            foreach (Transform child in transform.GetChild(0))
            {
                child.gameObject.SetActive(true);
            }
        }
    }


    protected override void LateUpdate()
    {

    }
}
