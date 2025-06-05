using UnityEngine;
using System.Collections;

public class SpiderWeb : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController playerController = collision.GetComponent<PlayerController>();

        if(playerController != null)
        {
            playerController.PlayerDebuff.Bind(1f); // 플레이어를 묶음
            StartCoroutine(ReleasePlayerAfterDelay(playerController));
        }

    }

    private IEnumerator ReleasePlayerAfterDelay(PlayerController player)
    {
        yield return new WaitForSeconds(1f); // 1초 대기
        Destroy(gameObject);   // 웹 오브젝트는 제거
    }
}
