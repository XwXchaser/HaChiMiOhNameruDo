using UnityEngine;
using System;

namespace HaChiMiOhNameruDo.Managers
{
    /// <summary>
    /// 节奏判定等级
    /// </summary>
    public enum RhythmJudgment
    {
        Perfect,    // 完美判定（±50ms）
        Good,       // 良好判定（±100ms）
        Normal,     // 普通判定（±200ms）
        Miss        // 未命中
    }

    /// <summary>
    /// 节奏管理器 - 单例
    /// 负责追踪音乐节拍、判定输入时机、触发节拍事件
    /// 支持手机平台和编辑器鼠标测试
    /// </summary>
    public class RhythmManager : MonoBehaviour
    {
        #region 单例

        public static RhythmManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        #endregion

        #region 音乐配置

        [Header("音乐设置")]
        [Tooltip("音乐节奏速度（每分钟节拍数）")]
        [SerializeField] private float bpm = 110f;
        
        [Tooltip("每小节的拍数（4/4 拍为 4）")]
        [SerializeField] private int beatsPerMeasure = 4;
        
        [Tooltip("音频延迟补偿（秒）")]
        [SerializeField] private float audioOffset = 0f;
        
        [Tooltip("音乐开始前的倒计时（秒）")]
        [SerializeField] private float startDelay = 1f;
        
        [Tooltip("音乐音频剪辑（可选，用于测试）")]
        [SerializeField] private AudioClip musicClip;
        
        [Tooltip("音频源组件")]
        [SerializeField] private AudioSource audioSource;

        #endregion

        #region 判定窗口配置

        [Header("判定窗口 (秒)")]
        [Tooltip("Perfect 判定窗口（±秒）")]
        [SerializeField] private float perfectWindow = 0.05f;  // ±50ms
        
        [Tooltip("Good 判定窗口（±秒）")]
        [SerializeField] private float goodWindow = 0.10f;     // ±100ms
        
        [Tooltip("Normal 判定窗口（±秒）")]
        [SerializeField] private float normalWindow = 0.20f;   // ±200ms

        #endregion

        #region 当前状态

        private bool isPlaying;                    // 是否正在播放
        private float musicStartTime;              // 音乐开始时间
        private float currentBeatTime;             // 当前节拍时间（0 到 beatDuration）
        private int currentBeat;                   // 当前节拍编号（从 0 开始）
        private int currentMeasure;                // 当前小节编号（从 0 开始）
        private float beatDuration;                // 每拍时长（60/BPM）

        #endregion

        #region 连击系统

        private int currentCombo;                  // 当前连击数
        private int maxCombo;                      // 最大连击数
        private float comboMultiplier = 1f;        // 连击倍率

        #endregion

        #region 事件

        /// <summary>
        /// 节拍命中事件 (beat, measure)
        /// </summary>
        public event Action<int, int> OnBeatHit;

        /// <summary>
        /// 新小节开始事件
        /// </summary>
        public event Action<int> OnMeasureStart;

        /// <summary>
        /// 输入判定事件 (judgment, combo)
        /// </summary>
        public event Action<RhythmJudgment, int> OnInputJudged;

        /// <summary>
        /// 音乐开始事件
        /// </summary>
        public event Action OnMusicStart;

        /// <summary>
        /// 音乐结束事件
        /// </summary>
        public event Action OnMusicEnd;

        #endregion

        #region 属性

        public bool IsPlaying => isPlaying;
        public float BPM => bpm;
        public int CurrentBeat => currentBeat;
        public int CurrentMeasure => currentMeasure;
        public int CurrentCombo => currentCombo;
        public float ComboMultiplier => comboMultiplier;
        public float BeatDuration => beatDuration;
        public float PerfectWindow => perfectWindow;
        public float GoodWindow => goodWindow;
        public float NormalWindow => normalWindow;

        #endregion

        #region Unity 生命周期

        private void Start()
        {
            beatDuration = 60f / bpm;
        }

