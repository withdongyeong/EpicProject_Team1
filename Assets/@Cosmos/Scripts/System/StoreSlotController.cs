using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class StoreSlotController : MonoBehaviour
{
    private StoreSlot[] storeSlots;

    

    private int _safeInt = 0;

    //이 밑은 희귀도에 따라서 분류된 리스트입니다
    private List<GameObject> _normalStoreTiles = new();
    private List<GameObject> _rareStoreTiles = new();
    private List<GameObject> _epicStoreTiles = new();
    private List<GameObject> _legendaryStoreTiles = new();
    //test
    public List<GameObject> testList = new List<GameObject>();
    public bool isTest;


    private void Awake()
    {
        storeSlots = GetComponentsInChildren<StoreSlot>();
        SetStoreTileList();

    }

    private void Start()
    {
        SetupStoreSlots();
    }


    private void SetupStoreSlots()
    {
        Debug.Log(storeSlots.Length);
        for (int i = 0; i < storeSlots.Length; i++)
        {
            float roll = Random.value * 100f;
            TileGrade chosenGrade;
            //TODO: 이거 줄 줄이기
            if(roll < GlobalSetting.Shop_ChanceList[StageSelectManager.Instance.StageNum].shop_NormalChance)
            {
                chosenGrade = TileGrade.Normal;
            }
            else if(roll < GlobalSetting.Shop_ChanceList[StageSelectManager.Instance.StageNum].shop_NormalChance + GlobalSetting.Shop_ChanceList[StageSelectManager.Instance.StageNum].shop_RareChance)
            {
                chosenGrade = TileGrade.Rare;
            }
            else if(roll < GlobalSetting.Shop_ChanceList[StageSelectManager.Instance.StageNum].shop_NormalChance + GlobalSetting.Shop_ChanceList[StageSelectManager.Instance.StageNum].shop_RareChance + GlobalSetting.Shop_ChanceList[StageSelectManager.Instance.StageNum].shop_EpicChance)
            {
                chosenGrade = TileGrade.Epic;
            }
            else
            {
                chosenGrade = TileGrade.Legendary;
            }

            List<GameObject> chosenList = _normalStoreTiles;
            switch(chosenGrade)
            {
                case TileGrade.Normal:
                    chosenList = _normalStoreTiles;
                    break;
                case TileGrade.Rare:
                    chosenList = _rareStoreTiles;
                    break;
                case TileGrade.Epic:
                    chosenList = _epicStoreTiles;
                    break;
                case TileGrade.Legendary:
                    chosenList = _legendaryStoreTiles;
                    break;

            }
            //test
            if (isTest)
            {
                chosenList = testList;
            }
            int randomIndex = Random.Range(0, chosenList.Count);
            GameObject chosenTile = chosenList[randomIndex];
            //선택된 타일이 상점에 등장해도 되는지 조건검사를 합니다.
            chosenTile = CheckTileCondition(chosenTile, chosenList);
            storeSlots[i].SetSlot(chosenTile.GetComponent<TileObject>().GetTileData().TileCost, chosenTile);
            
            //이미지 비율을 맞추기 위한 코드입니다.
            storeSlots[i].GetComponent<Image>().preserveAspect = true;
        
        }
    }

    public void ResetSlotBtn()
    {
        SoundManager.Instance.UISoundClip("RerollActivate");
        if(GoldManager.Instance.UseCurrentGold(1))
        {
            SetupStoreSlots();
        }
    }

    /// <summary>
    /// 타일들을 리소스 파일에서 자동으로 로드해서 희귀도 따라서 분류하는 스크립트입니다
    /// </summary>
    private void SetStoreTileList()
    {
        List<GameObject> allTilePrefabs = new();

        allTilePrefabs.AddRange(Resources.LoadAll<GameObject>("Prefabs/Tiles/BookTile"));
        allTilePrefabs.AddRange(Resources.LoadAll<GameObject>("Prefabs/Tiles/EquipTile"));
        allTilePrefabs.AddRange(Resources.LoadAll<GameObject>("Prefabs/Tiles/PotionTile"));
        allTilePrefabs.AddRange(Resources.LoadAll<GameObject>("Prefabs/Tiles/SummonTile"));
        allTilePrefabs.AddRange(Resources.LoadAll<GameObject>("Prefabs/Tiles/WeaponTile"));
        allTilePrefabs.AddRange(Resources.LoadAll<GameObject>("Prefabs/Tiles/TrinketTile"));

        foreach (GameObject tilePrefab in allTilePrefabs)
        {
            if(tilePrefab.GetComponent<TileObject>().GetTileData().TileGrade == TileGrade.Normal)
            {
                _normalStoreTiles.Add(tilePrefab);
            }
            else if (tilePrefab.GetComponent<TileObject>().GetTileData().TileGrade == TileGrade.Rare)
            {
                _rareStoreTiles.Add(tilePrefab);
            }
            else if (tilePrefab.GetComponent<TileObject>().GetTileData().TileGrade == TileGrade.Epic)
            {
                _epicStoreTiles.Add(tilePrefab);
            }
            else
            {
                _legendaryStoreTiles.Add(tilePrefab);
            }

        }
    }

    /// <summary>
    /// 입력한 타일 데이터가 현재 상점에 등장해도 되는 타일인지 판단합니다. 
    /// </summary>
    /// <param name="tile">조건을 판단할 타일입니다</param>
    /// <param name="list">내가 조건을 판단할 타일을 뽑은 리스트입니다.</param>
    /// <returns></returns>
    private GameObject CheckTileCondition(GameObject tile, List<GameObject> list)
    {
        _safeInt++;
        if (_safeInt > 15)
        {
            Debug.LogError("너무 많이 조건에 맞는 타일을 상점에 생성하려는 시도가 반복됨! 오류!");
            _safeInt = 0;
            return tile;
        }
        TileObject tileObject = tile.GetComponent<TileObject>();
        //조건 부분이 null이라면 만족입니다.
        if(tileObject.GetTileData().RequiredTile == null)
        {
            _safeInt = 0;
            return tile;
        }
        else
        {
            //조건을 만족하면 끝냅니다.
            if (GridManager.Instance.PlacedTileList.Contains(tileObject.GetTileData().RequiredTile.tileName))
            {
                _safeInt = 0;
                return tile;
            }
                
            else
            {
                //만족 안했으므로 다시 돌립니다.
                List<GameObject> newList = new(list);
                //조건을 만족 안하는 타일을 리스트에서 일시적으로 제거합니다.
                if(newList.Contains(tile))
                {
                    newList.Remove(tile);
                }
                if(newList.Count == 0)
                {
                    Debug.LogWarning("희귀도에 알맞는, 상점에 등장할 수 있는 타일이 없어용");
                    return null;
                }
                int randomIndex = Random.Range(0, newList.Count);
                GameObject chosenTile = newList[randomIndex];
                return CheckTileCondition(chosenTile, newList);
            }
            
        }
    }

    
}
