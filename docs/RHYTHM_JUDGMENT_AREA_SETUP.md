# 节奏游戏判定区域设置指南

本文档详细说明如何设置节奏游戏的判定区域和点击检测系统。

---

## 系统架构概述

当前系统使用 **音符点击判定** 方式，即玩家需要点击正在下落的音符本身来触发判定，而不是在固定区域点击。

### 核心组件

| 组件 | 作用 |
|------|------|
| `RhythmNoteVisualizer` | 音符生成、下落、位置更新 |
| `RhythmInputHandler` | 处理鼠标/触摸/键盘输入 |
| `RhythmManager` | 判定计算、连击统计、事件触发 |
| `RhythmFeedback` | 显示判定结果（Perfect/Normal/Miss） |

---

## 判定流程

```
用户点击屏幕
    ↓
RhythmInputHandler.HandleMouseInput()
    ↓
RhythmNoteVisualizer.ProcessNoteInput(screenPosition)
    ↓
检查点击位置是否在音符范围内 (IsClicked)
    ↓
计算判定结果 (CalculateJudgment)
    ↓
RhythmManager.ProcessInput(judgment)
    ↓
RhythmFeedback 显示判定效果
```

---

## Unity 场景设置步骤

### 步骤 1：创建 Canvas 和 UI 结构

1. **创建 Canvas**
   - 右键 Hierarchy → UI → Canvas
   - 设置 Canvas 的 `Render Mode` 为 `Screen Space - Overlay`

2. **创建音符容器**
   - 在 Canvas 下创建空 GameObject，命名为 `NoteContainer`
   - 添加 `RectTransform` 组件
   - 设置 `Anchor` 为 `stretch-stretch`（全屏拉伸）
   - 设置 `Left=0, Right=0, Top=0, Bottom=0`

3. **创建判定反馈 UI（可选但推荐）**
   - 在 Canvas 下创建空 GameObject，命名为 `FeedbackContainer`
   - 添加 `RectTransform` 组件
   - 设置 `Anchor` 为 `center-middle`
   - 设置 `Width=400, Height=100`
   - 在 `FeedbackContainer` 下创建 Text 子对象，命名为 `JudgmentText`
   - 在 `FeedbackContainer` 下创建 Text 子对象，命名为 `ComboText`

---

### 步骤 2：创建音符预制体

1. **创建音符预制体**
   - 在 Project 窗口右键 → Create → Prefab，命名为 `RhythmNote`
   - 双击打开预制体

2. **添加 Image 组件**
   - Add Component → UI → Image
   - 设置 `Color` 为白色或其他颜色
   - 设置 `Width=80, Height=80`（在 RectTransform 中）

3. **添加 Button 组件（必需）**
   - Add Component → UI → Button
   - Button 组件用于 Unity EventSystem 的射线检测
   - 不需要设置 OnClick 事件（代码会直接处理）

4. **保存预制体**

---

### 步骤 3：设置 RhythmNoteVisualizer

1. **创建 GameObject**
   - 在 Hierarchy 创建空 GameObject，命名为 `NoteVisualizer`

2. **添加组件**
   - Add Component → `RhythmNoteVisualizer`

3. **配置组件字段**
   ```
   ┌─────────────────────────────────────┐
   │ RhythmNoteVisualizer                │
   ├─────────────────────────────────────┤
   │ [容器设置]                          │
   │ Note Container: [NoteContainer]     │ ← 拖入步骤 1 创建的 NoteContainer
   │ Note Prefab: [RhythmNote]           │ ← 拖入步骤 2 创建的预制体
   │                                     │
   │ [判定设置]                          │
   │ Perfect Window: 0.1                 │ ← Perfect 判定容错时间（秒）
   │ Note Speed: 300                     │ ← 音符下落速度（像素/秒）
   │                                     │
   │ [引用]                              │
   │ Rhythm Manager: [RhythmManager]     │ ← 拖入 RhythmManager GameObject
   │ Beatmap Config: [你的谱面配置]       │ ← 拖入谱面配置文件
   └─────────────────────────────────────┘
   ```

---

### 步骤 4：设置 RhythmInputHandler

1. **创建 GameObject**
   - 在 Hierarchy 创建空 GameObject，命名为 `InputHandler`

2. **添加组件**
   - Add Component → `RhythmInputHandler`

3. **配置组件字段**
   ```
   ┌─────────────────────────────────────┐
   │ RhythmInputHandler                  │
   ├─────────────────────────────────────┤
   │ [输入设置]                          │
   │ Enable Mouse Input: ☑               │ ← 勾选（编辑器测试用）
   │ Enable Touch Input: ☐               │ ← 根据需要勾选
   │ Enable Keyboard Input: ☑            │ ← 根据需要勾选
   │                                     │
   │ [输入区域]                          │
   │ Input Area: [留空]                  │ ← 留空表示全屏输入
   │ Min Touch Distance: 0.1             │
   │                                     │
   │ [引用]                              │
   │ Rhythm Manager: [RhythmManager]     │
   │ Note Visualizer: [NoteVisualizer]   │ ← 关键！必须设置
   └─────────────────────────────────────┘
   ```

