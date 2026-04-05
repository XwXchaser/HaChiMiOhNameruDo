using UnityEngine;
using System.Collections;
using HaChiMiOhNameruDo.Gameplay;

namespace HaChiMiOhNameruDo.MiniGames.TissueGame
{
    /// <summary>
    /// 纸巾筒游戏管理器
    /// 负责游戏整体状态管理、流程控制
    /// </summary>
    public class TissueGameManager : MonoBehaviour
    {
        // 单例模式
        private static TissueGameManager _instance;
        public static TissueGameManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<TissueGameManager>();
                }
                return _instance;
            }
        }

        [Header("组件引用")]
        [Tooltip("纸巾筒组件")]
        public TissueBox tissueBox;

        [Tooltip("纸巾组件")]
        public TissuePaper tissuePaper;

        [Tooltip("堆积纸巾管理器")]
        public TissuePileManager pileManager;

        [Tooltip("输入处理器")]
        public TissueInputHandler inputHandler;

        [Header("配置引用")]
        public TissueGameConfig config;

        [Header("猫咪控制器")]
        [Tooltip("猫咪控制器（用于播放动画）")]
        private CatController catController;

        // 状态
        private TissueGameState currentState = TissueGameState.Idle;

        // 游戏数据
        private int score = 0;
        private int tissuesUsed = 0;

        // 事件
        public delegate void GameAction();
        public event GameAction OnGameStart;
        public event System.Action<int> OnScoreChanged;

        /// <summary>
        /// 游戏状态
        /// </summary>
        public enum TissueGameState
        {
            Idle,           // 空闲
            Playing,        // 游戏中
            Pulling,        // 抽取中
            Cutting,        // 切断中
            Empty,          // 纸巾耗尽
            ClearingChamber,// 清空弹仓
            Reloading       // 装填中
            // 无游戏结束状态 - 游戏无限进行
        }

        public TissueGameState CurrentState => currentState;
        public int Score => score;
        public int TissuesUsed => tissuesUsed;

        private void Awake()
        {
            if (config == null)
            {
                config = FindObjectOfType<TissueGameConfig>();
            }

            // 获取组件引用
            if (tissueBox == null)
                tissueBox = FindObjectOfType<TissueBox>();
            if (tissuePaper == null)
                tissuePaper = FindObjectOfType<TissuePaper>();
            if (pileManager == null)
                pileManager = FindObjectOfType<TissuePileManager>();
            if (inputHandler == null)
                inputHandler = FindObjectOfType<TissueInputHandler>();
            
            // 获取猫咪控制器
            catController = FindObjectOfType<CatController>();
        }

        private void OnEnable()
        {
            // 订阅输入事件
            if (inputHandler != null)
            {
                inputHandler.OnPullStart += HandlePullStart;
                inputHandler.OnPullUpdate += HandlePullUpdate;
                inputHandler.OnPullEnd += HandlePullEnd;
                inputHandler.OnSwipeLeft += HandleSwipeLeft;
                inputHandler.OnSwipeRight += HandleSwipeRight;
                inputHandler.OnSwipeUp += HandleSwipeUp;
                inputHandler.OnSwipeDown += HandleSwipeDown;
            }

            // 订阅纸巾筒事件
            if (tissueBox != null)
            {
                tissueBox.OnBoxEmpty += HandleBoxEmpty;
                tissueBox.OnChamberCleared += HandleChamberCleared;
                tissueBox.OnBoxReloaded += HandleBoxReloaded;
            }

            // 订阅纸巾事件
            if (tissuePaper != null)
            {
                tissuePaper.OnPaperCut += HandlePaperCut;
                tissuePaper.OnPaperRetracted += HandlePaperRetracted;
            }

            // 订阅堆积纸巾事件
            if (pileManager != null)
            {
                pileManager.OnPileCountChanged += HandlePileCountChanged;
                pileManager.OnPileCleared += HandlePileCleared;
            }
        }

        private void OnDisable()
        {
            // 取消订阅输入事件
            if (inputHandler != null)
            {
                inputHandler.OnPullStart -= HandlePullStart;
                inputHandler.OnPullUpdate -= HandlePullUpdate;
                inputHandler.OnPullEnd -= HandlePullEnd;
                inputHandler.OnSwipeLeft -= HandleSwipeLeft;
                inputHandler.OnSwipeRight -= HandleSwipeRight;
                inputHandler.OnSwipeUp -= HandleSwipeUp;
                inputHandler.OnSwipeDown -= HandleSwipeDown;
            }

            // 取消订阅纸巾筒事件
            if (tissueBox != null)
            {
                tissueBox.OnBoxEmpty -= HandleBoxEmpty;
                tissueBox.OnChamberCleared -= HandleChamberCleared;
                tissueBox.OnBoxReloaded -= HandleBoxReloaded;
            }

            // 取消订阅纸巾事件
            if (tissuePaper != null)
            {
                tissuePaper.OnPaperCut -= HandlePaperCut;
                tissuePaper.OnPaperRetracted -= HandlePaperRetracted;
            }

            // 取消订阅堆积纸巾事件
            if (pileManager != null)
            {
                pileManager.OnPileCountChanged -= HandlePileCountChanged;
                pileManager.OnPileCleared -= HandlePileCleared;
            }
        }

        /// <summary>
        /// 开始游戏
        /// </summary>
        public void StartGame()
        {
            if (currentState == TissueGameState.Playing) return;

            currentState = TissueGameState.Playing;
            score = 0;
            tissuesUsed = 0;

            // 准备猫咪状态
            if (catController == null)
                catController = FindObjectOfType<CatController>();
            
            catController?.PlayTissueIdle();

            // 确保所有组件都被激活
            if (tissueBox != null && !tissueBox.gameObject.activeSelf)
                tissueBox.gameObject.SetActive(true);
            if (tissuePaper != null && !tissuePaper.gameObject.activeSelf)
                tissuePaper.gameObject.SetActive(true);
            if (pileManager != null && !pileManager.gameObject.activeSelf)
                pileManager.gameObject.SetActive(true);
            if (inputHandler != null && !inputHandler.gameObject.activeSelf)
                inputHandler.gameObject.SetActive(true);

            // 重置所有组件
            tissueBox?.ResetBox();
            tissuePaper?.ResetPaper();
            pileManager?.ResetPile();
            inputHandler?.ResetHandler();

            OnGameStart?.Invoke();
            Debug.Log("[TissueGameManager] 游戏开始");
        }

        #region 输入处理

        private void HandlePullStart(Vector2 startPos, Vector2 delta)
        {
            if (currentState != TissueGameState.Playing) return;
            currentState = TissueGameState.Pulling;
            tissueBox?.HandlePull();
            // 触发猫咪扒拉动画
            catController?.PlayTissuePull();
        }

        private void HandlePullUpdate(Vector2 startPos, Vector2 delta)
        {
            if (currentState != TissueGameState.Pulling) return;
            
            // 根据拖动距离延伸纸巾（每次超过阈值时延伸）
            float pullDistance = Mathf.Abs(delta.y);
            int pullsNeeded = Mathf.FloorToInt(pullDistance / config.pullThreshold);
            
            // 简单处理：每次 Update 调用 Extend
            // 实际延伸逻辑由 TissuePaper 根据 pullCount 管理
            tissuePaper?.Extend();
            
            // 同时增加堆积计数
            pileManager?.AddPull();
        }

        private void HandlePullEnd()
        {
            if (currentState != TissueGameState.Pulling) return;
            currentState = TissueGameState.Playing;
            
            // 停止抽取动画
            tissueBox?.StopPullAnimation();
        }

        private void HandleSwipeLeft()
        {
            // 在纸巾筒区域：切断纸巾
            if (inputHandler.CurrentZone == TissueInputHandler.InputZone.TissueBox)
            {
                if (tissueBox != null && tissueBox.CanCut)
                {
                    currentState = TissueGameState.Cutting;
                    tissueBox.HandleCut();
                    tissuePaper?.Cut();
                    // 触发猫咪切断动画
                    catController?.PlayTissueCut();
                }
            }
            // 在堆积区域：清理纸巾
            else if (inputHandler.CurrentZone == TissueInputHandler.InputZone.TissuePile)
            {
                if (pileManager != null && pileManager.CanClear())
                {
                    pileManager.HandleSwipe();
                }
            }
            // 在清空弹仓后：装填新纸卷（从右向左划动）
            else if (currentState == TissueGameState.ClearingChamber)
            {
                if (tissueBox != null && tissueBox.CanReloadCheck())
                {
                    currentState = TissueGameState.Reloading;
                    tissueBox.HandleReload();
                }
            }
        }

        private void HandleSwipeRight()
        {
            // 在纸巾筒区域：切断纸巾
            if (inputHandler.CurrentZone == TissueInputHandler.InputZone.TissueBox)
            {
                if (tissueBox != null && tissueBox.CanCut)
                {
                    currentState = TissueGameState.Cutting;
                    tissueBox.HandleCut();
                    tissuePaper?.Cut();
                    // 触发猫咪切断动画
                    catController?.PlayTissueCut();
                }
            }
            // 在堆积区域：清理纸巾
            else if (inputHandler.CurrentZone == TissueInputHandler.InputZone.TissuePile)
            {
                if (pileManager != null && pileManager.CanClear())
                {
                    pileManager.HandleSwipe();
                }
            }
            // 在耗尽状态：清空弹仓
            else if (currentState == TissueGameState.Empty)
            {
                if (tissueBox != null && tissueBox.CanClearChamber)
                {
                    currentState = TissueGameState.ClearingChamber;
                    tissueBox.HandleClearChamber();
                }
            }
        }

        private void HandleSwipeUp()
        {
            // 预留：向上划动功能
        }

        private void HandleSwipeDown()
        {
            // 预留：向下划动功能
        }

        #endregion

        #region 组件事件处理

        private void HandleBoxEmpty()
        {
            currentState = TissueGameState.Empty;
            Debug.Log("[TissueGameManager] 纸巾耗尽，请向右划动清空弹仓");
        }

        private void HandleChamberCleared()
        {
            Debug.Log("[TissueGameManager] 弹仓已清空，请从屏幕右侧向中心划动装填");
        }

        private void HandleBoxReloaded()
        {
            currentState = TissueGameState.Playing;
            tissuesUsed++;
            Debug.Log("[TissueGameManager] 装填完成，继续游戏");
        }

        private void HandlePaperCut(int length)
        {
            // 根据切断时的扒拉次数计算得分
            int points = CalculateScore(length);
            score += points;
            OnScoreChanged?.Invoke(score);
            Debug.Log($"[TissueGameManager] 切断得分：{points}, 总分：{score}");

            currentState = TissueGameState.Playing;
        }

        private void HandlePaperRetracted()
        {
            currentState = TissueGameState.Playing;
        }

        private void HandlePileCountChanged(int currentCount, int maxCount)
        {
            // 仅用于调试输出，不再触发游戏结束
            Debug.Log($"[TissueGameManager] 堆积计数：{currentCount}/{maxCount}");
        }

        private void HandlePileCleared(int currentCount, int maxCount)
        {
            Debug.Log($"[TissueGameManager] 清理后剩余扒拉次数：{currentCount}");
        }

        #endregion

        /// <summary>
        /// 计算得分
        /// </summary>
        private int CalculateScore(int length)
        {
            // 理想长度得分最高
            int idealLength = config.idealTissueLength;
            int diff = Mathf.Abs(length - idealLength);

            if (diff == 0) return 100;  // 完美
            if (diff <= 2) return 50;   // 很好
            if (diff <= 5) return 25;   // 一般
            return 10;                   // 较差
        }

        /// <summary>
        /// 重置游戏
        /// </summary>
        public void ResetGame()
        {
            StopAllCoroutines();
            currentState = TissueGameState.Idle;
            score = 0;
            tissuesUsed = 0;

            tissueBox?.ResetBox();
            tissuePaper?.ResetPaper();
            pileManager?.ResetPile();
            inputHandler?.ResetHandler();

            Debug.Log("[TissueGameManager] 游戏重置");
        }
    }
}
