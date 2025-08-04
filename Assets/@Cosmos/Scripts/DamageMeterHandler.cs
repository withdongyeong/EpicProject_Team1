using UnityEngine;
using TMPro;

public class DamageMeterHandler : MonoBehaviour
{
    private GameObject damageTextPrefab; // DamageText 프리팹

    private void Awake()
    {
        // Resources 폴더에서 DamageText 프리팹 로드
        damageTextPrefab = Resources.Load<GameObject>("Prefabs/UI/DamagePanel/DamageText");

        if (damageTextPrefab == null)
        {
            Debug.LogError("DamageText 프리팹을 Resources/Prefabs/UI/DamagePanel/DamageText에서 찾을 수 없습니다.");
        }
    }


    // 데미지 매니저에서 호출할 함수
    public void AddDamageText(string damageText)
    {
        Debug.Log($"AddDamageText 호출됨: {damageText}");
        if (damageTextPrefab == null)
        {
            Debug.LogError("DamageText 프리팹이 로드되지 않았습니다.");
            return;
        }

        // 프리팹을 이 오브젝트(Content)의 자식으로 인스턴스화
        GameObject textInstance = Instantiate(damageTextPrefab, transform);

        // TextMeshProUGUI 컴포넌트 가져오기
        TextMeshProUGUI textComponent = textInstance.GetComponent<TextMeshProUGUI>();
        if (textComponent != null)
        {
            // 매개변수로 받은 텍스트 설정
            textComponent.text = damageText;
        }
        else
        {
            Debug.LogError("DamageText 프리팹에 TextMeshProUGUI 컴포넌트가 없습니다.");
        }
    }

    // Content의 모든 자식 오브젝트를 삭제하는 함수 (초기화 용도)
    public void ClearDamageText()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}