using UnityEngine;
using UnityEngine.UI;
using HaChiMiOhNameruDo.Managers;

namespace HaChiMiOhNameruDo.MiniGames.Rhythm
{
    /// <summary>
    /// 节奏判定 UI 显示组件
    /// 显示 Perfect/Good/Normal/Miss 等判定结果和连击数
    /// </summary>
    public class RhythmFeedback : MonoBehaviour
    {
        [Header("UI 组件引用")]
        [Tooltip("判定文本（Perfect/Good 等）")]
        [SerializeField] private Text judgmentText;
        
        [Tooltip("连击数文本")]
        [SerializeField] private Text comboText;
        
        [Tooltip("连击容器（用于动画）")]
        [SerializeField] private RectTransform comboContainer;
        
        [Tooltip("判定图像（可选，用于显示不同颜色的判定）")]
        [SerializeField] private Image judgmentImage;

        [Header("判定颜色")]
        [SerializeField] private Color perfectColor = new Color(1f, 0.8f, 0f);  // 金色
        [SerializeField] private Color goodColor = new Color(0f, 1f, 0.5f);    // 绿色
        [SerializeField] private Color normalColor = new Color(0.5f, 0.8f, 1f); // 蓝色
        [SerializeField] private Color missColor = new Color(1f, 0.3f, 0.3f);   // 红色

        [Header("动画设置")]
        [Tooltip("判定文本显示时长")]
        [SerializeField] private float judgmentDisplayDuration = 0.5f;
        
        [Tooltip("判定文本缩放动画曲线")]
        [SerializeField] private AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        
        [Tooltip("判定文本弹出大小")]
        [SerializeField] private float maxScale = 1.5f;

        [Header("连击动画")]
        [Tooltip("连击文本弹出动画")]
        [SerializeField] private bool enableComboAnimation = true;
        
        [Tooltip("连击弹出阈值")]
        [SerializeField] private int comboPopupThreshold = 5;

        // 内部状态
        private RhythmManager rhythmManager;
        private float judgmentTimer;
        private bool isShowingJudgment;
        private Vector3 originalComboScale;

        private void Awake()
        {
            // 自动获取 RhythmManager
            if (rhythmManager == null)
            {
                rhythmManager = FindObjectOfType<RhythmManager>();
            }

            // 保存原始缩放
            if (comboContainer != null)
            {
                originalComboScale = comboContainer.localScale;
            }
        }

        private void OnEnable()
        {
            // 订阅节奏管理器事件
            if (rhythmManager != null)
            {
                rhythmManager.OnInputJudged += HandleInputJudged;
            }
        }

        private void OnDisable()
        {
            // 取消订阅
            if (rhythmManager != null)
            {
                rhythmManager.OnInputJudged -= HandleInputJudged;
            }
        }

        private void Update()
        {
            // 更新判定文本显示
            if (isShowingJudgment)
            {
                judgmentTimer -= Time.deltaTime;
                
                if (judgmentTimer <= 0)
                {
                    HideJudgment();
                }
                else
                {
                    // 更新缩放动画
                    float progress = 1f - (judgmentTimer / judgmentDisplayDuration);
                    float scale = scaleCurve.Evaluate(progress) * (maxScale - 1f) + 1f;
                    
                    if (judgmentText != null)
                    {
                        judgmentText.transform.localScale = Vector3.one * scale;
                    }
                }
            }
        }

        /// <summary>
        /// 处理输入判定
        /// </summary>
        private void HandleInputJudged(RhythmJudgment judgment, int combo)
        {
            // 显示判定
            ShowJudgment(judgment);
            
            // 更新连击显示
            UpdateComboDisplay(combo);
        }

        /// <summary>
        /// 显示判定结果
        /// </summary>
        private void ShowJudgment(RhythmJudgment judgment)
        {
            if (judgmentText == null) return;

            // 设置文本
            string judgmentString = judgment.ToString().ToUpper();
            judgmentText.text = judgmentString;
            
            // 设置颜色
            Color judgmentColor = GetJudgmentColor(judgment);
            judgmentText.color = judgmentColor;
            
            if (judgmentImage != null)
            {
                judgmentImage.color = judgmentColor;
            }

            // 重置计时器和状态
            judgmentTimer = judgmentDisplayDuration;
            isShowingJudgment = true;
            
            // 重置缩放
            if (judgmentText != null)
            {
                judgmentText.transform.localScale = Vector3.one;
            }

            // 根据判定类型触发不同效果
            switch (judgment)
            {
                case RhythmJudgment.Perfect:
                    // Perfect 可以触发特殊效果
                    Debug.Log("[RhythmFeedback] Perfect! 太棒了！");
                    break;
                case RhythmJudgment.Miss:
                    // Miss 时隐藏判定文本
                    judgmentText.text = "MISS";
                    break;
            }
        }

