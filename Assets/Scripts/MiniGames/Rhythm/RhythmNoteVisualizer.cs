using UnityEngine;
using UnityEngine.UI;
using HaChiMiOhNameruDo.Managers;
using System.Collections.Generic;
using System.Linq;

namespace HaChiMiOhNameruDo.MiniGames.Rhythm
{
    /// <summary>
    /// 节奏音符可视化器
    /// 显示类似传统音游的下落音符，需要点击音符本身触发判定
    /// </summary>
    public class RhythmNoteVisualizer : MonoBehaviour
    {
        [Header("容器设置")]
        [Tooltip("音符容器（所有音符的父对象）")]
        [SerializeField] private RectTransform noteContainer;
        
        [Tooltip("预制音符对象")]
        [SerializeField] private GameObject notePrefab;

        [Header("判定设置")]
        [Tooltip("Perfect 判定容错时间（秒），例如 0.1 表示±100ms")]
        [SerializeField] private float perfectWindow = 0.1f;
        
        [Tooltip("音符下落速度（像素/秒），如果为 0 则根据 BPM 自动计算")]
        [SerializeField] private float noteSpeed = 0f;

        [Header("引用")]
        [SerializeField] private RhythmManager rhythmManager;
        [SerializeField] private BeatmapConfig beatmapConfig;

        // 内部状态
        private List<NoteVisual> activeNotes = new List<NoteVisual>();
        private HashSet<int> spawnedNoteIndices = new HashSet<int>(); // 记录所有已生成的音符索引
        private float musicStartTime;
        private bool isRunning;
        
        // 事件
        /// <summary>
        /// 音符点击事件 (note, inputPosition)
        /// </summary>
        public event System.Action<NoteVisual, Vector2> OnNoteClicked;

        // 计算得到的值
        private float actualNoteSpeed;
        private float perfectZoneHeight;

        private void Awake()
        {
            if (rhythmManager == null)
            {
                rhythmManager = FindObjectOfType<RhythmManager>();
            }
        }

        private void OnEnable()
        {
            if (rhythmManager != null)
            {
                rhythmManager.OnMusicStart += OnMusicStart;
                rhythmManager.OnMusicEnd += OnMusicEnd;
            }
        }

        private void OnDisable()
        {
            if (rhythmManager != null)
            {
                rhythmManager.OnMusicStart -= OnMusicStart;
                rhythmManager.OnMusicEnd -= OnMusicEnd;
            }
        }

        private void Update()
        {
            if (!isRunning) return;

            // 更新音符位置
            UpdateNotePositions();

            // 生成即将到来的音符
            SpawnUpcomingNotes();

            // 清理过期音符
            CleanupExpiredNotes();
        }

        #region 初始化

        /// <summary>
        /// 计算音符速度和判定区域
        /// </summary>
        private void CalculateSpeedAndZone()
        {
            if (rhythmManager == null) return;
            
            // 如果 noteSpeed 为 0，则根据 BPM 自动计算
            // 假设屏幕高度为 1080p，音符从顶部到底部需要 2 个节拍
            if (noteSpeed <= 0)
            {
                float beatDuration = 60f / rhythmManager.BPM;
                // 2 个节拍的时长内走完屏幕高度
                float screenHeight = Screen.height;
                actualNoteSpeed = screenHeight / (beatDuration * 2);
            }
            else
            {
                actualNoteSpeed = noteSpeed;
            }
            
            // Perfect 区域高度 = 速度 × 容错时间
            perfectZoneHeight = actualNoteSpeed * perfectWindow;
            
            Debug.Log($"[NoteVisualizer] 音符速度：{actualNoteSpeed:F2} px/s, Perfect 区域高度：{perfectZoneHeight:F2} px");
        }

        #endregion

        #region 事件处理

        private void OnMusicStart()
        {
            Debug.Log($"[NoteVisualizer] OnMusicStart 被调用");
            Debug.Log($"[NoteVisualizer] beatmapConfig: {(beatmapConfig == null ? "null" : "已设置")}");
            Debug.Log($"[NoteVisualizer] noteContainer: {(noteContainer == null ? "null" : "已设置")}");
            Debug.Log($"[NoteVisualizer] notePrefab: {(notePrefab == null ? "null" : "已设置")}");
            
            isRunning = true;
            musicStartTime = Time.time;
            CalculateSpeedAndZone();
            
            // 预生成即将出现的音符
            PreSpawnNotes();
            
            Debug.Log("[NoteVisualizer] 音符可视化开始");
        }

