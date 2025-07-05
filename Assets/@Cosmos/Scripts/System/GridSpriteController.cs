using System;
using UnityEngine;
using System.Linq;

public class GridSpriteController : MonoBehaviour
{
    [SerializeField]
    private Sprite[] cellOccupiedSprites;
    
    private Sprite r0;
    private Sprite r1w;
    private Sprite r1a;
    private Sprite r1s;
    private Sprite r1d;
    private Sprite r2wa;
    private Sprite r2as;
    private Sprite r2sd;
    private Sprite r2dw;
    private Sprite r3w;
    private Sprite r3a;
    private Sprite r3s;
    private Sprite r3d;
    private Sprite r4;

    private Sprite r2ad;
    private Sprite r2ws;


    
    


    private void Awake()
    {
        LoadSprites();
    }

    private void LoadSprites()
    {
        cellOccupiedSprites = Resources.LoadAll<Sprite>("NewBoard/RuleCell");
        r0 = cellOccupiedSprites.FirstOrDefault(s => s.name == "r0");
        r1w = cellOccupiedSprites.FirstOrDefault(s => s.name == "r1w");
        r1a = cellOccupiedSprites.FirstOrDefault(s => s.name == "r1a");
        r1s = cellOccupiedSprites.FirstOrDefault(s => s.name == "r1s");
        r1d = cellOccupiedSprites.FirstOrDefault(s => s.name == "r1d");
        r2wa = cellOccupiedSprites.FirstOrDefault(s => s.name == "r2wa");
        r2as = cellOccupiedSprites.FirstOrDefault(s => s.name == "r2as");
        r2sd = cellOccupiedSprites.FirstOrDefault(s => s.name == "r2sd");
        r2dw = cellOccupiedSprites.FirstOrDefault(s => s.name == "r2dw");
        r3w = cellOccupiedSprites.FirstOrDefault(s => s.name == "r3w");
        r3a = cellOccupiedSprites.FirstOrDefault(s => s.name == "r3a");
        r3s = cellOccupiedSprites.FirstOrDefault(s => s.name == "r3s");
        r3d = cellOccupiedSprites.FirstOrDefault(s => s.name == "r3d");
        r4 = cellOccupiedSprites.FirstOrDefault(s => s.name == "r4");
        r2ad = cellOccupiedSprites.FirstOrDefault(s => s.name == "r2ad");
        r2ws = cellOccupiedSprites.FirstOrDefault(s => s.name == "r2ws");
        
        if(r0 == null || r1w == null || r1a == null || r1s == null || r1d == null ||
           r2wa == null || r2as == null || r2sd == null || r2dw == null || r3w == null || r3a == null ||
           r3s == null || r3d == null || r4 == null)
        {
            Debug.LogError("One or more sprites could not be loaded. Please check the sprite names in the Resources folder.");
        }
    }


    public void SetSprite(Vector3Int[] cells)
    {
        foreach (Vector3Int cell in cells)
        {
            GridManager.Instance.SetCellSprite(cell, ChooseCellSprite(cells, cell));
        }
    }


