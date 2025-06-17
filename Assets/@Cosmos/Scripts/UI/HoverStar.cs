using UnityEngine;
using UnityEngine.EventSystems;

public class HoverStar : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            Debug.LogError("SpriteRenderer component not found on HoverStar.");
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("HoverStar OnPointerEnter called");
        sr.enabled = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("HoverStar OnPointerExit called");
        sr.enabled = true;
    }
}
