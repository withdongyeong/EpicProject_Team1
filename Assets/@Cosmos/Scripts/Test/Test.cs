using UnityEngine;
using UnityEngine.SceneManagement;

public class Test : MonoBehaviour
{
    // 다음 씬으로 넘어가는 함수
    public void NextScene()
    {
        SoundManager.Instance.BGMSoundClip("GameSceneBGM");
        SceneManager.LoadScene("GameScene");
    }
    
    public void TestNextScene()
    {
        SceneManager.LoadScene("StageScene_KYH");
    }
    // 다음 씬으로 넘어가는 함수
    public void NextScene2()
    {
        SoundManager.Instance.BGMSoundClip("OrcMage");
        SceneManager.LoadScene("GameScene2");
    }
}
