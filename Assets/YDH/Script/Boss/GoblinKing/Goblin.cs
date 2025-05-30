using UnityEngine;

public class Goblin : MonoBehaviour
{
    public float moveSpeed = 2f;
    private Vector2 moveDirection;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        PickRandomDirection();
    }

    void Update()
    {
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);

        //// 화면 벗어나는지 체크
        //Vector3 viewPos = mainCamera.WorldToViewportPoint(transform.position);

        //if (viewPos.x < 0 || viewPos.x > 1 || viewPos.y < 0 || viewPos.y > 1)
        //{
        //    // 반대 방향으로 튕기기
        //    moveDirection = -moveDirection;
        //}

        // 가끔씩 방향을 랜덤하게 바꿔줌 (원하면 더 정교한 로직 가능)
        if (Random.value < 0.005f)
        {
            PickRandomDirection();
        }
    }

    void PickRandomDirection()
    {
        float angle = Random.Range(0f, 360f);
        float rad = angle * Mathf.Deg2Rad;
        moveDirection = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)).normalized;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();

        if (playerHealth != null)
        {
            Destroy(this.gameObject);
            return;
        }

        BaseTile baseTile = collision.GetComponent<BaseTile>();

        if (baseTile != null)
        {
            baseTile.SetToChargeState();
            return;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        BattleField battleField = collision.GetComponent<BattleField>();

        if (battleField != null)
        {
            // 벗어난 방향 계산
            Vector3 directionToCenter = (battleField.transform.position - transform.position).normalized;

            // 되돌려보낼 위치 계산 (살짝 BattleField 쪽으로 이동)
            float returnDistance = 0.3f; // 되돌릴 거리 (원하는 만큼 조절)
            Vector3 pushBackPosition = transform.position + directionToCenter * returnDistance;
            PickRandomDirection();

            // 위치 이동
            transform.position = pushBackPosition;

            return;
        }
    }
}
