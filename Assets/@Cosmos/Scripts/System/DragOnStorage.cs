using System;
using UnityEngine;

public class DragOnStorage : DraggableObject
{
    
    private Vector3 _originalPosition;
    private float _rotateZ;
    private SellTilePanel _sellTilePanel;
    private StorageArea _storageArea;
    private TileObject _tileObject;

    private void Awake()
    {
        _sellTilePanel = FindAnyObjectByType<SellTilePanel>();
        _storageArea = FindAnyObjectByType<StorageArea>();
        _tileObject = GetComponent<TileObject>();
    }

    protected override void BeginDrag()
    {
        ShowSellPanel();
        // 드래그 시작 시 원래 위치와 회전값 저장
        _rotateZ = transform.rotation.eulerAngles.z;
        //드래그 시작 시 원래 위치 저장
        _originalPosition = transform.position;
        _rotateZ = transform.rotation.eulerAngles.z;
        foreach(Cell cell in gameObject.GetComponentsInChildren<Cell>())
        {
            if(cell.GetType() == typeof(Cell))
                cell.GetComponent<Collider2D>().enabled = false;
        }
    }
    
    protected override void Drag()
    {
        if(!_tileObject.IsStarDisplayEnabled)
            _tileObject.ShowStarCell();
    }
    
    protected override void EndDrag()
    {
        HideSellPanel();
        
        //1. 그리드 안에 배치 가능하다면 -> 배치
        if (DragManager.Instance.CanPlaceTile())
        {
            foreach (var coll in gameObject.GetComponentsInChildren<Collider2D>())
            {
                coll.enabled = true;
            }
            DragManager.Instance.PlaceObject();
            gameObject.AddComponent<DragOnGrid>();
            Destroy(this);
            return;
        }

        //2. 판매 패널이 열려있고 판매 가능하다면 -> 판매
        if (_sellTilePanel.IsCanSell)
        {
            _sellTilePanel.SellTileObject(_tileObject);
            return;
        }
        
        //3. 그리드 밖 보관 공간에 둔다면-> 보관함에 두기
        if (_storageArea.IsCanStore)
        {
            _storageArea.StoreTileObject(_tileObject);
            foreach (var coll in gameObject.GetComponentsInChildren<Collider2D>())
            {
                coll.enabled = true;
            }
            return;
        }
        
        //4. 그리드 안에 배치 불가능하다면 -> 원래 위치로
        if (!DragManager.Instance.CanPlaceTile())
        {
            foreach (var coll in gameObject.GetComponentsInChildren<Collider2D>())
            {
                coll.enabled = true;
            }
            //원래 위치로 되돌리기
            transform.position = _originalPosition;
            //원래 회전으로 되돌리기
            transform.rotation = Quaternion.Euler(0f, 0f, _rotateZ);
            return;
        }
        
        DragManager.Instance.DestroyObject();
    }
    
    private void ShowSellPanel()
    {
        if (_sellTilePanel == null)
        {
            _sellTilePanel = FindAnyObjectByType<SellTilePanel>();
        }
        
        _sellTilePanel.ShowPanel(_tileObject.GetTileData().TileCost);
    }
    
    public void HideSellPanel()
    {
        if (_sellTilePanel == null)
        {
            _sellTilePanel = FindAnyObjectByType<SellTilePanel>();
        }
        
        _sellTilePanel.HidePanel();
    }
}
