using System;
using UnityEngine;

public class CombineCell : MonoBehaviour
{
    private GameObject coreCell;
    private SpriteRenderer sr;
    private SkillBase[] skills;
    private TileObject tileObject;

    public Action OnStarListChanged;

    private void Awake()
    
    {
        if (coreCell == null)
        {
            coreCell = GetComponentInChildren<Cell>().gameObject;
        }
        skills = GetComponents<SkillBase>();
        sr = GetComponentInChildren<SpriteRenderer>();
        tileObject = GetComponentInParent<TileObject>();
        tileObject.OnStarListChanged += UpdateStarList;
    }


    public SpriteRenderer GetSprite()
    {
        return sr;
    }
    
    public GameObject GetCoreCell()
    {
        return coreCell;
    }
    
    public TileObject GetTileObject()
    {
        return tileObject;
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

    private void UpdateStarList()
    {
        OnStarListChanged?.Invoke();
    }

    private void OnDestroy()
    {
        if(tileObject!=null)
        {
            tileObject.OnStarListChanged -= UpdateStarList;
        }       
    }

}
