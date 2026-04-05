using UnityEngine;
using HaChiMiOhNameruDo.Managers;

namespace HaChiMiOhNameruDo.MiniGames.Rhythm
{
    /// <summary>
    /// 节奏测试控制器
    /// 用于在编辑器中测试节奏系统，播放音乐并显示输入反馈
    /// </summary>
    public class RhythmTestController : MonoBehaviour
    {
        [Header("音乐设置")]
        [Tooltip("要播放的音乐片段")]
        [SerializeField] private AudioClip musicClip;
        
        [Tooltip("音频源")]
        [SerializeField] private AudioSource audioSource;

        [Header("谱面配置")]
        [Tooltip("谱面配置文件")]
        [SerializeField] private BeatmapConfig beatmapConfig;

        [Header("引用")]
        [Tooltip("节奏管理器引用")]
        [SerializeField] private RhythmManager rhythmManager;
        
        [Tooltip("节奏输入处理器引用")]
        [SerializeField] private RhythmInputHandler inputHandler;
        
        [Tooltip("节奏音符可视化器引用（测试用）")]
        [SerializeField] private RhythmNoteVisualizer noteVisualizer;

        [Header("测试设置")]
        [Tooltip("是否自动开始音乐")]
        [SerializeField] private bool autoStartMusic = true;
        
        [Tooltip("开始前的延迟（秒）")]
        [SerializeField] private float startDelay = 1f;
        
        [Header("可视化设置（仅测试用）")]
        [Tooltip("是否显示音符下落可视化（测试用，正式游戏可隐藏）")]
        [SerializeField] private bool showNoteVisualizer = true;

        // 内部状态
        private bool isMusicPlaying;
        private float musicStartTime;

        private void Awake()
        {
            // 自动获取组件
            if (rhythmManager == null)
            {
                rhythmManager = FindObjectOfType<RhythmManager>();
            }

            if (inputHandler == null)
            {
                inputHandler = FindObjectOfType<RhythmInputHandler>();
            }

            if (noteVisualizer == null)
            {
                noteVisualizer = FindObjectOfType<RhythmNoteVisualizer>();
            }

            if (audioSource == null)
            {
                audioSource = GetComponent<AudioSource>();
                if (audioSource == null)
                {
                    audioSource = gameObject.AddComponent<AudioSource>();
                }
            }

            // 设置音频剪辑
            if (musicClip == null && beatmapConfig != null)
            {
                musicClip = beatmapConfig.musicClip;
            }

            if (musicClip != null)
            {
                audioSource.clip = musicClip;
            }
        }

        private void OnEnable()
        {
            // 订阅节奏管理器事件
            if (rhythmManager != null)
            {
                rhythmManager.OnBeatHit += OnBeatHit;
                rhythmManager.OnMeasureStart += OnMeasureStart;
                rhythmManager.OnInputJudged += OnInputJudged;
                rhythmManager.OnMusicStart += OnMusicStart;
                rhythmManager.OnMusicEnd += OnMusicEnd;
            }

            // 订阅输入处理器事件
            if (inputHandler != null)
            {
                inputHandler.OnRhythmInput += OnRhythmInput;
            }
        }

        private void OnDisable()
        {
            // 取消订阅
            if (rhythmManager != null)
            {
                rhythmManager.OnBeatHit -= OnBeatHit;
                rhythmManager.OnMeasureStart -= OnMeasureStart;
                rhythmManager.OnInputJudged -= OnInputJudged;
                rhythmManager.OnMusicStart -= OnMusicStart;
                rhythmManager.OnMusicEnd -= OnMusicEnd;
            }

            if (inputHandler != null)
            {
                inputHandler.OnRhythmInput -= OnRhythmInput;
            }
        }

        private void Start()
        {
            if (autoStartMusic)
            {
                Invoke(nameof(StartMusic), startDelay);
            }
        }

        private void Update()
        {
            // 同步音乐时间和节奏时间
            if (isMusicPlaying && audioSource != null)
            {
                // 检测音乐是否播放完毕
                if (!audioSource.isPlaying && audioSource.time >= audioSource.clip.length)
                {
                    StopMusic();
                }
            }

            // 键盘快捷键
            if (Input.GetKeyDown(KeyCode.M))
            {
                ToggleMusic();
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                ResetTest();
            }
            if (Input.GetKeyDown(KeyCode.V))
            {
                ToggleNoteVisualizer();
            }
        }

        #region 音乐控制

