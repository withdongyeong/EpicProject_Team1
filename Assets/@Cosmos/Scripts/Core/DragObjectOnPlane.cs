using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragObjectOnPlane : MonoBehaviour,IBeginDragHandler,IDragHandler,IEndDragHandler
{

    Transform draggedTransform;

    private int rotationZ = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpriteMask[] list = Object.FindObjectsByType<SpriteMask>(FindObjectsSortMode.InstanceID);
        foreach(SpriteMask mask in list)
        {
            Debug.Log(mask.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && draggedTransform != null)
        {
            RotatePreviewBlock();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("드래그 시작");
        //마우스 포인터로 누른 지점의 월드 포지션을 가져옵니다
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f));
        worldPos.z = 0f;

        //이게 누른 지점의 그리드 포지션입니다
        Vector3Int clickedGridPosition = GridManager.Instance.WorldToGridPosition(worldPos);

        //비어있는지 확인합니다 비어있으면 취소합니다
        if (GridManager.Instance.IsCellAvailable(clickedGridPosition))
        {
            return;
        }
        else
        {
            //통합된 셀 스크립트를 가져옵니다
            CombineCell cC = GridManager.Instance.GetCellData(clickedGridPosition).GetObjectData();

            //셀 스크립트를 통해 타일을 가져옵니다.
            Transform tile = cC.transform.parent;

            foreach(CombineCell combineCell in tile.GetComponentsInChildren<CombineCell>())
            {
                //통합된 셀 밑에 있는 각 셀들
                foreach (Cell cell in combineCell.GetComponentsInChildren<Cell>())
                {
                    Transform child = cell.transform;
                    Vector3Int gridPos = GridManager.Instance.WorldToGridPosition(child.position);
                    GridManager.Instance.ReleaseCell(gridPos);
                }

            }

            draggedTransform = tile;
            UpdateDragPosition();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (draggedTransform == null)
            return;
        UpdateDragPosition();
        
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (draggedTransform == null) return;

        //배치 불가능할시
        if (!(GridManager.Instance.CanPlaceBlock(draggedTransform)))
        {
            Debug.Log("배치 불가능한 위치입니다.");
            //TODO: 이거 파괴하지말고 인벤토리로 되돌려야합니다!
            Destroy(draggedTransform.gameObject);
            draggedTransform = null;
            return;
        }

        //배치 가능할시

        //배치 위치로 오브젝트 이동
        Vector3 corePos = GridManager.Instance.GridToWorldPosition(GridManager.Instance.WorldToGridPosition(draggedTransform.GetComponentInChildren<CombineCell>().coreCell.transform.position));
        draggedTransform.position = corePos;

        foreach (Cell cell in draggedTransform.GetComponentsInChildren<Cell>())
        {
            Transform child = cell.transform;
            Vector3Int gridPos = GridManager.Instance.WorldToGridPosition(child.position);
            GridManager.Instance.OccupyCell(gridPos, cell);
        }

    }

    private void UpdateDragPosition()
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f));
        worldPos.z = 0f;
        draggedTransform.position = worldPos;
    }

    private void RotatePreviewBlock()
    {
        rotationZ = (rotationZ + 90) % 360; // 90도씩 회전
        if(draggedTransform != null)
        {
            draggedTransform.transform.rotation = Quaternion.Euler(0, 0, rotationZ);
        }
    }
}
