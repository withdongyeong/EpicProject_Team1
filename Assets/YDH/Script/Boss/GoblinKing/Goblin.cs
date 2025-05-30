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

        // ȭ�� ������� üũ
        Vector3 viewPos = mainCamera.WorldToViewportPoint(transform.position);

        if (viewPos.x < 0 || viewPos.x > 1 || viewPos.y < 0 || viewPos.y > 1)
        {
            // �ݴ� �������� ƨ���
            moveDirection = -moveDirection;
        }

        // ������ ������ �����ϰ� �ٲ��� (���ϸ� �� ������ ���� ����)
        if (Random.value < 0.01f)
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
            return ;
        }

        BaseTile baseTile = collision.GetComponent<BaseTile>();

        if (baseTile != null)
        {
            baseTile.SetToChargeState();
        }
        

        //BaseTile�� �����ؼ� ���¸� ó������ ����
    }
}
