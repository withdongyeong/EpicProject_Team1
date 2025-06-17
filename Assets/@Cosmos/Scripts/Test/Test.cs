using UnityEngine;
using UnityEngine.SceneManagement;

public class Test : MonoBehaviour
{
    // 다음 씬으로 넘어가는 함수
    public void NextScene()
    {
        SceneManager.LoadScene("GameScene");
    }
    
    public void TestNextScene()
    {
        SceneManager.LoadScene("GameScene_75");
    }
}
