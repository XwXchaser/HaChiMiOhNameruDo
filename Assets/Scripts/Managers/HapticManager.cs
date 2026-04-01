using UnityEngine;

namespace HaChiMiOhNameruDo.Managers
{
    /// <summary>
    /// 震动反馈管理器 - 单例模式
    /// 负责处理设备的震动反馈
    /// </summary>
    public class HapticManager : MonoBehaviour
    {
        public static HapticManager Instance { get; private set; }

        [Header("震动强度设置")]
        [Range(0, 1)]
        [SerializeField] private float defaultIntensity = 0.5f;  // 默认震动强度
        [Range(0, 1)]
        [SerializeField] private float lightIntensity = 0.3f;   // 轻微震动（拍击毛球）
        [Range(0, 1)]
        [SerializeField] private float strongIntensity = 0.8f;  // 强烈震动（碎纸）

        private void Awake()
        {
            // 单例模式
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        /// <summary>
        /// 播放默认震动
        /// </summary>
        public void PlayDefaultHaptic()
        {
            PlayHaptic(defaultIntensity);
        }

        /// <summary>
        /// 播放轻微震动（用于毛球拍击）
        /// </summary>
        public void PlayLightHaptic()
        {
            PlayHaptic(lightIntensity);
        }

        /// <summary>
        /// 播放强烈震动（用于碎纸）
        /// </summary>
        public void PlayStrongHaptic()
        {
            PlayHaptic(strongIntensity);
        }

        /// <summary>
        /// 播放指定强度的震动
        /// </summary>
        /// <param name="intensity">震动强度 (0-1)</param>
        public void PlayHaptic(float intensity)
        {
            intensity = Mathf.Clamp01(intensity);

#if UNITY_ANDROID
            // Android 平台震动
            Handheld.Vibrate();
#elif UNITY_IOS
            // iOS 平台需要使用 Native Plugin 来实现不同强度的震动
            Handheld.Vibrate();
#else
            // 编辑器或其他平台 - 使用 Debug 日志模拟
            Debug.Log($"[Haptic] 震动反馈 - 强度：{intensity}");
#endif
        }

        /// <summary>
        /// 播放连续震动（用于划动厕纸）
        /// </summary>
        /// <param name="duration">震动持续时间（秒）</param>
        public void PlayContinuousHaptic(float duration)
        {
            StartCoroutine(ContinuousHapticCoroutine(duration));
        }

        private System.Collections.IEnumerator ContinuousHapticCoroutine(float duration)
        {
            float elapsed = 0f;
            float interval = 0.1f; // 每 0.1 秒震动一次

            while (elapsed < duration)
            {
                PlayDefaultHaptic();
                elapsed += interval;
                yield return new WaitForSeconds(interval);
            }
        }
    }
}
