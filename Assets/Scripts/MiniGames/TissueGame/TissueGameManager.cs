using UnityEngine;
using HaChiMiOhNameruDo.Gameplay;
using HaChiMiOhNameruDo.Managers;

namespace HaChiMiOhNameruDo.MiniGames.TissueGame
{
    /// <summary>
    /// 纸巾筒小游戏管理器
    /// 负责管理纸巾筒小游戏的整体逻辑
    /// </summary>
    public class TissueGameManager : MonoBehaviour
    {
        public static TissueGameManager Instance { get; private set; }

        [Header("组件引用")]
        [SerializeField] private TissueBox tissueBox;
        [SerializeField] private TissuePaper tissuePaper;
        [SerializeField] private CatController catController;

        [Header("游戏设置")]
        [SerializeField] private int maxTissueLength = 100;  // 最大厕纸长度（单位：段）
        [SerializeField] private float gameDuration = 30f;   // 游戏时长

        private int currentTissueLength;  // 当前厕纸长度
        private bool isGameActive;

        public int CurrentTissueLength => currentTissueLength;
        public bool IsGameActive => isGameActive;

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

        /// <summary>
        /// 开始游戏
        /// </summary>
        public void StartGame()
        {
            Debug.Log("[TissueGame] 游戏开始");
            gameObject.SetActive(true);
            isGameActive = true;
            currentTissueLength = 0;

            // 准备猫咪状态
            if (catController == null)
                catController = FindObjectOfType<CatController>();
            
            catController?.PrepareForTissue();

            // 重置纸巾筒和厕纸
            if (tissueBox == null)
                tissueBox = FindObjectOfType<TissueBox>();
            if (tissuePaper == null)
                tissuePaper = FindObjectOfType<TissuePaper>();
            
            tissueBox?.ResetBox();
            tissuePaper?.ResetPaper();
        }

        /// <summary>
        /// 结束游戏
        /// </summary>
        public void EndGame()
        {
            Debug.Log("[TissueGame] 游戏结束");
            isGameActive = false;

            // 隐藏组件
            tissueBox?.Hide();
            tissuePaper?.Hide();

            // 隐藏游戏对象
            gameObject.SetActive(false);
        }

        /// <summary>
        /// 玩家向下划动厕纸
        /// </summary>
        public void OnSwipeDown(float swipeDistance)
        {
            if (!isGameActive) return;

            Debug.Log($"[TissueGame] 向下划动 - 距离：{swipeDistance}");
            
            // 增加厕纸长度
            int addedLength = Mathf.FloorToInt(swipeDistance);
            currentTissueLength += addedLength;
            
            // 更新厕纸显示
            tissuePaper?.ExtendPaper(currentTissueLength);
            
            // 触发震动反馈
            Managers.HapticManager.Instance?.PlayDefaultHaptic();

            // 检查是否耗尽
            if (currentTissueLength >= maxTissueLength)
            {
                OnTissueDepleted();
            }
        }

        /// <summary>
        /// 玩家横向划动厕纸（切碎）
        /// </summary>
        public void OnSwipeHorizontal(float swipeDistance)
        {
            if (!isGameActive) return;

            Debug.Log($"[TissueGame] 横向划动 - 距离：{swipeDistance}");
            
            // 切碎厕纸
            tissuePaper?.CutPaper();
            
            // 触发强烈震动和音效
            Managers.HapticManager.Instance?.PlayStrongHaptic();
            AudioManager.Instance?.PlayCutSound();
        }

        /// <summary>
        /// 当厕纸耗尽时调用
        /// </summary>
        private void OnTissueDepleted()
        {
            Debug.Log("[TissueGame] 厕纸耗尽");
            
            // 厕纸框上移消失
            tissueBox?.MoveUpAndDisappear();
            
            // 延迟后结束游戏
            Invoke(nameof(EndGame), 1f);
        }
    }
}
