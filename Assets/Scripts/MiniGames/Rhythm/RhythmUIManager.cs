using UnityEngine;
using UnityEngine.UI;

namespace HaChiMiOhNameruDo.MiniGames.Rhythm
{
    /// <summary>
    /// 节奏 UI 管理器
    /// 统一管理所有节奏相关的 UI 组件
    /// </summary>
    public class RhythmUIManager : MonoBehaviour
    {
        [Header("UI 组件")]
        [Tooltip("判定文本")]
        [SerializeField] private Text judgmentText;
        
        [Tooltip("连击文本")]
        [SerializeField] private Text comboText;
        
        [Tooltip("连击容器")]
        [SerializeField] private RectTransform comboContainer;
        
        [Tooltip("音乐进度条")]
        [SerializeField] private Slider progressSlider;
        
        [Tooltip("BPM 文本")]
        [SerializeField] private Text bpmText;
        
        [Tooltip("节拍文本")]
        [SerializeField] private Text beatText;

        [Header("判定区域")]
        [Tooltip("Perfect 区域图像")]
        [SerializeField] private Image perfectZoneImage;
        
        [Tooltip("Good 区域图像")]
        [SerializeField] private Image goodZoneImage;
        
        [Tooltip("Normal 区域图像")]
        [SerializeField] private Image normalZoneImage;

        [Header("颜色设置")]
        [SerializeField] private Color perfectColor = new Color(1f, 0.8f, 0f);
        [SerializeField] private Color goodColor = new Color(0f, 1f, 0.5f);
        [SerializeField] private Color normalColor = new Color(0.5f, 0.8f, 1f);
        [SerializeField] private Color missColor = new Color(1f, 0.3f, 0.3f);

        [Header("判定区域颜色（半透明）")]
        [SerializeField] private Color perfectZoneColor = new Color(1f, 0.8f, 0f, 0.3f);
        [SerializeField] private Color goodZoneColor = new Color(0f, 1f, 0.5f, 0.3f);
        [SerializeField] private Color normalZoneColor = new Color(0.5f, 0.8f, 1f, 0.3f);

        private Managers.RhythmManager rhythmManager;

        private void Awake()
        {
            rhythmManager = FindObjectOfType<Managers.RhythmManager>();
        }

        private void OnEnable()
        {
            if (rhythmManager != null)
            {
                rhythmManager.OnInputJudged += OnInputJudged;
                rhythmManager.OnBeatHit += OnBeatHit;
                rhythmManager.OnMusicStart += OnMusicStart;
                rhythmManager.OnMusicEnd += OnMusicEnd;
            }
        }

        private void OnDisable()
        {
            if (rhythmManager != null)
            {
                rhythmManager.OnInputJudged -= OnInputJudged;
                rhythmManager.OnBeatHit -= OnBeatHit;
                rhythmManager.OnMusicStart -= OnMusicStart;
                rhythmManager.OnMusicEnd -= OnMusicEnd;
            }
        }

        private void Update()
        {
            // 更新音乐进度
            if (rhythmManager != null && rhythmManager.IsPlaying)
            {
                UpdateProgress();
            }
        }

        #region 事件处理

        private void OnInputJudged(Managers.RhythmJudgment judgment, int combo)
        {
            // 更新判定文本
            if (judgmentText != null)
            {
                judgmentText.text = judgment.ToString().ToUpper();
                judgmentText.color = GetJudgmentColor(judgment);
                
                // 重置缩放动画
                judgmentText.transform.localScale = Vector3.one * 1.5f;
            }

            // 更新连击文本
            if (comboText != null)
            {
                comboText.text = combo > 0 ? $"{combo}" : "";
                
                // 连击颜色
                if (combo >= 30) comboText.color = Color.red;
                else if (combo >= 20) comboText.color = new Color(1f, 0.5f, 0f);
                else if (combo >= 10) comboText.color = new Color(1f, 1f, 0f);
                else if (combo >= 5) comboText.color = new Color(0f, 1f, 1f);
                else comboText.color = Color.white;
            }

            // 连击动画
            if (combo >= 5 && comboContainer != null)
            {
                StartCoroutine(ComboPopup());
            }
        }