        /// <summary>
        /// 隐藏判定文本
        /// </summary>
        private void HideJudgment()
        {
            isShowingJudgment = false;
            judgmentTimer = 0;
            
            if (judgmentText != null)
            {
                judgmentText.transform.localScale = Vector3.one;
            }
        }

        /// <summary>
        /// 更新连击显示
        /// </summary>
        private void UpdateComboDisplay(int combo)
        {
            if (comboText == null) return;

            comboText.text = $"{combo}";
            
            // 连击动画
            if (enableComboAnimation && comboContainer != null && combo >= comboPopupThreshold)
            {
                StartCoroutine(ComboPopupAnimation());
            }

            // 连击文本颜色根据连击数变化
            if (combo >= 30)
            {
                comboText.color = Color.red; // Fever 模式
            }
            else if (combo >= 20)
            {
                comboText.color = new Color(1f, 0.5f, 0f);
            }
            else if (combo >= 10)
            {
                comboText.color = new Color(1f, 1f, 0f);
            }
            else if (combo >= 5)
            {
                comboText.color = new Color(0f, 1f, 1f);
            }
            else
            {
                comboText.color = Color.white;
            }
        }

        /// <summary>
        /// 连击弹出动画
        /// </summary>
        private System.Collections.IEnumerator ComboPopupAnimation()
        {
            if (comboContainer == null) yield break;

            float duration = 0.3f;
            float timer = 0;
            Vector3 startScale = originalComboScale;
            Vector3 endScale = originalComboScale * 1.3f;

            // 放大
            while (timer < duration / 2)
            {
                timer += Time.deltaTime;
                float t = timer / (duration / 2);
                comboContainer.localScale = Vector3.Lerp(startScale, endScale, t);
                yield return null;
            }

            // 缩小回原样
            timer = 0;
            while (timer < duration / 2)
            {
                timer += Time.deltaTime;
                float t = timer / (duration / 2);
                comboContainer.localScale = Vector3.Lerp(endScale, startScale, t);
                yield return null;
            }

            comboContainer.localScale = originalComboScale;
        }

        /// <summary>
        /// 获取判定颜色
        /// </summary>
        private Color GetJudgmentColor(RhythmJudgment judgment)
        {
            switch (judgment)
            {
                case RhythmJudgment.Perfect:
                    return perfectColor;
                case RhythmJudgment.Good:
                    return goodColor;
                case RhythmJudgment.Normal:
                    return normalColor;
                case RhythmJudgment.Miss:
                    return missColor;
                default:
                    return Color.white;
            }
        }

        #region 公共方法

        /// <summary>
        /// 手动显示判定（用于测试）
        /// </summary>
        public void ShowJudgmentManual(RhythmJudgment judgment)
        {
            ShowJudgment(judgment);
        }

        /// <summary>
        /// 设置判定文本
        /// </summary>
        public void SetJudgmentText(Text text)
        {
            judgmentText = text;
        }

        /// <summary>
        /// 设置连击文本
        /// </summary>
        public void SetComboText(Text text)
        {
            comboText = text;
        }

        #endregion

        #region 调试

        private void OnGUI()
        {
            if (!Application.isEditor) return;

            GUILayout.BeginArea(new Rect(320, 10, 200, 200));
            GUILayout.Label("=== 判定 UI 调试 ===");
            
            if (GUILayout.Button("显示 Perfect"))
            {
                ShowJudgment(RhythmJudgment.Perfect);
            }
            if (GUILayout.Button("显示 Good"))
            {
                ShowJudgment(RhythmJudgment.Good);
            }
            if (GUILayout.Button("显示 Normal"))
            {
                ShowJudgment(RhythmJudgment.Normal);
            }
            if (GUILayout.Button("显示 Miss"))
            {
                ShowJudgment(RhythmJudgment.Miss);
            }

            GUILayout.EndArea();
        }

        #endregion
    }
}
