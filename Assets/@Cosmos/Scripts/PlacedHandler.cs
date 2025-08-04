using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Unity.VisualScripting;
using Steamworks;
using System.Text;

public class PlacedHandler : MonoBehaviour
{

    
    private Dictionary<string, GameObject> _tileDictionary;
    
    private void Start()
    {
        InitDictionary();
    }

    //타일 이름을 키로 타일 오브젝트를 저장하는 딕셔너리를 초기화합니다.
    private void InitDictionary()
    {
        List<GameObject> allTiles = new List<GameObject>(JournalSlotManager.Instance.AllTiles);

        _tileDictionary = new Dictionary<string, GameObject>();
        foreach (GameObject tile in allTiles)
        {
            if (tile != null)
            {
                TileObject tileObject = tile.GetComponent<TileObject>();
                if (tileObject != null && !string.IsNullOrEmpty(tileObject.data.tileName))
                {
                    _tileDictionary[tileObject.data.tileName] = tile;
                }
            }
        }
    }
    
    // 첫 시작시 기본지급 되는 타일 메서드 입니다.
    public void FirstPresentTile()
    {
        GameObject staffTile = _tileDictionary["StaffTile"];
        StartCoroutine(CreateTileOnGrid(staffTile, Vector3.zero + Vector3.left, 0));
    }

    private GameObject MakeTileForGrid(GameObject tile) // 그리드에 배치할 타일을 생성하는 메서드입니다.
    {
        GameObject newTile = Instantiate(tile, Vector3.zero, Quaternion.identity); // 타일 생성
        
        foreach (Cell cell in newTile.GetComponentsInChildren<Cell>())
        {
            if(cell.GetType() == typeof(Cell))
            {
                cell.AddComponent<BoxCollider2D>();
                HoverTileInfo hti = cell.AddComponent<HoverTileInfo>();
                hti.SetTileObject(newTile.GetComponent<TileObject>());
            }
                
        }

        return newTile;
    }

    private IEnumerator CreateTileOnGrid(GameObject tile, Vector3 pos, int rotation) //타일을 그리드에 배치해주는 메서드 입니다
    {
        //1. 타일 생성
        
        GameObject newTile = MakeTileForGrid(tile);
        //2. 타일 위치 설정
        newTile.transform.SetParent(GridManager.Instance.TilesOnGrid.gameObject.transform);
        //3. 타일 위치 조정
        newTile.transform.localPosition = pos;
        
        //4. 타일의 드래그 컴포넌트 추가
        Vector3Int tilePos = GridManager.Instance.WorldToGridPosition(newTile.transform.position); // 타일의 그리드 위치 계산
        if (GridManager.Instance.IsWithinGrid(tilePos))
        {
            newTile.AddComponent<DragOnGrid>();
        }
        else
        {
            newTile.AddComponent<DragOnStorage>();
        }
        
        //5. 타일의 회전 설정
        newTile.transform.localRotation = Quaternion.Euler(0, 0, rotation);
        //6. 그리드의 Sprite 설정
        SetGridSprite(newTile);
        
        //7 1프레임 대기
        yield return null;
        
        
        if(newTile.GetComponent<DragOnStorage>() != null)
        {
            //드래그 온 스토리지 컴포넌트가 있다면, 스토리지에 배치하는 것이므로 여기서 끝
            yield break;
        }
        
        
        //8. 실제 Grid의 점유 상태 변경
        foreach (var cell in newTile.GetComponentsInChildren<Cell>())
        {
            
            Transform t = cell.transform;
            Vector3Int gridPos = GridManager.Instance.WorldToGridPosition(t.position);
            if (cell.GetType() == typeof(StarCell)) //스타셀일때
            {
                StarCell starCell = cell as StarCell;
                StarBase starSkill = starCell.GetStarSkill();
                GridManager.Instance.AddStarSkill(gridPos, starSkill);
                continue;
            }
            // 스타셀이 아니라 그냥 셀일때
            GridManager.Instance.OccupyCell(gridPos, cell);
        }
        
        //9. 타일 오브젝트 설정
        
        TileObject tileObject = newTile.GetComponent<TileObject>();
        tileObject.OnPlaced();
        //배치된 타일이 인접효과를 계산하게 합니다
        tileObject.UpdateStarList();
        //타일이 배치되었음을 알립니다. 현재 사용하는애가 없습니다.
        EventBus.PublishTilePlaced(tileObject);
       
    }
    
    //그리드 Sprite 설정
    private void SetGridSprite(GameObject tile)
    {
        Cell[] allCells = tile.GetComponentsInChildren<Cell>();
        List<Vector3Int> cellsPos = new List<Vector3Int>();

        foreach (var cell in allCells)
        {
            if (cell.GetType() == typeof(Cell))
                cellsPos.Add(GridManager.Instance.WorldToGridPosition(cell.transform.position));
        }
        GridManager.Instance.GridSpriteController.SetSprite(cellsPos.ToArray());
    }
    

