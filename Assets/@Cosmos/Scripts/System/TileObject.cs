using System;
using System.Collections.Generic;
using UnityEngine;

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
            List<SkillBase> skills = new List<SkillBase>();

            // 스타셀의 색상을 초기화하고, 해당 스타셀의 스킬이 조건을 만족하면 색을 바꿉니다.
            foreach (StarCell starCell in CombinedStarCell.GetComponentsInChildren<StarCell>())
            {
                // 스타셀의 색상을 초기화합니다.
                SpriteRenderer sr = starCell.GetComponent<SpriteRenderer>();
                sr.color = Color.black;

                // 스타셀의 위치를 가져오고, 해당 위치의 CellData가 존재하는지 확인합니다.
                Vector3Int gridPos = starCell.GetStarCellPosition();
                if (!GridManager.Instance.IsWithinGrid(gridPos) || GridManager.Instance.GetCellData(gridPos) == null)
                {
                    continue;
                }

                // 스타셀의 스킬을 가져오고, 해당 스킬이 조건을 만족하는지 확인합니다.
                SkillBase[] skillBases = GridManager.Instance.GetCellData(gridPos)?.GetCombineCell()?.Skills;
                foreach (SkillBase skill in skillBases)
                {
                    Debug.Log($"스타셀 위치: {starCell.GetStarSkill()}, 스킬: {skill.name}");
                    if (starCell.GetStarSkill() != null && starCell.GetStarSkill().CheckCondition(skill) && !skills.Contains(skill))
                    {
                        sr.color = Color.white; // 조건을 만족하면 색상을 흰색으로 변경
                        skills.Add(skill);
                    }
                }
            }
        }
        isStarDisplayEnabled = true;
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
}

