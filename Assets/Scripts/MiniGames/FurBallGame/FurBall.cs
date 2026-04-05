using UnityEngine;
using HaChiMiOhNameruDo.Gameplay;

namespace HaChiMiOhNameruDo.MiniGames.FurBallGame
{
    /// <summary>
    /// 毛球组件
    /// 负责毛球的运动、交互和动画
    /// </summary>
    public class FurBall : MonoBehaviour
    {
        [Header("运动设置")]
        [SerializeField] private float launchHeight = 5f;       // 拍击后上升高度
        [SerializeField] private float launchDuration = 0.5f;   // 上升时间
        [SerializeField] private float fallDuration = 1.0f;     // 下落时间
        [SerializeField] private float stayOutDuration = 1.5f;  // 在屏幕外停留时间

        [Header("组件引用")]
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private Collider2D ballCollider;
        [SerializeField] private SpriteRenderer spriteRenderer;

        // 状态
        private bool isInteractable;
        private bool isMoving;
        private Vector3 startPosition;
        private Vector3 targetPosition;

        // 运动相关
        private float moveTimer;
        private BallState currentState;

        private enum BallState
        {
            Idle,       // 静止状态
            Launching,  // 上升中
            StayingOut, // 停留在屏幕外
            Falling     // 下落中
        }

        private void Awake()
        {
            if (rb == null)
                rb = GetComponent<Rigidbody2D>();
            if (ballCollider == null)
                ballCollider = GetComponent<Collider2D>();
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            Hide();
            DisableInteraction();
        }

        private void Update()
        {
            if (!isMoving) return;

            moveTimer += Time.deltaTime;

            switch (currentState)
            {
                case BallState.Launching:
                    UpdateLaunching();
                    break;
                case BallState.StayingOut:
                    UpdateStayingOut();
                    break;
                case BallState.Falling:
                    UpdateFalling();
                    break;
            }
        }

        #region 运动逻辑

        private void UpdateLaunching()
        {
            // 上升运动 - 使用缓动
            float t = moveTimer / launchDuration;
            if (t >= 1f)
            {
                // 到达最高点
                transform.position = targetPosition;
                StartStayingOut();
            }
            else
            {
                // 使用 EaseOut 缓动
                t = 1 - Mathf.Pow(1 - t, 3);
                transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            }
        }

        private void UpdateStayingOut()
        {
            if (moveTimer >= stayOutDuration)
            {
                // 开始下落
                StartFalling();
            }
        }

        private void UpdateFalling()
        {
            // 下落运动 - 使用缓动
            float t = moveTimer / fallDuration;
            if (t >= 1f)
            {
                // 回到原位
                transform.position = startPosition;
                isMoving = false;
                currentState = BallState.Idle;
                OnReturned();
            }
            else
            {
                // 使用 EaseIn 缓动（模拟重力加速）
                t = t * t;
                transform.position = Vector3.Lerp(targetPosition, startPosition, t);
            }
        }

        #endregion

        #region 状态控制

        /// <summary>
        /// 重置毛球到初始位置
        /// </summary>
        public void ResetBall()
        {
            StopAllCoroutines();
            isMoving = false;
            currentState = BallState.Idle;
            moveTimer = 0;

            // 使用默认位置
            startPosition = transform.position;

            transform.position = startPosition;
        }

        /// <summary>
        /// 启用交互
        /// </summary>
        public void EnableInteraction()
        {
            isInteractable = true;
            Show();
        }

        /// <summary>
        /// 禁用交互
        /// </summary>
        public void DisableInteraction()
        {
            isInteractable = false;
        }

        /// <summary>
        /// 显示毛球
        /// </summary>
        public void Show()
        {
            if (spriteRenderer != null)
                spriteRenderer.enabled = true;
            if (ballCollider != null)
                ballCollider.enabled = true;
        }

        /// <summary>
        /// 隐藏毛球
        /// </summary>
        public void Hide()
        {
            if (spriteRenderer != null)
                spriteRenderer.enabled = false;
            if (ballCollider != null)
                ballCollider.enabled = false;
        }

        #endregion

        #region 拍击逻辑

        /// <summary>
        /// 当毛球被点击/拍击时调用
        /// </summary>
        private void OnMouseDown()
        {
            if (!isInteractable || isMoving || currentState != BallState.Idle)
                return;

            PawBall();
        }

        /// <summary>
        /// 执行拍击动作
        /// </summary>
        public void PawBall()
        {
            Debug.Log("[FurBall] 被拍击！");

            // 通知管理器
            FurBallGameManager.Instance?.OnBallPawed();

            // 开始上升运动
            StartLaunching();
        }

        private void StartLaunching()
        {
            currentState = BallState.Launching;
            isMoving = true;
            moveTimer = 0;

            // 计算目标位置（正上方）
            targetPosition = startPosition + Vector3.up * launchHeight;
        }

        private void StartStayingOut()
        {
            currentState = BallState.StayingOut;
            moveTimer = 0;
        }

        private void StartFalling()
        {
            currentState = BallState.Falling;
            moveTimer = 0;
        }

        #endregion

        #region 回调

        /// <summary>
        /// 当毛球回到原位时调用
        /// </summary>
        private void OnReturned()
        {
            Debug.Log("[FurBall] 回到原位");
            FurBallGameManager.Instance?.OnBallReturned();
        }

        #endregion
    }
}
