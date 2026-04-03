using UnityEngine;
using HaChiMiOhNameruDo.MiniGames.FurBallGame;
using HaChiMiOhNameruDo.MiniGames.TissueGame;

namespace HaChiMiOhNameruDo.Managers
{
    /// <summary>
    /// 游戏状态枚举
    /// </summary>
    public enum GameState
    {
        Idle,           // IDLE 状态 - 主界面
        FurBallGame,    // 小游戏 1 - 毛球
        TissueGame      // 小游戏 2 - 纸巾筒
    }

    /// <summary>
    /// 游戏管理器 - 单例模式
    /// 负责管理游戏状态切换和全局游戏逻辑
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("游戏状态")]
        [SerializeField] private GameState currentState = GameState.Idle;

        [Header("小游戏时长设置")]
        [SerializeField] private float miniGameDuration = 30f; // 每个小游戏持续 30 秒

        [Header("退出延迟设置")]
        [SerializeField] private float exitDelay = 2f; // 退出小游戏后等待 2 秒再返回 IDLE

        public GameState CurrentState => currentState;
        public float MiniGameDuration => miniGameDuration;
        public float ExitDelay => exitDelay;

        private float gameTimer;
        private bool isGameRunning;
        private bool isExitingGame;
        private float exitTimer;

        private void Awake()
        {
            // 单例模式
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        private void Start()
        {
            // 初始化为 IDLE 状态
            SetGameState(GameState.Idle);
        }

        private void Update()
        {
            if (isGameRunning)
            {
                gameTimer -= Time.deltaTime;
                if (gameTimer <= 0)
                {
                    EndMiniGame();
                }
            }

            // 处理退出延迟
            if (isExitingGame)
            {
                exitTimer -= Time.deltaTime;
                if (exitTimer <= 0)
                {
                    isExitingGame = false;
                    SetGameState(GameState.Idle);
                }
            }
        }

        /// <summary>
        /// 设置游戏状态
        /// </summary>
        public void SetGameState(GameState newState)
        {
            if (currentState == newState) return;

            // 离开当前状态
            ExitState(currentState);

            currentState = newState;
            Debug.Log($"[GameManager] 状态变更：{newState}");

            // 进入新状态
            EnterState(newState);
        }

        /// <summary>
        /// 进入状态时的处理
        /// </summary>
        private void EnterState(GameState state)
        {
            switch (state)
            {
                case GameState.Idle:
                    EnterIdleState();
                    break;
                case GameState.FurBallGame:
                    EnterFurBallGame();
                    break;
                case GameState.TissueGame:
                    EnterTissueGame();
                    break;
            }
        }

        /// <summary>
        /// 离开状态时的处理
        /// </summary>
        private void ExitState(GameState state)
        {
            switch (state)
            {
                case GameState.Idle:
                    ExitIdleState();
                    break;
                case GameState.FurBallGame:
                    ExitFurBallGame();
                    break;
                case GameState.TissueGame:
                    ExitTissueGame();
                    break;
            }
        }

        #region IDLE 状态

        private void EnterIdleState()
        {
            isGameRunning = false;
            // 显示主界面 UI，隐藏小游戏 UI
            UIManager.Instance?.ShowIdleUI();
        }

        private void ExitIdleState()
        {
            // 隐藏主界面 UI
            UIManager.Instance?.HideIdleUI();
        }

        #endregion

        #region 小游戏 1 - 毛球

        private void EnterFurBallGame()
        {
            isGameRunning = true;
            gameTimer = miniGameDuration;
            // 启动毛球小游戏
            FurBallGameManager.Instance?.StartGame();
            // 显示毛球小游戏 UI
            UIManager.Instance?.ShowFurBallGameUI();
        }

        private void ExitFurBallGame()
        {
            // 结束毛球小游戏
            FurBallGameManager.Instance?.EndGame();
        }

        #endregion

        #region 小游戏 2 - 纸巾筒

        private void EnterTissueGame()
        {
            isGameRunning = true;
            gameTimer = miniGameDuration;
            // 启动纸巾筒小游戏
            TissueGameManager.Instance?.StartGame();
            // 显示纸巾筒小游戏 UI
            UIManager.Instance?.ShowTissueGameUI();
        }

        private void ExitTissueGame()
        {
            // 结束纸巾筒小游戏
            TissueGameManager.Instance?.EndGame();
        }

        #endregion

        /// <summary>
        /// 开始小游戏 1 - 毛球
        /// </summary>
        public void StartFurBallGame()
        {
            SetGameState(GameState.FurBallGame);
        }

        /// <summary>
        /// 开始小游戏 2 - 纸巾筒
        /// </summary>
        public void StartTissueGame()
        {
            SetGameState(GameState.TissueGame);
        }

        /// <summary>
        /// 结束小游戏，返回 IDLE
        /// </summary>
        private void EndMiniGame()
        {
            SetGameState(GameState.Idle);
        }

        /// <summary>
        /// 退出小游戏（带延迟，让小游戏物体缓慢移出屏幕）
        /// </summary>
        public void ExitMiniGame()
        {
            if (!isGameRunning || isExitingGame) return;

            isExitingGame = true;
            exitTimer = exitDelay;
            isGameRunning = false;

            Debug.Log($"[GameManager] 退出小游戏，{exitDelay}秒后返回 IDLE");

            // 通知小游戏管理器结束游戏（让物体缓慢移出）
            if (currentState == GameState.FurBallGame)
            {
                FurBallGameManager.Instance?.EndGame();
            }
            else if (currentState == GameState.TissueGame)
            {
                TissueGameManager.Instance?.EndGame();
            }

            // 隐藏小游戏 UI
            UIManager.Instance?.HideAllUI();
        }

        /// <summary>
        /// 强制返回 IDLE 状态（例如玩家点击返回按钮）
        /// </summary>
        public void ReturnToIdle()
        {
            if (isGameRunning)
            {
                ExitMiniGame();
            }
            else
            {
                SetGameState(GameState.Idle);
            }
        }
    }
}
