using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class TileObject : MonoBehaviour
{
    public TileData data;
    private TileInfo tileInfo;
    private GameObject combinedStarCell;

    public string Description { get => tileInfo.Description; }
    public GameObject CombinedStarCell { get => combinedStarCell; }

    private bool isInitialized = false;
    private bool isStarDisplayEnabled = false; // 스타 셀 표시 여부
    public bool IsStarDisplayEnabled => isStarDisplayEnabled; // 외부에서 스타 셀 표시 여부를 확인할 수 있도록 공개

    //그리드의 스타 리스트가 변경되었을때의 액션입니다.
    public Action OnStarListChanged;

    //자신의 스타 리스트가 최종적으로 업데이트 되었을때의 액션입니다.
    public Action<List<StarBase>> OnStarListUpdateCompleted;

    private List<StarBase> starList = new();

    //놓였는지 안놓였는지 확인하는 bool입니다.
    private bool isPlaced = false;
    public bool IsPlaced => isPlaced;

    private void Awake()
    {
        InitializeTile();
        //GetComponentInChildren<CombinedStarCell>(true)?.gameObject.SetActive(true);
    }
    
    private void InitializeTile()
    {
        if (data == null)
        {
            Debug.LogError("TileData is not assigned in TileObject.");
            return;
        }
        tileInfo = new TileInfo(data);

        if (tileInfo.TileSprite == null)
        {
            Debug.LogError("Tile sprite is not assigned in TileObject.");
        }
        combinedStarCell = GetComponentInChildren<CombinedStarCell>(true) ? GetComponentInChildren<CombinedStarCell>(true).gameObject : null;
        if (combinedStarCell != null)
        {
            //combinedStarCell.SetActive(true); // 스타셀의 부모 오브젝트 비활성화
        }
        isInitialized = true;
    }

    /// <summary>
    /// 현재 적용되는 인접 효과를 다시 계산하는 메소드입니다.
    /// </summary>
    public void UpdateStarList()
    {
        //Debug.Log("재계산 시작합니다");
        starList.Clear();
        OnStarListChanged?.Invoke();
        OnStarListUpdateCompleted?.Invoke(starList);
        //Debug.Log(starList);
    }

    /// <summary>
    /// 타일이 받을 인접 효과에 새로운 인접효과를 넣어보려고 시도하는 함수입니다.
    /// </summary>
    /// <param name="starListInput">넣어볼 인접효과 입니다. 동일 인스턴스라면 안넣습니다.</param>
    public void AddToStarList(List<StarBase> starListInput)
    {
        foreach(StarBase star in starListInput)
        {
            if(!starList.Contains(star))
            {
                starList.Add(star);
            }
        }
    }
    
    public TileInfo GetTileData()
    {
        if(!isInitialized)
        {
            InitializeTile();
        }
        return tileInfo;
    }

    public Sprite GetTileSprite()
    {
        return data.tileSprite;
    }
    
    public void ShowStarCell()
    {
        if (combinedStarCell != null)
        {
            foreach (var sr in CombinedStarCell.GetComponentsInChildren<SpriteRenderer>())
            {
                sr.enabled = true;  
            }
           
            // 같은 타일을 중복으로 표시하지 않도록 하기 위해 List를 사용합니다.
            List<TileObject> skills = new List<TileObject>();
            List<string> archmagestaff = new List<string>();
            List<string> beetle = new List<string>();

            // 스타셀의 색상을 초기화하고, 해당 스타셀의 스킬이 조건을 만족하면 색을 바꿉니다.
            foreach (StarCell starCell in CombinedStarCell.GetComponentsInChildren<StarCell>())
            {
                // 스타셀의 색상을 초기화합니다.
                SpriteRenderer sr = starCell.GetComponent<SpriteRenderer>();
                Sprite spriteDisable = Resources.Load<Sprite>("Arts/UI/StarDisable");
                sr.sprite = spriteDisable;

                // 스타셀의 위치를 가져오고, 해당 위치의 CellData가 존재하는지 확인합니다.
                Vector3Int gridPos = starCell.GetStarCellPosition();
                if (!GridManager.Instance.IsWithinGrid(gridPos) || GridManager.Instance.GetCellData(gridPos) == null)
                {
                    continue;
                }

                // 스타셀의 스킬을 가져오고, 해당 스킬이 조건을 만족하는지 확인합니다.
                SkillBase[] skillBases = GridManager.Instance.GetCellData(gridPos)?.GetCombineCell()?.Skills;

                // 만약 스킬이 없다면, 해당 스타셀의 타일 오브젝트에서 스킬을 가져옵니다.
                if (skillBases == null || skillBases.Length == 0)
                {
                    skillBases = new SkillBase[1];
                    skillBases[0] = GridManager.Instance.GetCellData(gridPos).GetCombineCell().GetTileObject().GetComponentInChildren<SkillBase>();
                }


                foreach (SkillBase skill in skillBases)
                {
                    if (starCell.GetStarSkill() != null && starCell.GetStarSkill().CheckCondition(skill) && !skills.Contains(skill.TileObject))
                    {
                        if (starCell.GetStarSkill().GetType().Name.Contains("ArchmageStaffStarSkill"))
                        {
                            if (archmagestaff.Contains(skill.TileObject.name))
                            {
                                continue; // 이미 추가된 스킬이면 건너뜀
                            }
                            archmagestaff.Add(skill.TileObject.name); // ArchmageStaffStarSkill이 중복되지 않도록 관리
                        }
                        if (starCell.GetStarSkill().GetType().Name.Contains("BeetleSummonStarSkill"))
                        {
                            if (beetle.Contains(skill.TileObject.name))
                            {
                                continue; // 이미 추가된 스킬이면 건너뜀
                            }
                            beetle.Add(skill.TileObject.name); // BeetleStarSkill이 중복되지 않도록 관리
                        }
                        Sprite sprite = Resources.Load<Sprite>("Arts/UI/Star");
                        sr.sprite = sprite; // 조건을 만족하면 색상을 흰색으로 변경
                        skills.Add(skill.TileObject);
                    }
                }
            }

            SetStarEffect();

        }
        isStarDisplayEnabled = true;
    }


    public void SetStarEffect()
    {
        //배치 씬 인접효과 비주얼을 위한 코드
        if (combinedStarCell == null ||combinedStarCell.GetComponent<CombinedStarCell>().GetStarSkill() == null) return;
        int conditionCount = combinedStarCell.GetComponent<CombinedStarCell>().GetStarSkill().GetConditionCount();
        int activeStarCount = 0;
        foreach (var star in CombinedStarCell.GetComponentsInChildren<SpriteRenderer>())
        {
            if (star.sprite.name == "Star")
            {
                activeStarCount++;
            }
        }
        
        
        /*if (activeStarCount >= conditionCount)
        {
            GetComponentInChildren<CombineCell>().GetSprite().color = new Color(1f, 1f, 0.5f, 1f);
        }
        else
        {
            GetComponentInChildren<CombineCell>().GetSprite().color = new Color(1f, 1f, 1, 1f);
        }*/
        
        
        if (activeStarCount >= conditionCount)
        {
            foreach (var star in GetComponentsInChildren<LightController>())
            {
                star.SetLightProperties(6,3,0.8f,0.1f,0.4f);
            }
        }
        else
        {
            foreach (var star in GetComponentsInChildren<LightController>())
            {
                star.SetLightProperties(2,0,1f,0.1f,0.2f);
            }
        }
    }

    public void HideStarCell()
    {
        if (combinedStarCell != null)
        {
            foreach (var sr in CombinedStarCell.GetComponentsInChildren<SpriteRenderer>())
            {
                sr.enabled = false;
            }
        }
        isStarDisplayEnabled = false;
    }

    public void OnPlaced()
    {
        isPlaced = true;
    }

    public void OnDragged()
    {
        isPlaced = false;
    }
}

