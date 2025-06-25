using UnityEngine;

public class ArachneSpider : MonoBehaviour
{
    public int damage = 10;
    public float speed = 12f;
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        // �������� �̵�
        transform.Translate(Vector2.left * speed * Time.deltaTime);

        // ī�޶� ������ ������ �ı�
        Vector3 viewportPos = mainCamera.WorldToViewportPoint(transform.position);
        if (viewportPos.x < 0 || viewportPos.y < 0 || viewportPos.y > 1)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerHp playerHp = collision.GetComponent<PlayerHp>();

        if (playerHp != null)
        {
            playerHp.TakeDamage(damage);
            Destroy(gameObject); // �浹 �� �����̴� ����
        }
    }
}
