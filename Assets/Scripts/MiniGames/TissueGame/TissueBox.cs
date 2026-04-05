using UnityEngine;
using System.Collections;

namespace HaChiMiOhNameruDo.MiniGames.TissueGame
{
    /// <summary>
    /// 纸巾筒组件
    /// 负责纸巾筒的状态管理、显示、装填逻辑
    /// 支持 Holder（架子）和 Roll（纸卷）分离的美术结构
    /// </summary>
    public class TissueBox : MonoBehaviour
    {
        [Header("组件引用")]
        [Tooltip("纸巾筒架子（始终显示）")]
        [SerializeField] private SpriteRenderer holderRenderer;

        [Tooltip("纸巾卷精灵渲染器")]
        [SerializeField] private SpriteRenderer rollRenderer;

        [Tooltip("纸巾筒碰撞体")]
        [SerializeField] private Collider2D boxCollider;

        [Header("美术素材引用")]
        [Tooltip("纸巾卷默认状态（Idle）")]
        public Sprite rollIdle;

        [Tooltip("纸巾卷抽取动画帧 1")]
        public Sprite rollPull1;

        [Tooltip("纸巾卷抽取动画帧 2")]
        public Sprite rollPull2;

        [Header("动画设置")]
        [Tooltip("抽取动画播放速度（秒/帧）")]
        public float pullAnimationSpeed = 0.1f;

        [Tooltip("纸巾耗尽时纸卷消失时长")]
        public float disappearDuration = 1f;

        // 状态
        private TissueBoxState currentState = TissueBoxState.Idle;

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
            // 初始化
            if (holderRenderer == null)
                holderRenderer = GetComponent<SpriteRenderer>();
            if (boxCollider == null)
                boxCollider = GetComponent<Collider2D>();
        }

        private void Start()
        {
            // 设置初始精灵
            if (rollRenderer != null && rollIdle != null)
            {
                rollRenderer.sprite = rollIdle;
                rollRenderer.enabled = true;
            }
        }

        /// <summary>
        /// 重置纸巾筒到初始状态
        /// </summary>
        public void ResetBox()
        {
            StopAllCoroutines();
            currentState = TissueBoxState.Idle;
            ShowRoll();
            Debug.Log("[TissueBox] 重置完成");
        }

        /// <summary>
        /// 显示纸卷
        /// </summary>
        public void ShowRoll()
        {
            if (rollRenderer != null)
            {
                rollRenderer.enabled = true;
                if (rollIdle != null)
                    rollRenderer.sprite = rollIdle;
            }
        }

        /// <summary>
        /// 隐藏纸卷
        /// </summary>
        public void HideRoll()
        {
            if (rollRenderer != null)
                rollRenderer.enabled = false;
        }

        /// <summary>
        /// 显示纸巾筒（包括架子和纸卷）
        /// </summary>
        public void Show()
        {
            if (holderRenderer != null)
                holderRenderer.enabled = true;
            ShowRoll();
            if (boxCollider != null)
                boxCollider.enabled = true;
        }

        /// <summary>
        /// 隐藏纸巾筒
        /// </summary>
        public void Hide()
        {
            if (holderRenderer != null)
                holderRenderer.enabled = false;
            HideRoll();
            if (boxCollider != null)
                boxCollider.enabled = false;
        }

        /// <summary>
        /// 处理向下划动（抽取纸巾）
        /// </summary>
        public void HandlePull()
        {
            if (!CanPull) return;

            currentState = TissueBoxState.Pulling;
            Debug.Log("[TissueBox] 抽取纸巾");

            // 播放抽取动画
            StartCoroutine(PullAnimationCoroutine());

            // 触发震动反馈
            if (Managers.HapticManager.Instance != null)
                Managers.HapticManager.Instance.PlayDefaultHaptic();
        }

        /// <summary>
        /// 抽取动画协程（Roll_1 和 Roll_2 循环）
        /// </summary>
        private IEnumerator PullAnimationCoroutine()
        {
            if (rollRenderer == null) yield break;

            // 循环播放 Roll_1 和 Roll_2
            bool showFrame1 = true;
            float elapsed = 0f;

            while (currentState == TissueBoxState.Pulling)
            {
                elapsed += Time.deltaTime;
                if (elapsed >= pullAnimationSpeed)
                {
                    elapsed = 0f;
                    showFrame1 = !showFrame1;
                    rollRenderer.sprite = showFrame1 ? rollPull1 : rollPull2;
                }
                yield return null;
            }

            // 恢复默认状态
            if (rollIdle != null)
                rollRenderer.sprite = rollIdle;
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
                if (rollRenderer != null && rollIdle != null)
                    rollRenderer.sprite = rollIdle;
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
        /// 设置为耗尽状态
        /// </summary>
        public void SetEmpty()
        {
            currentState = TissueBoxState.Empty;
            OnBoxEmpty?.Invoke();
            Debug.Log("[TissueBox] 纸巾耗尽，纸卷将慢慢消失");

            // 纸卷慢慢消失
            StartCoroutine(DisappearCoroutine());
        }

        /// <summary>
        /// 纸卷消失动画协程
        /// </summary>
        private IEnumerator DisappearCoroutine()
        {
            float elapsed = 0f;

            while (elapsed < disappearDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / disappearDuration);

                // 淡出效果
                if (rollRenderer != null)
                {
                    Color c = rollRenderer.color;
                    c.a = 1 - t;
                    rollRenderer.color = c;
                }

                yield return null;
            }

            // 完全隐藏纸卷，只显示 Holder
            HideRoll();
            if (rollRenderer != null)
                rollRenderer.color = Color.white;  // 恢复透明度

            Debug.Log("[TissueBox] 纸卷已消失，只显示 Holder");
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
        /// 装填协程
        /// </summary>
        private IEnumerator ReloadCoroutine()
        {
            // 播放装填动画（纸卷淡入）
            float elapsed = 0f;
            float duration = 0.5f;

            // 确保纸卷隐藏
            HideRoll();

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);

                // 淡入效果
                if (rollRenderer != null)
                {
                    rollRenderer.enabled = true;
                    Color c = rollRenderer.color;
                    c.a = t;
                    rollRenderer.color = c;
                }

                yield return null;
            }

            // 恢复默认状态
            if (rollRenderer != null)
            {
                rollRenderer.color = Color.white;
                if (rollIdle != null)
                    rollRenderer.sprite = rollIdle;
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
