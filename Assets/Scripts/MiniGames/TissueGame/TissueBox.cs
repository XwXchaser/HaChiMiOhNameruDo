using UnityEngine;
using System.Collections;

namespace HaChiMiOhNameruDo.MiniGames.TissueGame
{
    /// <summary>
    /// 纸巾盒组件 - 包含纸巾架（静态）和纸巾筒（可动）
    /// 负责纸巾盒的状态管理、显示、装填逻辑
    /// 使用两个 SpriteRenderer：纸巾架（静态底座）和纸巾筒（上方，可替换纸卷）
    /// </summary>
    public class TissueBox : MonoBehaviour
    {
        [Header("组件引用 - 纸巾架（静态）")]
        [Tooltip("纸巾架精灵渲染器（静态底座）")]
        [SerializeField] private SpriteRenderer spriteRenderer;

        [Tooltip("纸巾架碰撞体")]
        [SerializeField] private Collider2D boxCollider;

        [Header("组件引用 - 纸巾筒（可动）")]
        [Tooltip("纸巾筒精灵渲染器（上方，可替换纸卷）")]
        [SerializeField] private SpriteRenderer rollSpriteRenderer;

        [Tooltip("纸巾筒碰撞体")]
        [SerializeField] private Collider2D rollCollider;

        [Header("美术素材引用 - 纸巾架（静态）")]
        [Tooltip("纸巾架默认状态（静态图片）")]
        public Sprite spriteIdle;

        [Header("美术素材引用 - 纸巾筒（可动）")]
        [Tooltip("纸巾筒默认状态（Idle）")]
        public Sprite spriteRollIdle;

        [Tooltip("纸巾筒抽取动画帧 1")]
        public Sprite spriteRollPull1;

        [Tooltip("纸巾筒抽取动画帧 2")]
        public Sprite spriteRollPull2;

        [Tooltip("纸巾筒耗尽/空纸卷状态")]
        public Sprite spriteRollEmpty;

        [Header("动画设置")]
        [Tooltip("抽取动画播放速度（秒/帧）")]
        public float pullAnimationSpeed = 0.3f;

        [Tooltip("纸巾耗尽时消失时长")]
        public float disappearDuration = 1f;

        // 状态
        private TissueBoxState currentState = TissueBoxState.Idle;

        [Header("组件引用 - 纸巾和纸巾堆")]
        [Tooltip("纸巾延伸组件")]
        [SerializeField] private TissuePaper tissuePaper;

        [Tooltip("纸巾堆管理组件")]
        [SerializeField] private TissuePileManager tissuePileManager;

        [Header("预制体引用")]
        [Tooltip("纸巾预制体（用于动态创建）")]
        public TissuePaper tissuePaperPrefab;

        [Tooltip("纸巾堆管理预制体（用于动态创建）")]
        public TissuePileManager tissuePileManagerPrefab;

        // 事件
        public delegate void BoxAction();
        public event BoxAction OnBoxEmpty;          // 纸巾耗尽事件
        public event BoxAction OnChamberCleared;    // 弹仓清空事件
        public event BoxAction OnBoxReloaded;       // 装填完成事件

        /// <summary>
        /// 纸巾筒状态
        /// </summary>
        public enum TissueBoxState
        {
            Idle,           // 空闲
            Pulling,        // 抽取中（播放动画）
            Empty,          // 耗尽
            ClearingChamber,// 清空弹仓中
            Reloading       // 装填中
        }

        public TissueBoxState CurrentState => currentState;
        public bool CanPull => currentState == TissueBoxState.Idle || currentState == TissueBoxState.Pulling;
        public bool CanCut => currentState == TissueBoxState.Idle;
        public bool CanClearChamber => currentState == TissueBoxState.Empty;
        public bool CanReload => currentState == TissueBoxState.ClearingChamber;

        private void Awake()
        {
            // 初始化纸巾架
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();
            if (boxCollider == null)
                boxCollider = GetComponent<Collider2D>();

            // 初始化纸巾筒
            if (rollSpriteRenderer == null)
                rollSpriteRenderer = GetComponentInChildren<SpriteRenderer>(true);
            if (rollCollider == null)
                rollCollider = GetComponentInChildren<Collider2D>(true);

            // 设置纸巾筒的渲染层级（确保在纸巾架上方）
            if (rollSpriteRenderer != null && spriteRenderer != null)
            {
                // 设置 sorting layer 和 order
                rollSpriteRenderer.sortingLayerName = spriteRenderer.sortingLayerName;
                rollSpriteRenderer.sortingOrder = spriteRenderer.sortingOrder + 1;
            }

            // 动态创建 TissuePaper GameObject（如果场景中没有找到）
            if (tissuePaper == null)
            {
                tissuePaper = FindObjectOfType<TissuePaper>();
                if (tissuePaper == null)
                {
                    Debug.Log("[TissueBox] 场景中没有找到 TissuePaper，动态创建...");
                    CreateTissuePaper();
                }
            }
        }