        private void OnMusicEnd()
        {
            isRunning = false;
            ClearAllNotes();
            Debug.Log("[NoteVisualizer] 音符可视化结束");
        }

        #endregion

        #region 音符生成

        /// <summary>
        /// 预生成音符
        /// </summary>
        private void PreSpawnNotes()
        {
            if (beatmapConfig == null || beatmapConfig.notes == null) return;

            // 直接遍历所有音符，生成 2 秒内的音符
            foreach (var noteData in beatmapConfig.notes)
            {
                float noteTime = beatmapConfig.GetNoteTime(noteData);
                if (noteTime <= 2f) // 预生成 2 秒内的音符
                {
                    int uniqueIndex = noteData.measure * 1000 + noteData.beatInMeasure; // 唯一索引
                    if (!spawnedNoteIndices.Contains(uniqueIndex))
                    {
                        SpawnNote(noteData, uniqueIndex);
                        spawnedNoteIndices.Add(uniqueIndex);
                    }
                }
            }
        }

        /// <summary>
        /// 生成即将到来的音符
        /// </summary>
        private void SpawnUpcomingNotes()
        {
            if (beatmapConfig == null || beatmapConfig.notes == null || !isRunning || rhythmManager == null) return;

            float currentTime = Time.time - musicStartTime;
            float spawnTime = currentTime + 2f; // 提前 2 秒生成

            // 限制每次 Update 生成的音符数，避免卡顿
            int maxNotesPerFrame = 8;
            int notesGenerated = 0;

            foreach (var noteData in beatmapConfig.notes)
            {
                if (notesGenerated >= maxNotesPerFrame) break;

                float noteTime = beatmapConfig.GetNoteTime(noteData);
                
                // 检查音符是否应该在当前时间到 spawnTime 之间生成
                if (noteTime > currentTime && noteTime <= spawnTime)
                {
                    // 使用唯一索引检查是否已生成（包括已销毁的音符）
                    int uniqueIndex = noteData.measure * 1000 + noteData.beatInMeasure;
                    if (!spawnedNoteIndices.Contains(uniqueIndex))
                    {
                        SpawnNote(noteData, uniqueIndex);
                        spawnedNoteIndices.Add(uniqueIndex);
                        notesGenerated++;
                    }
                }
            }
        }

        /// <summary>
        /// 生成单个音符
        /// </summary>
        private void SpawnNote(NoteData noteData, int beatIndex)
        {
            if (notePrefab == null || noteContainer == null)
            {
                Debug.LogWarning("[NoteVisualizer] 音符预制体或容器未设置");
                return;
            }

            // 实例化音符
            GameObject noteObj = Instantiate(notePrefab, noteContainer);
            
            // 创建 NoteVisual 实例
            NoteVisual noteVisual = new NoteVisual();
            
            // 计算生成位置（屏幕上方外）
            float spawnY = Screen.height / 2f + 100f;
            noteVisual.Initialize(noteObj, noteData, beatIndex, spawnY, actualNoteSpeed);

            activeNotes.Add(noteVisual);
            
            Debug.Log($"[NoteVisualizer] 生成音符：小节{noteData.measure}, 拍{noteData.beatInMeasure}, 类型{noteData.noteType}, 时间{beatmapConfig.GetNoteTime(noteData):F2}s");
        }

        #endregion

        #region 音符更新

        /// <summary>
        /// 更新音符位置
        /// </summary>
        private void UpdateNotePositions()
        {
            for (int i = activeNotes.Count - 1; i >= 0; i--)
            {
                var note = activeNotes[i];
                note.UpdatePosition(Time.deltaTime);
            }
        }

        /// <summary>
        /// 清理过期音符
        /// </summary>
        private void CleanupExpiredNotes()
        {
            float currentTime = Time.time - musicStartTime;

            for (int i = activeNotes.Count - 1; i >= 0; i--)
            {
                var note = activeNotes[i];
                
                // 音符离开屏幕下方一定距离后销毁
                if (note.RectTransform != null)
                {
                    float noteY = note.RectTransform.anchoredPosition.y;
                    if (noteY < -Screen.height / 2f - 100f)
                    {
                        if (note.GameObject != null)
                        {
                            Destroy(note.GameObject);
                        }
                        activeNotes.RemoveAt(i);
                    }
                }
            }
        }

