using UnityEngine;

namespace HaChiMiOhNameruDo.Gameplay
{
    /// <summary>
    /// 猫咪眨眼控制器（四态状态机）
    /// 状态流转：Open → Closing → Closed → Opening → Open
    /// 时序：
    ///   - Open 状态：等待 3.5~5.5 秒随机时间
    ///   - Closing 状态：60ms（眼睛闭合过程）
    ///   - Closed 状态：80ms（眼睛保持闭合）
    ///   - Opening 状态：60ms（眼睛睁开过程）
    /// 整个眨一次眼约 200ms
    /// </summary>
    public class CatBlinkController : MonoBehaviour
    {
        [Header("SpriteRenderer 组件")]
        [SerializeField] private SpriteRenderer spriteRenderer;

        [Header("眨眼间隔参数")]
        [Tooltip("最小眨眼间隔（秒），默认 3.5 秒")]
        [Range(2f, 6f)]
        [SerializeField] private float minBlinkInterval = 3.5f;

        [Tooltip("最大眨眼间隔（秒），默认 5.5 秒")]
        [Range(2f, 8f)]
        [SerializeField] private float maxBlinkInterval = 5.5f;

        [Header("眨眼状态时长（毫秒）")]
        [Tooltip("Closing 状态持续时间（毫秒），默认 60ms")]
        [Range(30f, 100f)]
        [SerializeField] private float closingDurationMs = 60f;

        [Tooltip("Closed 状态持续时间（毫秒），默认 80ms")]
        [Range(50f, 150f)]
        [SerializeField] private float closedDurationMs = 80f;

        [Tooltip("Opening 状态持续时间（毫秒），默认 60ms")]
        [Range(30f, 100f)]
        [SerializeField] private float openingDurationMs = 60f;

        [Header("眼睛精灵")]
        [Tooltip("睁眼状态精灵")]
        [SerializeField] private Sprite openSprite;

        [Tooltip("闭眼过程/睁眼过程精灵（半闭眼）")]
        [SerializeField] private Sprite halfSprite;

        [Tooltip("完全闭眼状态精灵")]
        [SerializeField] private Sprite closedSprite;

        // 眨眼状态枚举
        private enum BlinkState
        {
            Open,       // 睁眼状态
            Closing,    // 闭眼过程
            Closed,     // 完全闭眼
            Opening     // 睁眼过程
        }

        private BlinkState currentState;
        private float stateTimer;
        private float nextBlinkTime;

        private void Awake()
        {
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            // 初始化为 Open 状态
            currentState = BlinkState.Open;
            ScheduleNextBlink();

            // 设置初始精灵为睁眼
            if (spriteRenderer != null && openSprite != null)
            {
                spriteRenderer.sprite = openSprite;
            }
        }

        private void Update()
        {
            switch (currentState)
            {
                case BlinkState.Open:
                    UpdateOpenState();
                    break;
                case BlinkState.Closing:
                    UpdateClosingState();
                    break;
                case BlinkState.Closed:
                    UpdateClosedState();
                    break;
                case BlinkState.Opening:
                    UpdateOpeningState();
                    break;
            }
        }

        /// <summary>
        /// 更新 Open 状态（等待下次眨眼）
        /// </summary>
        private void UpdateOpenState()
        {
            if (Time.time >= nextBlinkTime)
            {
                // 开始闭眼过程
                currentState = BlinkState.Closing;
                stateTimer = 0f;

                // 立即切换到半闭眼精灵
                if (spriteRenderer != null && halfSprite != null)
                {
                    spriteRenderer.sprite = halfSprite;
                }
            }
        }

        /// <summary>
        /// 更新 Closing 状态（闭眼过程，60ms）
        /// </summary>
        private void UpdateClosingState()
        {
            stateTimer += Time.deltaTime;
            float durationSec = closingDurationMs / 1000f;

            if (stateTimer >= durationSec)
            {
                // 闭眼过程完成，进入 Closed 状态
                currentState = BlinkState.Closed;
                stateTimer = 0f;

                // 切换到完全闭眼精灵
                if (spriteRenderer != null && closedSprite != null)
                {
                    spriteRenderer.sprite = closedSprite;
                }
            }
        }

        /// <summary>
        /// 更新 Closed 状态（完全闭眼，80ms）
        /// </summary>
        private void UpdateClosedState()
        {
            stateTimer += Time.deltaTime;
            float durationSec = closedDurationMs / 1000f;

            if (stateTimer >= durationSec)
            {
                // 闭眼保持完成，进入 Opening 状态
                currentState = BlinkState.Opening;
                stateTimer = 0f;

                // 切换回半闭眼精灵
                if (spriteRenderer != null && halfSprite != null)
                {
                    spriteRenderer.sprite = halfSprite;
                }
            }
        }

        /// <summary>
        /// 更新 Opening 状态（睁眼过程，60ms）
        /// </summary>
        private void UpdateOpeningState()
        {
            stateTimer += Time.deltaTime;
            float durationSec = openingDurationMs / 1000f;

            if (stateTimer >= durationSec)
            {
                // 睁眼过程完成，回到 Open 状态
                currentState = BlinkState.Open;

                // 切换回睁眼精灵
                if (spriteRenderer != null && openSprite != null)
                {
                    spriteRenderer.sprite = openSprite;
                }

                // 安排下次眨眼
                ScheduleNextBlink();
            }
        }

        /// <summary>
        /// 安排下次眨眼
        /// </summary>
        private void ScheduleNextBlink()
        {
            nextBlinkTime = Time.time + Random.Range(minBlinkInterval, maxBlinkInterval);
        }

        /// <summary>
        /// 立即触发眨眼
        /// </summary>
        public void TriggerBlink()
        {
            if (currentState == BlinkState.Open)
            {
                currentState = BlinkState.Closing;
                stateTimer = 0f;

                if (spriteRenderer != null && halfSprite != null)
                {
                    spriteRenderer.sprite = halfSprite;
                }
            }
        }

        /// <summary>
        /// 设置眨眼间隔范围
        /// </summary>
        public void SetBlinkInterval(float min, float max)
        {
            minBlinkInterval = min;
            maxBlinkInterval = max;
            ScheduleNextBlink();
        }

        /// <summary>
        /// 设置各状态时长（毫秒）
        /// </summary>
        public void SetBlinkDurations(float closingMs, float closedMs, float openingMs)
        {
            closingDurationMs = closingMs;
            closedDurationMs = closedMs;
            openingDurationMs = openingMs;
        }

        /// <summary>
        /// 设置眼睛精灵
        /// </summary>
        public void SetEyeSprites(Sprite open, Sprite half, Sprite closed)
        {
            openSprite = open;
            halfSprite = half;
            closedSprite = closed;
        }
    }
}