---

### 步骤 5：设置 RhythmManager

1. **创建 GameObject**
   - 在 Hierarchy 创建空 GameObject，命名为 `RhythmManager`

2. **添加组件**
   - Add Component → `RhythmManager`

3. **配置组件字段**
   ```
   ┌─────────────────────────────────────┐
   │ RhythmManager                       │
   ├─────────────────────────────────────┤
   │ [BPM 设置]                          │
   │ BPM: 120                            │ ← 与谱面配置一致
   │                                     │
   │ [判定窗口]                          │
   │ Perfect Window: 0.1                 │ ← 与 NoteVisualizer 一致
   │ Normal Window: 0.2                  │
   │                                     │
   │ [引用]                              │
   │ Feedback: [RhythmFeedback]          │ ← 可选
   └─────────────────────────────────────┘
   ```

---

### 步骤 6：设置 RhythmFeedback（可选）

1. **创建 GameObject**
   - 在 Hierarchy 创建空 GameObject，命名为 `RhythmFeedback`

2. **添加组件**
   - Add Component → `RhythmFeedback`

3. **配置组件字段**
   ```
   ┌─────────────────────────────────────┐
   │ RhythmFeedback                      │
   ├─────────────────────────────────────┤
   │ [UI 引用]                           │
   │ Judgment Text: [JudgmentText]       │ ← 显示 Perfect/Normal
   │ Combo Text: [ComboText]             │ ← 显示连击数
   │                                     │
   │ [引用]                              │
   │ Rhythm Manager: [RhythmManager]     │
   └─────────────────────────────────────┘
   ```

---

### 步骤 7：设置 RhythmTestController

1. **创建 GameObject**
   - 在 Hierarchy 创建空 GameObject，命名为 `TestController`

2. **添加组件**
   - Add Component → `RhythmTestController`

3. **配置组件字段**
   ```
   ┌─────────────────────────────────────┐
   │ RhythmTestController                │
   ├─────────────────────────────────────┤
   │ [引用]                              │
   │ Beatmap Config: [你的谱面配置]       │
   │ Rhythm Manager: [RhythmManager]     │
   │ Note Visualizer: [NoteVisualizer]   │
   │                                     │
   │ [UI]                                │
   │ Start Button: [开始测试按钮]         │
   └─────────────────────────────────────┘
   ```

---

### 步骤 8：确保有 EventSystem

1. **检查 EventSystem**
   - 在 Hierarchy 中查找 `EventSystem` GameObject
   - 如果不存在，右键 Hierarchy → UI → Event System

2. **EventSystem 组件**
   ```
   ┌─────────────────────────────────────┐
   │ EventSystem                         │
   ├─────────────────────────────────────┤
   │ Send Navigation Events: ☑           │
   │ Pixel Adjustment Threshold: 0.5     │
   │                                     │
   │ [Standalone Input Module]           │
   │ Horizontal Axis: Horizontal         │
   │ Vertical Axis: Vertical             │
   │ Submit Button: Submit               │
   │ Cancel Button: Cancel               │
   └─────────────────────────────────────┘
   ```

---

## 判定区域可视化

### Perfect 判定区域

Perfect 判定区域不是一个固定的 UI 区域，而是根据音符位置动态计算的：

```
Perfect 区域高度 = 音符速度 × Perfect Window 时间

例如：
- 音符速度 = 300 像素/秒
- Perfect Window = 0.1 秒
- Perfect 区域高度 = 300 × 0.1 = 30 像素
```

这意味着当音符中心距离判定线（y=0）不超过 30 像素时，点击会触发 Perfect 判定。

### 在 Scene 视图中显示判定区域

在 `RhythmNoteVisualizer.cs` 中添加以下代码，可以在 Scene 视图中看到判定区域的可视化：

```csharp
private void OnDrawGizmos()
{
    if (!Application.isEditor) return;
    
    // 绘制 Perfect 判定区域（绿色）
    Gizmos.color = new Color(0, 1, 0, 0.3f);
    Vector3 perfectZoneCenter = new Vector3(0, 0, 0);
    Vector3 perfectZoneSize = new Vector3(200, perfectZoneHeight * 2, 0);
    Gizmos.DrawCube(perfectZoneCenter, perfectZoneSize);
    
    // 绘制边界线
    Gizmos.color = Color.green;
    float halfHeight = perfectZoneHeight;
    Vector3 topLine = new Vector3(0, halfHeight, 0);
    Vector3 bottomLine = new Vector3(0, -halfHeight, 0);
    
    // 上边界
    Vector3[] topPoints = new Vector3[]
    {
        new Vector3(-100, halfHeight, 0),
        new Vector3(100, halfHeight, 0)
    };
    Gizmos.DrawLine(topPoints[0], topPoints[1]);
    
    // 下边界
    Vector3[] bottomPoints = new Vector3[]
    {
        new Vector3(-100, -halfHeight, 0),
        new Vector3(100, -halfHeight, 0)
    };
    Gizmos.DrawLine(bottomPoints[0], bottomPoints[1]);
}
```

