using System;
using UnityEngine;

public class CombineCell : MonoBehaviour
{
    public GameObject coreCell;
    private SpriteRenderer sr;
    public SkillBase[] skills;

    private void Awake()
    
    {
        if (coreCell == null)
        {
            coreCell = GetComponentInChildren<Cell>().gameObject;
        }
        skills = GetComponents<SkillBase>();
        sr = GetComponentInChildren<SpriteRenderer>();
    }


    public SpriteRenderer GetSprite()
    {
        return sr;
    }
    public void ExecuteSkill()
    {
        if (skills == null || skills.Length == 0)
        {
            Debug.LogWarning("No skills assigned to CombineCell.");
            return;
        }

        foreach (var skill in skills)
        {
            if (skill != null)
            {
                skill.TryActivate(coreCell);
            }
            else
            {
                Debug.LogWarning($"Skill {skill?.skillName} is on cooldown or not assigned.");
            }
        }
    }
    
}
