using UnityEngine;
using HaChiMiOhNameruDo.Managers;

namespace HaChiMiOhNameruDo.MiniGames.Rhythm
{
    /// <summary>
    /// 节奏输入处理器
    /// 支持鼠标点击（编辑器测试）和触摸输入（手机平台）
    /// </summary>
    public class RhythmInputHandler : MonoBehaviour
    {
        [Header("输入设置")]
        [Tooltip("是否启用鼠标输入（编辑器测试用）")]
        [SerializeField] private bool enableMouseInput = true;
        
        [Tooltip("是否启用触摸输入（手机平台用）")]
        [SerializeField] private bool enableTouchInput = true;
        
        [Tooltip("是否启用键盘空格键输入（测试用）")]
        [SerializeField] private bool enableKeyboardInput = true;

        [Header("输入区域")]
        [Tooltip("输入判定区域（null 表示全屏）")]
        [SerializeField] private RectTransform inputArea;
        
        [Tooltip("最小触摸距离（防止误触）")]
        [SerializeField] private float minTouchDistance = 0.1f;

        [Header("引用")]
        [Tooltip("节奏管理器引用")]
        [SerializeField] private RhythmManager rhythmManager;
        
        [Tooltip("节奏音符可视化器引用（用于音符点击判定）")]
        [SerializeField] private RhythmNoteVisualizer noteVisualizer;
        
        [Tooltip("节奏判定系统引用（用于判定区域判定）")]
        [SerializeField] private RhythmJudgmentSystem judgmentSystem;

        // 事件
        /// <summary>
        /// 节奏输入事件
        /// </summary>
        public event System.Action OnRhythmInput;

        /// <summary>
        /// 触摸开始事件
        /// </summary>
        public event System.Action<Vector2> OnTouchStarted;

        /// <summary>
        /// 触摸结束事件
        /// </summary>
        public event System.Action<Vector2> OnTouchEnded;

        // 触摸跟踪
        private Vector2[] previousTouchPositions;
        private bool[] wasTouching;

        private void Awake()
        {
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
            
            // 自动获取 RhythmJudgmentSystem
            if (judgmentSystem == null)
            {
                judgmentSystem = FindObjectOfType<RhythmJudgmentSystem>();
            }

            // 初始化触摸跟踪
            previousTouchPositions = new Vector2[10];
            wasTouching = new bool[10];
        }

        private void Update()
        {
            // 鼠标输入（编辑器测试）
            if (enableMouseInput)
            {
                HandleMouseInput();
            }

            // 触摸输入（手机平台）
            if (enableTouchInput)
            {
                HandleTouchInput();
            }

            // 键盘输入（测试用）
            if (enableKeyboardInput)
            {
                HandleKeyboardInput();
            }
        }

        #region 鼠标输入处理

        /// <summary>
        /// 处理鼠标输入
        /// </summary>
        private void HandleMouseInput()
        {
            // 左键点击
            if (Input.GetMouseButtonDown(0))
            {
                if (IsPointInInputArea(Input.mousePosition))
                {
                    // 优先尝试音符点击判定
                    if (noteVisualizer != null)
                    {
                        noteVisualizer.ProcessNoteInput(Input.mousePosition);
                    }
                    // 如果有判定系统，使用判定系统处理
                    else if (judgmentSystem != null)
                    {
                        // 检查点击位置是否在任何一个判定区域内
                        var hitZone = judgmentSystem.GetZoneAtPoint(Input.mousePosition);
                        if (hitZone != null)
                        {
                            // 在判定区域内点击，触发判定系统的输入处理
                            Debug.Log($"[RhythmInput] 在判定区域 {hitZone.name} 内点击");
                            // 由 RhythmJudgmentSystem 通过事件处理
                        }
                        else
                        {
                            // 不在任何判定区域内，使用传统输入方式
                            TriggerRhythmInput();
                        }
                    }
                    else
                    {
                        // 如果没有可视化器或判定系统，使用传统输入方式
                        TriggerRhythmInput();
                    }
                }
            }

            // 右键点击（测试用，触发不同音符类型）
            if (Input.GetMouseButtonDown(1))
            {
                if (IsPointInInputArea(Input.mousePosition))
                {
                    TriggerRhythmInput();
                    Debug.Log("[RhythmInput] 右键输入（滑动音符）");
                }
            }
        }

        #endregion

        #region 触摸输入处理

