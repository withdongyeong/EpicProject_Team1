using UnityEngine;

public class Goblin : MonoBehaviour
{
    public float moveSpeed = 2f;
    private Vector2 moveDirection;

    void Start()
    {
        PickRandomDirection();
    }

    void Update()
    {
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);

        // 가끔씩 방향을 랜덤하게 바꿔줌 (원하면 더 정교한 로직 가능)
        if (Random.value < 0.005f)
        {
            PickRandomDirection();
        }

        CellReset();
    }

    void PickRandomDirection()
    {
        float angle = Random.Range(0f, 360f);
        float rad = angle * Mathf.Deg2Rad;
        moveDirection = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)).normalized;
    }

    void CellReset()
    {
        Cell currentCell = GridManager.Instance.GetCellData(new Vector3Int((int)this.transform.position.x, (int)this.transform.position.y,0));

        //셀 초기화 - 함수(지금은 구현 안됨)
        //if (currentCell != null && currentCell.GetState() == BaseTile.TileState.Ready)
        //{
        //    currentTile.SetToChargeState();
        //}
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerHp playerHp = collision.GetComponent<PlayerHp>();

        if (playerHp != null)
        {
            Destroy(this.gameObject);
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
            float returnDistance = 0.5f; // 되돌릴 거리 (원하는 만큼 조절)
            Vector3 pushBackPosition = transform.position + directionToCenter * returnDistance;
            PickRandomDirection();

            // 위치 이동
            transform.position = pushBackPosition;

            return;
        }
    }
}
