using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Localization.Plugins.XLIFF.V12;

public class PlacedHandler : MonoBehaviour
{
    public GameObject _tile;
    private GameObject _placedTile;

    

    public void FirstPresent()
    {
        Placed(_tile);
    }

    private void Placed(GameObject tile)
    {
        _placedTile = Instantiate(tile, Vector3.zero, Quaternion.identity);
        _placedTile.AddComponent<DragOnGrid>();
        foreach (Cell cell in _placedTile.GetComponentsInChildren<Cell>())
        {
            if(cell.GetType() == typeof(Cell))
            {
                cell.AddComponent<BoxCollider2D>();
                HoverTileInfo hti = cell.AddComponent<HoverTileInfo>();
                hti.SetTileObject(_placedTile.GetComponent<TileObject>());
            }
                
        }
        PlaceTile();
    }

    private void PlaceTile()
    {
        _placedTile.transform.SetParent(GridManager.Instance.TilesOnGrid.gameObject.transform);
        _placedTile.transform.localPosition = Vector3.zero;
        SetGridSprite();
        foreach (var cell in _placedTile.GetComponentsInChildren<Cell>())
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

        SoundManager.Instance.UISoundClip("DeploymentActivate");
        
        
        TileObject tileObject = _placedTile.GetComponent<TileObject>();

        tileObject.OnPlaced();
        //배치된 타일이 인접효과를 계산하게 합니다
        tileObject.UpdateStarList();
        //타일이 배치되었음을 알립니다. 현재 사용하는애가 없습니다.
        EventBus.PublishTilePlaced(tileObject);
       
    }

    
    //그리드 Sprite 설정
    private void SetGridSprite()
    {
        Cell[] allCells = _placedTile.GetComponentsInChildren<Cell>();
        List<Vector3Int> cellsPos = new List<Vector3Int>();

        foreach (var cell in allCells)
        {
            if (cell.GetType() == typeof(Cell))
                cellsPos.Add(GridManager.Instance.WorldToGridPosition(cell.transform.position));
        }
        GridManager.Instance.GridSpriteController.SetSprite(cellsPos.ToArray());
    }
}
