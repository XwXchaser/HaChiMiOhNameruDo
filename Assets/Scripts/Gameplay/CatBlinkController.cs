using UnityEngine;

namespace HaChiMiOhNameruDo.Gameplay
{
    /// <summary>
    /// 猫咪眨眼控制器
    /// 使用 Unity Animator 控制眨眼动画
    /// </summary>
    public class CatBlinkController : MonoBehaviour
    {
        [Header("Animator 组件")]
        [SerializeField] private Animator animator;

        [Header("眨眼参数")]
        [Range(2f, 6f)]
        [SerializeField] private float minBlinkInterval = 3.5f;  // 最小眨眼间隔
        [Range(2f, 8f)]
        [SerializeField] private float maxBlinkInterval = 5.5f;  // 最大眨眼间隔

        private float nextBlinkTime;

        private void Awake()
        {
            if (animator == null)
                animator = GetComponent<Animator>();
        }

        private void Start()
        {
            ScheduleNextBlink();
        }

        private void Update()
        {
            if (Time.time >= nextBlinkTime)
            {
                TriggerBlink();
                ScheduleNextBlink();
            }
        }

        /// <summary>
        /// 触发眨眼动画
        /// </summary>
        public void TriggerBlink()
        {
            if (animator != null)
            {
                animator.SetTrigger("BlinkTrigger");
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
        /// 设置眨眼间隔范围
        /// </summary>
        public void SetBlinkInterval(float min, float max)
        {
            minBlinkInterval = min;
            maxBlinkInterval = max;
            ScheduleNextBlink();
        }
    }
}
