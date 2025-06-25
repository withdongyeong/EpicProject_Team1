using System;
using System.Collections.Generic;
using UnityEngine;

public class TileObject : MonoBehaviour
{
    public TileData data;
    [SerializeField]
    private TileInfo tileInfo;
    private GameObject combinedStarCell;

    public string Description { get => tileInfo.Description; }
    public GameObject CombinedStarCell { get => combinedStarCell; }

    private bool isInitialized = false;

    //그리드의 스타 리스트가 변경되었을때의 액션입니다.
    public Action OnStarListChanged;

    //자신의 스타 리스트가 최종적으로 업데이트 되었을때의 액션입니다.
    public Action<List<StarBase>> OnStarListUpdateCompleted;

    private List<StarBase> starList = new();


    private void Awake()
    {
        InitializeTile();
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
        combinedStarCell = GetComponentInChildren<CombinedStarCell>() ? GetComponentInChildren<CombinedStarCell>().gameObject : null;
        if (combinedStarCell != null)
        {
            //combinedStarCell.SetActive(false); // 스타셀의 부모 오브젝트 비활성화
        }
        isInitialized = true;
    }

    /// <summary>
    /// 현재 적용되는 인접 효과를 다시 계산하는 메소드입니다.
    /// </summary>
    public void UpdateStarList()
    {
        Debug.Log("재계산 시작합니다");
        starList.Clear();
        OnStarListChanged?.Invoke();
        OnStarListUpdateCompleted?.Invoke(starList);
        Debug.Log(starList);
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
}