        /// <summary>
        /// 开始音乐
        /// </summary>
        public void StartMusic()
        {
            if (audioSource == null || musicClip == null)
            {
                Debug.LogError("[RhythmTest] 没有设置音乐片段！");
                return;
            }

            // 同步 BPM 和谱面配置
            if (beatmapConfig != null && rhythmManager != null)
            {
                rhythmManager.SetBPM(beatmapConfig.bpm);
            }

            // 播放音乐
            audioSource.Play();
            isMusicPlaying = true;
            musicStartTime = Time.time;

            // 启动节奏管理器
            if (rhythmManager != null)
            {
                rhythmManager.StartMusic();
            }

            Debug.Log("[RhythmTest] 音乐开始播放");
            Debug.Log($"[RhythmTest] 音乐时长：{musicClip.length}秒");
            Debug.Log($"[RhythmTest] BPM: {(beatmapConfig != null ? beatmapConfig.bpm : rhythmManager.BPM)}");
        }

        /// <summary>
        /// 停止音乐
        /// </summary>
        public void StopMusic()
        {
            if (audioSource != null)
            {
                audioSource.Stop();
            }

            isMusicPlaying = false;

            if (rhythmManager != null)
            {
                rhythmManager.StopMusic();
            }

            Debug.Log("[RhythmTest] 音乐停止");
        }

        /// <summary>
        /// 暂停音乐
        /// </summary>
        public void PauseMusic()
        {
            if (audioSource != null)
            {
                audioSource.Pause();
            }

            isMusicPlaying = false;

            if (rhythmManager != null)
            {
                rhythmManager.PauseMusic();
            }

            Debug.Log("[RhythmTest] 音乐暂停");
        }

        /// <summary>
        /// 恢复音乐
        /// </summary>
        public void ResumeMusic()
        {
            if (audioSource != null)
            {
                audioSource.Play();
            }

            isMusicPlaying = true;

            if (rhythmManager != null)
            {
                rhythmManager.ResumeMusic();
            }

            Debug.Log("[RhythmTest] 音乐恢复");
        }

        /// <summary>
        /// 切换音乐播放状态
        /// </summary>
        public void ToggleMusic()
        {
            if (isMusicPlaying)
            {
                PauseMusic();
            }
            else
            {
                ResumeMusic();
            }
        }

        /// <summary>
        /// 重置测试
        /// </summary>
        public void ResetTest()
        {
            StopMusic();
            audioSource.time = 0;
            Debug.Log("[RhythmTest] 测试已重置");
        }

        /// <summary>
        /// 切换音符可视化显示状态
        /// </summary>
        public void ToggleNoteVisualizer()
        {
            showNoteVisualizer = !showNoteVisualizer;
            
            if (noteVisualizer != null)
            {
                noteVisualizer.gameObject.SetActive(showNoteVisualizer);
            }
            
            Debug.Log($"[RhythmTest] 音符可视化：{(showNoteVisualizer ? "开启" : "关闭")}");
        }

        /// <summary>
        /// 设置音符可视化显示状态
        /// </summary>
        public void SetNoteVisualizerEnabled(bool enabled)
        {
            showNoteVisualizer = enabled;
            
            if (noteVisualizer != null)
            {
                noteVisualizer.gameObject.SetActive(enabled);
            }
        }

        #endregion

        #region 事件回调

        /// <summary>
        /// 节拍命中事件
        /// </summary>
        private void OnBeatHit(int beat, int measure)
        {
            // 只在强拍时输出日志，避免刷屏
            if (rhythmManager.IsStrongBeat())
            {
                Debug.Log($"[RhythmTest] 强拍 - 小节 {measure}, 节拍 {beat}");
            }
        }

        /// <summary>
        /// 新小节开始事件
        /// </summary>
        private void OnMeasureStart(int measure)
        {
            Debug.Log($"[RhythmTest] === 小节 {measure} 开始 ===");
        }

        /// <summary>
        /// 输入判定事件
        /// </summary>
        private void OnInputJudged(RhythmJudgment judgment, int combo)
        {
            string judgmentText = judgment.ToString().ToUpper();
            string comboText = combo > 0 ? $"连击：{combo}" : "";
            string multiplierText = rhythmManager.ComboMultiplier > 1f ? 
                $"倍率：{rhythmManager.ComboMultiplier:F1}x" : "";
            
            Debug.Log($"[RhythmTest] 输入判定：{judgmentText} {comboText} {multiplierText}");
        }