        /// <summary>
        /// 动态创建 TissuePaper GameObject
        /// </summary>
        private void CreateTissuePaper()
        {
            if (tissuePaperPrefab != null)
            {
                // 从 Prefab 实例化，保留所有美术素材引用
                TissuePaper paper = Instantiate(tissuePaperPrefab, transform);
                paper.transform.localPosition = Vector3.zero;
                paper.gameObject.name = "TissuePaper";
                tissuePaper = paper;
                Debug.Log("[TissueBox] 已从 Prefab 实例化 TissuePaper");
            }
            else
            {
                // 如果没有 Prefab，尝试从场景查找
                tissuePaper = FindObjectOfType<TissuePaper>();
                if (tissuePaper != null)
                {
                    Debug.Log("[TissueBox] 从场景中找到 TissuePaper");
                }
                else
                {
                    Debug.LogError("[TissueBox] 没有配置 tissuePaperPrefab 且场景中没有找到 TissuePaper！");
                }
            }
        }

        /// <summary>
        /// 设置纸巾和纸巾堆引用（由 TissueGameManager 在运行时调用）
        /// </summary>
        public void SetupReferences(TissuePaper paper, TissuePileManager pileManager)
        {
            tissuePaper = paper;
            tissuePileManager = pileManager;
            
            // 如果 tissuePileManager 为 null，动态创建
            if (tissuePileManager == null)
            {
                tissuePileManager = FindObjectOfType<TissuePileManager>();
                if (tissuePileManager == null)
                {
                    Debug.Log("[TissueBox] 场景中没有找到 TissuePileManager，动态创建...");
                    CreateTissuePileManager();
                }
            }
            
            Debug.Log($"[TissueBox] SetupReferences: tissuePaper={(tissuePaper != null ? "assigned" : "null")}, tissuePileManager={(tissuePileManager != null ? "assigned" : "null")}");
        }

        /// <summary>
        /// 动态创建 TissuePileManager GameObject
        /// </summary>
        private void CreateTissuePileManager()
        {
            if (tissuePileManagerPrefab != null)
            {
                // 从 Prefab 实例化，保留所有美术素材引用
                TissuePileManager pileManager = Instantiate(tissuePileManagerPrefab, transform);
                pileManager.transform.localPosition = new Vector3(0f, -2f, 0f);
                pileManager.gameObject.name = "TissuePileManager";
                tissuePileManager = pileManager;
                Debug.Log("[TissueBox] 已从 Prefab 实例化 TissuePileManager");
            }
            else
            {
                // 如果没有 Prefab，尝试从场景查找
                tissuePileManager = FindObjectOfType<TissuePileManager>();
                if (tissuePileManager != null)
                {
                    Debug.Log("[TissueBox] 从场景中找到 TissuePileManager");
                }
                else
                {
                    Debug.LogError("[TissueBox] 没有配置 tissuePileManagerPrefab 且场景中没有找到 TissuePileManager！");
                }
            }
        }

        private void Start()
        {
            // 设置纸巾架初始精灵
            if (spriteRenderer != null)
            {
                if (spriteIdle != null)
                {
                    spriteRenderer.sprite = spriteIdle;
                }
                // 确保纸巾架始终可见（即使没有配置精灵）
                spriteRenderer.enabled = true;
                Debug.Log($"[TissueBox] Start: spriteRenderer.enabled = {spriteRenderer.enabled}, sprite = {(spriteIdle != null ? spriteIdle.name : "null")}");
            }
            else
            {
                Debug.LogError("[TissueBox] Start: spriteRenderer 为 null！");
            }

            // 设置纸巾筒初始精灵
            if (rollSpriteRenderer != null)
            {
                if (spriteRollIdle != null)
                {
                    rollSpriteRenderer.sprite = spriteRollIdle;
                }
                // 确保纸巾筒始终可见（即使没有配置精灵）
                rollSpriteRenderer.enabled = true;
                Debug.Log($"[TissueBox] Start: rollSpriteRenderer.enabled = {rollSpriteRenderer.enabled}, sprite = {(spriteRollIdle != null ? spriteRollIdle.name : "null")}");
            }
            else
            {
                // 如果没有配置 rollSpriteRenderer，尝试从子对象获取
                rollSpriteRenderer = GetComponentInChildren<SpriteRenderer>(true);
                if (rollSpriteRenderer != null)
                {
                    if (spriteRollIdle != null)
                    {
                        rollSpriteRenderer.sprite = spriteRollIdle;
                    }
                    rollSpriteRenderer.enabled = true;
                    Debug.Log($"[TissueBox] Start: 从子对象获取 rollSpriteRenderer, enabled = {rollSpriteRenderer.enabled}");
                }
                else
                {
                    Debug.LogError("[TissueBox] Start: 无法获取 rollSpriteRenderer！");
                }
            }
        }

