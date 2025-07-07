using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class GameVolume : MonoBehaviour
{
    private const string BGMVolumeKey = "BGMVolume";
    private const string SFXVolumeKey = "SFXVolume";

    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider bgmSlider; // BGM 음량 슬라이더
    [SerializeField] private Slider sfxSlider; // SFX 음량 슬라이더

    private void Start()
    {
        // BGM 슬라이더 초기화
        if (bgmSlider != null)
        {
            float savedBGMVolume = PlayerPrefs.GetFloat(BGMVolumeKey, 1f);
            bgmSlider.value = savedBGMVolume;
            SetBGMVolume(savedBGMVolume);
            bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        }
        else
        {
            Debug.LogError("BGM Slider가 설정되지 않았습니다!");
        }

        // SFX 슬라이더 초기화
        if (sfxSlider != null)
        {
            float savedSFXVolume = PlayerPrefs.GetFloat(SFXVolumeKey, 1f);
            sfxSlider.value = savedSFXVolume;
            SetSFXVolume(savedSFXVolume);
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        }
        else
        {
            Debug.LogError("SFX Slider가 설정되지 않았습니다!");
        }
    }

    public void SetBGMVolume(float volume)
    {
        audioMixer.SetFloat("BGMVolume", Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20);
        PlayerPrefs.SetFloat(BGMVolumeKey, volume);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20);
        PlayerPrefs.SetFloat(SFXVolumeKey, volume);
        PlayerPrefs.Save();
    }
}