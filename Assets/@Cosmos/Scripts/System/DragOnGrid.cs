using UnityEngine;



/// <summary>
/// Grid에서 Grid로 드래그할 때 사용되는 DraggableObject 클래스입니다.
/// </summary>
public class DragOnGrid : DraggableObject
{
    protected override void BeginDrag()
    {
        
    }
    
    
    protected override void EndDrag()
    {
        //1. 그리드 안에 배치 가능하다면
        if (DragManager.Instance.CanPlaceTile())
        {
            DragManager.Instance.PlaceObject();
            return;
        }
        //2. 그리드 밖에 배치한다면
        
        //3. 보관함에 배치한다면
        
        //그 외
        DragManager.Instance.DestroyObject();
    }
}
