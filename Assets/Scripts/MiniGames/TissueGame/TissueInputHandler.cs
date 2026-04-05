using UnityEngine;

namespace HaChiMiOhNameruDo.MiniGames.TissueGame
{
    /// <summary>
    /// 纸巾游戏输入处理器
    /// 支持多个判定区域的输入处理
    /// 支持纵向划动（抽取）和横向划动（切断/清理/装填）
    /// </summary>
    public class TissueInputHandler : MonoBehaviour
    {
        [Header("判定区域")]
        [Tooltip("纸巾筒判定区域（Collider2D）")]
        public Collider2D tissueBoxZone;

        [Tooltip("纸巾堆积判定区域（Collider2D）")]
        public Collider2D tissuePileZone;

        [Header("配置引用")]
        public TissueGameConfig config;

        // 输入状态
        private bool isDragging = false;
        private Vector2 dragStartPos;
        private Vector2 dragCurrentPos;
        private Vector2 dragDelta;

        // 当前拖动的判定区域
        private InputZone currentZone = InputZone.None;

        // 事件
        public delegate void InputAction(Vector2 startPos, Vector2 delta);
        public event InputAction OnPullStart;         // 抽取开始
        public event InputAction OnPullUpdate;        // 抽取中
        public event System.Action OnPullEnd;         // 抽取结束
        public event System.Action OnSwipeLeft;       // 向左划动（切断/清理）
        public event System.Action OnSwipeRight;      // 向右划动（切断/清空弹仓）
        public event System.Action OnSwipeUp;         // 向上划动
        public event System.Action OnSwipeDown;       // 向下划动

        /// <summary>
        /// 判定区域枚举
        /// </summary>
        public enum InputZone
        {
            None,
            TissueBox,      // 纸巾筒区域
            TissuePile      // 纸巾堆积区域
        }

        public InputZone CurrentZone => currentZone;

        private void Awake()
        {
            if (config == null)
            {
                config = FindObjectOfType<TissueGameConfig>();
            }
        }

        private void Update()
        {
            HandleMouseInput();
            HandleTouchInput();
        }

        /// <summary>
        /// 处理鼠标输入
        /// </summary>
        private void HandleMouseInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                StartDrag(worldPos);
            }
            else if (Input.GetMouseButton(0))
            {
                Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                UpdateDrag(worldPos);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                EndDrag();
            }
        }

        /// <summary>
        /// 处理触摸输入
        /// </summary>
        private void HandleTouchInput()
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        Vector2 worldPos = Camera.main.ScreenToWorldPoint(touch.position);
                        StartDrag(worldPos);
                        break;

                    case TouchPhase.Moved:
                        Vector2 movePos = Camera.main.ScreenToWorldPoint(touch.position);
                        UpdateDrag(movePos);
                        break;

                    case TouchPhase.Ended:
                    case TouchPhase.Canceled:
                        EndDrag();
                        break;
                }
            }
        }

        /// <summary>
        /// 开始拖动
        /// </summary>
        private void StartDrag(Vector2 worldPos)
        {
            // 检查点击位置在哪个判定区域
            if (tissueBoxZone != null && tissueBoxZone.OverlapPoint(worldPos))
            {
                currentZone = InputZone.TissueBox;
                isDragging = true;
                dragStartPos = worldPos;
                dragCurrentPos = worldPos;
                dragDelta = Vector2.zero;

                OnPullStart?.Invoke(dragStartPos, dragDelta);
                Debug.Log($"[TissueInputHandler] 开始在纸巾筒区域拖动：{worldPos}");
            }
            else if (tissuePileZone != null && tissuePileZone.OverlapPoint(worldPos))
            {
                currentZone = InputZone.TissuePile;
                isDragging = true;
                dragStartPos = worldPos;
                dragCurrentPos = worldPos;
                dragDelta = Vector2.zero;

                Debug.Log($"[TissueInputHandler] 开始在纸巾堆积区域拖动：{worldPos}");
            }
            else
            {
                currentZone = InputZone.None;
                isDragging = false;
            }
        }

        /// <summary>
        /// 更新拖动
        /// </summary>
        private void UpdateDrag(Vector2 worldPos)
        {
            if (!isDragging) return;

            dragDelta = worldPos - dragStartPos;
            dragCurrentPos = worldPos;

            // 只在纸巾筒区域触发抽取更新
            if (currentZone == InputZone.TissueBox)
            {
                OnPullUpdate?.Invoke(dragStartPos, dragDelta);
            }
        }

        /// <summary>
        /// 结束拖动
        /// </summary>
        private void EndDrag()
        {
            if (!isDragging) return;

            // 根据拖动距离判断划动方向
            if (Mathf.Abs(dragDelta.x) > Mathf.Abs(dragDelta.y))
            {
                // 横向划动
                if (Mathf.Abs(dragDelta.x) > config.swipeThreshold)
                {
                    if (dragDelta.x > 0)
                    {
                        OnSwipeRight?.Invoke();
                        Debug.Log($"[TissueInputHandler] 向右划动：{dragDelta.x}，区域：{currentZone}");
                    }
                    else
                    {
                        OnSwipeLeft?.Invoke();
                        Debug.Log($"[TissueInputHandler] 向左划动：{dragDelta.x}，区域：{currentZone}");
                    }
                }
            }
            else
            {
                // 纵向划动
                if (Mathf.Abs(dragDelta.y) > config.pullThreshold)
                {
                    if (dragDelta.y > 0)
                    {
                        OnSwipeUp?.Invoke();
                        Debug.Log($"[TissueInputHandler] 向上划动：{dragDelta.y}");
                    }
                    else
                    {
                        OnSwipeDown?.Invoke();
                        Debug.Log($"[TissueInputHandler] 向下划动：{dragDelta.y}");
                    }
                }
            }

            // 触发抽取结束事件（仅在纸巾筒区域）
            if (currentZone == InputZone.TissueBox)
            {
                OnPullEnd?.Invoke();
            }

            // 重置状态
            isDragging = false;
            currentZone = InputZone.None;
            dragDelta = Vector2.zero;
        }

        /// <summary>
        /// 检查点是否在指定区域内
        /// </summary>
        public bool IsPointInZone(Vector2 point, InputZone zone)
        {
            switch (zone)
            {
                case InputZone.TissueBox:
                    return tissueBoxZone != null && tissueBoxZone.OverlapPoint(point);
                case InputZone.TissuePile:
                    return tissuePileZone != null && tissuePileZone.OverlapPoint(point);
                default:
                    return false;
            }
        }

        /// <summary>
        /// 重置输入处理器
        /// </summary>
        public void ResetHandler()
        {
            isDragging = false;
            currentZone = InputZone.None;
            dragDelta = Vector2.zero;
        }
    }
}
