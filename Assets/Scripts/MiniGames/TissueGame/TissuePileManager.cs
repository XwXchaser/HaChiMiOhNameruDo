using UnityEngine;
using System.Collections;

namespace HaChiMiOhNameruDo.MiniGames.TissueGame
{
    /// <summary>
    /// 堆积纸巾管理器
    /// 负责管理 L1/L2 两层纸巾堆的显示和清理
    /// </summary>
    public class TissuePileManager : MonoBehaviour
    {
        [Header("美术素材引用")]
        [Tooltip("纸巾堆 L2 精灵（5 次后显示）")]
        public Sprite pileL2;

        [Tooltip("纸巾堆 L1 精灵（10 次后叠加在 L2 上）")]
        public Sprite pileL1;

        [Header("组件引用")]
        [Tooltip("L2 精灵渲染器")]
        public SpriteRenderer l2Renderer;

        [Tooltip("L1 精灵渲染器")]
        public SpriteRenderer l1Renderer;

        [Header("配置引用")]
        public TissueGameConfig config;

        // 扒拉次数计数
        private int pullCount = 0;

        // 划动计数（用于清理）
        private int swipeCount = 0;

        // 状态
        private TissuePileState currentState = TissuePileState.Idle;

        // 事件
        public delegate void PileAction(int currentCount, int maxCount);
        public event PileAction OnPileCountChanged;   // 堆积计数变化事件
        public event PileAction OnPileCleared;        // 纸巾清除事件

        /// <summary>
        /// 堆积纸巾状态
        /// </summary>
        public enum TissuePileState
        {
            Idle,       // 空闲
            Clearing    // 清除中
        }

        public TissuePileState CurrentState => currentState;
        public int PullCount => pullCount;
        public int SwipeCount => swipeCount;

        [Header("渲染设置")]
        [Tooltip("排序层级（确保在正确图层渲染）")]
        public int sortingLayerOrder = 0;

        private void Awake()
        {
            if (config == null)
            {
                config = FindObjectOfType<TissueGameConfig>();
            }

            InitializeRenderers();
        }

        /// <summary>
        /// 初始化精灵渲染器
        /// </summary>
        private void InitializeRenderers()
        {
            // 初始化精灵
            if (l2Renderer != null)
            {
                l2Renderer.sprite = pileL2;
                l2Renderer.enabled = false;  // 初始隐藏
                l2Renderer.sortingOrder = sortingLayerOrder;
            }

            if (l1Renderer != null)
            {
                l1Renderer.sprite = pileL1;
                l1Renderer.enabled = false;  // 初始隐藏
                l1Renderer.sortingOrder = sortingLayerOrder + 1;  // L1 在 L2 上层
            }
        }

        /// <summary>
        /// 设置 L2 和 L1 渲染器（用于运行时动态创建时设置引用）
        /// </summary>
        public void SetRenderers(SpriteRenderer l2, SpriteRenderer l1)
        {
            l2Renderer = l2;
            l1Renderer = l1;
            InitializeRenderers();
        }

        /// <summary>
        /// 增加扒拉次数（每次向下划动调用）
        /// </summary>
        public void AddPull()
        {
            if (currentState == TissuePileState.Clearing) return;
            if (pullCount >= 10) return;  // 达到上限不再增加

            pullCount++;
            UpdatePileDisplay();

            OnPileCountChanged?.Invoke(pullCount, 10);
            Debug.Log($"[TissuePileManager] 扒拉次数：{pullCount}/10");
        }

        /// <summary>
        /// 更新纸巾堆显示
        /// </summary>
        private void UpdatePileDisplay()
        {
            if (l2Renderer != null)
            {
                // 5 次后显示 L2
                l2Renderer.enabled = (pullCount >= 5);
            }

            if (l1Renderer != null)
            {
                // 10 次后显示 L1（叠加在 L2 上）
                l1Renderer.enabled = (pullCount >= 10);
            }

            Debug.Log($"[TissuePileManager] 显示更新：L2={(pullCount >= 5)} L1={(pullCount >= 10)}");
        }