        /// <summary>
        /// 重置纸巾盒到初始状态
        /// </summary>
        public void ResetBox()
        {
            StopAllCoroutines();
            currentState = TissueBoxState.Idle;
            Show();
            if (spriteRenderer != null && spriteIdle != null)
                spriteRenderer.sprite = spriteIdle;
            if (rollSpriteRenderer != null && spriteRollIdle != null)
                rollSpriteRenderer.sprite = spriteRollIdle;
            Debug.Log("[TissueBox] 重置完成");
        }

        /// <summary>
        /// 显示纸巾盒（纸巾架和纸巾筒）
        /// </summary>
        public void Show()
        {
            if (spriteRenderer != null)
                spriteRenderer.enabled = true;
            if (boxCollider != null)
                boxCollider.enabled = true;
            if (rollSpriteRenderer != null)
                rollSpriteRenderer.enabled = true;
            if (rollCollider != null)
                rollCollider.enabled = true;
        }

        /// <summary>
        /// 隐藏纸巾盒（纸巾架和纸巾筒）
        /// </summary>
        public void Hide()
        {
            if (spriteRenderer != null)
                spriteRenderer.enabled = false;
            if (boxCollider != null)
                boxCollider.enabled = false;
            if (rollSpriteRenderer != null)
                rollSpriteRenderer.enabled = false;
            if (rollCollider != null)
                rollCollider.enabled = false;
        }

        /// <summary>
        /// 处理向下划动（抽取纸巾）
        /// </summary>
        public void HandlePull()
        {
            if (!CanPull)
            {
                Debug.LogWarning($"[TissueBox] 无法抽取，当前状态：{currentState}");
                return;
            }

            // 确保 GameObject 和脚本都被激活
            if (!gameObject.activeSelf)
            {
                Debug.Log("[TissueBox] 激活 GameObject");
                gameObject.SetActive(true);
            }

            if (!enabled)
            {
                Debug.Log("[TissueBox] 激活脚本");
                enabled = true;
            }

            currentState = TissueBoxState.Pulling;
            Debug.Log("[TissueBox] 抽取纸巾");

            // 延伸纸巾
            if (tissuePaper != null)
            {
                tissuePaper.Extend();
            }
            else
            {
                Debug.LogWarning("[TissueBox] tissuePaper 引用未分配");
            }

            // 增加扒拉次数
            if (tissuePileManager != null)
            {
                tissuePileManager.AddPull();
            }
            else
            {
                Debug.LogWarning("[TissueBox] tissuePileManager 引用未分配");
            }

            // 播放抽取动画（只有纸巾筒动画，纸巾架保持静态）
            StartCoroutine(PullAnimationCoroutine());

            // 触发震动反馈
            if (Managers.HapticManager.Instance != null)
                Managers.HapticManager.Instance.PlayDefaultHaptic();
        }

        /// <summary>
        /// 抽取动画协程（只有纸巾筒播放 spriteRollPull1 和 spriteRollPull2 循环）
        /// 纸巾架保持静态 spriteIdle
        /// </summary>
        private IEnumerator PullAnimationCoroutine()
        {
            // 确保纸巾架始终可见
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = true;
                if (spriteIdle != null)
                    spriteRenderer.sprite = spriteIdle;
            }

            if (rollSpriteRenderer == null) yield break;
            if (spriteRollPull1 == null || spriteRollPull2 == null) yield break;

            // 确保纸巾筒可见
            rollSpriteRenderer.enabled = true;

            // 循环播放 spriteRollPull1 和 spriteRollPull2
            bool showFrame1 = true;
            float elapsed = 0f;

            while (currentState == TissueBoxState.Pulling)
            {
                elapsed += Time.deltaTime;
                if (elapsed >= pullAnimationSpeed)
                {
                    elapsed = 0f;
                    showFrame1 = !showFrame1;

                    // 只更新纸巾筒
                    rollSpriteRenderer.sprite = showFrame1 ? spriteRollPull1 : spriteRollPull2;
                }
                yield return null;
            }

            // 恢复默认状态
            if (rollSpriteRenderer != null && spriteRollIdle != null)
                rollSpriteRenderer.sprite = spriteRollIdle;
        }

        /// <summary>
        /// 停止抽取动画
        /// </summary>
        public void StopPullAnimation()
        {
            if (currentState == TissueBoxState.Pulling)
            {
                currentState = TissueBoxState.Idle;
                StopAllCoroutines();
                if (spriteRenderer != null && spriteIdle != null)
                    spriteRenderer.sprite = spriteIdle;
                if (rollSpriteRenderer != null && spriteRollIdle != null)
                    rollSpriteRenderer.sprite = spriteRollIdle;
            }
        }

