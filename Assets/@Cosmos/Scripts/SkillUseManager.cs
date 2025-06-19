//using UnityEngine;
//using System.Collections.Generic;

//public class SkillUseManager : Singleton<SkillUseManager>
//{
//    private TileObject currentTile;
//    private GridManager gm;
//    [SerializeField]
//    private float cooldownFactor = 1.0f; // 쿨타임 조정 인자
//    [SerializeField]
//    private int skillActivationCount = 1; // 스킬 활성화 횟수
    
//    public float CooldownFactor => cooldownFactor;
//    public int SkillActivationCount => skillActivationCount;
//    protected override void Awake()
//    {
//        base.Awake();
//        gm = GridManager.Instance;
//    }

//    public void UseSkill(Vector3Int cellPos)
//    {
//        cooldownFactor = 1.0f;
//        skillActivationCount = 1;
//        Cell cell = gm.GetCellData(cellPos);
//        CombineCell comCell = cell.GetCombineCell();
//        //이 밑 부터 곽민준 추가입니다
//        List<Cell> cells = new(); //타일의 모든 셀들을 담을 리스트입니다
//        CombineCell[] combineCells = comCell.GetTileObject().GetComponentsInChildren<CombineCell>(); //타일의 모든 셀들을 찾는 배열

//        //모든 셀들을 넣어줍니다
//        foreach (CombineCell combineCell in combineCells)
//        {
//            cells.AddRange(combineCell.GetComponentsInChildren<Cell>());
//        }

//        //최종적으로 발동할 스킬 리스트입니다
//        List<StarBase> starResult = new();

//        //모든 셀에서 한셀 한셀 검사합니다
//        foreach (Cell cell1 in cells)
//        {
//            //현재 검사하는 셀의 스타 스킬을 그리드 포지션을 이용해서 가져옵니다
//            List<StarBase> starSkills = gm.GetStarSkills(gm.WorldToGridPosition(cell1.transform.position));
//            if (starSkills != null)
//            {
//                foreach (StarBase starSkill in starSkills)
//                {
//                    if (!starResult.Contains(starSkill))
//                    {
//                        starResult.Add(starSkill);
//                    }
//                }
//            }
//        }
//        if (starResult.Count > 0)
//        {
//            ActivateStarSkill(starResult, comCell.GetTileObject());
//        }

//        comCell.ExecuteSkill();
//    }

//    public void MultiplyCooldown(float factor) //예시 0.1f 는 10프로 감소 , 곱연산
//    {
//        Debug.Log("MultiplyCooldown called with factor: " + factor);
//        cooldownFactor *= (1-factor);
//    }
    
//    public void SetSkillActivationCount(int count) //예시 2 는 +2 번
//    {
//        Debug.Log("SetSkillActivationCount called with count: " + count);
//        skillActivationCount += count;
//    }
    
    

//    private void ActivateStarSkill(List<StarBase> starSkills, TileObject tileObject)
//    {
//        foreach (StarBase starSkill in starSkills)
//        {
//            Debug.Log("Activating star skill: " + starSkill.name);
//            starSkill.Activate(tileObject);
//        }
//    }
//}
