using UnityEngine;
using System.Collections;

public class TemporaryWall : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(DestroyAfterDelay());
        EventBus.SubscribeBossDeath(OnGameEnd);
        EventBus.SubscribePlayerDeath(OnGameEnd);
    }

    private IEnumerator DestroyAfterDelay()
    {
        //여기서 길막기
        GridManager.Instance.AddUnmovableGridPosition(GridManager.Instance.WorldToGridPosition(transform.position));

        yield return new WaitForSeconds(3f);

        //여기서 길뚫기
        WallClear();
    }

    private void OnGameEnd()
    {
        WallClear();
    }

    private void OnDestroy()
    {
        Debug.Log("삭제됨");
        WallClear();
    }

    private void WallClear()
    {
        GridManager.Instance.RemoveUnmovableGridPosition(GridManager.Instance.WorldToGridPosition(transform.position));
    }
}
