using UnityEngine;

namespace HaChiMiOhNameruDo.Gameplay
{
    /// <summary>
    /// 输入处理器
    /// 负责处理触摸/鼠标输入，检测点击和滑动手势
    /// </summary>
    public class InputHandler : MonoBehaviour
    {
        [Header("手势检测设置")]
        [SerializeField] private float swipeThreshold = 0.5f;     // 滑动最小距离阈值
        [SerializeField] private float tapTimeThreshold = 0.3f;   // 点击最大时间阈值

        // 输入状态
        private Vector2 startTouchPosition;
        private Vector2 currentTouchPosition;
        private float touchStartTime;
        private bool isTouching;

        // 事件
        public System.Action<Vector2> OnTap;
        public System.Action<Vector2, SwipeDirection> OnSwipe;
        public System.Action<Vector2, float> OnSwipeDown;
        public System.Action<Vector2, float> OnSwipeHorizontal;

        public enum SwipeDirection
        {
            None,
            Up,
            Down,
            Left,
            Right
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

                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        OnTouchBegan(touch.position);
                        break;
                    case TouchPhase.Moved:
                        OnTouchMoved(touch.position);
                        break;
                    case TouchPhase.Ended:
                        OnTouchEnded(touch.position);
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
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                OnTouchBegan(Input.mousePosition);
            }
            else if (Input.GetMouseButton(0))
            {
                OnTouchMoved(Input.mousePosition);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                OnTouchEnded(Input.mousePosition);
            }
        }

        #endregion

        #region 触摸事件处理

        private void OnTouchBegan(Vector2 position)
        {
            isTouching = true;
            startTouchPosition = position;
            currentTouchPosition = position;
            touchStartTime = Time.time;
        }

        private void OnTouchMoved(Vector2 position)
        {
            if (!isTouching) return;
            currentTouchPosition = position;
        }

        private void OnTouchEnded(Vector2 position)
        {
            if (!isTouching) return;
            isTouching = false;

            Vector2 delta = position - startTouchPosition;
            float touchDuration = Time.time - touchStartTime;

            // 判断是点击还是滑动
            if (delta.magnitude < swipeThreshold && touchDuration < tapTimeThreshold)
            {
                // 点击
                OnTap?.Invoke(startTouchPosition);
            }
            else if (delta.magnitude >= swipeThreshold)
            {
                // 滑动
                HandleSwipe(delta);
            }

            currentTouchPosition = Vector2.zero;
        }

        private void OnTouchCancelled()
        {
            isTouching = false;
            currentTouchPosition = Vector2.zero;
        }

        #endregion

        #region 滑动处理

        private void HandleSwipe(Vector2 delta)
        {
            // 判断滑动方向
            SwipeDirection direction = GetSwipeDirection(delta);
            OnSwipe?.Invoke(startTouchPosition, direction);

            // 检查特定方向的滑动
            if (direction == SwipeDirection.Down)
            {
                OnSwipeDown?.Invoke(startTouchPosition, delta.magnitude);
            }
            else if (direction == SwipeDirection.Left || direction == SwipeDirection.Right)
            {
                OnSwipeHorizontal?.Invoke(startTouchPosition, delta.magnitude);
            }
        }

        private SwipeDirection GetSwipeDirection(Vector2 delta)
        {
            float absX = Mathf.Abs(delta.x);
            float absY = Mathf.Abs(delta.y);

            if (absX > absY)
            {
                // 水平滑动
                return delta.x > 0 ? SwipeDirection.Right : SwipeDirection.Left;
            }
            else
            {
                // 垂直滑动
                return delta.y > 0 ? SwipeDirection.Up : SwipeDirection.Down;
            }
        }

        #endregion
    }
}