        /// <summary>
        /// 节奏输入事件
        /// </summary>
        private void OnRhythmInput()
        {
            float musicTime = rhythmManager.GetMusicTime();
            float currentBeat = musicTime / rhythmManager.BeatDuration;
            
            Debug.Log($"[RhythmTest] 玩家输入 - 音乐时间：{musicTime:F3}s, 节拍：{currentBeat:F2}");
        }

        /// <summary>
        /// 音乐开始事件
        /// </summary>
        private void OnMusicStart()
        {
            Debug.Log("[RhythmTest] 音乐开始事件");
        }

        /// <summary>
        /// 音乐结束事件
        /// </summary>
        private void OnMusicEnd()
        {
            Debug.Log("[RhythmTest] 音乐结束事件");
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 设置音乐片段
        /// </summary>
        public void SetMusicClip(AudioClip clip)
        {
            musicClip = clip;
            if (audioSource != null)
            {
                audioSource.clip = clip;
            }
        }

        /// <summary>
        /// 设置谱面配置
        /// </summary>
        public void SetBeatmapConfig(BeatmapConfig config)
        {
            beatmapConfig = config;
            if (config != null && rhythmManager != null)
            {
                rhythmManager.SetBPM(config.bpm);
            }
        }

        /// <summary>
        /// 获取当前音乐时间
        /// </summary>
        public float GetMusicTime()
        {
            if (audioSource != null && isMusicPlaying)
            {
                return audioSource.time;
            }
            return 0;
        }

        /// <summary>
        /// 获取当前音乐进度（0-1）
        /// </summary>
        public float GetMusicProgress()
        {
            if (audioSource != null && musicClip != null)
            {
                return audioSource.time / musicClip.length;
            }
            return 0;
        }

        #endregion

        #region 调试 GUI

        private void OnGUI()
        {
            if (!Application.isEditor) return;

            // 扩大高度以容纳可视化控制
            GUILayout.BeginArea(new Rect(Screen.width - 310, 10, 300, 380));
            GUILayout.BeginVertical("box");

            GUILayout.Label("=== 节奏测试控制器 ===", GUILayout.Height(20));
            GUILayout.Space(5);

            // 音乐信息
            GUILayout.Label($"音乐：{(musicClip != null ? musicClip.name : "未设置")}");
            GUILayout.Label($"时长：{(musicClip != null ? musicClip.length.ToString("F2") : "0")}秒");
            GUILayout.Label($"BPM: {(beatmapConfig != null ? beatmapConfig.bpm : rhythmManager.BPM)}");
            GUILayout.Space(5);

            // 播放状态
            GUILayout.Label($"播放状态：{(isMusicPlaying ? "播放中" : "已停止")}");
            GUILayout.Label($"音乐时间：{GetMusicTime():F2}s / {(musicClip != null ? musicClip.length.ToString("F2") : "0")}s");
            GUILayout.Label($"进度：{GetMusicProgress() * 100:F1}%");
            GUILayout.Space(5);

            // 节奏状态
            if (rhythmManager != null)
            {
                GUILayout.Label($"当前节拍：{rhythmManager.CurrentBeat}");
                GUILayout.Label($"当前小节：{rhythmManager.CurrentMeasure}");
                GUILayout.Label($"当前连击：{rhythmManager.CurrentCombo}");
                GUILayout.Label($"倍率：{rhythmManager.ComboMultiplier:F1}x");
            }
            GUILayout.Space(5);

            // 可视化状态
            GUILayout.Label($"音符可视化：{(showNoteVisualizer ? "开启" : "关闭")} (V)");
            GUILayout.Space(5);

            // 控制按钮
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("开始 (M)"))
            {
                StartMusic();
            }
            if (GUILayout.Button("停止"))
            {
                StopMusic();
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("暂停/恢复 (M)"))
            {
                ToggleMusic();
            }
            if (GUILayout.Button("重置 (R)"))
            {
                ResetTest();
            }
            GUILayout.EndHorizontal();

            // 可视化开关
            if (GUILayout.Button($"切换可视化 (V): {(showNoteVisualizer ? "开启" : "关闭")}"))
            {
                ToggleNoteVisualizer();
            }

            // 手动触发输入
            if (GUILayout.Button("触发测试输入（空格）"))
            {
                if (inputHandler != null)
                {
                    inputHandler.TriggerRhythmInput();
                }
            }

            GUILayout.EndVertical();
            GUILayout.EndArea();
        }

        #endregion
    }
}
