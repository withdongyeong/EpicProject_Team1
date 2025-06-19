using NUnit.Framework;
using System;
using System.Collections.Generic;
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

        //밑에 두개는 인접효과가 변경될때 호출되는 액션과 함수의 이름입니다.
        tileObject.OnStarListChanged += UpdateStarList;
        tileObject.OnStarListUpdateCompleted += GiveSkillStarList;
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

    private void GiveSkillStarList(List<StarBase> starBases)
    {
        foreach(SkillBase skill in skills)
        {
            skill.UpdateStarList(starBases);
        }
    }



    private void OnDestroy()
    {
        if(tileObject!=null)
        {
            tileObject.OnStarListChanged -= UpdateStarList;
        }       
    }

}
