using UnityEngine;
using UnityEngine.EventSystems;

public class DragObjectOnPlane : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    Transform draggedTransform;
    Vector3 beforeMovePosition; //물체가 드래그되기 전 있던 포지션입니다

    
    [SerializeField]
    private float rotateZ;
    private SmoothRotator rotator;
    
    void Start()
    {
        SpriteMask[] list = Object.FindObjectsByType<SpriteMask>(FindObjectsSortMode.InstanceID);
        foreach (SpriteMask mask in list)
        {
            Debug.Log(mask.gameObject);
        }
        rotator = gameObject.AddComponent<SmoothRotator>();
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
        if (eventData.button != PointerEventData.InputButton.Left)
            return;
        
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

            //타일의 포지션이 배치의 기준이므로 이걸 복사합니다.
            beforeMovePosition = tile.position;

            //타일 밑의 각 콤바인 셀을 가져옵니다.
            foreach (CombineCell combineCell in tile.GetComponentsInChildren<CombineCell>())
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
            //현재 tile의 rotation을 가져옵니다
            rotateZ = draggedTransform != null ? draggedTransform.rotation.eulerAngles.z : 0f;
            UpdateDragPosition();
            
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;
        if (draggedTransform == null)
            return;
        UpdateDragPosition();
        UpdatePreviewCell();

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;
        if (draggedTransform == null) return;

        Vector3 corePos;
        //배치 불가능할시
        if (!(GridManager.Instance.CanPlaceBlock(draggedTransform)))
        {
            GridManager.Instance.ChangeCellSpriteAll();
            Debug.Log("배치 불가능한 위치입니다.");
            //배치가 불가능한 경우 원래 위치로 되돌립니다
            corePos = beforeMovePosition;
            //원래 rotation으로 되돌립니다
            draggedTransform.rotation = Quaternion.Euler(0f, 0f, rotateZ);
        }
        else // 배치가 가능할시
        {
            //배치 위치로 오브젝트 이동
            Vector3 firstCombineCellCore = draggedTransform.GetComponentInChildren<CombineCell>().coreCell.transform.position;
            corePos = GridManager.Instance.GridToWorldPosition(GridManager.Instance.WorldToGridPosition(firstCombineCellCore));
        }
        draggedTransform.position = corePos;
        Debug.Log(GridManager.Instance.WorldToGridPosition(draggedTransform.GetComponentInChildren<CombineCell>().coreCell.transform.position));

        foreach (Cell cell in draggedTransform.GetComponentsInChildren<Cell>())
        {
            Transform child = cell.transform;
            Vector3Int gridPos = GridManager.Instance.WorldToGridPosition(child.position);
            GridManager.Instance.OccupyCell(gridPos, cell);
        }
        draggedTransform = null;
    }

    private void UpdateDragPosition()
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f));
        worldPos.z = 0f;
        draggedTransform.position = worldPos;
    }

    private void UpdatePreviewCell()
    {
        GridManager.Instance.ChangeCellSpriteAll();
        foreach (Cell cell in draggedTransform.GetComponentsInChildren<Cell>())
        {
            Transform child = cell.transform;
            Vector3Int gridPos = GridManager.Instance.WorldToGridPosition(child.position);
            GridManager.Instance.ChangeCellSprite(gridPos, true);
        }
        
    }

    private void RotatePreviewBlock()
    {
        rotator.RotateZ(draggedTransform,UpdatePreviewCell);
        
    }
}
