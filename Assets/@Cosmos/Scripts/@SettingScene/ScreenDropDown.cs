using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScreenDropDown : MonoBehaviour
{
    
    [System.Serializable]
    public struct ResolutionOption
    {
        public int width;
        public int height;
        public string Label => $"{width}x{height}";
    }

    [SerializeField]
    private List<ResolutionOption> allowedResolutions = new List<ResolutionOption>
    {
        new ResolutionOption { width = 1600, height = 900 },
        new ResolutionOption { width = 1280, height = 720 },
        new ResolutionOption { width = 1920, height = 1080 }
    };

    private TMP_Dropdown _resolutionDropDown;
    

    private void Start()
    {
        _resolutionDropDown = GetComponent<TMP_Dropdown>();
        _resolutionDropDown.ClearOptions();
        
        // 1. 옵션 구성
        List<string> options = new List<string>();
        foreach (var res in allowedResolutions)
        {
            options.Add(res.Label);
        }

        _resolutionDropDown.AddOptions(options);

        // 2. 저장된 해상도 (예: "1920x1080")
        string savedResolution = SaveManager.Resolution;

        // 3. 문자열 비교를 통해 인덱스 탐색
        int defaultIndex = allowedResolutions.FindIndex(r => r.Label == savedResolution);
        if (defaultIndex < 0) defaultIndex = 0;

        // 4. 드롭다운 UI 설정
        _resolutionDropDown.value = defaultIndex;
        _resolutionDropDown.RefreshShownValue();

        // 5. 리스너 연결 (GameManager의 해상도 설정 함수 호출)
        _resolutionDropDown.onValueChanged.AddListener(OnResolutionChanged);
    }
    
    private void OnResolutionChanged(int index)
    {
        if (index < 0 || index >= allowedResolutions.Count) return;

        string selectedResolution = allowedResolutions[index].Label;
        GameManager.Instance.SetResolution(selectedResolution);
    }
}