    public void SavePlacedTiles()
    {

        if (GameStateManager.Instance.CurrentState == GameState.Playing ||
            GameStateManager.Instance.CurrentState == GameState.Defeat)
        {
            if (LifeManager.Instance.Life == 0)
            {
                return; //목숨이 없으면 저장하지 않습니다.
            }
            if (LifeManager.Instance.Life > 0)
            {
                //목숨이 1개라도 있다면
                LifeManager.Instance.RemoveLife(1); //하나 감소
            }
        }
        
        

        //1. 데이터 모으기
        SavedTileData savedTileData = new SavedTileData();
        savedTileData.gold = GoldManager.Instance.CurrentGold;
        savedTileData.life = LifeManager.Instance.Life;
        savedTileData.stageIndex = StageSelectManager.Instance.StageNum;
        savedTileData.infiniteModeCount = StageSelectManager.Instance.InfiniteModeCount;
        savedTileData.difficulty = GameManager.Instance.DifficultyLevel;
        savedTileData.savedTiles = new List<PlacedTileData>();
        savedTileData.PurchasedTiles = new List<string>(PurchasedTileManager.Instance.PurchasedTiles);
        
        foreach (TileObject tile in GridManager.Instance.TilesOnGrid.GetComponentsInChildren<TileObject>())
        {
            if (tile != null)
            {
                PlacedTileData data = new PlacedTileData
                {
                    tileName = tile.data.tileName,
                    position = tile.transform.localPosition,
                    rotation = Mathf.RoundToInt(tile.transform.rotation.eulerAngles.z)
                };
                savedTileData.savedTiles.Add(data);
            }
        }
        
        // 2. 데이터를 Json화
        string json = JsonUtility.ToJson(savedTileData, true);

        // 3. JSON 문자열을 바이트 배열로 변환합니다. (Steam API가 요구하는 형식)
        byte[] fileData = Encoding.UTF8.GetBytes(json);

        // 4. Steam 클라우드에 "save_session.dat"이라는 이름으로 파일을 씁니다.
        bool success = SteamRemoteStorage.FileWrite("save_session.dat", fileData, fileData.Length);

        if(success)
        {
            Debug.Log("Steam 클라우드에 저장 성공!");
        }
        else
        {
            Debug.LogError("Steam 클라우드 저장 실패.");
        }
    }
    
    public void LoadPlacedTiles()
    {
        // 1. 클라우드에 "save_session.dat" 파일이 있는지 확인합니다.
        if (SteamRemoteStorage.FileExists("save_session.dat"))
        {
            // 2. 파일 크기를 가져와서 해당 크기만큼의 바이트 배열(버퍼)을 만듭니다.
            int fileSize = SteamRemoteStorage.GetFileSize("save_session.dat");
            byte[] buffer = new byte[fileSize];

            // 3. 파일을 읽어서 버퍼에 저장합니다.
            int bytesRead = SteamRemoteStorage.FileRead("save_session.dat", buffer, buffer.Length);

            if (bytesRead > 0)
            {
                // 4. 읽어온 바이트 배열을 JSON 문자열로 변환합니다.
                string json = Encoding.UTF8.GetString(buffer);

                // 5. JSON 문자열을 SavedTileData 객체로 변환합니다.
                SavedTileData savedTileData = JsonUtility.FromJson<SavedTileData>(json);

                // --- 여기부터는 기존 불러오기 로직과 동일합니다. ---
                GridManager.Instance.ResetGridCompletely();
                GoldManager.Instance.SetCurrentGold(savedTileData.gold);
                LifeManager.Instance.SetLife(savedTileData.life);
                StageSelectManager.Instance.SetStageNum(savedTileData.stageIndex);
                StageSelectManager.Instance.SetInfiniteModeCount(savedTileData.infiniteModeCount);
                GameManager.Instance.SetDifficultyLevel(savedTileData.difficulty);
                PurchasedTileManager.Instance.SetPurchasedTiles(savedTileData.PurchasedTiles);
                //FindAnyObjectByType<StoreSlotController>().SetupStoreSlots();

                foreach (PlacedTileData placedTile in savedTileData.savedTiles)
                {
                    if (_tileDictionary.TryGetValue(placedTile.tileName, out GameObject tilePrefab))
                    {
                        StartCoroutine(CreateTileOnGrid(tilePrefab, placedTile.position, placedTile.rotation));
                    }
                }
                Debug.Log("Steam 클라우드에서 불러오기 성공!");
                //데이터 삭제
                DeletePlacedTiles();
                return;
            }
            
            
        }
        Debug.LogError("Steam 클라우드에서 불러오기 실패: 파일이 존재하지 않거나 읽기 오류 발생.");
    }
    
    
    public void DeletePlacedTiles()
    {
        // Steam 클라우드에서 "save_session.dat" 파일을 삭제합니다.
        if (!SteamRemoteStorage.FileExists("save_session.dat"))
        {
            return;
        }
        if (SteamRemoteStorage.FileDelete("save_session.dat"))
        {
            Debug.Log("Steam 클라우드에서 파일 삭제 성공.");
        }
        else
        {
            Debug.LogError("Steam 클라우드에서 파일 삭제 실패.");
        }
    }
}