        /// <summary>
        /// 处理触摸输入
        /// </summary>
        private void HandleTouchInput()
        {
            int touchCount = Input.touchCount;

            // 重置触摸状态
            for (int i = 0; i < wasTouching.Length; i++)
            {
                wasTouching[i] = false;
            }

            // 处理每个触摸
            for (int i = 0; i < touchCount && i < wasTouching.Length; i++)
            {
                Touch touch = Input.GetTouch(i);
                Vector2 touchPos = touch.position;

                // 检查是否在输入区域内
                if (IsPointInInputArea(touchPos))
                {
                    // 新触摸开始
                    if (touch.phase == TouchPhase.Began)
                    {
                        wasTouching[i] = true;
                        previousTouchPositions[i] = touchPos;
                        OnTouchStarted?.Invoke(touchPos);
                        
                        // 尝试音符点击判定
                        if (noteVisualizer != null)
                        {
                            noteVisualizer.ProcessNoteInput(touchPos);
                        }
                        // 如果有判定系统，使用判定系统处理
                        else if (judgmentSystem != null)
                        {
                            var hitZone = judgmentSystem.GetZoneAtPoint(touchPos);
                            if (hitZone != null)
                            {
                                Debug.Log($"[RhythmInput] 触摸在判定区域 {hitZone.name} 内开始");
                            }
                        }
                        
                        Debug.Log($"[RhythmInput] 触摸开始 #{i} at {touchPos}");
                    }
                    // 触摸结束（触发节奏输入）
                    else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                    {
                        if (wasTouching[i])
                        {
                            OnTouchEnded?.Invoke(touchPos);
                            Debug.Log($"[RhythmInput] 触摸结束 #{i} at {touchPos}");
                        }
                    }
                    // 触摸移动
                    else if (touch.phase == TouchPhase.Moved)
                    {
                        // 可以处理滑动音符
                        Vector2 delta = touchPos - previousTouchPositions[i];
                        if (delta.magnitude > minTouchDistance)
                        {
                            Debug.Log($"[RhythmInput] 触摸滑动 #{i} delta={delta}");
                            previousTouchPositions[i] = touchPos;
                        }
                    }
                }
            }
        }

        #endregion

        #region 键盘输入处理

        /// <summary>
        /// 处理键盘输入
        /// </summary>
        private void HandleKeyboardInput()
        {
            // 空格键输入
            if (Input.GetKeyDown(KeyCode.Space))
            {
                TriggerRhythmInput();
                Debug.Log("[RhythmInput] 空格键输入");
            }

            // D 键输入（模拟右击）
            if (Input.GetKeyDown(KeyCode.D))
            {
                TriggerRhythmInput();
                Debug.Log("[RhythmInput] D 键输入（滑动）");
            }

            // F 键输入（模拟双击）
            if (Input.GetKeyDown(KeyCode.F))
            {
                TriggerRhythmInput();
                Debug.Log("[RhythmInput] F 键输入（双击）");
            }
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 检查点是否在输入区域内
        /// </summary>
        private bool IsPointInInputArea(Vector2 screenPoint)
        {
            if (inputArea == null)
            {
                // 没有设置区域，全屏都是输入区域
                return true;
            }

            // 将屏幕坐标转换为 UI 局部坐标
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                inputArea,
                screenPoint,
                null,
                out Vector2 localPoint
            );

            // 检查是否在矩形内
            Rect rect = inputArea.rect;
            return rect.Contains(localPoint);
        }

        /// <summary>
        /// 触发节奏输入
        /// </summary>
        public void TriggerRhythmInput()
        {
            if (rhythmManager == null || !rhythmManager.IsPlaying)
            {
                Debug.LogWarning("[RhythmInput] 节奏管理器未运行，忽略输入");
                return;
            }

            // 触发节奏管理器的输入判定
            rhythmManager.ProcessInput();

            // 触发输入事件
            OnRhythmInput?.Invoke();
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 设置输入区域
        /// </summary>
        public void SetInputArea(RectTransform area)
        {
            inputArea = area;
        }

        /// <summary>
        /// 启用/禁用鼠标输入
        /// </summary>
        public void SetMouseInputEnabled(bool enabled)
        {
            enableMouseInput = enabled;
        }

        /// <summary>
        /// 启用/禁用触摸输入
        /// </summary>
        public void SetTouchInputEnabled(bool enabled)
        {
            enableTouchInput = enabled;
        }

        /// <summary>
        /// 启用/禁用键盘输入
        /// </summary>
        public void SetKeyboardInputEnabled(bool enabled)
        {
            enableKeyboardInput = enabled;
        }

        #endregion

        #region 调试

        private void OnGUI()
        {
            if (!Application.isEditor) return;

            GUILayout.BeginArea(new Rect(10, 10, 300, 200));
            GUILayout.Label("=== 节奏输入调试 ===");
            GUILayout.Label($"鼠标输入：{enableMouseInput}");
            GUILayout.Label($"触摸输入：{enableTouchInput}");
            GUILayout.Label($"键盘输入：{enableKeyboardInput}");
            GUILayout.Label($"触摸数量：{Input.touchCount}");
            
            if (rhythmManager != null)
            {
                GUILayout.Label($"节奏播放中：{rhythmManager.IsPlaying}");
                GUILayout.Label($"当前节拍：{rhythmManager.CurrentBeat}");
                GUILayout.Label($"当前连击：{rhythmManager.CurrentCombo}");
            }

            if (GUILayout.Button("触发测试输入"))
            {
                TriggerRhythmInput();
            }

            GUILayout.EndArea();
        }

        #endregion
    }
}