        /// <summary>
        /// 清除所有音符
        /// </summary>
        private void ClearAllNotes()
        {
            foreach (var note in activeNotes)
            {
                if (note.GameObject != null)
                {
                    Destroy(note.GameObject);
                }
            }
            activeNotes.Clear();
            spawnedNoteIndices.Clear(); // 清除已生成记录
        }

        #endregion

        #region 输入判定

        /// <summary>
        /// 处理音符点击输入
        /// </summary>
        /// <param name="screenPosition">点击的屏幕位置</param>
        public void ProcessNoteInput(Vector2 screenPosition)
        {
            Debug.Log($"[NoteVisualizer] ProcessNoteInput 被调用，screenPosition={screenPosition}, isRunning={isRunning}, activeNotes.Count={activeNotes.Count}");
            
            if (noteContainer == null)
            {
                Debug.LogError("[NoteVisualizer] noteContainer 为 null！请在 Inspector 中设置 Note Container 引用");
                return;
            }
            
            if (!isRunning)
            {
                Debug.LogWarning("[NoteVisualizer] isRunning=false，忽略输入");
                return;
            }
            
            if (activeNotes.Count == 0)
            {
                Debug.LogWarning("[NoteVisualizer] 没有活跃的音符，忽略输入");
                return;
            }

            Debug.Log($"[NoteVisualizer] 开始坐标转换，noteContainer={noteContainer.name}");
            
            // 将屏幕坐标转换为局部坐标
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                noteContainer, screenPosition, null, out Vector2 localPoint))
            {
                Debug.LogWarning($"[NoteVisualizer] 坐标转换失败！screenPosition={screenPosition}");
                return;
            }
            
            Debug.Log($"[NoteVisualizer] 局部坐标：{localPoint}");
            
            // 找到所有可点击的音符（点击范围内的）
            List<NoteVisual> clickableNotes = new List<NoteVisual>();
            
            foreach (var note in activeNotes)
            {
                if (note.IsClicked(localPoint))
                {
                    Debug.Log($"[NoteVisualizer] 音符被点击：拍数={note.Data.beatInMeasure}, 位置={note.RectTransform.anchoredPosition}");
                    clickableNotes.Add(note);
                }
            }

            if (clickableNotes.Count == 0)
            {
                Debug.LogWarning("[NoteVisualizer] 没有音符被点击到");
                return;
            }

            // 找到最接近判定中心的音符
            NoteVisual closestNote = null;
            float minDistance = float.MaxValue;
            
