using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using HaChiMiOhNameruDo.Managers;
using HaChiMiOhNameruDo.MiniGames.Rhythm;

namespace HaChiMiOhNameruDo.Editor
{
    /// <summary>
    /// 节奏游戏场景设置工具
    /// </summary>
    public class RhythmSceneSetupTool : EditorWindow
    {
        private AudioClip musicClip;
        private float bpm = 110f;
        private float perfectWindow = 0.1f;

        [MenuItem("Tools/Rhythm Game/Setup Scene")]
        public static void ShowWindow()
        {
            GetWindow<RhythmSceneSetupTool>("节奏游戏设置");
        }

        private void OnGUI()
        {
            GUILayout.Label("=== 节奏游戏场景设置 ===", EditorStyles.boldLabel);
            GUILayout.Space(10);

            musicClip = (AudioClip)EditorGUILayout.ObjectField("音乐片段", musicClip, typeof(AudioClip), false);
            bpm = EditorGUILayout.FloatField("BPM", bpm);
            perfectWindow = EditorGUILayout.FloatField("Perfect 容错时间 (秒)", perfectWindow);

            GUILayout.Space(10);

            if (GUILayout.Button("设置场景", GUILayout.Height(30)))
            {
                SetupScene();
            }

            GUILayout.Space(5);

            if (GUILayout.Button("仅创建 UI 结构"))
            {
                CreateUIStructure();
            }

            if (GUILayout.Button("仅创建 RhythmSystem"))
            {
                CreateRhythmSystem();
            }

            GUILayout.Space(20);
            GUILayout.Label("=== 说明 ===", EditorStyles.boldLabel);
            GUILayout.Label("1. 点击'设置场景'会自动创建所有必需的对象");
            GUILayout.Label("2. 确保场景中有 Canvas 组件");
            GUILayout.Label("3. 设置完成后请检查各组件的引用是否正确");
        }

        [MenuItem("Tools/Rhythm Game/Setup Scene")]
        public static void SetupScene()
        {
            // 查找或创建 Canvas
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                GameObject canvasObj = new GameObject("Canvas");
                canvas = canvasObj.AddComponent<Canvas>();
                canvasObj.AddComponent<CanvasScaler>();
                canvasObj.AddComponent<GraphicRaycaster>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                Undo.RegisterCreatedObjectUndo(canvasObj, "Create Canvas");
            }

            CreateUIStructure(canvas);
            CreateRhythmSystem();

            Debug.Log("[RhythmSetup] 场景设置完成！");
            EditorUtility.DisplayDialog("设置完成", "节奏游戏场景设置完成！\n\n请检查各组件的引用是否正确。", "确定");
        }

        /// <summary>
        /// 创建 UI 结构（无参数版本，用于按钮调用）
        /// </summary>
        public static void CreateUIStructure()
        {
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                GameObject canvasObj = new GameObject("Canvas");
                canvas = canvasObj.AddComponent<Canvas>();
                canvasObj.AddComponent<CanvasScaler>();
                canvasObj.AddComponent<GraphicRaycaster>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                Undo.RegisterCreatedObjectUndo(canvasObj, "Create Canvas");
            }
            CreateUIStructure(canvas);
        }

        /// <summary>
        /// 创建 UI 结构（带 Canvas 参数版本）
        /// </summary>
        public static void CreateUIStructure(Canvas canvas)
        {
            // 创建 RhythmUI
            GameObject rhythmUI = FindOrCreateChild(canvas.transform, "RhythmUI");
            RectTransform rhythmUIRect = rhythmUI.GetComponent<RectTransform>();
            if (rhythmUIRect == null)
            {
                rhythmUIRect = rhythmUI.AddComponent<RectTransform>();
            }
            
            // 设置锚点为 stretch-stretch
            rhythmUIRect.anchorMin = Vector2.zero;
            rhythmUIRect.anchorMax = Vector2.one;
            rhythmUIRect.sizeDelta = Vector2.zero;
            rhythmUIRect.anchoredPosition = Vector2.zero;

            // 创建 NoteContainer
            GameObject noteContainer = FindOrCreateChild(rhythmUI.transform, "NoteContainer");
            RectTransform noteContainerRect = noteContainer.GetComponent<RectTransform>();
            if (noteContainerRect == null)
            {
                noteContainerRect = noteContainer.AddComponent<RectTransform>();
            }
            
            // 设置锚点为 stretch-stretch
            noteContainerRect.anchorMin = Vector2.zero;
            noteContainerRect.anchorMax = Vector2.one;
            noteContainerRect.sizeDelta = Vector2.zero;
            noteContainerRect.anchoredPosition = Vector2.zero;

            Debug.Log("[RhythmSetup] UI 结构创建完成");
        }

        /// <summary>
        /// 创建 RhythmSystem
        /// </summary>
        public static void CreateRhythmSystem()
        {
            // 创建 RhythmSystem 空对象
            GameObject rhythmSystem = new GameObject("RhythmSystem");
            
            // 添加组件
            RhythmManager rhythmManager = rhythmSystem.AddComponent<RhythmManager>();
            RhythmNoteVisualizer noteVisualizer = rhythmSystem.AddComponent<RhythmNoteVisualizer>();
            RhythmInputHandler inputHandler = rhythmSystem.AddComponent<RhythmInputHandler>();
            RhythmFeedback feedback = rhythmSystem.AddComponent<RhythmFeedback>();

            // 查找 NoteContainer
            RectTransform noteContainer = FindObjectOfType<Canvas>()?.transform.Find("RhythmUI/NoteContainer") as RectTransform;
            
            // 使用 SerializedObject 设置序列化字段
            SetSerializedField(noteVisualizer, "noteContainer", noteContainer);
            SetSerializedField(inputHandler, "noteVisualizer", noteVisualizer);

            Undo.RegisterCreatedObjectUndo(rhythmSystem, "Create RhythmSystem");
            Debug.Log("[RhythmSetup] RhythmSystem 创建完成");
        }

        /// <summary>
        /// 使用 SerializedObject 设置私有序列化字段
        /// </summary>
        private static void SetSerializedField(Object target, string fieldName, Object value)
        {
            SerializedObject so = new SerializedObject(target);
            SerializedProperty property = so.FindProperty(fieldName);
            if (property != null)
            {
                property.objectReferenceValue = value;
                so.ApplyModifiedProperties();
            }
            else
            {
                Debug.LogWarning($"[RhythmSetup] 未找到字段：{fieldName}");
            }
        }

        /// <summary>
        /// 查找或创建子对象
        /// </summary>
        private static GameObject FindOrCreateChild(Transform parent, string name)
        {
            Transform child = parent.Find(name);
            if (child == null)
            {
                child = new GameObject(name).transform;
                child.SetParent(parent);
                child.localPosition = Vector3.zero;
                child.localRotation = Quaternion.identity;
                child.localScale = Vector3.one;
                Undo.RegisterCreatedObjectUndo(child.gameObject, $"Create {name}");
            }
            return child.gameObject;
        }
    }
}
