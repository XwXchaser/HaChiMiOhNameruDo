using UnityEngine;
using System;
using System.Collections.Generic;
using HaChiMiOhNameruDo.Managers;

namespace HaChiMiOhNameruDo.MiniGames.Rhythm
{
    /// <summary>
    /// 节奏判定系统
    /// 负责处理基于判定区域的节奏输入判定
    /// 支持多判定区域、音符追踪、判定计算
    /// </summary>
    public class RhythmJudgmentSystem : MonoBehaviour
    {
        [Header("判定区域")]
        [Tooltip("所有判定区域列表")]
        [SerializeField] private List<JudgmentZone> judgmentZones = new List<JudgmentZone>();
        
        [Header("引用")]
        [Tooltip("节奏管理器引用")]
        [SerializeField] private RhythmManager rhythmManager;
        
        [Tooltip("节奏音符可视化器引用")]
        [SerializeField] private RhythmNoteVisualizer noteVisualizer;
        
        [Tooltip("节奏反馈组件引用")]
        [SerializeField] private RhythmFeedback rhythmFeedback;
        
        // 内部状态
        private bool isInitialized;
        
        // 事件
        /// <summary>
        /// 判定触发事件 (judgment, zone, note)
        /// </summary>
        public event Action<RhythmJudgment, JudgmentZone, RhythmNoteVisualizer.NoteVisual> OnJudgmentTriggered;
        
        /// <summary>
        /// 音符命中事件
        /// </summary>
        public event Action<RhythmNoteVisualizer.NoteVisual> OnNoteHit;
        
        /// <summary>
        /// 音符 Miss 事件
        /// </summary>
        public event Action<RhythmNoteVisualizer.NoteVisual> OnNoteMissed;
        
        private void Awake()
        {
            Initialize();
        }
        
        /// <summary>
        /// 初始化判定系统
        /// </summary>
        public void Initialize()
        {
            if (isInitialized) return;
            
            // 自动获取 RhythmManager
            if (rhythmManager == null)
            {
                rhythmManager = FindObjectOfType<RhythmManager>();
            }
            
            // 自动获取 RhythmNoteVisualizer
            if (noteVisualizer == null)
            {
                noteVisualizer = FindObjectOfType<RhythmNoteVisualizer>();
            }
            
            // 自动获取 RhythmFeedback
            if (rhythmFeedback == null)
            {
                rhythmFeedback = FindObjectOfType<RhythmFeedback>();
            }
            
            // 获取所有 JudgmentZone
            if (judgmentZones.Count == 0)
            {
                judgmentZones.AddRange(FindObjectsOfType<JudgmentZone>());
            }
            
            // 初始化所有判定区域
            foreach (var zone in judgmentZones)
            {
                if (zone != null)
                {
                    zone.Initialize();
                }
            }
            
            isInitialized = true;
            Debug.Log("[RhythmJudgmentSystem] 初始化完成");
        }
        
        private void OnEnable()
        {
            // 订阅音符命中事件
            if (noteVisualizer != null)
            {
                noteVisualizer.OnNoteClicked += HandleNoteClicked;
            }
            
            // 订阅节奏管理器事件
            if (rhythmManager != null)
            {
                rhythmManager.OnInputJudged += HandleGlobalInputJudged;
            }
        }
        
        private void OnDisable()
        {
            // 取消订阅
            if (noteVisualizer != null)
            {
                noteVisualizer.OnNoteClicked -= HandleNoteClicked;
            }
            
            if (rhythmManager != null)
            {
                rhythmManager.OnInputJudged -= HandleGlobalInputJudged;
            }
        }
        
        /// <summary>
        /// 处理音符点击
        /// </summary>
        private void HandleNoteClicked(RhythmNoteVisualizer.NoteVisual note, Vector2 inputPosition)
        {
            if (!rhythmManager.IsPlaying) return;
            
            // 检查输入位置是否在任何一个判定区域内
            JudgmentZone hitZone = null;
            foreach (var zone in judgmentZones)
            {
                if (zone != null && zone.IsPointInZone(inputPosition))
                {
                    hitZone = zone;
                    break;
                }
            }
            
            // 如果没有命中任何判定区域，使用默认判定
            if (hitZone == null)
            {
                Debug.LogWarning("[RhythmJudgmentSystem] 音符点击不在任何判定区域内");
                // 仍然处理音符，但不关联特定区域
                ProcessNoteHit(note, null);
                return;
            }
            
            // 计算音符与当前节拍的时机差
            float timingDelta = CalculateTimingDelta(note);
            
            // 使用判定区域计算判定结果
            RhythmJudgment judgment = hitZone.CalculateJudgment(timingDelta);
            
            // 触发判定
            TriggerJudgment(judgment, hitZone, note);
        }
        
