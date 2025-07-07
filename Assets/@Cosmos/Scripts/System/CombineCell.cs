using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CombineCell : MonoBehaviour
{
    private GameObject coreCell;
    private SpriteRenderer sr;
    [SerializeField]
    private SkillBase[] skills;
    private TileObject tileObject;
    
    
    [SerializeField]
    private CoolDownEffect[] coolDownEffects;

    public Action OnStarListChanged;

    public SkillBase[] Skills => skills;

    private Dictionary<int, List<CoolDownEffect>> grouped = new();
    private List<int> sortedKeys = new List<int>();

    private void Awake()
    
    {
        if (coreCell == null)
        {
            coreCell = GetComponentInChildren<Cell>().gameObject;
        }
        skills = GetComponents<SkillBase>();
        sr = GetComponentInChildren<SpriteRenderer>();
        sr.sortingLayerName = "Tile";
        tileObject = GetComponentInParent<TileObject>();
        coolDownEffects = GetComponentsInChildren<CoolDownEffect>();

        //밑에 두개는 인접효과가 변경될때 호출되는 액션과 함수의 이름입니다.
        tileObject.OnStarListChanged += UpdateStarList;
        tileObject.OnStarListUpdateCompleted += GiveSkillStarList;
        EventBus.SubscribeGameStart(InitCoolDownGroup);
        EventBus.SubscribeSceneLoaded(StopAllCouroutines);
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
            Debug.Log("No skills assigned to CombineCell.");
            return;
        }
        
        foreach (var skill in skills)
        {
            if (skill != null)
            {
                skill.TryActivate();
            }
            else
            {
                Debug.LogWarning($"Skill {skill?.name} is on cooldown or not assigned.");
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

    public void CoolDownEffectActivate(float coolDownTime)
    {
        StopAllCoroutines();
        StartCoroutine(CoolDownActivateCoroutine(coolDownTime));
    }

    private IEnumerator CoolDownActivateCoroutine(float coolDownTime)
    {
        
        //sr.color = new Color(1, 1, 1, 1f);
        foreach (var effect in coolDownEffects)
        {
            effect.Init();
        }
        
        
        int count = sortedKeys.Count;
        coolDownTime/= count;
        foreach (int y in sortedKeys)
        {
            foreach (var effect in grouped[y])
            {
                effect.StartCoolDown(coolDownTime);
            }
            
            yield return new WaitForSeconds(coolDownTime);
        }
        
        foreach (var effect in coolDownEffects)
        {
            effect.CompleteEffect();
        }
        
    }

    
    public void InitCoolDownGroup()
    {
        grouped.Clear(); // 기존 그룹화된 데이터를 초기화
        sortedKeys.Clear(); // 기존 정렬된 키 리스트 초기화
        foreach (var effect in coolDownEffects)
        {
            int yLevel = Mathf.RoundToInt(effect.transform.parent.position.y);
            if (!grouped.ContainsKey(yLevel))
            {
                grouped[yLevel] = new List<CoolDownEffect>();
            }
            Debug.Log(yLevel+ " 레벨에 " + effect.name + " 추가");
            grouped[yLevel].Add(effect);
        }

        sortedKeys = grouped.Keys.OrderBy(y => y).ToList(); // Y 레벨을 기준으로 정렬
    }

    private void StopAllCouroutines(Scene scene = default, LoadSceneMode mode = default)
    {
        // 씬이 변경될 때 모든 코루틴을 중지합니다.
        StopAllCoroutines();
    }
    private void OnDestroy()
    {
        if(tileObject!=null)
        {
            tileObject.OnStarListChanged -= UpdateStarList;
        }       
        EventBus.UnsubscribeGameStart(InitCoolDownGroup);
        EventBus.UnsubscribeSceneLoaded(StopAllCouroutines);
    }

}
