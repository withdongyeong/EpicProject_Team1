using UnityEngine;

public class Goblin : MonoBehaviour
{
    public float moveSpeed = 2f;
    private Vector2 moveDirection;
    private GridSystem _gridSystem;

    void Start()
    {
        _gridSystem = FindAnyObjectByType<GridSystem>();
        PickRandomDirection();
    }

    void Update()
    {
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);

        // ������ ������ �����ϰ� �ٲ��� (���ϸ� �� ������ ���� ����)
        if (Random.value < 0.005f)
        {
            PickRandomDirection();
        }

        TileReset();
    }

    void PickRandomDirection()
    {
        float angle = Random.Range(0f, 360f);
        float rad = angle * Mathf.Deg2Rad;
        moveDirection = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)).normalized;
    }

    void TileReset()
    {
        BaseTile currentTile = _gridSystem.GetTileAt((int)this.transform.position.x, (int)this.transform.position.y);

        if (currentTile != null && currentTile.GetState() == BaseTile.TileState.Ready)
        {
            currentTile.SetToChargeState();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();

        if (playerHealth != null)
        {
            Destroy(this.gameObject);
            return;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log(3);
        BattleField battleField = collision.GetComponent<BattleField>();

        if (battleField != null)
        {
            // ��� ���� ���
            Vector3 directionToCenter = (battleField.transform.position - transform.position).normalized;

            // �ǵ������� ��ġ ��� (��¦ BattleField ������ �̵�)
            float returnDistance = 0.3f; // �ǵ��� �Ÿ� (���ϴ� ��ŭ ����)
            Vector3 pushBackPosition = transform.position + directionToCenter * returnDistance;
            PickRandomDirection();

            // ��ġ �̵�
            transform.position = pushBackPosition;

            return;
        }
    }
}
