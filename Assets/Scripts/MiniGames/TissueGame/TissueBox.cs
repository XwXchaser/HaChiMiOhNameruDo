using UnityEngine;
using System.Collections;

namespace HaChiMiOhNameruDo.MiniGames.TissueGame
{
    /// <summary>
    /// 纸巾筒组件 - 简化版
    /// 负责纸巾筒的状态管理、显示、装填逻辑
    /// 使用单个 SpriteRenderer，通过 Sprite 数组配置动画
    /// </summary>
    public class TissueBox : MonoBehaviour
    {
        [Header("组件引用")]
        [Tooltip("纸巾筒精灵渲染器")]
        [SerializeField] private SpriteRenderer spriteRenderer;

        [Tooltip("纸巾筒碰撞体")]
        [SerializeField] private Collider2D boxCollider;

        [Header("美术素材引用")]
        [Tooltip("纸巾筒默认状态（Idle）")]
        public Sprite spriteIdle;

        [Tooltip("纸巾筒抽取动画帧 1")]
        public Sprite spritePull1;

        [Tooltip("纸巾筒抽取动画帧 2")]
        public Sprite spritePull2;

        [Header("动画设置")]
        [Tooltip("抽取动画播放速度（秒/帧）")]
        public float pullAnimationSpeed = 0.1f;

        [Tooltip("纸巾耗尽时消失时长")]
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
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();
            if (boxCollider == null)
                boxCollider = GetComponent<Collider2D>();
        }

        private void Start()
        {
            // 设置初始精灵
            if (spriteRenderer != null && spriteIdle != null)
            {
                spriteRenderer.sprite = spriteIdle;
                spriteRenderer.enabled = true;
            }
        }

        /// <summary>
        /// 重置纸巾筒到初始状态
        /// </summary>
        public void ResetBox()
        {
            StopAllCoroutines();
            currentState = TissueBoxState.Idle;
            Show();
            if (spriteRenderer != null && spriteIdle != null)
                spriteRenderer.sprite = spriteIdle;
            Debug.Log("[TissueBox] 重置完成");
        }

        /// <summary>
        /// 显示纸巾筒
        /// </summary>
        public void Show()
        {
            if (spriteRenderer != null)
                spriteRenderer.enabled = true;
            if (boxCollider != null)
                boxCollider.enabled = true;
        }

        /// <summary>
        /// 隐藏纸巾筒
        /// </summary>
        public void Hide()
        {
            if (spriteRenderer != null)
                spriteRenderer.enabled = false;
            if (boxCollider != null)
                boxCollider.enabled = false;
        }

        /// <summary>
        /// 处理向下划动（抽取纸巾）
        /// </summary>
        public void HandlePull()
        {
            if (!CanPull) return;

            // 确保 GameObject 激活
            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }

            currentState = TissueBoxState.Pulling;
            Debug.Log("[TissueBox] 抽取纸巾");

            // 播放抽取动画
            StartCoroutine(PullAnimationCoroutine());

            // 触发震动反馈
            if (Managers.HapticManager.Instance != null)
                Managers.HapticManager.Instance.PlayDefaultHaptic();
        }

        /// <summary>
        /// 抽取动画协程（spritePull1 和 spritePull2 循环）
        /// </summary>
        private IEnumerator PullAnimationCoroutine()
        {
            if (spriteRenderer == null) yield break;
            if (spritePull1 == null || spritePull2 == null) yield break;

            // 循环播放 spritePull1 和 spritePull2
            bool showFrame1 = true;
            float elapsed = 0f;

            while (currentState == TissueBoxState.Pulling)
            {
                elapsed += Time.deltaTime;
                if (elapsed >= pullAnimationSpeed)
                {
                    elapsed = 0f;
                    showFrame1 = !showFrame1;
                    spriteRenderer.sprite = showFrame1 ? spritePull1 : spritePull2;
                }
                yield return null;
            }

            // 恢复默认状态
            if (spriteIdle != null)
                spriteRenderer.sprite = spriteIdle;
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
            Debug.Log("[TissueBox] 纸巾耗尽，将慢慢消失");

            // 慢慢消失
            StartCoroutine(DisappearCoroutine());
        }

        /// <summary>
        /// 消失动画协程
        /// </summary>
        private IEnumerator DisappearCoroutine()
        {
            float elapsed = 0f;

            while (elapsed < disappearDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / disappearDuration);

                // 淡出效果
                if (spriteRenderer != null)
                {
                    Color c = spriteRenderer.color;
                    c.a = 1 - t;
                    spriteRenderer.color = c;
                }

                yield return null;
            }

            // 完全隐藏
            Hide();
            if (spriteRenderer != null)
                spriteRenderer.color = Color.white;  // 恢复透明度

            Debug.Log("[TissueBox] 已消失");
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
            // 播放装填动画（淡入）
            float elapsed = 0f;
            float duration = 0.5f;

            // 确保隐藏
            Hide();

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);

                // 淡入效果
                if (spriteRenderer != null)
                {
                    spriteRenderer.enabled = true;
                    Color c = spriteRenderer.color;
                    c.a = t;
                    spriteRenderer.color = c;
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
