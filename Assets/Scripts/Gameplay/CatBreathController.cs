using UnityEngine;

namespace HaChiMiOhNameruDo.Gameplay
{
    /// <summary>
    /// 猫咪呼吸控制器
    /// 使用 sin() 函数产生平滑的呼吸缩放效果
    /// 呼吸频率：约 1.8Hz（周期约 0.56 秒）
    /// 缩放幅度：X: 0.8%, Y: 1.2%（Y 略大于 X 模拟胸腔起伏）
    /// </summary>
    public class CatBreathController : MonoBehaviour
    {
        [Header("呼吸参数")]
        [Tooltip("呼吸频率（Hz），默认 1.8Hz 对应周期约 0.56 秒")]
        [Range(0.5f, 3f)]
        [SerializeField] private float breathFrequency = 1.8f;

        [Tooltip("X 轴缩放幅度（百分比），默认 0.8%")]
        [Range(0.001f, 0.02f)]
        [SerializeField] private float xAmplitude = 0.008f;

        [Tooltip("Y 轴缩放幅度（百分比），默认 1.2%")]
        [Range(0.001f, 0.02f)]
        [SerializeField] private float yAmplitude = 0.012f;

        [Header("基础缩放")]
        [Tooltip("基础缩放值")]
        [SerializeField] private Vector3 baseScale = Vector3.one;

        // 持续递增的呼吸时间
        private float breathTime;

        private Transform cachedTransform;

        private void Awake()
        {
            cachedTransform = transform;
        }

        private void Update()
        {
            // 持续递增 breathTime
            breathTime += Time.deltaTime * breathFrequency * Mathf.PI * 2f;

            // 使用 sin() 产生 -1~1 的平滑震荡
            float breathValue = Mathf.Sin(breathTime);

            // 计算缩放：基础缩放 + sin 值 * 幅度
            // X 轴：1.0 + sin(breathTime) * 0.008
            // Y 轴：1.0 + sin(breathTime) * 0.012
            Vector3 targetScale = baseScale + new Vector3(
                breathValue * xAmplitude,
                breathValue * yAmplitude,
                0f
            );

            cachedTransform.localScale = targetScale;
        }

        /// <summary>
        /// 设置呼吸频率
        /// </summary>
        public void SetBreathFrequency(float frequency)
        {
            breathFrequency = frequency;
        }

        /// <summary>
        /// 设置呼吸幅度
        /// </summary>
        public void SetBreathAmplitude(float xAmp, float yAmp)
        {
            xAmplitude = xAmp;
            yAmplitude = yAmp;
        }

        /// <summary>
        /// 重置呼吸时间（可选，用于同步）
        /// </summary>
        public void ResetBreathTime()
        {
            breathTime = 0f;
        }
    }
}
