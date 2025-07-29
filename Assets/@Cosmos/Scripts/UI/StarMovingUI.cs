using UnityEngine;
using UnityEngine.UI;

public class StarMovingUI : MonoBehaviour
{
    private Vector2 startPoint;
    private Vector2 endPoint;
    private RectTransform rect;
    private float enableTime = 0;

    private float moveSpeed = 1.4f;


    private void Awake()
    {
        rect = GetComponent<RectTransform>();

        float x = StageSelectManager.Instance.StageNum * 64 - 32;
        float y = -(StageSelectManager.Instance.StageNum % 2) * 128 + 160;
        startPoint = new Vector2(x, y);

        float x2 = StageSelectManager.Instance.StageNum * 64 + 32;
        float y2 = -((StageSelectManager.Instance.StageNum + 1) % 2) * 128 + 160;
        endPoint = new Vector2(x2, y2);

        rect.anchoredPosition = startPoint;

        int num = Mathf.Min(10, StageSelectManager.Instance.StageNum);
        transform.parent.GetComponent<Image>().sprite = StageSelectManager.Instance.StageUISprites[num];
    }

    private void Update()
    {
        Vector2 newPosition = Vector2.Lerp(startPoint, endPoint, enableTime / moveSpeed);
        rect.anchoredPosition = newPosition;
        enableTime += Time.unscaledDeltaTime;
    }

    private void OnEnable()
    {
        rect.anchoredPosition = startPoint;
        enableTime = 0;
    }


}
