using UnityEngine;
using System.Collections;

public class TreantWindMagic : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController playerHealth = collision.GetComponent<PlayerController>();
        if (playerHealth != null)
        {
            playerHealth.CurrentX = 0;
            StartCoroutine(MoveToCenterX(collision.transform));
        }
    }

    private IEnumerator MoveToCenterX(Transform target)
    {
        float duration = 0.05f; // �̵� �ð� (1��)
        float elapsed = 0f;

        Vector3 startPos = target.position;
        Vector3 endPos = new Vector3(0f, startPos.y, startPos.z); // X=0���� �̵�, �������� �״��

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // �ε巴�� �̵�
            target.position = Vector3.Lerp(startPos, endPos, t);

            yield return null;
        }

        // ��Ȯ�� 0���� ����
        target.position = endPos;
    }
}
