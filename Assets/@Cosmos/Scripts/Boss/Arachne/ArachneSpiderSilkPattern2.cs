using UnityEngine;

public class ArachneSpiderSilkPattern2 : MonoBehaviour
{
    public GameObject _spiderSilkPrefeb;

    public int gridSize = 8;
    public float cellSize = 1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 2초마다 MyFunction을 호출, 시작 지연 시간은 1.2초
        InvokeRepeating("MyFunction", 1.2f, 5f);
    }

    void MyFunction()
    {
        int Y = Random.Range(0, 8);

        Vector3 pos = GridManager.Instance.GridToWorldPosition(new Vector3Int(8, Y, 0));
        GameObject spiderSilk = GameObject.Instantiate(_spiderSilkPrefeb, pos + new Vector3(cellSize, 0, 0), Quaternion.identity);

        // 초기 스케일
        spiderSilk.transform.localScale = new Vector3(0.1f, 1, 1);

        // 생성된 오브젝트에 초기값 전달
        SpiderSilk silkScript = spiderSilk.GetComponent<SpiderSilk>();

        if (silkScript != null)
        {
            silkScript.Init(spiderSilk.transform.position, gridSize); // 필요 파라미터 전달
        }
    }
}
