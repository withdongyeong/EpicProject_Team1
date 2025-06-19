using UnityEngine;
using System.Collections;

public class SpiderWeb : MonoBehaviour
{
    bool IsHitPlayer;

    private void Start()
    {
        IsHitPlayer = false;
        StartCoroutine(DestroyAfterDelay());
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!collision.gameObject.CompareTag("Player"))return;
        PlayerController playerController = collision.GetComponent<PlayerController>();

        Debug.Log(playerController.CurrentX + ", " + playerController.CurrentY);
        Vector3 PlayerPosition = GridManager.Instance.GridToWorldPosition(new Vector3Int(playerController.CurrentX, playerController.CurrentY, 0));

        if (playerController != null && PlayerPosition == this.transform.position)
        {
            IsHitPlayer = true;
            playerController.Bind(1f);
            StartCoroutine(ReleasePlayerAfterDelay(playerController));
        }
    }

    private IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(5f);

        if (!IsHitPlayer)
        {
            Destroy(gameObject);  // 플레이어가 없으면 제거
        }
    }

    private IEnumerator ReleasePlayerAfterDelay(PlayerController player)
    {
        yield return new WaitForSeconds(0.5f); // 0.5초 대기
        Destroy(gameObject);   // 웹 오브젝트는 제거
    }
}
