using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class GameVolum : MonoBehaviour
{
    private const string VolumeKey = "GameVolume";
    
    public AudioMixer audioMixer;
    private Slider _volumSlider;

    private void Start()
    {
        _volumSlider = GetComponent<Slider>();
        
        float savedVolume = PlayerPrefs.GetFloat(VolumeKey, 1f);
        _volumSlider.value = savedVolume;
        SetVolum(savedVolume);
        
        _volumSlider.onValueChanged.AddListener(SetVolum);
    }

    public void SetVolum (float volume)
    {
        audioMixer.SetFloat("volume", Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20);
        PlayerPrefs.SetFloat(VolumeKey, volume);
        PlayerPrefs.Save();
    }
}
