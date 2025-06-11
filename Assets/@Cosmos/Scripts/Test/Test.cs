using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 씬 전환 및 게임 제어를 담당하는 테스트 스크립트
/// </summary>
public class Test : MonoBehaviour
{
    /// <summary>
    /// 게임 씬으로 이동하여 새 게임 시작
    /// </summary>
    public void NextScene()
    {
        Debug.Log("[Test] 게임 씬으로 이동");
        
        // 타임스케일 리셋
        Time.timeScale = 1f;
        
        SceneManager.LoadScene("GameScene");
    }
}