        private void OnBeatHit(int beat, int measure)
        {
            // 更新节拍显示
            if (beatText != null)
            {
                int beatInMeasure = beat % 4 + 1; // 1-4
                beatText.text = $"{measure + 1}-{beatInMeasure}";
                
                // 强拍高亮
                if (rhythmManager.IsStrongBeat())
                {
                    beatText.color = Color.red;
                }
                else
                {
                    beatText.color = Color.white;
                }
            }
        }

        private void OnMusicStart()
        {
            if (bpmText != null)
            {
                bpmText.text = $"BPM: {rhythmManager.BPM}";
            }
        }

        private void OnMusicEnd()
        {
            if (judgmentText != null)
            {
                judgmentText.text = "";
            }
            if (comboText != null)
            {
                comboText.text = "";
            }
        }

        #endregion

        #region 辅助方法

        private void UpdateProgress()
        {
            if (progressSlider == null) return;

            // 需要音乐总时长，可以通过 RhythmTestController 获取
            var testController = FindObjectOfType<RhythmTestController>();
            if (testController != null)
            {
                progressSlider.value = testController.GetMusicProgress();
            }
        }

        private Color GetJudgmentColor(Managers.RhythmJudgment judgment)
        {
            switch (judgment)
            {
                case Managers.RhythmJudgment.Perfect:
                    return perfectColor;
                case Managers.RhythmJudgment.Good:
                    return goodColor;
                case Managers.RhythmJudgment.Normal:
                    return normalColor;
                case Managers.RhythmJudgment.Miss:
                    return missColor;
                default:
                    return Color.white;
            }
        }

        private System.Collections.IEnumerator ComboPopup()
        {
            if (comboContainer == null) yield break;

            float duration = 0.2f;
            float timer = 0;
            Vector3 startScale = Vector3.one;
            Vector3 endScale = Vector3.one * 1.3f;

            // 放大
            while (timer < duration / 2)
            {
                timer += Time.deltaTime;
                float t = timer / (duration / 2);
                comboContainer.localScale = Vector3.Lerp(startScale, endScale, t);
                yield return null;
            }

            // 缩小
            timer = 0;
            while (timer < duration / 2)
            {
                timer += Time.deltaTime;
                float t = timer / (duration / 2);
                comboContainer.localScale = Vector3.Lerp(endScale, startScale, t);
                yield return null;
            }

            comboContainer.localScale = Vector3.one;
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 设置判定区域视觉
        /// </summary>
        public void SetupJudgmentZones(float perfectHeight, float goodHeight, float normalHeight)
        {
            if (perfectZoneImage != null)
            {
                perfectZoneImage.color = perfectZoneColor;
                RectTransform rect = perfectZoneImage.rectTransform;
                rect.sizeDelta = new Vector2(rect.sizeDelta.x, perfectHeight * 2);
                rect.anchoredPosition = new Vector2(0, 100); // judgmentLineY
            }

            if (goodZoneImage != null)
            {
                goodZoneImage.color = goodZoneColor;
                RectTransform rect = goodZoneImage.rectTransform;
                rect.sizeDelta = new Vector2(rect.sizeDelta.x, goodHeight * 2);
                rect.anchoredPosition = new Vector2(0, 100);
            }

            if (normalZoneImage != null)
            {
                normalZoneImage.color = normalZoneColor;
                RectTransform rect = normalZoneImage.rectTransform;
                rect.sizeDelta = new Vector2(rect.sizeDelta.x, normalHeight * 2);
                rect.anchoredPosition = new Vector2(0, 100);
            }
        }

        #endregion
    }
}