        private void Update()
        {
            if (!isPlaying) return;

            // 更新节拍追踪
            float timeSinceMusicStart = Time.time - musicStartTime - startDelay;
            
            if (timeSinceMusicStart < 0) return;

            // 计算当前节拍
            int expectedBeat = Mathf.FloorToInt(timeSinceMusicStart / beatDuration);
            
            if (expectedBeat > currentBeat)
            {
                // 触发新节拍
                currentBeat = expectedBeat;
                currentMeasure = currentBeat / beatsPerMeasure;
                currentBeatTime = timeSinceMusicStart % beatDuration;

                // 触发事件
                OnBeatHit?.Invoke(currentBeat, currentMeasure);

                // 检查新小节
                if (currentBeat % beatsPerMeasure == 0)
                {
                    OnMeasureStart?.Invoke(currentMeasure);
                }
            }

            currentBeatTime = timeSinceMusicStart % beatDuration;
        }

        #endregion

        #region 音乐控制

        /// <summary>
        /// 开始音乐和节奏追踪（不带音频）
        /// </summary>
        public void StartMusic()
        {
            StartMusicInternal(null);
        }

        /// <summary>
        /// 开始音乐和节奏追踪（带音频播放）
        /// </summary>
        /// <param name="clip">音乐音频剪辑</param>
        public void StartMusic(AudioClip clip)
        {
            StartMusicInternal(clip);
        }

        /// <summary>
        /// 内部方法：开始音乐和节奏追踪
        /// </summary>
        private void StartMusicInternal(AudioClip clip)
        {
            // 获取或创建 AudioSource
            if (audioSource == null)
            {
                audioSource = GetComponent<AudioSource>();
            }
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
            
            // 设置音频剪辑
            if (clip != null)
            {
                audioSource.clip = clip;
                musicClip = clip;
            }
            else if (musicClip != null)
            {
                audioSource.clip = musicClip;
            }
            
            // 播放音频
            if (audioSource.clip != null)
            {
                audioSource.Play();
                Debug.Log($"[Rhythm] 播放音频：{audioSource.clip.name}, 时长：{audioSource.clip.length}s");
            }
            else
            {
                Debug.Log("[Rhythm] 没有设置音频剪辑，仅播放节奏");
            }
            
            isPlaying = true;
            musicStartTime = Time.time;
            currentBeat = -1;  // 从 -1 开始，第一个节拍时为 0
            currentMeasure = 0;
            currentCombo = 0;
            maxCombo = 0;
            comboMultiplier = 1f;
            beatDuration = 60f / bpm;

            Debug.Log($"[Rhythm] 音乐开始 - BPM: {bpm}, Beat Duration: {beatDuration:F3}s");
        }

        /// <summary>
        /// 停止音乐和节奏追踪
        /// </summary>
        public void StopMusic()
        {
            isPlaying = false;
            currentBeat = 0;
            currentMeasure = 0;
            currentCombo = 0;
            comboMultiplier = 1f;
            
            // 停止音频
            if (audioSource != null && audioSource.isPlaying)
            {
                audioSource.Stop();
            }

            Debug.Log("[Rhythm] 音乐停止");
        }

        /// <summary>
        /// 暂停音乐
        /// </summary>
        public void PauseMusic()
        {
            isPlaying = false;
            
            // 暂停音频
            if (audioSource != null && audioSource.isPlaying)
            {
                audioSource.Pause();
            }
            
            Debug.Log("[Rhythm] 音乐暂停");
        }

        /// <summary>
        /// 恢复音乐
        /// </summary>
        public void ResumeMusic()
        {
            if (!isPlaying)
            {
                isPlaying = true;
                musicStartTime = Time.time - (currentBeat * beatDuration) - startDelay;
                
                // 恢复音频
                if (audioSource != null && audioSource.clip != null)
                {
                    audioSource.UnPause();
                }
                
                Debug.Log("[Rhythm] 音乐恢复");
            }
        }

        #endregion

        #region 输入判定

        /// <summary>
        /// 处理节奏输入（自动根据时机判定）
        /// </summary>
        public void ProcessInput()
        {
            if (!isPlaying) return;

            float timeSinceMusicStart = Time.time - musicStartTime - startDelay;
            if (timeSinceMusicStart < 0) return;

            // 计算与下一个节拍的偏移
            float timeToNextBeat = beatDuration - currentBeatTime;
            float timeFromLastBeat = currentBeatTime;

            // 取最近的节拍作为判定基准
            float timingOffset;
            if (timeToNextBeat < timeFromLastBeat)
            {
                // 更接近下一个节拍
                timingOffset = timeToNextBeat;
            }
            else
            {
                // 更接近上一个节拍
                timingOffset = -timeFromLastBeat;
            }

            // 判定输入
            RhythmJudgment judgment = JudgeInput(timingOffset);

            // 处理判定结果
            HandleJudgment(judgment);
        }

