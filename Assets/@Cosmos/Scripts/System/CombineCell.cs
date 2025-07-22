using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    /// <summary>
    /// 현재 CombineCell에 할당된 스킬들을 반환합니다.
    /// </summary>
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

    /// <summary>
    /// CombineCell의 스프라이트 렌더러를 반환합니다.
    /// </summary>
    public SpriteRenderer GetSprite()
    {
        return sr;
    }
    
    /// <summary>
    /// 코어 셀 게임오브젝트를 반환합니다.
    /// </summary>
    public GameObject GetCoreCell()
    {
        return coreCell;
    }
    
    /// <summary>
    /// 타일 오브젝트를 반환합니다.
    /// </summary>
    public TileObject GetTileObject()
    {
        return tileObject;
    }
    
    /// <summary>
    /// 할당된 모든 스킬을 실행 시도합니다.
    /// </summary>
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

    /// <summary>
    /// 인접효과 리스트가 변경되었을 때 호출되는 메서드입니다.
    /// </summary>
    private void UpdateStarList()
    {
        OnStarListChanged?.Invoke();
    }

    /// <summary>
    /// 스킬들에게 업데이트된 인접효과 리스트를 전달합니다.
    /// </summary>
    /// <param name="starBases">업데이트된 인접효과 리스트</param>
    private void GiveSkillStarList(List<StarBase> starBases)
    {
        foreach(SkillBase skill in skills)
        {
            skill.UpdateStarList(starBases);
        }
    }

    /// <summary>
    /// 쿨다운 이펙트를 활성화합니다.
    /// </summary>
    /// <param name="coolDownTime">총 쿨다운 시간</param>
    public void CoolDownEffectActivate(float coolDownTime)
    {
        StopAllCoroutines();
        StartCoroutine(CoolDownActivateCoroutine(coolDownTime));
    }

    /// <summary>
    /// 일시정지를 고려한 쿨다운 이펙트 실행 코루틴입니다.
    /// </summary>
    /// <param name="coolDownTime">총 쿨다운 시간</param>
    private IEnumerator CoolDownActivateCoroutine(float coolDownTime)
    {
        foreach (var effect in coolDownEffects)
        {
            effect.Init();
        }
        
        int count = sortedKeys.Count;
        float groupCoolDownTime = coolDownTime / count;
        
        foreach (int y in sortedKeys)
        {
            foreach (var effect in grouped[y])
            {
                effect.StartCoolDown(groupCoolDownTime);
            }
            
            // 일시정지를 고려한 대기
            yield return StartCoroutine(WaitForGameTime(groupCoolDownTime));
        }
        
        foreach (var effect in coolDownEffects)
        {
            effect.CompleteEffect();
        }
    }

    /// <summary>
    /// 일시정지를 고려하여 게임 시간 기준으로 대기하는 코루틴입니다.
    /// </summary>
    /// <param name="waitTime">대기할 시간(초)</param>
    private IEnumerator WaitForGameTime(float waitTime)
    {
        float elapsedTime = 0f;
        
        while (elapsedTime < waitTime)
        {
            // 일시정지 중이 아닐 때만 시간을 증가
            if (Time.timeScale > 0)
            {
                elapsedTime += Time.deltaTime;
            }
            yield return null;
        }
    }

    /// <summary>
    /// 게임 시작시 쿨다운 이펙트들을 Y 레벨별로 그룹화합니다.
    /// </summary>
    public void InitCoolDownGroup()
    {
        grouped.Clear();
        sortedKeys.Clear();
        
        foreach (var effect in coolDownEffects)
        {
            int yLevel = Mathf.RoundToInt(effect.transform.parent.position.y);
            if (!grouped.ContainsKey(yLevel))
            {
                grouped[yLevel] = new List<CoolDownEffect>();
            }
            grouped[yLevel].Add(effect);
        }

        sortedKeys = grouped.Keys.OrderBy(y => y).ToList();
    }

    /// <summary>
    /// 씬 변경시 모든 코루틴을 중지합니다. Additive 씬 로드는 제외됩니다.
    /// </summary>
    /// <param name="scene">로드된 씬</param>
    /// <param name="mode">씬 로드 모드</param>
    private void StopAllCouroutines(Scene scene = default, LoadSceneMode mode = default)
    {
        // Additive 모드(설정창 등)는 무시하고, Single 모드일 때만 코루틴 중지
        if (mode == LoadSceneMode.Single)
        {
            StopAllCoroutines();
        }
    }

    private void OnDestroy()
    {
        if(tileObject != null)
        {
            tileObject.OnStarListChanged -= UpdateStarList;
            tileObject.OnStarListUpdateCompleted -= GiveSkillStarList;
        }       
        EventBus.UnsubscribeGameStart(InitCoolDownGroup);
        EventBus.UnsubscribeSceneLoaded(StopAllCouroutines);
    }
}