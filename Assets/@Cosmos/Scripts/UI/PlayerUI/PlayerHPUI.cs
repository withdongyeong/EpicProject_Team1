using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHPUI : MonoBehaviour
{
   //hp와 protect의 잔량을 표시하는 이미지 UI입니다
   private Image protectUI;
   private Image hPUI;

   //hp와 보호막 수치가 들어가있는 스크립트입니다
   private PlayerHp _playerHp;
   private PlayerProtection playerProtect;

   //현재 hp, 최대 hp, 현재 보호막 수치입니다
   private int currenthP;
   private int maxHP;
   private int currentProtect;

   [Header("UI 설정")]
   public bool isMinimal = false; // true면 플레이어를 따라다님
   public Vector3 offsetPosition = new Vector3(0, 2, 0); // 플레이어 위 오프셋
   public float uiScale = 0.005f; // UI 크기 조절

   private void Awake()
   {
       EventBus.SubscribeGameStart(Init);
   }
   
   private void Update()
   {
       ChangeBar();
   }
   
   private void OnDestroy()
   {
       EventBus.UnsubscribeGameStart(Init);
   }
   
   private void Init()
   {
       //UI 초기화
       protectUI = transform.GetChild(0).GetComponent<Image>();
       hPUI = transform.GetChild(1).GetComponent<Image>();

       _playerHp = FindAnyObjectByType<PlayerHp>();
       if (_playerHp != null)
       {
           maxHP = _playerHp.MaxHealth;
           playerProtect = FindAnyObjectByType<PlayerProtection>();
           
           // isMinimal이 true일 때만 플레이어를 따라다니도록 설정
           if (isMinimal)
           {
               SetupFollowPlayer();
           }
       }
   }

   private void SetupFollowPlayer()
   {
       // 만약 Screen Space UI라면 World Space로 변경 필요
       Canvas canvas = GetComponentInParent<Canvas>();
       if (canvas != null && canvas.renderMode != RenderMode.WorldSpace)
       {
           SetupWorldSpaceCanvas();
       }
       else
       {
           // 이미 World Space Canvas이거나 Canvas가 없다면 직접 플레이어 자식으로 설정
           transform.SetParent(_playerHp.transform);
           transform.localPosition = offsetPosition;
       }
   }

   private void SetupWorldSpaceCanvas()
   {
       // 새로운 World Space Canvas 생성
       GameObject worldCanvas = new GameObject("PlayerUI_WorldCanvas");
       worldCanvas.transform.SetParent(_playerHp.transform);
       worldCanvas.transform.localPosition = offsetPosition;
   
       Canvas newCanvas = worldCanvas.AddComponent<Canvas>();
       newCanvas.renderMode = RenderMode.WorldSpace;
       newCanvas.worldCamera = Camera.main;
   
       // Canvas Scaler 추가로 크기 자동 조정
       CanvasScaler scaler = worldCanvas.AddComponent<CanvasScaler>();
       scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
       scaler.referenceResolution = new Vector2(1920, 1080);
       scaler.dynamicPixelsPerUnit = 64;
   
       // 적절한 크기 설정
       RectTransform canvasRect = worldCanvas.GetComponent<RectTransform>();
       canvasRect.sizeDelta = new Vector2(108, 21);
   
       // 전체 스케일을 작게 조정
       worldCanvas.transform.localScale = new Vector3(uiScale, uiScale, uiScale);
   
       // 이 UI를 새 Canvas의 자식으로 이동
       transform.SetParent(worldCanvas.transform);
       transform.localPosition = Vector3.zero;
       transform.localScale = Vector3.one;
   }

   /// <summary>
   /// 바를 현재 HP에 맞춰서 변경하는 메서드입니다
   /// </summary>
   private void ChangeBar()
   {
       if (_playerHp == null || playerProtect == null) return;
       
       //현재 HP와 현재 보호막을 가져옵니다
       currenthP = _playerHp.CurrentHealth;
       currentProtect = playerProtect.ProtectionAmount;

       //최대 HP + 현재 보호막에서 최대 HP가 차지하는 분량을 계산합니다
       float maxHPFillAmount = (float)maxHP / (float)(maxHP + currentProtect);
       //최대 HP가 차지하는 분량에서 현재 HP가 차지하는 분량만큼 hpUI를 채웁니다
       hPUI.fillAmount = maxHPFillAmount * currenthP / maxHP;
       //현재 HP가 차지하는 분량 + 현재 보호막이 차지하는 분량만큼 보호막 UI를 채웁니다
       protectUI.fillAmount = (1 - maxHPFillAmount) + hPUI.fillAmount;
   }
}