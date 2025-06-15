using UnityEngine;
using System.Collections.Generic;

public class SkillUseManager : Singleton<SkillUseManager>
{
    private TileObject currentTile;
    private GridManager gm;
    [SerializeField]
    private float cooldownFactor = 1.0f; // 쿨타임 조정 인자
    [SerializeField]
    private int skillActivationCount = 1; // 스킬 활성화 횟수
    
    public float CooldownFactor => cooldownFactor;
    public int SkillActivationCount => skillActivationCount;
    protected void Awake()
    {
        base.Awake();
        gm = GridManager.Instance;
    }
    
    public void UseSkill(Vector3Int cellPos)
    {
        cooldownFactor = 1.0f;
        skillActivationCount = 1;
        Cell cell = gm.GetCellData(cellPos);
        CombineCell comCell = cell.GetCombineCell();
        List<StarBase> starSkills = gm.GetStarSkills(cellPos);
        if(starSkills.Count > 0)
        {
            ActivateStarSkill(starSkills);
        }
        
        comCell.ExecuteSkill();
    }

    public void MultiplyCooldown(float factor) //예시 0.1f 는 10프로 감소 , 곱연산
    {
        Debug.Log("MultiplyCooldown called with factor: " + factor);
        cooldownFactor *= (1-factor);
    }
    
    public void SetSkillActivationCount(int count) //예시 2 는 +2 번
    {
        Debug.Log("SetSkillActivationCount called with count: " + count);
        skillActivationCount += count;
    }
    
    

    private void ActivateStarSkill(List<StarBase> starSkills)
    {
        foreach (StarBase starSkill in starSkills)
        {
            Debug.Log("Activating star skill: " + starSkill.name);
            starSkill.Activate();
        }
    }
}
