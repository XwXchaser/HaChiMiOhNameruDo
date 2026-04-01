using UnityEngine;

namespace HaChiMiOhNameruDo.MiniGames.TissueGame
{
    /// <summary>
    /// 纸巾筒小游戏输入处理器
    /// 负责处理厕纸的划动输入
    /// </summary>
    public class TissueInputHandler : MonoBehaviour
    {
        [Header("组件引用")]
        [SerializeField] private TissueGameManager tissueGameManager;
        [SerializeField] private Collider2D tissueCollider;  // 厕纸碰撞体

        [Header("手势检测设置")]
        [SerializeField] private float swipeThreshold = 50f;    // 滑动最小距离阈值（像素）

        // 输入状态
        private Vector2 startTouchPosition;
        private Vector2 currentTouchPosition;
        private bool isTouching;
        private bool isOverTissue;  // 是否在厕纸上方

        private void Awake()
        {
            if (tissueGameManager == null)
                tissueGameManager = GetComponent<TissueGameManager>();
        }

        private void Update()
        {
            HandleTouchInput();
            HandleMouseInput();
        }

        #region 触摸输入处理

        private void HandleTouchInput()
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                Vector2 touchWorldPos = Camera.main.ScreenToWorldPoint(touch.position);

                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        OnTouchBegan(touch.position, touchWorldPos);
                        break;
                    case TouchPhase.Moved:
                        OnTouchMoved(touch.position, touchWorldPos);
                        break;
                    case TouchPhase.Ended:
                        OnTouchEnded(touch.position, touchWorldPos);
                        break;
                    case TouchPhase.Canceled:
                        OnTouchCancelled();
                        break;
                }
            }
        }

        #endregion

        #region 鼠标输入处理（用于编辑器测试）

        private void HandleMouseInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                OnTouchBegan(Input.mousePosition, mouseWorldPos);
            }
            else if (Input.GetMouseButton(0))
            {
                Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                OnTouchMoved(Input.mousePosition, mouseWorldPos);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                OnTouchEnded(Input.mousePosition, mouseWorldPos);
            }
        }

        #endregion

        #region 触摸事件处理

        private void OnTouchBegan(Vector2 screenPosition, Vector2 worldPosition)
        {
            // 检查是否在厕纸上方
            isOverTissue = IsOverTissue(worldPosition);
            
            if (isOverTissue)
            {
                isTouching = true;
                startTouchPosition = screenPosition;
                currentTouchPosition = screenPosition;
            }
        }

        private void OnTouchMoved(Vector2 screenPosition, Vector2 worldPosition)
        {
            if (!isTouching || !isOverTissue) return;
            currentTouchPosition = screenPosition;
        }

        private void OnTouchEnded(Vector2 screenPosition, Vector2 worldPosition)
        {
            if (!isTouching || !isOverTissue) return;
            isTouching = false;

            Vector2 delta = screenPosition - startTouchPosition;

            // 判断滑动方向和距离
            if (delta.magnitude >= swipeThreshold)
            {
                HandleSwipe(delta);
            }
        }

        private void OnTouchCancelled()
        {
            isTouching = false;
        }

        #endregion

        #region 滑动处理

        private void HandleSwipe(Vector2 delta)
        {
            float absX = Mathf.Abs(delta.x);
            float absY = Mathf.Abs(delta.y);

            if (absY > absX && delta.y < 0)
            {
                // 向下滑动 - 拉出厕纸
                Debug.Log($"[TissueInput] 向下滑动 - 距离：{absY}");
                tissueGameManager?.OnSwipeDown(absY);
            }
            else if (absX > absY)
            {
                // 横向滑动 - 切碎厕纸
                Debug.Log($"[TissueInput] 横向滑动 - 距离：{absX}");
                tissueGameManager?.OnSwipeHorizontal(absX);
            }
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 检查是否在厕纸上方
        /// </summary>
        private bool IsOverTissue(Vector2 worldPosition)
        {
            if (tissueCollider == null) return false;

            // 使用 Physics2D.OverlapPoint 检查
            return Physics2D.OverlapPoint(worldPosition) == tissueCollider;
        }

        #endregion
    }
}
