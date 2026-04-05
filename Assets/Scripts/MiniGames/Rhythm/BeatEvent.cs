using UnityEngine;
using System;

namespace HaChiMiOhNameruDo.MiniGames.Rhythm
{
    /// <summary>
    /// 节拍事件类型
    /// </summary>
    public enum BeatEventType
    {
        None,           // 无事件
        NoteSpawn,      // 音符生成
        EffectTrigger,  // 特效触发
        BGMChange,      // BGM 变化
        VisualChange    // 视觉变化
    }

    /// <summary>
    /// 节拍事件数据
    /// 用于定义在特定节拍发生的事件
    /// </summary>
    [Serializable]
    public class BeatEvent
    {
        [Tooltip("事件 ID（唯一标识）")]
        public int eventId;
        
        [Tooltip("事件类型")]
        public BeatEventType eventType;
        
        [Tooltip("小节编号（从 0 开始）")]
        public int measure;
        
        [Tooltip("音符在小节中的拍数（0-3 表示 4/4 拍）")]
        public int beatInMeasure;
        
        [Tooltip("事件数据（JSON 格式，用于存储额外信息）")]
        public string eventData;
        
        [Tooltip("事件是否已触发")]
        public bool hasTriggered;
        
        /// <summary>
        /// 获取事件的总拍数
        /// </summary>
        /// <param name="beatsPerMeasure">每小节拍数</param>
        /// <returns>总拍数</returns>
        public int GetTotalBeat(int beatsPerMeasure)
        {
            return measure * beatsPerMeasure + beatInMeasure;
        }
        
        /// <summary>
        /// 获取事件的时间（秒）
        /// </summary>
        /// <param name="bpm">BPM</param>
        /// <param name="beatsPerMeasure">每小节拍数</param>
        /// <returns>时间（秒）</returns>
        public float GetEventTime(float bpm, int beatsPerMeasure)
        {
            float beatDuration = 60f / bpm;
            int totalBeat = GetTotalBeat(beatsPerMeasure);
            return totalBeat * beatDuration;
        }
        
        /// <summary>
        /// 从 JSON 数据解析事件数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <returns>解析后的数据</returns>
        public T ParseEventData<T>() where T : class
        {
            if (string.IsNullOrEmpty(eventData))
            {
                return null;
            }
            
            try
            {
                return JsonUtility.FromJson<T>(eventData);
            }
            catch (Exception e)
            {
                Debug.LogError($"[BeatEvent] 解析事件数据失败：{e.Message}");
                return null;
            }
        }
        
        /// <summary>
        /// 设置事件数据为 JSON
        /// </summary>
        /// <param name="data">数据对象</param>
        public void SetEventData(object data)
        {
            eventData = JsonUtility.ToJson(data);
        }
        
        /// <summary>
        /// 重置事件触发状态
        /// </summary>
        public void Reset()
        {
            hasTriggered = false;
        }
    }
    
    /// <summary>
    /// 音符生成事件数据
    /// </summary>
    [Serializable]
    public class NoteSpawnData
    {
        [Tooltip("音符类型")]
        public NoteType noteType;
        
        [Tooltip("音符强度/分数倍率")]
        public float intensity = 1f;
        
        [Tooltip("长按音符持续时间（拍数）")]
        public float holdDuration = 1f;
        
        [Tooltip("是否是强拍音符")]
        public bool isStrongBeat;
        
        [Tooltip("音符生成位置索引（用于多轨道）")]
        public int trackIndex;
    }
    
    /// <summary>
    /// 特效触发事件数据
    /// </summary>
    [Serializable]
    public class EffectTriggerData
    {
        [Tooltip("特效类型名称")]
        public string effectName;
        
        [Tooltip("特效强度")]
        public float intensity = 1f;
        
        [Tooltip("特效持续时间（秒）")]
        public float duration = 0.5f;
    }
    
    /// <summary>
    /// BGM 变化事件数据
    /// </summary>
    [Serializable]
    public class BGMChangeData
    {
        [Tooltip("新的 BGM 音频剪辑名称")]
        public string newBGMName;
        
        [Tooltip("淡入时间（秒）")]
        public float fadeInTime = 0.5f;
        
        [Tooltip("淡出时间（秒）")]
        public float fadeOutTime = 0.5f;
    }
    
    /// <summary>
    /// 视觉变化事件数据
    /// </summary>
    [Serializable]
    public class VisualChangeData
    {
        [Tooltip("背景颜色")]
        public Color backgroundColor = Color.white;
        
        [Tooltip("背景淡入时间（秒）")]
        public float fadeInTime = 0.5f;
        
        [Tooltip("摄像机震动强度")]
        public float shakeIntensity = 0f;
    }
}
