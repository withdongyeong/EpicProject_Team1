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
        float duration = 0.05f; // 이동 시간 (1초)
        float elapsed = 0f;

        Vector3 startPos = target.position;
        Vector3 endPos = new Vector3(0f, startPos.y, startPos.z); // X=0으로 이동, 나머지는 그대로

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // 부드럽게 이동
            target.position = Vector3.Lerp(startPos, endPos, t);

            yield return null;
        }

        // 정확히 0으로 보정
        target.position = endPos;
    }
}