        /// <summary>
        /// 处理节奏输入（使用预设的判定结果，用于音符可视化）
        /// </summary>
        /// <param name="judgment">预设的判定结果</param>
        public void ProcessInput(RhythmJudgment judgment)
        {
            if (!isPlaying) return;
            
            // 直接使用传入的判定结果
            HandleJudgment(judgment);
        }

        /// <summary>
        /// 判定输入时机
        /// </summary>
        /// <param name="timingOffset">与节拍的偏移时间（秒）</param>
        /// <returns>判定等级</returns>
        public RhythmJudgment JudgeInput(float timingOffset)
        {
            float absOffset = Mathf.Abs(timingOffset);

            if (absOffset <= perfectWindow)
                return RhythmJudgment.Perfect;
            if (absOffset <= goodWindow)
                return RhythmJudgment.Good;
            if (absOffset <= normalWindow)
                return RhythmJudgment.Normal;
            return RhythmJudgment.Miss;
        }

        /// <summary>
        /// 处理判定结果
        /// </summary>
        private void HandleJudgment(RhythmJudgment judgment)
        {
            if (judgment != RhythmJudgment.Miss)
            {
                currentCombo++;
                if (currentCombo > maxCombo)
                {
                    maxCombo = currentCombo;
                }
                UpdateComboMultiplier();
            }
            else
            {
                currentCombo = 0;
                comboMultiplier = 1f;
            }

            Debug.Log($"[Rhythm] 判定：{judgment}, 连击：{currentCombo}, 倍率：{comboMultiplier:F1}x");
            OnInputJudged?.Invoke(judgment, currentCombo);
        }

        /// <summary>
        /// 更新连击倍率
        /// </summary>
        private void UpdateComboMultiplier()
        {
            // 连击倍率计算
            if (currentCombo >= 30) comboMultiplier = 2.0f;  // Fever
            else if (currentCombo >= 20) comboMultiplier = 1.5f;
            else if (currentCombo >= 10) comboMultiplier = 1.3f;
            else if (currentCombo >= 5) comboMultiplier = 1.1f;
            else comboMultiplier = 1f;
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 获取当前节拍在小节中的位置 (0-3)
        /// </summary>
        public int GetBeatInMeasure() => currentBeat % beatsPerMeasure;

        /// <summary>
        /// 检查是否是强拍 (第 1 拍)
        /// </summary>
        public bool IsStrongBeat() => GetBeatInMeasure() == 0;

        /// <summary>
        /// 获取当前音乐时间（秒）
        /// </summary>
        public float GetMusicTime()
        {
            if (!isPlaying) return 0;
            return Time.time - musicStartTime - startDelay;
        }

        /// <summary>
        /// 获取当前音乐进度（0-1）
        /// </summary>
        /// <param name="musicDuration">音乐总时长</param>
        public float GetMusicProgress(float musicDuration)
        {
            return Mathf.Clamp01(GetMusicTime() / musicDuration);
        }

        #endregion

        #region 调试

        /// <summary>
        /// 设置 BPM（用于调试）
        /// </summary>
        public void SetBPM(float newBPM)
        {
            bpm = newBPM;
            beatDuration = 60f / bpm;
            Debug.Log($"[Rhythm] BPM 已更改为：{bpm}");
        }

        /// <summary>
        /// 手动触发节拍（用于调试）
        /// </summary>
        public void DebugTriggerBeat()
        {
            currentBeat++;
            currentMeasure = currentBeat / beatsPerMeasure;
            OnBeatHit?.Invoke(currentBeat, currentMeasure);

            if (currentBeat % beatsPerMeasure == 0)
            {
                OnMeasureStart?.Invoke(currentMeasure);
            }

            Debug.Log($"[Rhythm Debug] 触发节拍 {currentBeat}, 小节 {currentMeasure}");
        }

        #endregion
    }
}
