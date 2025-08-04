using UnityEngine;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// 데미지 미터 UI를 실제로 화면에 표시하는 핸들러 클래스
/// 타일 등급에 따라 텍스트 색상을 다르게 적용
/// </summary>
public class DamageMeterHandler : MonoBehaviour
{
    private GameObject damageTextPrefab; // DamageText 프리팹
    
    [Header("등급별 텍스트 색상 설정")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color rareColor = Color.blue;
    [SerializeField] private Color epicColor = Color.magenta;
    [SerializeField] private Color legendaryColor = Color.yellow;
    [SerializeField] private Color mythicColor = Color.red;
    
    // 등급별 타일 종류 딕셔너리
    private Dictionary<TileGrade, List<string>> tileGradeDict;
    
    private void Awake()
    {
        // Resources 폴더에서 DamageText 프리팹 로드
        damageTextPrefab = Resources.Load<GameObject>("Prefabs/UI/DamagePanel/DamageMeterText");

        if (damageTextPrefab == null)
        {
            Debug.LogError("DamageText 프리팹을 Resources/Prefabs/UI/DamagePanel/DamageMeterText에서 찾을 수 없습니다.");
        }
        
        // 타일 등급 딕셔너리 초기화
        InitializeTileGradeDictionary();
    }
    
    /// <summary>
    /// 타일 등급별 분류 딕셔너리 초기화
    /// </summary>
    private void InitializeTileGradeDictionary()
    {
        tileGradeDict = new Dictionary<TileGrade, List<string>>()
        {
            {TileGrade.Normal, new List<string>
                {
                    "FireBoltTile", "GuideStaffTile", "StaffTile", "WandTile"
                }
            },
            {TileGrade.Rare, new List<string>
                {
                    "DamageTotemTile", "FrostStaffTile", "SwordTile", "TurtleTile"
                }
            },
            {TileGrade.Epic, new List<string>
                {
                    "ArchmageStaffTile", "FireBallTile", "FlamingSwordTile", "IcicleTile", 
                    "ManaTurretTile", "NecronomiconTile", "ThunderStormCloudTile", "MoaiTotemTile"
                }
            },
            {TileGrade.Legendary, new List<string>
                {
                    "KabutoTile", "WarFlagTile"
                }
            },
            {TileGrade.Mythic, new List<string>
                {
                    "FrostHammerTile", "HellFireTile", "MagmaBladeTile"
                }
            }
        };
    }

    /// <summary>
    /// 데미지 매니저에서 호출할 함수
    /// 원본 키와 표시 텍스트를 받아서 등급별 색상 적용
    /// </summary>
    /// <param name="originalKey">원본 타일 키 (색상 판단용)</param>
    /// <param name="displayText">표시할 데미지 텍스트 ("타일명: 데미지" 형식)</param>
    public void AddDamageText(string originalKey, string displayText)
    {
        Debug.Log($"AddDamageText 호출됨 - 키: {originalKey}, 텍스트: {displayText}");
        if (damageTextPrefab == null)
        {
            Debug.LogError("DamageText 프리팹이 로드되지 않았습니다.");
            return;
        }

        // 프리팹을 이 오브젝트(Content)의 자식으로 인스턴스화
        GameObject textInstance = Instantiate(damageTextPrefab, transform);

        // TextMeshProUGUI 컴포넌트 가져오기
        TextMeshProUGUI textComponent = textInstance.GetComponent<TextMeshProUGUI>();
        if (textComponent != null)
        {
            // Rich Text 활성화
            textComponent.richText = true;
            
            // 표시 텍스트에서 타일 이름과 데미지 분리
            string displayTileName = ExtractTileNameFromText(displayText);
            string damageValue = ExtractDamageFromText(displayText);
            
            // 원본 키로 색상 결정
            Color tileColor = GetColorByTileGrade(originalKey);
            string colorHex = ColorUtility.ToHtmlStringRGBA(tileColor);
            
            // Rich Text로 번역된 타일명에 색상 적용
            string coloredText = $"<color=#{colorHex}>{displayTileName}</color>: {damageValue}";
            textComponent.text = coloredText;
            
            Debug.Log($"원본키: {originalKey}, 번역명: {displayTileName}, 적용된 색상: #{colorHex}");
        }
        else
        {
            Debug.LogError("DamageText 프리팹에 TextMeshProUGUI 컴포넌트가 없습니다.");
        }
    }
    
    /// <summary>
    /// 데미지 텍스트에서 타일 이름 추출
    /// </summary>
    /// <param name="damageText">"타일명: 데미지" 형식의 텍스트</param>
    /// <returns>추출된 타일 이름</returns>
    private string ExtractTileNameFromText(string damageText)
    {
        if (string.IsNullOrEmpty(damageText))
            return "";
            
        // ": " 기준으로 분할하여 타일 이름 추출
        string[] parts = damageText.Split(':');
        if (parts.Length > 0)
        {
            return parts[0].Trim();
        }
        
        return damageText;
    }
    
    /// <summary>
    /// 데미지 텍스트에서 데미지 값 추출
    /// </summary>
    /// <param name="damageText">"타일명: 데미지" 형식의 텍스트</param>
    /// <returns>추출된 데미지 값</returns>
    private string ExtractDamageFromText(string damageText)
    {
        if (string.IsNullOrEmpty(damageText))
            return "";
            
        // ": " 기준으로 분할하여 데미지 값 추출
        string[] parts = damageText.Split(':');
        if (parts.Length > 1)
        {
            return parts[1].Trim();
        }
        
        return "";
    }

    /// <summary>
    /// 타일 이름으로 등급을 찾아 해당 색상 반환
    /// </summary>
    /// <param name="tileName">타일 이름</param>
    /// <returns>등급에 맞는 색상</returns>
    private Color GetColorByTileGrade(string tileName)
    {
        // Fire는 무시 (이미 빨간색으로 설정됨)
        if (tileName.Contains("Fire"))
        {
            return Color.red; // 기본 빨간색 유지
        }
        
        // 각 등급별로 타일 이름 확인
        foreach (var gradeEntry in tileGradeDict)
        {
            TileGrade grade = gradeEntry.Key;
            List<string> tileNames = gradeEntry.Value;
            
            if (tileNames.Contains(tileName))
            {
                return GetColorByGrade(grade);
            }
        }
        
        // 등급을 찾지 못한 경우 기본 색상 (Normal) 반환
        Debug.LogWarning($"타일 '{tileName}'의 등급을 찾을 수 없습니다. Normal 색상 적용.");
        return normalColor;
    }
    
    /// <summary>
    /// 등급에 따른 색상 반환
    /// </summary>
    /// <param name="grade">타일 등급</param>
    /// <returns>해당 등급의 색상</returns>
    private Color GetColorByGrade(TileGrade grade)
    {
        switch (grade)
        {
            case TileGrade.Normal: return normalColor;
            case TileGrade.Rare: return rareColor;
            case TileGrade.Epic: return epicColor;
            case TileGrade.Legendary: return legendaryColor;
            case TileGrade.Mythic: return mythicColor;
            default: return normalColor;
        }
    }

    /// <summary>
    /// Content의 모든 자식 오브젝트를 삭제하는 함수 (초기화 용도)
    /// </summary>
    public void ClearDamageText()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}