            foreach (var note in clickableNotes)
            {
                float distance = Mathf.Abs(note.RectTransform.anchoredPosition.y);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestNote = note;
                }
            }

            if (closestNote != null)
            {
                Debug.Log($"[NoteVisualizer] 触发 OnNoteClicked 事件，音符：小节{closestNote.Data.measure}, 拍{closestNote.Data.beatInMeasure}");
                
                // 触发音符点击事件（由 RhythmJudgmentSystem 处理判定）
                OnNoteClicked?.Invoke(closestNote, screenPosition);
                
                // 如果没有外部系统处理，使用传统方式处理
                // 计算判定结果
                RhythmJudgment judgment = CalculateJudgment(minDistance);
                
                Debug.Log($"[NoteVisualizer] 判定：{judgment}, 距离：{minDistance:F2}, perfectZoneHeight={perfectZoneHeight}");
                
                // 触发判定
                if (rhythmManager != null)
                {
                    rhythmManager.ProcessInput(judgment);
                }
                
                // 销毁音符
                if (closestNote.GameObject != null)
                {
                    Destroy(closestNote.GameObject);
                }
                activeNotes.Remove(closestNote);
            }
        }

        /// <summary>
        /// 根据距离计算判定结果
        /// </summary>
        private RhythmJudgment CalculateJudgment(float distanceFromCenter)
        {
            if (distanceFromCenter <= perfectZoneHeight)
            {
                return RhythmJudgment.Perfect;
            }
            else
            {
                return RhythmJudgment.Normal;
            }
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 设置 Perfect 容错时间
        /// </summary>
        public void SetPerfectWindow(float window)
        {
            perfectWindow = window;
            if (isRunning)
            {
                CalculateSpeedAndZone();
            }
        }

        /// <summary>
        /// 设置音符速度
        /// </summary>
        public void SetNoteSpeed(float speed)
        {
            noteSpeed = speed;
            if (isRunning)
            {
                CalculateSpeedAndZone();
            }
        }

        /// <summary>
        /// 手动启动音乐可视化（用于编辑器测试，绕过事件系统）
        /// </summary>
        public void StartMusicInternal(BeatmapConfig config)
        {
            Debug.Log("[NoteVisualizer] StartMusicInternal 被调用");
            
            // 设置 BeatmapConfig
            beatmapConfig = config;
            
            // 同步 BPM 到 RhythmManager
            if (rhythmManager != null && config != null)
            {
                rhythmManager.SetBPM(config.bpm);
                Debug.Log($"[NoteVisualizer] 已设置 RhythmManager BPM: {config.bpm}");
            }
            
            // 直接调用 OnMusicStart 逻辑
            isRunning = true;
            musicStartTime = Time.time;
            CalculateSpeedAndZone();
            PreSpawnNotes();
            
            Debug.Log("[NoteVisualizer] 音符可视化已手动启动");
            Debug.Log($"[NoteVisualizer] beatmapConfig 已设置：{beatmapConfig != null}");
            Debug.Log($"[NoteVisualizer] rhythmManager: {rhythmManager != null}");
            Debug.Log($"[NoteVisualizer] isRunning: {isRunning}");
            Debug.Log($"[NoteVisualizer] musicStartTime: {musicStartTime}");
            Debug.Log($"[NoteVisualizer] actualNoteSpeed: {actualNoteSpeed}");
        }

        #endregion

        /// <summary>
        /// 单个音符的视觉表现
        /// </summary>
        public class NoteVisual
        {
        public GameObject GameObject { get; private set; }
        public RectTransform RectTransform { get; private set; }
        public NoteData Data { get; private set; }
        public int BeatIndex { get; private set; }
        
        private float speed;
        private Image noteImage;
        private Button noteButton;

        public void Initialize(GameObject noteObj, NoteData data, int beatIndex, float startY, float speed)
        {
            Data = data;
            BeatIndex = beatIndex;
            this.speed = speed;

            GameObject = noteObj;
            RectTransform = noteObj.GetComponent<RectTransform>();
            noteImage = noteObj.GetComponent<Image>();
            noteButton = noteObj.GetComponent<Button>();

            // 设置初始位置
            RectTransform.anchoredPosition = new Vector2(0, startY);

            // 根据音符类型设置外观
            SetNoteType(data.noteType);

            // 添加按钮组件（如果还没有）
            if (noteButton == null)
            {
                noteButton = noteObj.AddComponent<Button>();
            }
        }

        public void UpdatePosition(float deltaTime)
        {
            if (RectTransform == null) return;

            Vector2 newPos = RectTransform.anchoredPosition;
            newPos.y -= speed * deltaTime;
            RectTransform.anchoredPosition = newPos;
        }

        /// <summary>
        /// 检查是否被点击
        /// </summary>
        public bool IsClicked(Vector2 clickPosition)
        {
            if (RectTransform == null) return false;

            // 获取音符的 Rect（本地空间，以 (0,0) 为中心）
            Rect noteRect = RectTransform.rect;
            
            // 计算音符在世界空间中的实际 Rect
            // 需要将音符的 anchoredPosition 考虑进去
            Vector2 noteCenter = RectTransform.anchoredPosition;
            
            // 计算点击位置相对于音符中心的偏移
            Vector2 localClick = clickPosition - noteCenter;
            
            // 检查点击是否在音符范围内
            bool clicked = noteRect.Contains(localClick);
            
            Debug.Log($"[NoteVisualizer] IsClicked: noteCenter={noteCenter}, clickPosition={clickPosition}, localClick={localClick}, rect={noteRect}, clicked={clicked}");
            
            return clicked;
        }

        private void SetNoteType(NoteType type)
        {
            if (noteImage == null) return;

            switch (type)
            {
                case NoteType.Tap:
                    noteImage.color = Color.white;
                    break;
                case NoteType.Hold:
                    noteImage.color = Color.yellow;
                    break;
                case NoteType.Swipe:
                    noteImage.color = Color.cyan;
                    break;
                case NoteType.DoubleTap:
                    noteImage.color = Color.magenta;
                    break;
                default:
                    noteImage.color = Color.gray;
                    break;
            }
        }
    }
}

