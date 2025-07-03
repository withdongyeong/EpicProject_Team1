using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class StoreSlotController : MonoBehaviour
{
    private StoreSlot[] storeSlots;
    
    

    private int _safeInt = 0;

    private GameObject _safePrefab;

    //이 밑은 희귀도에 따라서 분류된 리스트입니다
    private List<GameObject> _normalStoreTiles = new();
    private List<GameObject> _rareStoreTiles = new();
    private List<GameObject> _epicStoreTiles = new();
    private List<GameObject> _legendaryStoreTiles = new();
    private List<GameObject> _mythicStoreTiles = new();
    //가이드용
    public List<GameObject> guideList = new List<GameObject>();
    public bool isGuide = false;


    private void Awake()
    {
        storeSlots = GetComponentsInChildren<StoreSlot>();
        SetStoreTileList();

    }

    private void Start()
    {
        SetupStoreSlots();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.D))
        {
            ResetSlotBtn();
        }
    }


    private void SetupStoreSlots()
    {
        List<GameObject> appeardTileList = new();
        
        for (int i = 0; i < storeSlots.Length; i++)
        {
            float roll = Random.value * 100f;
            TileGrade chosenGrade;
            //현재 확률
            ShopChanceClass chanceList = GlobalSetting.Shop_ChanceList[StageSelectManager.Instance.StageNum];
            //TODO: 이거 줄 줄이기
            if (roll < chanceList.shop_NormalChance)
            {
                chosenGrade = TileGrade.Normal;
            }
            else if (roll < chanceList.shop_NormalChance + chanceList.shop_RareChance)
            {
                chosenGrade = TileGrade.Rare;
            }
            else if (roll < chanceList.shop_NormalChance + chanceList.shop_RareChance + chanceList.shop_EpicChance)
            {
                chosenGrade = TileGrade.Epic;
            }
            else if(roll < chanceList.shop_NormalChance + chanceList.shop_RareChance + chanceList.shop_EpicChance + chanceList.shop_LegendaryChance)
            {
                chosenGrade = TileGrade.Legendary;
            }
            else
            {
                chosenGrade = TileGrade.Mythic;
            }

            List<GameObject> chosenList = _normalStoreTiles;
            switch (chosenGrade)
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
                case TileGrade.Mythic:
                    chosenList = _mythicStoreTiles;
                    break;

            }
            //가이드용
            if (isGuide)
            {
                chosenList = guideList;
                storeSlots[i].SetSlot(chosenList[0].GetComponent<TileObject>().GetTileData().TileCost, chosenList[0]);
                storeSlots[i].GetComponent<Image>().SetNativeSize();
                continue;
            }
            int randomIndex = Random.Range(0, chosenList.Count);
            GameObject chosenTile = chosenList[randomIndex];
            //선택된 타일이 상점에 등장해도 되는지 조건검사를 합니다.
            chosenTile = CheckTileCondition(chosenTile, chosenList, appeardTileList);
            storeSlots[i].SetSlot(chosenTile.GetComponent<TileObject>().GetTileData().TileCost, chosenTile);
            appeardTileList.Add(chosenTile);
            
            //이미지 비율을 맞추기 위한 코드입니다.
            //storeSlots[i].GetComponent<Image>().preserveAspect = true;
            storeSlots[i].GetComponent<Image>().SetNativeSize();
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
        _safePrefab = Resources.Load<GameObject>("Prefabs/Tiles/WeaponTile/GuideStaffTile");

        foreach (GameObject tilePrefab in allTilePrefabs)
        {
            TileGrade grade = tilePrefab.GetComponent<TileObject>().GetTileData().TileGrade;
            if (grade == TileGrade.Normal)
            {
                _normalStoreTiles.Add(tilePrefab);
            }
            else if (grade == TileGrade.Rare)
            {
                _rareStoreTiles.Add(tilePrefab);
            }
            else if (grade == TileGrade.Epic)
            {
                _epicStoreTiles.Add(tilePrefab);
            }
            else if (grade == TileGrade.Legendary)
            {
                _legendaryStoreTiles.Add(tilePrefab);
            }
            else
            {
                _mythicStoreTiles.Add(tilePrefab);
            }

        }
    }

    /// <summary>
    /// 입력한 타일 데이터가 현재 상점에 등장해도 되는 타일인지 판단합니다. 
    /// </summary>
    /// <param name="tile">조건을 판단할 타일입니다</param>
    /// <param name="list">내가 조건을 판단할 타일을 뽑은 리스트입니다.</param>
    /// <returns></returns>
    private GameObject CheckTileCondition(GameObject tile, List<GameObject> list,List<GameObject> alreadyPlacedList)
    {
        _safeInt++;
        if (_safeInt > 20)
        {
            Debug.LogError("너무 많이 조건에 맞는 타일을 상점에 생성하려는 시도가 반복됨! 오류!");
            _safeInt = 0;
            return tile;
        }
        TileObject tileObject = tile.GetComponent<TileObject>();
        bool isAvailable = true;
        //필요한 타일이 있는지 검사합니다.
        if(tileObject.GetTileData().RequiredTile != null)
        {
            if(!GridManager.Instance.PlacedTileList.Contains(tileObject.GetTileData().RequiredTile.tileName))
            {
                isAvailable = false;
            }

        }

        //겹치면 안되는 타일이 있는지 검사합니다.
        if(tileObject.GetTileData().RejectTileList.Count > 0)
        {
            foreach(TileData tileData in tileObject.GetTileData().RejectTileList)
            {
                if (GridManager.Instance.PlacedTileList.Contains(tileData.tileName))
                {
                    isAvailable = false;
                }
            }    
            
        }

        //이미 상점에 뜬 타일인지 검사합니다.
        if(alreadyPlacedList != null)
        {
            if(alreadyPlacedList.Contains(tile))
            {
                isAvailable = false;
            }
        }

        if(isAvailable)
        {
            _safeInt = 0;
            return tile;
        }
        else
        {
            //만족 안했으므로 다시 돌립니다.
            List<GameObject> newList = new(list);
            //조건을 만족 안하는 타일을 리스트에서 일시적으로 제거합니다.
            if (newList.Contains(tile))
            {
                newList.Remove(tile);
            }
            if (newList.Count == 0)
            {
                Debug.LogWarning("희귀도에 알맞는, 상점에 등장할 수 있는 타일이 없어용");
                return _safePrefab;
            }
            int randomIndex = Random.Range(0, newList.Count);
            GameObject chosenTile = newList[randomIndex];
            return CheckTileCondition(chosenTile, newList,alreadyPlacedList);
        }

    }

    
   
}
