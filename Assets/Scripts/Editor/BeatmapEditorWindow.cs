#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using HaChiMiOhNameruDo.Managers;

namespace HaChiMiOhNameruDo.MiniGames.Rhythm.Editor
{
    /// <summary>
    /// 谱面配置编辑器窗口
    /// 用于可视化的配置和测试谱面
    /// </summary>
    public class BeatmapEditorWindow : EditorWindow
    {
        private BeatmapConfig currentBeatmap;
        private Vector2 scrollPosition;
        private float testProgress;
        private bool isTesting;
        private int selectedNoteIndex = -1;

        [MenuItem("HaChiMiOhNameruDo/谱面配置器")]
        public static void ShowWindow()
        {
            var window = GetWindow<BeatmapEditorWindow>("谱面配置器");
            window.minSize = new Vector2(600, 400);
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();

            // 标题
            GUILayout.Label("谱面配置器", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            // 谱面选择
            EditorGUILayout.BeginHorizontal();
            currentBeatmap = (BeatmapConfig)EditorGUILayout.ObjectField(
                "谱面配置", 
                currentBeatmap, 
                typeof(BeatmapConfig), 
                false
            );
            
            if (currentBeatmap != null)
            {
                if (GUILayout.Button("加载", GUILayout.Width(60)))
                {
                    LoadBeatmap();
                }
            }
            EditorGUILayout.EndHorizontal();

            if (currentBeatmap == null)
            {
                EditorGUILayout.HelpBox("请选择或创建一个谱面配置文件", MessageType.Info);
            }
            else
            {
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
                
                DrawBeatmapInspector();
                
                EditorGUILayout.EndScrollView();
            }

            // 测试区域
            DrawTestArea();

            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// 绘制谱面检查器
        /// </summary>
        private void DrawBeatmapInspector()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("基础设置", EditorStyles.boldLabel);
            
            currentBeatmap.beatmapName = EditorGUILayout.TextField("谱面名称", currentBeatmap.beatmapName);
            currentBeatmap.musicClip = (AudioClip)EditorGUILayout.ObjectField(
                "音乐文件", 
                currentBeatmap.musicClip, 
                typeof(AudioClip), 
                false
            );
            currentBeatmap.musicDuration = EditorGUILayout.FloatField("音乐时长 (秒)", currentBeatmap.musicDuration);
            currentBeatmap.bpm = EditorGUILayout.FloatField("BPM", currentBeatmap.bpm);
            currentBeatmap.beatsPerMeasure = EditorGUILayout.IntField("每小节拍数", currentBeatmap.beatsPerMeasure);
            currentBeatmap.difficulty = EditorGUILayout.IntSlider("难度", currentBeatmap.difficulty, 1, 5);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("谱面数据", EditorStyles.boldLabel);

            // 自动生成选项
            currentBeatmap.autoGenerateNotes = EditorGUILayout.Toggle("自动生成谱面", currentBeatmap.autoGenerateNotes);
            if (currentBeatmap.autoGenerateNotes)
            {
                EditorGUI.indentLevel++;
                currentBeatmap.notesPerMeasure = EditorGUILayout.IntSlider(
                    "每小节音符数", 
                    currentBeatmap.notesPerMeasure, 
                    1, 
                    8
                );
                
                if (GUILayout.Button("生成谱面"))
                {
                    AutoGenerateBeatmap();
                }
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField($"音符列表 (共 {(currentBeatmap.notes != null ? currentBeatmap.notes.Length : 0)} 个)", EditorStyles.boldLabel);

            if (currentBeatmap.notes != null && currentBeatmap.notes.Length > 0)
            {
                // 音符列表头部
                EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
                GUILayout.Label("序号", GUILayout.Width(40));
                GUILayout.Label("小节", GUILayout.Width(60));
                GUILayout.Label("拍数", GUILayout.Width(60));
                GUILayout.Label("类型", GUILayout.Width(80));
                GUILayout.Label("强度", GUILayout.Width(60));
                GUILayout.Label("强拍", GUILayout.Width(50));
                GUILayout.Label("操作", GUILayout.Width(100));
                EditorGUILayout.EndHorizontal();

                for (int i = 0; i < currentBeatmap.notes.Length; i++)
                {
                    var note = currentBeatmap.notes[i];
                    
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label(i.ToString(), GUILayout.Width(40));
                    
                    // 编辑小节编号
                    note.measure = EditorGUILayout.IntField(note.measure, GUILayout.Width(60));
                    // 编辑拍数
                    note.beatInMeasure = EditorGUILayout.IntField(note.beatInMeasure, GUILayout.Width(60));
                    note.noteType = (NoteType)EditorGUILayout.EnumPopup(note.noteType, GUILayout.Width(80));
                    note.intensity = EditorGUILayout.Slider(note.intensity, 0.5f, 2f, GUILayout.Width(60));
                    note.isStrongBeat = EditorGUILayout.Toggle(note.isStrongBeat, GUILayout.Width(50));
                    
                    if (GUILayout.Button("删除", GUILayout.Width(80)))
                    {
                        DeleteNote(i);
                        break;
                    }
                    
                    EditorGUILayout.EndHorizontal();
                    
                    currentBeatmap.notes[i] = note;
                }

                // 添加音符按钮
                EditorGUILayout.Space();
                if (GUILayout.Button("添加音符"))
                {
                    AddNote();
                }
            }
            else
            {
                EditorGUILayout.HelpBox("暂无音符数据，点击下面的按钮添加或自动生成", MessageType.Info);
            }

            // 保存按钮
            EditorGUILayout.Space();
            if (GUI.changed)
            {
                EditorUtility.SetDirty(currentBeatmap);
            }
            
            if (GUILayout.Button("保存谱面", GUILayout.Height(30)))
            {
                SaveBeatmap();
            }
        }

        /// <summary>
        /// 绘制测试区域
        /// </summary>
        private void DrawTestArea()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("测试区域", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            
            if (!isTesting)
            {
                if (GUILayout.Button("开始测试", GUILayout.Height(30)))
                {
                    StartTest();
                }
            }
            else
            {
                if (GUILayout.Button("停止测试", GUILayout.Height(30)))
                {
                    StopTest();
                }
            }

            if (GUILayout.Button("重置", GUILayout.Height(30)))
            {
                ResetTest();
            }

            EditorGUILayout.EndHorizontal();

            // 进度条
            if (currentBeatmap != null)
            {
                testProgress = EditorGUILayout.Slider("进度", testProgress, 0f, 1f);
                
                // 当前时间显示
                float currentTime = testProgress * currentBeatmap.musicDuration;
                float currentBeat = currentTime / (60f / currentBeatmap.bpm);
                
                EditorGUILayout.LabelField($"时间：{currentTime:F2}s / 节拍：{currentBeat:F1}");
            }

            // 测试日志
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("测试日志", EditorStyles.boldLabel);
            
            // 显示测试状态
            if (isTesting)
            {
                EditorGUILayout.HelpBox("测试进行中... 点击音符进行判定测试", MessageType.Info);
            }
        }

        /// <summary>
        /// 加载谱面
        /// </summary>
        private void LoadBeatmap()
        {
            if (currentBeatmap == null) return;
            
            Debug.Log($"[谱面编辑器] 加载谱面：{currentBeatmap.beatmapName}");
            Debug.Log($"BPM: {currentBeatmap.bpm}, 时长：{currentBeatmap.musicDuration}s");
            Debug.Log($"音符数量：{(currentBeatmap.notes != null ? currentBeatmap.notes.Length : 0)}");
        }

        /// <summary>
        /// 保存谱面
        /// </summary>
        private void SaveBeatmap()
        {
            if (currentBeatmap == null) return;
            
            EditorUtility.SetDirty(currentBeatmap);
            AssetDatabase.SaveAssets();
            Debug.Log($"[谱面编辑器] 谱面已保存：{currentBeatmap.beatmapName}");
        }

        /// <summary>
        /// 自动生成谱面
        /// </summary>
        private void AutoGenerateBeatmap()
        {
            if (currentBeatmap == null) return;
            
            currentBeatmap.AutoGenerateSimpleBeatmap();
            EditorUtility.SetDirty(currentBeatmap);
            Debug.Log($"[谱面编辑器] 已自动生成谱面");
        }

        /// <summary>
        /// 添加音符
        /// </summary>
        private void AddNote()
        {
            if (currentBeatmap == null) return;
            
            if (currentBeatmap.notes == null)
            {
                currentBeatmap.notes = new NoteData[1];
            }
            else
            {
                var newNotes = new NoteData[currentBeatmap.notes.Length + 1];
                currentBeatmap.notes.CopyTo(newNotes, 0);
                currentBeatmap.notes = newNotes;
            }
            
            int lastNoteIndex = currentBeatmap.notes.Length - 1;
            currentBeatmap.notes[lastNoteIndex] = new NoteData
            {
                measure = lastNoteIndex / currentBeatmap.beatsPerMeasure, // 自动计算小节
                beatInMeasure = lastNoteIndex % currentBeatmap.beatsPerMeasure, // 自动计算拍数
                noteType = NoteType.Tap,
                intensity = 1f,
                holdDuration = 1f,
                isStrongBeat = (lastNoteIndex % currentBeatmap.beatsPerMeasure == 0) // 第一拍是强拍
            };
            
            EditorUtility.SetDirty(currentBeatmap);
        }

        /// <summary>
        /// 删除音符
        /// </summary>
        private void DeleteNote(int index)
        {
            if (currentBeatmap == null || index < 0 || index >= currentBeatmap.notes.Length) return;
            
            var newNotes = new NoteData[currentBeatmap.notes.Length - 1];
            int j = 0;
            for (int i = 0; i < currentBeatmap.notes.Length; i++)
            {
                if (i != index)
                {
                    newNotes[j++] = currentBeatmap.notes[i];
                }
            }
            currentBeatmap.notes = newNotes;
            
            EditorUtility.SetDirty(currentBeatmap);
        }

        /// <summary>
        /// 开始测试
        /// </summary>
        private void StartTest()
        {
            if (currentBeatmap == null)
            {
                Debug.LogWarning("[谱面编辑器] 请先选择谱面配置");
                return;
            }
            
            // 获取 RhythmManager 组件（挂载在 RhythmSystem GameObject 上）
            RhythmManager rhythmManager = FindObjectOfType<RhythmManager>();
            if (rhythmManager == null)
            {
                Debug.LogError("[谱面编辑器] 场景中没有找到 RhythmManager 组件，请先设置场景");
                EditorUtility.DisplayDialog("错误", "场景中没有找到 RhythmManager 组件！\n\n请先使用 Tools > Rhythm Game > Setup Scene 设置场景。", "确定");
                return;
            }
            
            Debug.Log($"[谱面编辑器] 找到 RhythmManager 组件，挂载在 GameObject: {rhythmManager.gameObject.name} 上");
            
            // 同步 BPM 配置到 RhythmManager 脚本的私有字段
            rhythmManager.GetType().GetField("bpm", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(rhythmManager, currentBeatmap.bpm);
            Debug.Log($"[谱面编辑器] 已设置 RhythmManager.bpm = {currentBeatmap.bpm}");
            
            // 获取 RhythmNoteVisualizer 组件（同样挂载在 RhythmSystem GameObject 上）
            RhythmNoteVisualizer noteVisualizer = FindObjectOfType<RhythmNoteVisualizer>();
            if (noteVisualizer == null)
            {
                Debug.LogWarning("[谱面编辑器] 场景中没有找到 RhythmNoteVisualizer 组件");
            }
            else
            {
                Debug.Log($"[谱面编辑器] 找到 RhythmNoteVisualizer 组件，挂载在 GameObject: {noteVisualizer.gameObject.name} 上");
                
                // 设置 BeatmapConfig
                var beatmapField = noteVisualizer.GetType().GetField("beatmapConfig", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (beatmapField != null)
                {
                    beatmapField.SetValue(noteVisualizer, currentBeatmap);
                    Debug.Log("[谱面编辑器] 已设置 BeatmapConfig");
                }
                
                // 手动触发音符生成（绕过事件系统）
                Debug.Log("[谱面编辑器] 手动触发音符生成...");
                noteVisualizer.StartMusicInternal(currentBeatmap);
            }
            
            isTesting = true;
            
            // 调用 RhythmManager.StartMusic(clip) 播放音乐
            Debug.Log($"[谱面编辑器] 调用 RhythmManager.StartMusic(clip)");
            rhythmManager.StartMusic(currentBeatmap.musicClip);
            
            Debug.Log("[谱面编辑器] 开始测试 - 音乐已启动");
        }

        /// <summary>
        /// 停止测试
        /// </summary>
        private void StopTest()
        {
            // 获取 RhythmManager
            RhythmManager rhythmManager = FindObjectOfType<RhythmManager>();
            if (rhythmManager != null)
            {
                rhythmManager.StopMusic();
            }
            
            isTesting = false;
            Debug.Log("[谱面编辑器] 停止测试 - 音乐已停止");
        }

        /// <summary>
        /// 重置测试
        /// </summary>
        private void ResetTest()
        {
            testProgress = 0;
            isTesting = false;
            
            // 获取 RhythmManager
            RhythmManager rhythmManager = FindObjectOfType<RhythmManager>();
            if (rhythmManager != null)
            {
                rhythmManager.StopMusic();
            }
            
            Debug.Log("[谱面编辑器] 测试已重置");
        }
    }

    /// <summary>
    /// 音符数据 PropertyDrawer
    /// </summary>
    [CustomPropertyDrawer(typeof(NoteData))]
    public class NoteDataDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            var beatProp = property.FindPropertyRelative("beatInMeasure");
            var typeProp = property.FindPropertyRelative("noteType");
            var intensityProp = property.FindPropertyRelative("intensity");
            
            float width = position.width / 3;
            
            beatProp.intValue = EditorGUI.Popup(
                new Rect(position.x, position.y, width - 5, position.height),
                beatProp.intValue,
                new[] { "拍 1", "拍 2", "拍 3", "拍 4" }
            );
            
            typeProp.intValue = EditorGUI.Popup(
                new Rect(position.x + width, position.y, width - 5, position.height),
                typeProp.intValue,
                new[] { "无", "单点", "长按", "滑动", "双点" }
            );
            
            intensityProp.floatValue = EditorGUI.Slider(
                new Rect(position.x + width * 2, position.y, width - 5, position.height),
                intensityProp.floatValue,
                0.5f, 2f
            );
            
            EditorGUI.EndProperty();
        }
    }
}
#endif
