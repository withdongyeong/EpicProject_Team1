using UnityEngine;

/// <summary>
/// 커서 이미지를 설정하는 매니저입니다.
/// </summary>
public class CursorManager : Singleton<CursorManager>
{
    [SerializeField] private Texture2D defaultCursorTexture;   // 기본 커서
    [SerializeField] private Texture2D clickCursorTexture;     // 클릭 중 커서
    [SerializeField] private Vector2 hotSpot = Vector2.zero;   // 클릭 위치 기준
    [SerializeField] private CursorMode cursorMode = CursorMode.Auto;

    private void Start()
    {
        if (defaultCursorTexture == null)
        {
            defaultCursorTexture = Resources.Load<Texture2D>("Cursor/default_cursor");
        }

        if (clickCursorTexture == null)
        {
            clickCursorTexture = Resources.Load<Texture2D>("Cursor/click_cursor");
        }

        SetCursor(defaultCursorTexture);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SetCursor(clickCursorTexture);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            SetCursor(defaultCursorTexture);
        }
    }

    private void SetCursor(Texture2D texture)
    {
        if (texture != null)
        {
            Cursor.SetCursor(texture, hotSpot, cursorMode);
        }
    }
}