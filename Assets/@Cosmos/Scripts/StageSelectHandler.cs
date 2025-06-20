using System;
using UnityEngine;




/// <summary>
/// StageSelectHandler는 스테이지 선택을 처리하는 클래스입니다.
/// 현재는 보스 프리팹 , 배경만 동적 설정 가능합니다.
/// 모든 보스 프리팹은 @COSMOS/Resources/BossPrefab 폴더에 있어야 합니다.
/// 배경에 관해서는 구체적으로 결정된게 없기에 아직은 고정된 배경으로 해놨는데
/// 코드를 보면 알겠지만 이것도 원하는대로 동적설정이 가능합니다.
///
/// -임시사용방법-
/// 버튼 이벤트에 이 스크립트의 StageSelect 메소드를 연결하고 원하는 보스의 숫자를 매개변수로 보냅니다.
/// 보스 이름과 숫자를 1:1 매핑 시켰습니다.
/// 예를 들어 아라크네는 1, 오크마법사는 2입니다.
/// 만약 보스 추가시  밑에 Boss enum에 추가하고 사용하면 됩니다.
/// </summary>
public class StageSelectHandler : MonoBehaviour
{
    private enum Boss
    {
        Arachne = 1,
        OrcMage = 2,
        Slime = 3
    }
    
    public static GameObject enemyPrefab;
    public static Sprite backgroundSprite;
    
    public void StageSelect(int bossNum)
    {
        Boss selectedBoss = (Boss)bossNum;
        enemyPrefab = Resources.Load<GameObject>($"BossPrefab/{selectedBoss}");
        backgroundSprite = Resources.Load<Sprite>($"Arts/Background/Vertical3");  
        if (enemyPrefab == null || backgroundSprite == null)
        {
            Debug.LogError($"Stage {selectedBoss} resources not found.");
            return;
        }
        SceneLoader.LoadStage();
    }
    
}