        /// <summary>
        /// 处理横向划动（清理）
        /// 每 3 次划动清理一层
        /// </summary>
        public void HandleSwipe()
        {
            if (currentState == TissuePileState.Clearing) return;
            if (pullCount == 0) return;

            swipeCount++;
            Debug.Log($"[TissuePileManager] 划动计数：{swipeCount}/{config.swipesPerClear}");

            // 检查是否达到清除条件
            if (swipeCount >= config.swipesPerClear)
            {
                ClearOneLayer();
                swipeCount = 0;
            }
        }

        /// <summary>
        /// 清理一层纸巾
        /// 先清理 L1，再清理 L2
        /// </summary>
        private void ClearOneLayer()
        {
            if (pullCount == 0) return;

            currentState = TissuePileState.Clearing;

            // 根据当前层数决定清理哪一层
            if (pullCount >= 10)
            {
                // 先清理 L1
                StartCoroutine(ClearL1Coroutine());
            }
            else if (pullCount >= 5)
            {
                // 清理 L2
                StartCoroutine(ClearL2Coroutine());
            }
            else
            {
                // 少于 5 次，直接重置
                pullCount = 0;
                UpdatePileDisplay();
                currentState = TissuePileState.Idle;
            }
        }

        /// <summary>
        /// 清理 L1 动画协程
        /// </summary>
        private IEnumerator ClearL1Coroutine()
        {
            Debug.Log("[TissuePileManager] 清理 L1");

            // 播放 L1 消失动画
            float elapsed = 0f;
            float duration = 0.3f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                
                // 淡出效果
                Color c = l1Renderer.color;
                c.a = 1 - t;
                l1Renderer.color = c;

                yield return null;
            }

            l1Renderer.enabled = false;
            l1Renderer.color = Color.white;  // 恢复透明度

            // L1 清理后，扒拉次数减少到 5
            pullCount = 5;
            OnPileCleared?.Invoke(pullCount, 10);

            currentState = TissuePileState.Idle;
            Debug.Log($"[TissuePileManager] L1 清理完成，当前扒拉次数：{pullCount}");
        }

        /// <summary>
        /// 清理 L2 动画协程
        /// </summary>
        private IEnumerator ClearL2Coroutine()
        {
            Debug.Log("[TissuePileManager] 清理 L2");

            // 播放 L2 消失动画
            float elapsed = 0f;
            float duration = 0.3f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                
                // 淡出效果
                Color c = l2Renderer.color;
                c.a = 1 - t;
                l2Renderer.color = c;

                yield return null;
            }

            l2Renderer.enabled = false;
            l2Renderer.color = Color.white;  // 恢复透明度

            // L2 清理后，扒拉次数归零
            pullCount = 0;
            OnPileCleared?.Invoke(pullCount, 10);

            currentState = TissuePileState.Idle;
            Debug.Log($"[TissuePileManager] L2 清理完成，当前扒拉次数：{pullCount}");
        }

        /// <summary>
        /// 重置纸巾堆
        /// </summary>
        public void ResetPile()
        {
            StopAllCoroutines();
            pullCount = 0;
            swipeCount = 0;
            currentState = TissuePileState.Idle;

            // 隐藏所有纸巾堆
            if (l2Renderer != null)
            {
                l2Renderer.enabled = false;
                l2Renderer.color = Color.white;
            }

            if (l1Renderer != null)
            {
                l1Renderer.enabled = false;
                l1Renderer.color = Color.white;
            }

            Debug.Log("[TissuePileManager] 重置完成");
        }

        /// <summary>
        /// 检查是否可以清理（有堆积纸巾）
        /// </summary>
        public bool CanClear()
        {
            return pullCount > 0 && currentState != TissuePileState.Clearing;
        }

    }
}