        /// <summary>
        /// 处理横向划动（切断纸巾）
        /// </summary>
        public void HandleCut()
        {
            if (!CanCut) return;

            Debug.Log("[TissueBox] 切断纸巾");

            // 触发强烈震动
            if (Managers.HapticManager.Instance != null)
                Managers.HapticManager.Instance.PlayStrongHaptic();

            // 播放切断音效
            if (Managers.AudioManager.Instance != null)
                Managers.AudioManager.Instance.PlayCutSound();
        }

        /// <summary>
        /// 设置为耗尽状态（显示空纸卷）
        /// </summary>
        public void SetEmpty()
        {
            currentState = TissueBoxState.Empty;
            OnBoxEmpty?.Invoke();
            Debug.Log("[TissueBox] 纸巾耗尽，显示空纸卷");

            // 切换到空纸卷精灵
            if (rollSpriteRenderer != null && spriteRollEmpty != null)
            {
                rollSpriteRenderer.sprite = spriteRollEmpty;
            }
        }

        /// <summary>
        /// 消失动画协程（纸巾架慢慢消失，纸巾筒保持空卷状态）
        /// </summary>
        private IEnumerator DisappearCoroutine()
        {
            float elapsed = 0f;

            while (elapsed < disappearDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / disappearDuration);

                // 纸巾架淡出效果
                if (spriteRenderer != null)
                {
                    Color c = spriteRenderer.color;
                    c.a = 1 - t;
                    spriteRenderer.color = c;
                }

                yield return null;
            }

            // 完全隐藏纸巾架，纸巾筒保持空卷状态
            if (spriteRenderer != null)
                spriteRenderer.enabled = false;
            if (boxCollider != null)
                boxCollider.enabled = false;
            if (spriteRenderer != null)
                spriteRenderer.color = Color.white;  // 恢复透明度

            Debug.Log("[TissueBox] 纸巾架已消失，保留空纸卷");
        }

        /// <summary>
        /// 处理向右划动（清空弹仓）
        /// </summary>
        public void HandleClearChamber()
        {
            if (!CanClearChamber) return;

            currentState = TissueBoxState.ClearingChamber;
            Debug.Log("[TissueBox] 清空弹仓");

            StartCoroutine(ClearChamberCoroutine());
        }

        /// <summary>
        /// 清空弹仓协程
        /// </summary>
        private IEnumerator ClearChamberCoroutine()
        {
            // 播放清空动画（可以添加视觉效果）
            float elapsed = 0f;
            float duration = 0.3f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                yield return null;
            }

            OnChamberCleared?.Invoke();
            Debug.Log("[TissueBox] 弹仓已清空，请从屏幕右侧向中心划动装填");
        }

        /// <summary>
        /// 处理从右向左划动（装填新纸卷）
        /// </summary>
        public void HandleReload()
        {
            if (!CanReload) return;

            currentState = TissueBoxState.Reloading;
            Debug.Log("[TissueBox] 装填新纸卷");

            StartCoroutine(ReloadCoroutine());
        }

        /// <summary>
        /// 装填协程（纸巾架和纸巾筒同时淡入）
        /// </summary>
        private IEnumerator ReloadCoroutine()
        {
            // 播放装填动画（淡入）
            float elapsed = 0f;
            float duration = 0.5f;

            // 确保隐藏
            Hide();

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);

                // 淡入效果 - 纸巾架
                if (spriteRenderer != null)
                {
                    spriteRenderer.enabled = true;
                    Color c = spriteRenderer.color;
                    c.a = t;
                    spriteRenderer.color = c;
                }

                // 淡入效果 - 纸巾筒
                if (rollSpriteRenderer != null)
                {
                    rollSpriteRenderer.enabled = true;
                    Color rollC = rollSpriteRenderer.color;
                    rollC.a = t;
                    rollSpriteRenderer.color = rollC;
                }

                yield return null;
            }

            // 恢复默认状态
            if (spriteRenderer != null)
            {
                spriteRenderer.color = Color.white;
                if (spriteIdle != null)
                    spriteRenderer.sprite = spriteIdle;
            }

            if (rollSpriteRenderer != null)
            {
                rollSpriteRenderer.color = Color.white;
                if (spriteRollIdle != null)
                    rollSpriteRenderer.sprite = spriteRollIdle;
            }

            // 装填完成
            currentState = TissueBoxState.Idle;
            OnBoxReloaded?.Invoke();
            Debug.Log("[TissueBox] 装填完成，可以继续游戏");
        }

        /// <summary>
        /// 检查是否可以装填（纸巾未耗尽前不能触发）
        /// </summary>
        public bool CanReloadCheck()
        {
            return currentState == TissueBoxState.ClearingChamber;
        }
    }
}
