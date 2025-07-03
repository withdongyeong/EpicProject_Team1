using UnityEngine;

public class CloudHandler : MonoBehaviour
{
    private int _cloudLevel = 0; // 구름 레벨, 기본값은 0
    private GameObject[] _cloudPrefab; // 구름 프리팹
    private Cloud _currentCloud; // 현재 활성화된 구름 오브젝트
    private BaseBoss _boss; // 보스 인스턴스 참조

    /// <summary>
    /// 구름 레벨을 설정합니다. 레벨이 0 이상이어야 하며, 레벨에 따라 구름 오브젝트가 업데이트됩니다.
    /// </summary>
    public int CloudLevel { get => _cloudLevel;
        set
        {
            if (value >= 0)
            {
                _cloudLevel = value;
                // 구름 레벨이 변경되면 구름 오브젝트 업데이트
                UpdateCloudVisuals();
            }
        }
    }

    /// <summary>
    /// 현재 활성화된 구름 오브젝트를 반환합니다. 구름 레벨에 따라 다를 수 있습니다.
    /// </summary>
    public Cloud CurrentCloud => _currentCloud;

    /// <summary>
    /// 구름 레벨에 따라 구름 오브젝트를 업데이트합니다.
    /// </summary>
    private void UpdateCloudVisuals()
    {
        if (_currentCloud == null)
        {
            if (_cloudPrefab == null)
            {
                Initialize();
            }
            // 구름 오브젝트가 없으면 새로 생성
            if (_cloudPrefab != null && _cloudPrefab.Length > 0 && _cloudLevel < _cloudPrefab.Length && _boss != null)
            {
                _currentCloud = Instantiate(_cloudPrefab[_cloudLevel - 1], _boss.transform).GetComponent<Cloud>();
                _currentCloud.transform.localPosition = new Vector3(0, 2, 0); // 위치 초기화
            }
        }
        else
        {
            // 구름 레벨이 변경되면 기존 구름 오브젝트를 제거하고 새로 생성
            if (_cloudLevel < _cloudPrefab.Length)
            {
                Destroy(_currentCloud);
                _currentCloud = Instantiate(_cloudPrefab[_cloudLevel - 1], _boss.transform).GetComponent<Cloud>();
                _currentCloud.transform.localPosition = new Vector3(0, 2, 0); // 위치 초기화
            }
        }
    }

    private void Initialize()
    {
        // 구름 프리팹을 Resources 폴더에서 로드
        _cloudPrefab = Resources.LoadAll<GameObject>("Prefabs/Summons/Cloud");
        if (_cloudPrefab == null)
        {
            Debug.LogError("Cloud prefab not found in Resources folder.");
        }
        _boss = FindAnyObjectByType<BaseBoss>();
    }
}
