using UnityEngine;
using HaChiMiOhNameruDo.Gameplay;
using HaChiMiOhNameruDo.Managers;

namespace HaChiMiOhNameruDo.MiniGames.FurBallGame
{
    /// <summary>
    /// 毛球小游戏管理器
    /// 负责管理毛球小游戏的整体逻辑
    /// </summary>
    public class FurBallGameManager : MonoBehaviour
    {
        public static FurBallGameManager Instance { get; private set; }

        [Header("组件引用")]
        [SerializeField] private FurBall furBall;
        [SerializeField] private CatController catController;
        [SerializeField] private InputHandler inputHandler;

        [Header("游戏设置")]
        [SerializeField] private float gameDuration = 30f;  // 游戏时长

        private bool isGameActive;

        private void Awake()
        {
            // 单例模式
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        private void Start()
        {
            // 初始状态：不激活
            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            // 订阅输入事件
            if (inputHandler == null)
                inputHandler = FindObjectOfType<InputHandler>();
            
            if (inputHandler != null)
            {
                inputHandler.OnTap += HandleScreenTap;
            }
        }

        private void OnDisable()
        {
            // 取消订阅输入事件
            if (inputHandler != null)
            {
                inputHandler.OnTap -= HandleScreenTap;
            }
        }

        /// <summary>
        /// 开始游戏
        /// </summary>
        public void StartGame()
        {
            Debug.Log("[FurBallGame] 游戏开始");
            gameObject.SetActive(true);
            isGameActive = true;

            // 准备猫咪状态
            if (catController == null)
                catController = FindObjectOfType<CatController>();
            
            catController?.PrepareForFurBall();

            // 生成/重置毛球
            if (furBall == null)
                furBall = FindObjectOfType<FurBall>();
            
            furBall?.ResetBall();
            furBall?.EnableInteraction();
        }

        /// <summary>
        /// 结束游戏
        /// </summary>
        public void EndGame()
        {
            Debug.Log("[FurBallGame] 游戏结束");
            isGameActive = false;

            // 禁用毛球交互
            furBall?.DisableInteraction();
            furBall?.Hide();

            // 隐藏游戏对象
            gameObject.SetActive(false);
        }

        /// <summary>
        /// 当毛球被拍击时调用
        /// </summary>
        public void OnBallPawed()
        {
            if (!isGameActive) return;

            Debug.Log("[FurBallGame] 毛球被拍击");
            
            // 触发震动反馈
            Managers.HapticManager.Instance?.PlayLightHaptic();
        }

        /// <summary>
        /// 当毛球回到原位时调用
        /// </summary>
        public void OnBallReturned()
        {
            if (!isGameActive) return;

            Debug.Log("[FurBallGame] 毛球回到原位");
        }

        /// <summary>
        /// 处理屏幕点击（触发猫咪拍击动画）
        /// </summary>
        private void HandleScreenTap(Vector2 position)
        {
            if (!isGameActive) return;

            // 检查是否点击到 UI（如果是则不触发）
            if (UnityEngine.EventSystems.EventSystem.current != null)
            {
                var eventData = new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current);
                eventData.position = position;
                var raycastResults = new System.Collections.Generic.List<UnityEngine.EventSystems.RaycastResult>();
                UnityEngine.EventSystems.EventSystem.current.RaycastAll(eventData, raycastResults);
                
                // 如果点击到 UI，则不触发猫咪动画
                if (raycastResults.Count > 0)
                {
                    return;
                }
            }

            // 触发猫咪拍击动画
            if (catController == null)
                catController = FindObjectOfType<CatController>();
            
            catController?.DoPaws();
        }
    }
}
