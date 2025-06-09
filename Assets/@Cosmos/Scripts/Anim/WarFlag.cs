using UnityEngine;
using System.Collections;

public class WarFlag : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component not found on WarFlag prefab.");
            return;
        }

        // 애니메이션 길이를 기반으로 삭제 타이밍 계산  
        StartCoroutine(DestroyAfterAnimation());
    }

    private IEnumerator DestroyAfterAnimation()
    {
        // 현재 애니메이션 상태의 길이를 가져옴
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float animationLength = stateInfo.length;

        // 애니메이션이 끝날 때까지 대기
        yield return new WaitForSeconds(animationLength + 0.2f);

        // 오브젝트 삭제
        Destroy(gameObject);
    }
}