---

## 常见问题排查

### 问题 1：点击音符没有反应

**检查清单**：
- [ ] `RhythmInputHandler.enableMouseInput` 是否勾选
- [ ] `RhythmInputHandler.noteVisualizer` 是否设置
- [ ] 场景中是否有 `EventSystem`
- [ ] 音符预制体是否有 `Button` 组件
- [ ] 音符预制体是否有 `Image` 组件

### 问题 2：音符不显示

**检查清单**：
- [ ] `RhythmNoteVisualizer.noteContainer` 是否设置
- [ ] `RhythmNoteVisualizer.notePrefab` 是否设置
- [ ] `RhythmNoteVisualizer.beatmapConfig` 是否设置
- [ ] 谱面配置中是否有音符数据

### 问题 3：判定结果不正确

**检查清单**：
- [ ] `RhythmManager.perfectWindow` 与 `RhythmNoteVisualizer.perfectWindow` 是否一致
- [ ] 音符速度是否设置合理（建议 200-500 像素/秒）
- [ ] 点击时机是否在音符进入判定区域时

---

## 调试技巧

### 启用详细日志

在 `RhythmNoteVisualizer.cs` 的 `ProcessNoteInput` 方法中添加：

```csharp
public void ProcessNoteInput(Vector2 screenPosition)
{
    Debug.Log($"[NoteVisualizer] 收到点击：{screenPosition}");
    
    // ... 其余代码
}
```

### 使用 Unity 的 EventSystem 调试

1. 打开菜单：Window → Analysis → Event System
2. 点击音符时查看射线检测结果
3. 确认点击事件正确传递到音符对象

### 检查音符生成日志

运行游戏后，控制台应该显示：

```
[NoteVisualizer] 生成音符：小节 0, 拍 0, 类型 Tap, 时间 0.00s
[NoteVisualizer] 生成音符：小节 0, 拍 1, 类型 Tap, 时间 0.50s
[NoteVisualizer] 生成音符：小节 0, 拍 2, 类型 Tap, 时间 1.00s
...
```

---

## 完整 Hierarchy 结构示例

```
Hierarchy
├── Canvas
│   ├── NoteContainer          ← 音符父对象
│   ├── FeedbackContainer      ← 判定反馈 UI
│   │   ├── JudgmentText
│   │   └── ComboText
│   └── StartButton            ← 开始测试按钮
├── EventSystem                ← 必需！
├── RhythmManager
├── NoteVisualizer
├── InputHandler
├── RhythmFeedback
└── TestController
```

---

## 快速设置脚本

如果你想要快速设置所有组件，可以使用编辑器脚本自动创建：

```csharp
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class RhythmSceneQuickSetup : EditorWindow
{
    [MenuItem("Tools/Rhythm Game/Quick Setup Scene")]
    public static void QuickSetup()
    {
        // 创建 Canvas
        GameObject canvas = new GameObject("Canvas");
        Canvas canvasComponent = canvas.AddComponent<Canvas>();
        canvasComponent.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.AddComponent<CanvasScaler>();
        canvas.AddComponent<GraphicRaycaster>();
        
        // 创建 NoteContainer
        GameObject noteContainer = new GameObject("NoteContainer");
        noteContainer.transform.SetParent(canvas.transform);
        RectTransform noteRect = noteContainer.AddComponent<RectTransform>();
        noteRect.anchorMin = Vector2.zero;
        noteRect.anchorMax = Vector2.one;
        noteRect.offsetMin = Vector2.zero;
        noteRect.offsetMax = Vector2.zero;
        
        // 创建 EventSystem
        GameObject eventSystem = new GameObject("EventSystem");
        eventSystem.AddComponent<EventSystem>();
        eventSystem.AddComponent<StandaloneInputModule>();
        
        Debug.Log("[QuickSetup] 场景设置完成！");
    }
}
```

---

## 总结

判定区域设置的关键点：

1. **音符预制体必须有 Button 组件** - 用于 EventSystem 射线检测
2. **RhythmInputHandler.noteVisualizer 必须设置** - 用于转发点击事件
3. **场景中必须有 EventSystem** - 用于处理 UI 交互
4. **Perfect Window 决定判定严格程度** - 值越小判定越严格

按照以上步骤设置后，点击下落的音符应该可以正确触发判定并显示反馈。
