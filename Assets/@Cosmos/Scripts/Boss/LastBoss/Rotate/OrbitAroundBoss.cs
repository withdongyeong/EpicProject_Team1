using UnityEngine;

/// <summary>
/// 보스를 중심으로 무기가 2D 평면에서 원형 회전하도록 만드는 컴포넌트
/// </summary>
public class OrbitAroundBoss : MonoBehaviour
{
    public Transform bossTransform;
    public float radius = 1.5f;
    public float rotationSpeed = 90f; // degree per second

    private float currentAngle = 0f;

    void Update()
    {
        if (bossTransform == null) return;

        currentAngle += rotationSpeed * Time.deltaTime;
        float rad = currentAngle * Mathf.Deg2Rad;

        Vector3 offset = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f) * radius;
        transform.position = bossTransform.position + offset;
    }
}