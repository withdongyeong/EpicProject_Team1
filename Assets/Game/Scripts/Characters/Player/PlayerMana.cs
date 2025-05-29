using System;
using UnityEngine;

public class PlayerMana : MonoBehaviour
{
    #region 마나 관련 변수들
    //최대 마나입니다
    private float _maxMana = 5;
    //현재 마나입니다
    private float _currentMana;
    //초당 마나 재생량입니다
    private float _manaRegen = 1f;
    #endregion

    #region private 변수 게터들
    //private 필드들을 받아오는 게터입니다
    public float MaxMana => _maxMana;
    public float CurrentMana => _currentMana;
    public float ManaRegen => _manaRegen;
    #endregion

    #region 이벤트들
    //플레이어 시작할때를 받아오는 이벤트입니다 전투 시작시~ 에 해당합니다
    public event Action OnPlayerInstantiate;

    //UI에서 언제 마나가 바뀌는지 알기 위한 이벤트입니다 앞이 현재마나 뒤가 맥스마나입니다
    public event Action<float,float> OnManaChanged;
    #endregion

    #region Start,Update
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        //전투 시작시 최대 마나를 늘린다거나 마나 재생을 늘리는 효과를 받아옵니다
        OnPlayerInstantiate?.Invoke();

        //시작시 현재 마나를 맥스 마나랑 같게 설정합니다
        SetCurrentMana(0);
    }

    // Update is called once per frame
    private void Update()
    {
        ModifyCurrentMana(ManaRegen * Time.deltaTime);
    }
    #endregion

    #region 최대마나, 현재마나, 마나재생량을 변경하는 함수들
    //최대 마나를 준 값만큼 변경하는 함수입니다 최소값은 1입니다
    public void ModifyMaxMana(float mana)
    {
        _maxMana = Mathf.Max(_maxMana - mana, 1f);
        SendEventToUI();
    }

    //현재 마나를 사용하는 함수입니다
    public bool UseCurrentMana(float mana)
    {
        //마나를 사용하는 함수라서 0보다 커야합니다
        if (mana >= 0)
        {
            //만약 현재 마나가 요청한 사용량보다 많으면 사용하고 true를 반환합니다
            if (_currentMana >= mana)
            {
                _currentMana -= mana;
                SendEventToUI();
                return true;
            }
            else //현재 마나가 사용량보다 적으면 false를 반환합니다
                return false;
        }
        else
            return false;
    }

    //현재 마나를 회복시키는 함수입니다
    public void ModifyCurrentMana(float mana)
    {
        _currentMana = Mathf.Clamp(_currentMana + mana, 0f, _maxMana);
        SendEventToUI();
    }

    //현재 마나를 원하는 값으로 변경하는 함수입니다
    public void SetCurrentMana(float mana)
    {
        _currentMana = Mathf.Clamp(mana, 0f, _maxMana);
        SendEventToUI();
    }

    //마나 재생량을 변경하는 함수입니다. 최소값은 0.2입니다.
    public void ModifyManaRegen(float regen)
    {
        _manaRegen = Mathf.Max(_manaRegen + regen, 0.2f);
        //마나리젠값이 변경될때마다 UI에 보여줘야한다면 SendEventToUI 넣어야합니다
    }
    #endregion

    #region UI에 이벤트를 보내는 함수
    private void SendEventToUI()
    {
        OnManaChanged?.Invoke(_currentMana, _maxMana);
    }
    #endregion
}
