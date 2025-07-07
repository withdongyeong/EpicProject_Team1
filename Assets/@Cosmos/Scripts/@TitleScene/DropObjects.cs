using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DropObjects : MonoBehaviour
{
    [Header("드롭할 스프라이트들")]
    [SerializeField] private Sprite[] dropSprites;
    [SerializeField] private GameObject dropPrefab; 

    
    [Header("소환 설정")]
    [SerializeField] private int spawnCount = 20;           // 한 번에 소환할 개수
    [SerializeField] private float spawnInterval = 0.1f;    // 소환 간격 (초)
    [SerializeField] private float spawnRangeX = 10f;       // 소환 범위 X축
    [SerializeField] private float excludeCenterWidth = 4f; // 중앙 제외 구간 너비
    [SerializeField] private float spawnHeight = 6f;        // 소환 높이 (천장)
    
    [Header("드롭 설정")]
    [SerializeField] private float fallSpeed = 2f;         // 떨어지는 속도
    [SerializeField] private float rotationSpeed = 90f;    // 회전 속도 (도/초)
    [SerializeField] private float lifeTime = 5f;          // 생존 시간
    
    [Header("랜덤 변수")]
    [SerializeField] private float fallSpeedVariation = 0.5f;     // 떨어지는 속도 변동
    [SerializeField] private float rotationSpeedVariation = 30f;  // 회전 속도 변동
    [SerializeField] private float scaleVariation = 0.3f;         // 크기 변동
    [SerializeField] private Vector2 scaleRange = new Vector2(0.5f, 1.2f); // 크기 범위
    
    [Header("라이트 설정")]
    [SerializeField] private bool enableLight = true;             // 라이트 활성화
    [SerializeField] private float lightIntensity = 0.3f;         // 라이트 강도
    [SerializeField] private float lightRadius = 2f;             // 라이트 반지름
    [SerializeField] private Color lightColor = Color.white;      // 라이트 색상
    [SerializeField] private float lightIntensityVariation = 0.1f; // 라이트 강도 변동
    
    [Header("자동 실행")]
    [SerializeField] private bool autoStart = true;        // 시작 시 자동 실행
    [SerializeField] private bool loop = false;            // 반복 실행
    [SerializeField] private float loopInterval = 3f;      // 반복 간격

    private Camera mainCamera;
    private Coroutine dropCoroutine;

    private void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
            mainCamera = FindAnyObjectByType<Camera>();
            
        if (autoStart)
            StartDrop();
    }

    /// <summary>
    /// 드롭 연출 시작
    /// </summary>
    public void StartDrop()
    {
        if (dropCoroutine != null)
            StopCoroutine(dropCoroutine);
            
        dropCoroutine = StartCoroutine(DropCoroutine());
    }

    /// <summary>
    /// 드롭 연출 중지
    /// </summary>
    public void StopDrop()
    {
        if (dropCoroutine != null)
        {
            StopCoroutine(dropCoroutine);
            dropCoroutine = null;
        }
    }

    /// <summary>
    /// 드롭 코루틴
    /// </summary>
    private IEnumerator DropCoroutine()
    {
        do
        {
            // 설정된 개수만큼 스프라이트 소환
            for (int i = 0; i < spawnCount; i++)
            {
                if (dropSprites.Length > 0)
                {
                    CreateDropObject();
                    yield return new WaitForSeconds(spawnInterval);
                }
            }
            
            // 반복이 설정되어 있으면 대기 후 반복
            if (loop)
                yield return new WaitForSeconds(loopInterval);
                
        } while (loop);
        
        dropCoroutine = null;
    }

    /// <summary>
    /// 드롭 오브젝트 생성
    /// </summary>
    private void CreateDropObject()
    {
        // 랜덤 스프라이트 선택
        Sprite randomSprite = dropSprites[Random.Range(0, dropSprites.Length)];

        // 프리팹 복제
        GameObject dropObj = Instantiate(dropPrefab, GetRandomSpawnPosition(), Quaternion.identity);
        dropObj.name = $"DropObject_{randomSprite.name}";

        // SpriteRenderer 교체
        SpriteRenderer sr = dropObj.GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.sprite = randomSprite;

        // 랜덤 크기
        float randomScale = Random.Range(scaleRange.x, scaleRange.y);
        dropObj.transform.localScale = Vector3.one * randomScale;

        // 랜덤 회전
        dropObj.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));

        // DropItem 초기화
        DropItem dropComponent = dropObj.GetComponent<DropItem>();
        if (dropComponent != null)
        {
            dropComponent.Initialize(
                fallSpeed + Random.Range(-fallSpeedVariation, fallSpeedVariation),
                rotationSpeed + Random.Range(-rotationSpeedVariation, rotationSpeedVariation),
                lifeTime,
                enableLight,
                lightIntensity + Random.Range(-lightIntensityVariation, lightIntensityVariation),
                lightRadius,
                lightColor
            );
        }
    }


    /// <summary>
    /// 랜덤 소환 위치 계산 (중앙 제외)
    /// </summary>
    private Vector3 GetRandomSpawnPosition()
    {
        float randomX;
        float halfRange = spawnRangeX / 2f;
        float halfExclude = excludeCenterWidth / 2f;
        
        // 중앙 제외 구간을 피해서 양쪽 끝에서만 소환
        if (Random.value < 0.5f)
        {
            // 왼쪽 구간 (-halfRange ~ -halfExclude)
            randomX = Random.Range(-halfRange, -halfExclude);
        }
        else
        {
            // 오른쪽 구간 (halfExclude ~ halfRange)
            randomX = Random.Range(halfExclude, halfRange);
        }
        
        Vector3 basePos = transform.position;
        return new Vector3(basePos.x + randomX, basePos.y + spawnHeight, basePos.z);
    }

    /// <summary>
    /// 기즈모 그리기 (에디터에서 소환 범위 확인)
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Vector3 center = transform.position + Vector3.up * spawnHeight;
        float halfRange = spawnRangeX / 2f;
        float halfExclude = excludeCenterWidth / 2f;
        
        // 전체 소환 범위 (회색)
        Gizmos.color = Color.gray;
        Gizmos.DrawWireCube(center, new Vector3(spawnRangeX, 0.1f, 0.1f));
        
        // 실제 소환 구간 (노란색)
        Gizmos.color = Color.yellow;
        // 왼쪽 구간
        Vector3 leftCenter = center + Vector3.left * (halfExclude + (halfRange - halfExclude) / 2f);
        Gizmos.DrawWireCube(leftCenter, new Vector3(halfRange - halfExclude, 0.1f, 0.1f));
        // 오른쪽 구간
        Vector3 rightCenter = center + Vector3.right * (halfExclude + (halfRange - halfExclude) / 2f);
        Gizmos.DrawWireCube(rightCenter, new Vector3(halfRange - halfExclude, 0.1f, 0.1f));
        
        // 중앙 제외 구간 (빨간색)
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(center, new Vector3(excludeCenterWidth, 0.1f, 0.1f));
        
        // 중심점
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, 0.2f);
    }
}