using System;
using UnityEngine;

public class CombineCell : MonoBehaviour
{
    public GameObject coreCell;
    public GameObject tile;
    private SpriteRenderer sr;
    public SkillBase[] skills;
    public TileObject tileObject;

    private void Awake()
    
    {
        if (coreCell == null)
        {
            coreCell = GetComponentInChildren<Cell>().gameObject;
        }
        skills = GetComponents<SkillBase>();
        sr = GetComponentInChildren<SpriteRenderer>();
        tileObject = GetComponentInParent<TileObject>();
        tile = tileObject.gameObject;
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
