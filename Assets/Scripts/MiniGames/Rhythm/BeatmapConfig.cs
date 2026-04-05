using UnityEngine;

namespace HaChiMiOhNameruDo.MiniGames.Rhythm
{
    /// <summary>
    /// 音符类型
    /// </summary>
    public enum NoteType
    {
        None,       // 无音符
        Tap,        // 单点音符（拍打毛球）
        Hold,       // 长按音符（按住毛球）
        Swipe,      // 滑动音符（抽纸巾）
        DoubleTap   // 双点音符（同时拍打）
    }

    /// <summary>
    /// 单个音符数据
    /// </summary>
    [System.Serializable]
    public class NoteData
    {
        [Tooltip("小节编号（从 0 开始）")]
        public int measure;
        
        [Tooltip("音符在小节中的拍数（0-3 表示 4/4 拍）")]
        public int beatInMeasure;
        
        [Tooltip("音符类型")]
        public NoteType noteType;
        
        [Tooltip("音符强度/分数倍率")]
        [Range(0.5f, 2f)]
        public float intensity = 1f;
        
        [Tooltip("长按音符持续时间（拍数）")]
        public float holdDuration = 1f;
        
        [Tooltip("是否是强拍音符")]
        public bool isStrongBeat;
    }

    /// <summary>
    /// 谱面配置 - ScriptableObject
    /// 用于配置音乐的节奏谱面
    /// </summary>
    [CreateAssetMenu(fileName = "NewBeatmap", menuName = "HaChiMiOhNameruDo/Rhythm/Beatmap Config")]
    public class BeatmapConfig : ScriptableObject
    {
        [Header("基础信息")]
        [Tooltip("谱面名称")]
        public string beatmapName = "New Beatmap";
        
        [Tooltip("对应的音乐文件")]
        public AudioClip musicClip;
        
        [Tooltip("音乐时长（秒）")]
        public float musicDuration = 30f;
        
        [Tooltip("BPM（每分钟节拍数）")]
        public float bpm = 110f;
        
        [Tooltip("每小节拍数")]
        public int beatsPerMeasure = 4;

        [Header("谱面数据")]
        [Tooltip("音符列表")]
        public NoteData[] notes;

        [Header("难度设置")]
        [Tooltip("难度等级 1-5")]
        [Range(1, 5)]
        public int difficulty = 1;

        [Header("自动生成的谱面")]
        [Tooltip("是否根据 BPM 自动生成谱面")]
        public bool autoGenerateNotes = true;
        
        [Tooltip("自动生成时每小节的音符数")]
        [Range(1, 8)]
        public int notesPerMeasure = 4;

        /// <summary>
        /// 获取指定拍数的音符
        /// </summary>
        /// <param name="measure">小节编号</param>
        /// <param name="beatInMeasure">拍数（0-3）</param>
        /// <returns>音符数据数组</returns>
        public NoteData[] GetNotesAtBeat(int measure, int beatInMeasure)
        {
            if (notes == null) return new NoteData[0];

            var result = new System.Collections.Generic.List<NoteData>();

            foreach (var note in notes)
            {
                if (note.measure == measure && note.beatInMeasure == beatInMeasure)
                {
                    result.Add(note);
                }
            }

            return result.ToArray();
        }

        /// <summary>
        /// 获取指定时间范围内的音符
        /// </summary>
        /// <param name="startTime">开始时间（秒）</param>
        /// <param name="endTime">结束时间（秒）</param>
        /// <returns>音符数据数组</returns>
        public NoteData[] GetNotesInTimeRange(float startTime, float endTime)
        {
            if (notes == null) return new NoteData[0];

            var result = new System.Collections.Generic.List<NoteData>();
            float beatDuration = 60f / bpm;

            foreach (var note in notes)
            {
                float noteTime = GetNoteTime(note);
                if (noteTime >= startTime && noteTime <= endTime)
                {
                    result.Add(note);
                }
            }

            return result.ToArray();
        }

        /// <summary>
        /// 获取音符的时间（秒）
        /// </summary>
        public float GetNoteTime(NoteData note)
        {
            float beatDuration = 60f / bpm;
            int totalBeat = note.measure * beatsPerMeasure + note.beatInMeasure;
            return totalBeat * beatDuration;
        }

        /// <summary>
        /// 获取音符的总拍数
        /// </summary>
        private int GetNoteBeat(NoteData note)
        {
            return note.measure * beatsPerMeasure + note.beatInMeasure;
        }

        /// <summary>
        /// 验证谱面数据
        /// </summary>
        public bool Validate()
        {
            if (notes == null || notes.Length == 0)
            {
                Debug.LogWarning($"[Beatmap] {beatmapName}: 没有音符数据");
                return false;
            }

            if (bpm <= 0)
            {
                Debug.LogWarning($"[Beatmap] {beatmapName}: BPM 必须大于 0");
                return false;
            }

            if (musicDuration <= 0)
            {
                Debug.LogWarning($"[Beatmap] {beatmapName}: 音乐时长必须大于 0");
                return false;
            }

            return true;
        }

        /// <summary>
        /// 根据 BPM 自动生成简单谱面
        /// </summary>
        public void AutoGenerateSimpleBeatmap()
        {
            int totalBeats = Mathf.FloorToInt(musicDuration / (60f / bpm));
            notes = new NoteData[totalBeats];

            for (int i = 0; i < totalBeats; i++)
            {
                notes[i] = new NoteData
                {
                    measure = i / beatsPerMeasure, // 计算小节编号
                    beatInMeasure = i % beatsPerMeasure,
                    noteType = NoteType.Tap,
                    intensity = (i % beatsPerMeasure == 0) ? 1.5f : 1f, // 强拍强度更高
                    isStrongBeat = (i % beatsPerMeasure == 0)
                };
            }

            Debug.Log($"[Beatmap] 自动生成了 {totalBeats} 个音符，共 {Mathf.CeilToInt((float)totalBeats / beatsPerMeasure)} 个小节");
        }

        /// <summary>
        /// 导出谱面数据为 JSON 格式
        /// </summary>
        public string ToJson()
        {
            return JsonUtility.ToJson(this, true);
        }

        /// <summary>
        /// 从 JSON 导入谱面数据
        /// </summary>
        public static BeatmapConfig FromJson(string json)
        {
            return JsonUtility.FromJson<BeatmapConfig>(json);
        }
    }
}
