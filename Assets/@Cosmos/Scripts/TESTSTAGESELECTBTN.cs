using UnityEngine;

public class TESTSTAGESELECTBTN : MonoBehaviour
{
    
    //빌딩씬에서 쓰는 스테이지 선택 버튼
    public void OnClick()
    {
        StageSelectManager.Instance.StageSelect();
    }
    
    
    //타이틀씬에서 쓰는 빌딩 씬으로 이동하는 버튼
    public void GOBUILDSCENE()
    {
        SceneLoader.LoadBuilding();
    }
    
    public void GOTITLESCENE()
    {
        SceneLoader.LoadTitle();
    }
}
