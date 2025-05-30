using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManaUI : MonoBehaviour
{
    //마나 슬라이더 UI 입니다
    private Slider _manaSlider;

    //플레이어 마나 스크립트입니다
    private PlayerMana _playerMana;

    #region 만약 숫자가 필요하면 꺼내세요
    //private TextMeshProUGUI _manaText1; // 이게 앞에 있는 색깔이 있는 텍스트입니다
    //private TextMeshProUGUI _manaText2; // 하얀색 텍스트입니다
    //private RectMask2D _rectMask; //색깔이 있는 텍스트를 가리는 마스크입니다
    #endregion



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //각 변수들 초기화합니다
        _playerMana = FindAnyObjectByType<PlayerMana>();
        _manaSlider = GetComponent<Slider>();

        Debug.Log(_playerMana.gameObject.name);
        //이벤트에 연결합니다
        _playerMana.OnManaChanged += UpdateManaUI;
        #region 숫자 방식
        //TODO: 자식에 들어있는 텍스트 찾는거 수정
        //_manaText2 = _playerMana.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();
        //_rectMask = _playerMana.transform.GetChild(1).GetChild(1).GetComponent<RectMask2D>();
        // _manaText1 = _rectMask.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        #endregion

    }

    //마나 슬라이더를 업데이트합니다
    private void UpdateManaUI(float currentMana,float maxMana)
    {
        Debug.Log(Mathf.Clamp(currentMana / maxMana, 0f, 1f));
        _manaSlider.value = Mathf.Clamp(currentMana / maxMana, 0f, 1f);

        #region 숫자 방식
        //만약에 마나를 int로 취급할거라면 사용할것.
        //string stringText = $"{currentMana.ToString("F1")}/{maxMana.ToString("F1")}";
        //_manaText.text = stringText;
        //_manaText1.text = stringText;
        //_manaText2.text = stringText;
        //_rectMask.padding.
        #endregion
    }
}
