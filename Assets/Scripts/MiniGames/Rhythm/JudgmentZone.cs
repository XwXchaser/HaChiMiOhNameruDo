using UnityEngine;
using HaChiMiOhNameruDo.Managers;

namespace HaChiMiOhNameruDo.MiniGames.Rhythm
{
    /// <summary>
    /// 节奏判定区域
    /// 定义一个可点击的判定区域，支持可视/不可视切换
    /// 挂载到带有 RectTransform 的 GameObject 上
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class JudgmentZone : MonoBehaviour
    {
        [Header("区域设置")]
        [Tooltip("判定区域的 RectTransform（自动获取）")]
        public RectTransform zoneRect;
        
        [Header("判定参数")]
        [Tooltip("Perfect 判定的时间容差（秒）")]
        public float perfectWindow = 0.1f;
        
        [Tooltip("Normal 判定的时间容差（秒）")]
        public float normalWindow = 0.2f;
        
        [Header("视觉设置")]
        [Tooltip("是否在编辑器中显示区域边框")]
        public bool showInEditor = true;
        
        [Tooltip("是否在游戏中显示区域")]
        public bool showInGame = false;
        
        [Header("奖励配置")]
        [Tooltip("奖励配置 ScriptableObject")]
        public RhythmRewardConfig rewardConfig;
        
        // 内部状态
        private CanvasGroup canvasGroup;
        
        private void Awake()
        {
            Initialize();
        }
        
        /// <summary>
        /// 初始化组件
        /// </summary>
        public void Initialize()
        {
            if (zoneRect == null)
                zoneRect = GetComponent<RectTransform>();
            
            // 添加 CanvasGroup 控制显示/隐藏
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            
            UpdateVisualState();
        }
        
        /// <summary>
        /// 更新视觉状态
        /// </summary>
        public void UpdateVisualState()
        {
            bool shouldShow = Application.isEditor ? showInEditor : showInGame;
            canvasGroup.alpha = shouldShow ? 1f : 0f;
            canvasGroup.interactable = false; // 不拦截输入
            canvasGroup.blocksRaycasts = false; // 不阻挡射线
        }
        
        /// <summary>
        /// 检查点是否在区域内
        /// </summary>
        /// <param name="screenPoint">屏幕坐标</param>
        /// <returns>是否在区域内</returns>
        public bool IsPointInZone(Vector2 screenPoint)
        {
            return RectTransformUtility.RectangleContainsScreenPoint(
                zoneRect, screenPoint, null);
        }
        
        /// <summary>
        /// 根据时间差计算判定
        /// </summary>
        /// <param name="timeDelta">时间差（秒）</param>
        /// <returns>判定等级</returns>
        public RhythmJudgment CalculateJudgment(float timeDelta)
        {
            float absDelta = Mathf.Abs(timeDelta);
            
            if (absDelta <= perfectWindow)
                return RhythmJudgment.Perfect;
            else if (absDelta <= normalWindow)
                return RhythmJudgment.Normal;
            else
                return RhythmJudgment.Miss;
        }
        
        /// <summary>
        /// 获取该区域的奖励配置
        /// </summary>
        public RhythmRewardConfig GetRewardConfig()
        {
            return rewardConfig;
        }
        
        /// <summary>
        /// 设置奖励配置
        /// </summary>
        public void SetRewardConfig(RhythmRewardConfig config)
        {
            rewardConfig = config;
        }
        
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!showInEditor) return;
            
            if (zoneRect == null)
                zoneRect = GetComponent<RectTransform>();
            
            // 在 Scene 视图中绘制区域边框
            Vector3[] corners = new Vector3[4];
            zoneRect.GetWorldCorners(corners);
            
            // 绘制边框
            Gizmos.color = Color.cyan;
            for (int i = 0; i < 4; i++)
            {
                Gizmos.DrawLine(corners[i], corners[(i + 1) % 4]);
            }
            
            // 绘制中心点
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(zoneRect.position, 5f);
        }
#endif
    }
}