        /// <summary>
        /// 处理全局输入判定（当没有音符可视化器时使用）
        /// </summary>
        private void HandleGlobalInputJudged(RhythmJudgment judgment, int combo)
        {
            // 当使用传统输入方式时，触发反馈
            if (rhythmFeedback != null)
            {
                rhythmFeedback.ShowJudgmentManual(judgment);
            }
        }
        
        /// <summary>
        /// 计算音符与当前节拍的时机差
        /// </summary>
        private float CalculateTimingDelta(RhythmNoteVisualizer.NoteVisual note)
        {
            if (note == null || note.Data == null) return float.MaxValue;
            
            // 获取音符的预期时间
            float noteTime = GetNoteExpectedTime(note);
            
            // 获取当前音乐时间
            float currentTime = rhythmManager.GetMusicTime();
            
            // 返回时间差
            return currentTime - noteTime;
        }
        
        /// <summary>
        /// 获取音符的预期时间
        /// </summary>
        private float GetNoteExpectedTime(RhythmNoteVisualizer.NoteVisual note)
        {
            if (note == null || note.Data == null) return 0f;
            
            // 从 RhythmManager 获取 BPM 和每小节拍数
            float beatDuration = rhythmManager.BeatDuration;
            int totalBeat = note.Data.measure * 4 + note.Data.beatInMeasure; // 假设 4/4 拍
            return totalBeat * beatDuration;
        }
        
        /// <summary>
        /// 处理音符命中
        /// </summary>
        private void ProcessNoteHit(RhythmNoteVisualizer.NoteVisual note, JudgmentZone zone)
        {
            // 触发音符命中事件
            OnNoteHit?.Invoke(note);
            
            // 如果有关联的判定区域，触发判定
            if (zone != null)
            {
                float timingDelta = CalculateTimingDelta(note);
                RhythmJudgment judgment = zone.CalculateJudgment(timingDelta);
                TriggerJudgment(judgment, zone, note);
            }
        }
        
        /// <summary>
        /// 触发判定
        /// </summary>
        private void TriggerJudgment(RhythmJudgment judgment, JudgmentZone zone, RhythmNoteVisualizer.NoteVisual note)
        {
            // 触发判定事件
            OnJudgmentTriggered?.Invoke(judgment, zone, note);
            
            // 通知 RhythmManager
            if (rhythmManager != null)
            {
                rhythmManager.ProcessInput(judgment);
            }
            
            // 处理 Miss
            if (judgment == RhythmJudgment.Miss)
            {
                OnNoteMissed?.Invoke(note);
            }
            
            Debug.Log($"[RhythmJudgmentSystem] 判定：{judgment}, 区域：{zone?.name}, 音符：{note?.Data?.noteType}");
        }
        
        /// <summary>
        /// 添加判定区域
        /// </summary>
        public void AddJudgmentZone(JudgmentZone zone)
        {
            if (zone != null && !judgmentZones.Contains(zone))
            {
                judgmentZones.Add(zone);
                zone.Initialize();
                Debug.Log($"[RhythmJudgmentSystem] 添加判定区域：{zone.name}");
            }
        }
        
        /// <summary>
        /// 移除判定区域
        /// </summary>
        public void RemoveJudgmentZone(JudgmentZone zone)
        {
            if (judgmentZones.Contains(zone))
            {
                judgmentZones.Remove(zone);
                Debug.Log($"[RhythmJudgmentSystem] 移除判定区域：{zone.name}");
            }
        }
        
        /// <summary>
        /// 获取所有判定区域
        /// </summary>
        public List<JudgmentZone> GetAllJudgmentZones()
        {
            return new List<JudgmentZone>(judgmentZones);
        }
        
        /// <summary>
        /// 检查点是否在任何判定区域内
        /// </summary>
        public JudgmentZone GetZoneAtPoint(Vector2 screenPoint)
        {
            foreach (var zone in judgmentZones)
            {
                if (zone != null && zone.IsPointInZone(screenPoint))
                {
                    return zone;
                }
            }
            return null;
        }
        
        /// <summary>
        /// 重置判定系统
        /// </summary>
        public void Reset()
        {
            // 重置所有判定区域
            foreach (var zone in judgmentZones)
            {
                if (zone != null)
                {
                    zone.UpdateVisualState();
                }
            }
            
            Debug.Log("[RhythmJudgmentSystem] 重置完成");
        }
        
        #region 调试
        
        private void OnDrawGizmos()
        {
            if (!Application.isEditor) return;
            
            // 在 Scene 视图中绘制所有判定区域
            foreach (var zone in judgmentZones)
            {
                if (zone == null) continue;
                
                var rect = zone.zoneRect;
                if (rect == null) continue;
                
                Vector3[] corners = new Vector3[4];
                rect.GetWorldCorners(corners);
                
                Gizmos.color = Color.cyan;
                for (int i = 0; i < 4; i++)
                {
                    Gizmos.DrawLine(corners[i], corners[(i + 1) % 4]);
                }
            }
        }
        
        #endregion
    }
}
