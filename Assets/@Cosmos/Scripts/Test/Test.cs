using UnityEngine;
using UnityEngine.SceneManagement;

public class Test : MonoBehaviour
{
    // 다음 씬으로 넘어가는 함수
    public void NextScene()
    {
        SoundManager.Instance.BGMSoundClip("GameSceneBGM");
        SceneManager.LoadScene("GameScene");
        // 게임 격자 옮김 (게임에 따라 다르게 할 수도 있을듯)
        GridManager.Instance.transform.position = new Vector3(7f, 0, 0);
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
        // 게임 격자 옮김 (게임에 따라 다르게 할 수도 있을듯)
        GridManager.Instance.transform.position = new Vector3(7f, 0, 0);
    }
}
