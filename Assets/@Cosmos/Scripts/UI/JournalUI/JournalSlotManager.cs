using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class JournalSlotManager : Singleton<JournalSlotManager>
{
    private int _unlockLevel;

    private GameObject _journalSlotPrefab;
    private GameObject _lockedJournalSlotPrefab;

    private GameObject _safePrefab;

    private List<GameObject> _showOnUnlock = new();
    private int[] _unlockedTileNumList = new int[5];

    private GameObject _tileInfoPanel;

    private Transform _showUnlockedTilePanel;
    private JournalSlot _unlockedTileSlot;

    private bool _isInit;
    private bool _isJournalOpen = false;

    private InfoPanel _infoPanel;

    private Transform _slotParent; //슬롯의 부모, 그러니까 StoreSlotController가 붙은 쯤의 위치입니다.
    private GameObject _scrollView; //이거 끄면 저널이 안보이게됩니다.

    private LocalizedString _localizedString;

    //이 밑은 희귀도에 따라서 분류된 리스트입니다
    //private List<GameObject> _normalStoreTiles = new();
    //private List<GameObject> _rareStoreTiles = new();
    //private List<GameObject> _epicStoreTiles = new();
    //private List<GameObject> _legendaryStoreTiles = new();
    //private List<GameObject> _mythicStoreTiles = new();

    private List<GameObject>[] _storeTiles = new List<GameObject>[5];

    //아직 해금 안된 리스트입ㄴ디ㅏ
    private List<GameObject>[] _lockedStoreTiles = new List<GameObject>[5];


    //첫상점에 무조건 뜨는 리스트입니다
    private List<GameObject> _firstStoreTiles = new();

    //리스트에 접근하는 프로퍼티입니다
    //public List<GameObject> NormalStoreTiles => _normalStoreTiles;
    //public List<GameObject> RareStoreTiles => _rareStoreTiles;
    //public List<GameObject> EpicStoreTiles => _epicStoreTiles;
    //public List<GameObject> LegendaryStoreTiles => _legendaryStoreTiles;
    //public List<GameObject> MythicStoreTiles => _mythicStoreTiles;
    public List<GameObject>[] StoreTiles => _storeTiles;
    public List<GameObject> FirstStoreTiles => _firstStoreTiles;
    public bool IsJournalOpen => _isJournalOpen;


    protected override void Awake()
    {
        base.Awake();
        _journalSlotPrefab = Resources.Load<GameObject>("Prefabs/UI/Journal/JournalSlot");
        _lockedJournalSlotPrefab = Resources.Load<GameObject>("Prefabs/UI/Journal/LockedJournalSlot");
        _scrollView = transform.GetChild(0).GetChild(0).gameObject;
        _slotParent = _scrollView.transform.GetChild(0).GetChild(0);
        _infoPanel = FindAnyObjectByType<InfoPanel>(FindObjectsInactive.Include);
        _showUnlockedTilePanel = transform.GetChild(0).GetChild(1);
        _unlockedTileSlot = _showUnlockedTilePanel.GetComponentInChildren<JournalSlot>();
        EventBus.SubscribeSceneLoaded(CloseJournalOnSceneChange);
        _showOnUnlock.Add(Resources.Load<GameObject>("Prefabs/Tiles/SummonTIle/SwordTile"));
        _showOnUnlock.Add(Resources.Load<GameObject>("Prefabs/Tiles/SummonTIle/DamageTotemTile"));
        _showOnUnlock.Add(Resources.Load<GameObject>("Prefabs/Tiles/SummonTIle/CloudTile"));
        _showOnUnlock.Add(Resources.Load<GameObject>("Prefabs/Tiles/SummonTIle/TurtleTile"));
        EventBus.SubscribeSceneLoaded(ShowUnlockTilesOnTitle);
        _localizedString = new LocalizedString("EpicProject_Table", "UI_Text_UnlockedTileCount");
        _isInit = false;

        for(int i=0; i<_storeTiles.Length; i++)
        {
            _storeTiles[i] = new List<GameObject>();
            _lockedStoreTiles[i] = new List<GameObject>();
        }
    }


    private void Start()
    {
        //DontDestroyOnLoad(Instantiate(_eventSystem));
        SetStoreTileList();
        _showUnlockedTilePanel.gameObject.SetActive(false);
        _isInit = true; 
    }

    public void SetStoreTileList()
    {
        //현재 해금된 애들. 이 번호보다 작거나 같으면 해금된거에요
        _unlockLevel = GameManager.Instance.CurrentUnlockLevel;

        List<GameObject> allTilePrefabs = new();

        allTilePrefabs.AddRange(Resources.LoadAll<GameObject>("Prefabs/Tiles/WeaponTile"));
        allTilePrefabs.AddRange(Resources.LoadAll<GameObject>("Prefabs/Tiles/BookTile"));
        allTilePrefabs.AddRange(Resources.LoadAll<GameObject>("Prefabs/Tiles/SummonTile"));
        allTilePrefabs.AddRange(Resources.LoadAll<GameObject>("Prefabs/Tiles/EquipTile"));
        allTilePrefabs.AddRange(Resources.LoadAll<GameObject>("Prefabs/Tiles/PotionTile"));
        allTilePrefabs.AddRange(Resources.LoadAll<GameObject>("Prefabs/Tiles/TrinketTile"));
        _safePrefab = Resources.Load<GameObject>("Prefabs/Tiles/WeaponTile/GuideStaffTile");

        ClearStoreTileLists();

        foreach (GameObject tilePrefab in allTilePrefabs)
        {
            TileInfo tileInfo = tilePrefab.GetComponent<TileObject>().GetTileData();
            TileGrade grade = tileInfo.TileGrade;

            _unlockedTileNumList[tileInfo.UnlockInt]++;
            //해금 된 별자리들
            if (tileInfo.UnlockInt <= _unlockLevel)
            {
                //초심자의 지팡이는 제외입니다
                if(tileInfo.TileName != "GuideStaffTile")
                {
                    //넣어줍니다
                    _storeTiles[(int)grade].Add(tilePrefab);
                }
                if(grade == TileGrade.Normal) // 맨 처음에 띄어줄 지팡이랑 화염살만 넣어줍니다
                {
                    foreach (TileData tileData in GlobalSetting.Shop_FirstTileDataList)
                    {
                        if (tileData.tileName == tileInfo.TileName)
                        {
                            _firstStoreTiles.Add(tilePrefab);
                        }
                    }
                }
            }
            else
            {   
                //해금 안된 애들 넣어줍니다
                _lockedStoreTiles[(int)grade].Add(tilePrefab);
            }
        }

        InstantiateAllJournalSlots();
    }


    private void InstantiateAllJournalSlots()
    {
        //이거 안하면 오류납니다.
        _scrollView.SetActive(true);
        //먼저 저널 슬롯 싹 비웁니다
        for (int i = _slotParent.childCount - 1; i >= 0; i--)
        {
            Destroy(_slotParent.GetChild(i).gameObject);
        }

        for(int i = 0; i < _storeTiles.Length; i++)
        {
            InstantiateJournalSlotList(_storeTiles[i]);
        }
        InstantiateLockedJournalSlotList();

        //볼장 다 봤으니 다시 끕니다
        _scrollView.SetActive(false);
    }

    private void InstantiateJournalSlotList(List<GameObject> objects)
    {
        foreach(GameObject tile in objects)
        {
            JournalSlot journalSlot = Instantiate(_journalSlotPrefab, _slotParent).GetComponentInChildren<JournalSlot>();
            journalSlot.SetSlot(tile);
            journalSlot.GetComponent<Image>().SetNativeSize();

        }
    }

    private void InstantiateLockedJournalSlotList()
    {
        for(int i = 0; i<_lockedStoreTiles.Length; i++)
        {
            foreach (GameObject tile in _lockedStoreTiles[i])
            {
                LockedJournalSlot journalSlot = Instantiate(_lockedJournalSlotPrefab, _slotParent).GetComponentInChildren<LockedJournalSlot>();
                journalSlot.SetSlot(tile);
                journalSlot.GetComponent<Image>().SetNativeSize();
            }
        }   
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            CloseJournal();
        }
    }


    public void CloseJournal()
    {
        SoundManager.Instance.UISoundClip("ButtonActivate");
        if (DragManager.Instance.GetCurrentDragObject() == null)
        {
            _scrollView.SetActive(false);

            if (_infoPanel != null)
            {
                _infoPanel.Hide();
            }
            _isJournalOpen = false;
        }
    }

    public void CloseJournalOnSceneChange(Scene scene, LoadSceneMode mode)
    {
        if(_isInit)
        {
            _scrollView.SetActive(false);
            _isJournalOpen = false;
        }
        
    }

    public void ToggleJournal()
    {
        if(_scrollView.activeSelf)
        {
            _scrollView.SetActive(false);
            _isJournalOpen = false;
        }
        else
        {
            _scrollView.SetActive(true);
            _isJournalOpen = true;
        }
    }

    public void ShowUnlockTiles()
    {
        if(SaveManager.ShownUnlockLevel < SaveManager.UnlockLevel)
        {
            SaveManager.SaveShownUnlockLevel(SaveManager.ShownUnlockLevel + 1);
            if(SaveManager.ShownUnlockLevel - 1 < _showOnUnlock.Count)
            {
                if (_showOnUnlock[SaveManager.ShownUnlockLevel - 1] != null)
                {
                    _showUnlockedTilePanel.gameObject.SetActive(true);
                    _unlockedTileSlot.SetSlot(_showOnUnlock[SaveManager.ShownUnlockLevel - 1]);

                    _localizedString.StringChanged += (text) =>
                    {
                        _showUnlockedTilePanel.GetChild(3).GetComponent<TextMeshProUGUI>().text = text.Replace("{0}", _unlockedTileNumList[SaveManager.ShownUnlockLevel].ToString());
                    };

                }
            }
                       
        }
    }

    public void HideUnlockTiles()
    {
        _showUnlockedTilePanel.gameObject.SetActive(false);
        ShowUnlockTiles();
    }

    private void ShowUnlockTilesOnTitle(Scene scene, LoadSceneMode mode)
    {
        if(SceneLoader.IsInTitle())
        {
            ShowUnlockTiles();
        }
    }

    private void ClearStoreTileLists()
    {
        foreach(List<GameObject> tileLists in _storeTiles)
        {
            tileLists.Clear();
        }

        foreach (List<GameObject> tileLists in _lockedStoreTiles)
        {
            tileLists.Clear();
        }
        _unlockedTileNumList = new int[5];
        _firstStoreTiles.Clear();
    }

    private void OnDestroy()
    {
        EventBus.UnsubscribeSceneLoaded(CloseJournalOnSceneChange);
        EventBus.UnsubscribeSceneLoaded(ShowUnlockTilesOnTitle);
    }

}
