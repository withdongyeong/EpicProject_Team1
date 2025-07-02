using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScreenDropDown : MonoBehaviour
{
    
    private const string ResolutionKey = "ResolutionIndex";
    
    [System.Serializable]
    public struct ResolutionOption
    {
        public int width;
        public int height;
        public string Label => $"{width}x{height}";
    }

    private TMP_Dropdown _resolutionDropDown;
    private List<ResolutionOption> allowedResolutions = new List<ResolutionOption>()
    {
        new ResolutionOption { width = 1600, height = 900 },
        new ResolutionOption { width = 1280, height = 720 },
        new ResolutionOption { width = 1920, height = 1080 }
    };
    

    private void Start()
    {
        _resolutionDropDown = GetComponent<TMP_Dropdown>();
        _resolutionDropDown.ClearOptions();
        
        List<string> options = new List<string>();
        int currentIndex = 0;

        for (int i = 0; i < allowedResolutions.Count; i++)
        {
            var res = allowedResolutions[i];
            string label = res.Label;

            // 현재 해상도라면 인덱스 저장
            if (res.width == Screen.width &&
                res.height == Screen.height)
            {
                label += " *";
                currentIndex = i;
            }

            options.Add(label);
        }

        _resolutionDropDown.AddOptions(options);
        
        int savedIndex = PlayerPrefs.GetInt(ResolutionKey, currentIndex);
        savedIndex = Mathf.Clamp(savedIndex, 0, allowedResolutions.Count - 1);

        
        
        _resolutionDropDown.value = savedIndex;
        _resolutionDropDown.RefreshShownValue();

        OnResolutionChanged(savedIndex);
        _resolutionDropDown.onValueChanged.AddListener(OnResolutionChanged);
    }

    public void OnResolutionChanged(int index)
    {
        var selected = allowedResolutions[index];
        Screen.SetResolution(selected.width, selected.height, FullScreenMode.Windowed);
        _resolutionDropDown.value = index;
        _resolutionDropDown.RefreshShownValue();
        PlayerPrefs.SetInt(ResolutionKey, index);
        PlayerPrefs.Save();
        Debug.Log($"Resolution set to: {selected.Label}");
    }
}
