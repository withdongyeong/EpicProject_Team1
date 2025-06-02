using UnityEngine;
using System.Collections;

public class SpiderWeb : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController playerController = collision.GetComponent<PlayerController>();

        if(playerController != null)
        {
            playerController.IsBind = true; // �÷��̾ ����
            StartCoroutine(ReleasePlayerAfterDelay(playerController));
        }

    }

    private IEnumerator ReleasePlayerAfterDelay(PlayerController player)
    {
        yield return new WaitForSeconds(1f); // 1�� ���
        player.IsBind = false;              // �ٽ� �̵� �����ϰ�
        Destroy(gameObject);   // �� ������Ʈ�� ����
    }
}
