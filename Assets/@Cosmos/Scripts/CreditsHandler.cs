using UnityEngine;
using TMPro;

public class CreditsHandler : MonoBehaviour
{
    private TextMeshProUGUI creditsText; // 크레딧 텍스트
    private float normalSpeed = 100f; // 기본 스크롤 속도 (픽셀/초)
    private float fastSpeed = 300f; // 빠른 스크롤 속도 (픽셀/초)
    private float endPosition = 600f; // 텍스트가 도달하면 종료되는 Y 위치

    private float currentSpeed; // 현재 스크롤 속도
    private RectTransform textRectTransform;
    private bool isFastMode = false;
    private bool isEndPosition = false;

    void Start()
    {
        creditsText = FindAnyObjectByType<TextMeshProUGUI>();
        if (creditsText == null)
        {
            Debug.LogError("Credits Text가 설정되지 않았습니다!");
            return;
        }

        textRectTransform = creditsText.GetComponent<RectTransform>();
        currentSpeed = normalSpeed; // 초기 속도는 기본 속도
    }

    void Update()
    {
        // 텍스트를 위로 이동
        textRectTransform.anchoredPosition += Vector2.up * currentSpeed * Time.deltaTime;

        // 마우스 버튼 누르는 동안 속도 증가
        isFastMode = Input.GetMouseButton(0); // 마우스 왼쪽 버튼 누르는 중
        currentSpeed = isFastMode ? fastSpeed : normalSpeed;
        Debug.Log(isFastMode ? "빠른 스크롤 모드" : "기본 스크롤 모드");

        // Esc 키로 즉시 씬 전환
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(!isEndPosition)
            {
                SceneLoader.LoadTitle();
                isEndPosition = true; // 씬 전환이 한 번만 일어나도록 설정
            }
        }

        // 텍스트가 endPosition 이상으로 올라가면 씬 전환
        if (textRectTransform.anchoredPosition.y >= endPosition)
        {
            if (!isEndPosition)
            {
                SceneLoader.LoadTitle();
                isEndPosition = true; // 씬 전환이 한 번만 일어나도록 설정
            }
        }
    }
}