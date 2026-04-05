using UnityEngine;
using System.Collections;

namespace HaChiMiOhNameruDo.MiniGames.TissueGame
{
    /// <summary>
    /// 纸巾组件
    /// 负责纸巾的状态管理、延伸（Short/Long 状态切换）、切断效果
    /// </summary>
    public class TissuePaper : MonoBehaviour
    {
        [Header("组件引用")]
        [SerializeField] private SpriteRenderer spriteRenderer;

        [Header("美术素材引用")]
        [Tooltip("短纸巾精灵（初次被扒拉或切断后）")]
        public Sprite tissueShort;

        [Tooltip("长纸巾精灵（一直扒拉不切断）")]
        public Sprite tissueLong;

        [Header("延伸设置")]
        [Tooltip("切换到长纸巾所需的扒拉次数")]
        public int longThreshold = 5;

        [Header("渲染设置")]
        [Tooltip("排序层级（确保在正确图层渲染）")]
        public int sortingLayerOrder = 0;

        // 状态
        private TissuePaperState currentState = TissuePaperState.Connected;

        // 扒拉次数（用于决定 Short/Long 状态）
        private int pullCount = 0;

        // 事件
        public delegate void PaperAction(int length);
        public event PaperAction OnPaperExtended;     // 纸巾延伸事件
        public event PaperAction OnPaperCut;          // 纸巾切断事件
        public event System.Action OnPaperRetracted;  // 纸巾收回事件

        /// <summary>
        /// 纸巾状态
        /// </summary>
        public enum TissuePaperState
        {
            Connected,      // 连接中（延伸）
            Cut,            // 已切断
            Retracting,     // 收回中
            Retracted       // 已收回
        }

        public TissuePaperState CurrentState => currentState;
        public int PullCount => pullCount;
        public bool IsLong => pullCount >= longThreshold;
        public bool IsCut => currentState == TissuePaperState.Cut;

        private void Awake()
        {
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();
            
            if (spriteRenderer != null)
            {
                spriteRenderer.sortingOrder = sortingLayerOrder;
            }
        }

        /// <summary>
        /// 设置 SpriteRenderer（用于运行时动态创建时设置引用）
        /// </summary>
        public void SetSpriteRenderer(SpriteRenderer renderer)
        {
            spriteRenderer = renderer;
            if (spriteRenderer != null)
            {
                spriteRenderer.sortingOrder = sortingLayerOrder;
            }
        }

        private void Start()
        {
            // 初始显示纸巾（即使扒拉次数为 0 也显示）
            Show();
            UpdateTissueSprite();
        }

        /// <summary>
        /// 重置纸巾
        /// </summary>
        public void ResetPaper()
        {
            StopAllCoroutines();
            currentState = TissuePaperState.Connected;
            pullCount = 0;
            UpdateTissueSprite();
            Show();
            Debug.Log("[TissuePaper] 重置完成");
        }

        /// <summary>
        /// 显示纸巾
        /// </summary>
        public void Show()
        {
            if (spriteRenderer != null)
                spriteRenderer.enabled = true;
        }

        /// <summary>
        /// 隐藏纸巾
        /// </summary>
        public void Hide()
        {
            if (spriteRenderer != null)
                spriteRenderer.enabled = false;
        }

        /// <summary>
        /// 延伸纸巾（每次扒拉调用）
        /// </summary>
        public void Extend()
        {
            if (currentState != TissuePaperState.Connected) return;

            pullCount++;
            UpdateTissueSprite();

            OnPaperExtended?.Invoke(pullCount);
            Debug.Log($"[TissuePaper] 延伸，当前扒拉次数：{pullCount}, 状态：{(IsLong ? "Long" : "Short")}");
        }

        /// <summary>
        /// 更新纸巾精灵（Short/Long 切换）
        /// </summary>
        private void UpdateTissueSprite()
        {
            if (spriteRenderer == null) return;

            // 确保精灵已设置
            if (pullCount >= longThreshold && tissueLong != null)
            {
                // 长纸巾状态
                spriteRenderer.sprite = tissueLong;
            }
            else if (pullCount > 0 && tissueShort != null)
            {
                // 短纸巾状态（初次扒拉后）
                spriteRenderer.sprite = tissueShort;
            }
            else if (tissueShort != null)
            {
                // 初始状态显示短纸巾（或者可以设置一个默认精灵）
                spriteRenderer.sprite = tissueShort;
            }
            
            Debug.Log($"[TissuePaper] 更新精灵：pullCount={pullCount}, sprite={(spriteRenderer.sprite != null ? spriteRenderer.sprite.name : "null")}");
        }

        /// <summary>
        /// 切断纸巾
        /// </summary>
        public void Cut()
        {
            if (currentState != TissuePaperState.Connected) return;
            if (pullCount == 0) return;

            currentState = TissuePaperState.Cut;
            OnPaperCut?.Invoke(pullCount);
            Debug.Log($"[TissuePaper] 切断，扒拉次数：{pullCount}");

            // 开始收回协程
            StartCoroutine(RetractCoroutine());
        }

        /// <summary>
        /// 收回纸巾协程
        /// </summary>
        private IEnumerator RetractCoroutine()
        {
            currentState = TissuePaperState.Retracting;

            // 播放收回动画（淡出效果）
            float elapsed = 0f;
            float duration = 0.3f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);

                // 淡出效果
                Color c = spriteRenderer.color;
                c.a = 1 - t;
                spriteRenderer.color = c;

                yield return null;
            }

            spriteRenderer.color = Color.white;  // 恢复透明度
            pullCount = 0;
            UpdateTissueSprite();

            currentState = TissuePaperState.Retracted;
            OnPaperRetracted?.Invoke();
            Debug.Log("[TissuePaper] 收回完成");
        }

        /// <summary>
        /// 设置长纸巾阈值
        /// </summary>
        public void SetLongThreshold(int threshold)
        {
            longThreshold = threshold;
        }

        /// <summary>
        /// 检查是否可以切断
        /// </summary>
        public bool CanCut()
        {
            return currentState == TissuePaperState.Connected && pullCount > 0;
        }
    }
}