    private Sprite ChooseCellSprite(Vector3Int[] cells, Vector3Int pos)
    {
        Vector3Int posW = new Vector3Int(pos.x , pos.y+1, pos.z);
        Vector3Int posA = new Vector3Int(pos.x-1, pos.y, pos.z);
        Vector3Int posS = new Vector3Int(pos.x, pos.y-1, pos.z);
        Vector3Int posD = new Vector3Int(pos.x+1, pos.y, pos.z);
        if (!CheckCell(cells ,posW) && !CheckCell(cells ,posA) && !CheckCell(cells ,posS) && !CheckCell(cells ,posD))
        {
            return r4 ; // 모든 방향에 셀 없음 
        }
        if (CheckCell(cells ,posW) && !CheckCell(cells ,posA) && !CheckCell(cells ,posS) && !CheckCell(cells ,posD))
        {
            return r3w ; // 위쪽에 셀이 존재
        }
        if (!CheckCell(cells ,posW) && CheckCell(cells ,posA) && !CheckCell(cells ,posS) && !CheckCell(cells ,posD))
        {
            return r3a ; // 왼쪽에 셀이 존재
        }
        if (!CheckCell(cells ,posW) && !CheckCell(cells ,posA) && CheckCell(cells ,posS) && !CheckCell(cells ,posD))
        {
            return r3s ; // 아래쪽에 셀이 존재
        }
        if (!CheckCell(cells ,posW) && !CheckCell(cells ,posA) && !CheckCell(cells ,posS) && CheckCell(cells ,posD))
        {
            return r3d ; // 오른쪽에 셀이 존재
        }
        if (!CheckCell(cells ,posW) && !CheckCell(cells ,posA) && CheckCell(cells ,posS) && CheckCell(cells ,posD))
        {
            return r2wa ; // 위쪽과 왼쪽에 셀이 존재하지 않음
        }
        if (CheckCell(cells ,posW) && !CheckCell(cells ,posA) && !CheckCell(cells ,posS) && CheckCell(cells ,posD))
        {
            return r2as ; // 왼쪽과 아래쪽에 셀이 존재하지 않음
        }
        if (CheckCell(cells ,posW) && CheckCell(cells ,posA) && !CheckCell(cells ,posS) && !CheckCell(cells ,posD))
        {
            return r2sd ; // 아래쪽과 오른쪽에 셀이 존재하지 않음
        }
        if (!CheckCell(cells ,posW) && CheckCell(cells ,posA) && CheckCell(cells ,posS) && !CheckCell(cells ,posD))
        {
            return r2dw ; // 오른쪽과 위쪽에 셀이 존재하지 않음
        }
        
        if (!CheckCell(cells ,posW) && CheckCell(cells ,posA) && CheckCell(cells ,posS) && CheckCell(cells ,posD))
        {
            return r1w ; // 왼쪽, 아래쪽, 오른쪽에 셀이 존재하지 않음
        }
        if (CheckCell(cells ,posW) && !CheckCell(cells ,posA) && CheckCell(cells ,posS) && CheckCell(cells ,posD))
        {
            return r1a ; // 위쪽, 아래쪽, 오른쪽에 셀이 존재하지 않음
        }
        if (CheckCell(cells ,posW) && CheckCell(cells ,posA) && !CheckCell(cells ,posS) && CheckCell(cells ,posD))
        {
            return r1s ; // 위쪽, 왼쪽, 오른쪽에 셀이 존재하지 않음
        }
        if (CheckCell(cells ,posW) && CheckCell(cells ,posA) && CheckCell(cells ,posS) && !CheckCell(cells ,posD))
        {
            return r1d ; // 위쪽, 왼쪽, 아래쪽에 셀이 존재하지 않음
        }
        if (CheckCell(cells ,posW) && CheckCell(cells ,posA) && CheckCell(cells ,posS) && CheckCell(cells ,posD))
        {
            return r0 ; // 모든 방향에 셀이 존재하지 않음
        }
        
        if (!CheckCell(cells ,posW) && CheckCell(cells ,posA) && !CheckCell(cells ,posS) && CheckCell(cells ,posD))
        {
            return r2ws ; // ws 방향만 셸이 존재하지 않음
        }
        if (CheckCell(cells ,posW) && !CheckCell(cells ,posA) && CheckCell(cells ,posS) && !CheckCell(cells ,posD))
        {
            return r2ad ; // ad 방향만 셸이 존재하지 않음
        }
        
        
        Debug.LogError("Invalid cell configuration at position: " + pos);
        return null; // 잘못된 셀 구성
    }
    private bool CheckCell(Vector3Int[] cells ,Vector3Int pos)
    {
        if(!GridManager.Instance.IsWithinGrid(pos)) return false;
        foreach (Vector3Int cell in cells)
        {
            if(cell == pos)
            {
                return true; // 해당 pos에 존재
            }
        }
        return false; // 해당 pos에 존재하지 않음
    }
}
