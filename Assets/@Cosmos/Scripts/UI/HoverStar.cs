using UnityEngine;
using UnityEngine.EventSystems;

public class HoverStar : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private GameObject combinedStarCell;

    private void Awake()
    {
        combinedStarCell = transform.parent.gameObject;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("HoverStar OnPointerEnter called");

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("HoverStar OnPointerExit called");
    }
}
