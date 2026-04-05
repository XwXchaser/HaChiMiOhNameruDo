# 节奏判定系统指南

本文档介绍如何使用节奏判定系统（Rhythm Judgment System），包括判定区域、节拍事件和核心判定系统。

## 目录

1. [系统概述](#系统概述)
2. [核心组件](#核心组件)
3. [Unity 场景设置](#unity-场景设置)
4. [代码使用示例](#代码使用示例)
5. [调试指南](#调试指南)

---

## 系统概述

节奏判定系统是一个基于判定区域的节奏游戏核心系统，支持：

- **多判定区域**：可以设置多个不同的判定区域，每个区域有独立的判定参数
- **音符可视化**：下落式音符，点击音符触发判定
- **判定反馈**：Perfect/Good/Normal/Miss 判定等级显示
- **节拍事件**：在特定节拍触发事件（特效、BGM 变化等）

### 判定等级

| 等级 | 说明 | 默认时间窗口 |
|------|------|-------------|
| Perfect | 完美判定 | ±50ms |
| Good | 良好判定 | ±100ms |
| Normal | 普通判定 | ±200ms |
| Miss | 未命中 | 超出以上范围 |

---

## 核心组件

### 1. `RhythmJudgmentSystem.cs`

核心判定系统，负责：
- 管理所有判定区域
- 处理音符点击事件
- 计算判定结果
- 触发判定反馈

```csharp
// 获取组件引用
var judgmentSystem = FindObjectOfType<RhythmJudgmentSystem>();

// 添加判定区域
judgmentSystem.AddJudgmentZone(judgmentZone);

// 移除判定区域
judgmentSystem.RemoveJudgmentZone(judgmentZone);

// 获取点击位置所在的判定区域
var zone = judgmentSystem.GetZoneAtPoint(Input.mousePosition);
```

### 2. `JudgmentZone.cs`

判定区域组件，挂载到带有 `RectTransform` 的 GameObject 上。

```csharp
// 检查点是否在区域内
bool isInZone = judgmentZone.IsPointInZone(screenPoint);

// 根据时间差计算判定
RhythmJudgment judgment = judgmentZone.CalculateJudgment(timeDelta);
```

**Inspector 设置参数：**

| 参数 | 说明 | 默认值 |
|------|------|--------|
| `zoneRect` | 判定区域的 RectTransform | 自动获取 |
| `perfectWindow` | Perfect 判定时间容差（秒） | 0.1 |
| `normalWindow` | Normal 判定时间容差（秒） | 0.2 |
| `showInEditor` | 在编辑器中显示区域边框 | true |
| `showInGame` | 在游戏中显示区域 | false |

### 3. `BeatEvent.cs`

节拍事件系统，定义在特定节拍发生的事件。

**事件类型：**

| 类型 | 说明 |
|------|------|
| `None` | 无事件 |
| `NoteSpawn` | 音符生成 |
| `EffectTrigger` | 特效触发 |
| `BGMChange` | BGM 变化 |
| `VisualChange` | 视觉变化 |

**使用示例：**

```csharp
// 创建节拍事件
var beatEvent = new BeatEvent
{
    eventId = 1,
    eventType = BeatEventType.NoteSpawn,
    measure = 0,
    beatInMeasure = 0,
    eventData = JsonUtility.ToJson(new NoteSpawnData
    {
        noteType = NoteType.Tap,
        intensity = 1.5f,
        isStrongBeat = true
    })
};

// 获取事件时间
float eventTime = beatEvent.GetEventTime(bpm: 110f, beatsPerMeasure: 4);
```

### 4. `RhythmNoteVisualizer.cs`

音符可视化器，显示下落式音符。

**Inspector 设置参数：**

| 参数 | 说明 |
|------|------|
| `noteContainer` | 音符容器（所有音符的父对象） |
| `notePrefab` | 音符预制体 |
| `perfectWindow` | Perfect 判定容错时间 |
| `noteSpeed` | 音符下落速度（0 为自动计算） |
| `rhythmManager` | 节奏管理器引用 |
| `beatmapConfig` | 谱面配置引用 |

### 5. `RhythmInputHandler.cs`

输入处理器，支持鼠标、触摸和键盘输入。

**Inspector 设置参数：**

| 参数 | 说明 | 默认值 |
|------|------|--------|
| `enableMouseInput` | 启用鼠标输入 | true |
| `enableTouchInput` | 启用触摸输入 | true |
| `enableKeyboardInput` | 启用键盘输入 | true |
| `inputArea` | 输入判定区域 | null（全屏） |
| `rhythmManager` | 节奏管理器引用 | 自动获取 |
| `noteVisualizer` | 音符可视化器引用 | 自动获取 |
| `judgmentSystem` | 判定系统引用 | 自动获取 |

---

## Unity 场景设置

### 步骤 1：创建 Canvas 和判定区域

1. 在 Hierarchy 中创建 `Canvas`（如果还没有）
2. 在 Canvas 下创建一个空 GameObject，命名为 `JudgmentZone`
3. 添加 `RectTransform` 组件（如果还没有）
4. 添加 `JudgmentZone` 组件
5. 调整 `RectTransform` 到合适的位置和大小（通常放在屏幕下方）
6. 可选：添加 `Image` 组件作为可视化区域（设置颜色为半透明）

### 步骤 2：创建判定系统

1. 在 Hierarchy 中创建一个空 GameObject，命名为 `RhythmJudgmentSystem`
2. 添加 `RhythmJudgmentSystem` 组件
3. 在 Inspector 中：
   - 将步骤 1 创建的 `JudgmentZone` 拖入 `Judgment Zones` 列表
   - 设置 `Rhythm Manager` 引用
   - 设置 `Rhythm Feedback` 引用

### 步骤 3：创建音符可视化器

1. 在 Canvas 下创建一个空 GameObject，命名为 `NoteContainer`
2. 创建音符预制体：
   - 创建一个 UI Image，命名为 `RhythmNote`
   - 添加 `Button` 组件
   - 调整大小（例如 80x80）
   - 拖入 Project 窗口创建预制体
3. 在 Hierarchy 中创建一个空 GameObject，命名为 `RhythmNoteVisualizer`
4. 添加 `RhythmNoteVisualizer` 组件
5. 在 Inspector 中：
   - 将 `NoteContainer` 拖入 `Note Container`
   - 将音符预制体拖入 `Note Prefab`
   - 设置 `Rhythm Manager` 引用
   - 设置 `Beatmap Config` 引用

### 步骤 4：创建输入处理器

1. 在 Hierarchy 中创建一个空 GameObject，命名为 `RhythmInputHandler`
2. 添加 `RhythmInputHandler` 组件
3. 在 Inspector 中：
   - 设置输入选项（鼠标/触摸/键盘）
   - 将 `Rhythm Manager` 拖入对应字段
   - 将 `RhythmNoteVisualizer` 拖入 `Note Visualizer`
   - 将 `RhythmJudgmentSystem` 拖入 `Judgment System`

### 步骤 5：创建判定反馈 UI

1. 在 Canvas 下创建一个空 GameObject，命名为 `RhythmFeedback`
2. 添加 `RhythmFeedback` 组件
3. 创建子对象：
   - `JudgmentText`：UI Text，显示 Perfect/Good 等
   - `ComboText`：UI Text，显示连击数
4. 在 Inspector 中设置引用

---

## 代码使用示例

### 示例 1：手动触发判定

```csharp
using HaChiMiOhNameruDo.MiniGames.Rhythm;
using HaChiMiOhNameruDo.Managers;

public class ExampleScript : MonoBehaviour
{
    private RhythmJudgmentSystem judgmentSystem;
    
    private void Start()
    {
        judgmentSystem = FindObjectOfType<RhythmJudgmentSystem>();
    }
    
    // 订阅判定事件
    private void OnEnable()
    {
        if (judgmentSystem != null)
        {
            judgmentSystem.OnJudgmentTriggered += HandleJudgment;
        }
    }
    
    private void OnDisable()
    {
        if (judgmentSystem != null)
        {
            judgmentSystem.OnJudgmentTriggered -= HandleJudgment;
        }
    }
    
    private void HandleJudgment(RhythmJudgment judgment, JudgmentZone zone, RhythmNoteVisualizer.NoteVisual note)
    {
        Debug.Log($"判定：{judgment}, 区域：{zone?.name}");
        
        switch (judgment)
        {
            case RhythmJudgment.Perfect:
                // Perfect 逻辑
                break;
            case RhythmJudgment.Good:
                // Good 逻辑
                break;
            case RhythmJudgment.Normal:
                // Normal 逻辑
                break;
            case RhythmJudgment.Miss:
                // Miss 逻辑
                break;
        }
    }
}
```

### 示例 2：动态添加判定区域

```csharp
public class DynamicZoneManager : MonoBehaviour
{
    public RectTransform zonePrefab;
    public RhythmJudgmentSystem judgmentSystem;
    
    public void AddNewZone(Vector2 position, Vector2 size)
    {
        // 创建新的判定区域
        GameObject zoneObj = new GameObject("DynamicJudgmentZone");
        zoneObj.transform.SetParent(transform);
        
        RectTransform rect = zoneObj.AddComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        
        JudgmentZone zone = zoneObj.AddComponent<JudgmentZone>();
        zone.perfectWindow = 0.1f;
        zone.normalWindow = 0.2f;
        
        // 添加到判定系统
        judgmentSystem.AddJudgmentZone(zone);
    }
}
```

### 示例 3：使用节拍事件

```csharp
public class BeatEventHandler : MonoBehaviour
{
    private RhythmManager rhythmManager;
    
    private void Start()
    {
        rhythmManager = FindObjectOfType<RhythmManager>();
        
        if (rhythmManager != null)
        {
            rhythmManager.OnBeatHit += HandleBeatHit;
        }
    }
    
    private void HandleBeatHit(int beat, int measure)
    {
        Debug.Log($"节拍 {beat}, 小节 {measure}");
        
        // 在这里检查是否有节拍事件需要触发
        // 可以使用 BeatmapConfig 中的事件数据
    }
}
```

---

## 调试指南

### 1. 使用 RhythmFeedback 的调试 UI

在编辑器模式下，`RhythmFeedback` 组件会显示一个调试面板，可以手动触发不同判定等级的显示。

### 2. 使用 RhythmInputHandler 的调试 UI

在编辑器模式下，`RhythmInputHandler` 组件会显示一个调试面板，显示当前输入状态和节奏信息。

### 3. 查看 Gizmos

在 Scene 视图中，`JudgmentZone` 组件会绘制青色边框和绿色中心点，方便调整位置。

### 4. 日志输出

所有组件都会输出详细的日志信息，可以在 Console 中查看：

```
[RhythmJudgmentSystem] 判定：Perfect, 区域：JudgmentZone, 音符：Tap
[NoteVisualizer] 生成音符：小节 0, 拍 0, 类型 Tap, 时间 0.00s
[RhythmInput] 在判定区域 JudgmentZone 内点击
```

### 5. 常见问题

**问题：音符点击没有反应**

- 检查 `RhythmNoteVisualizer` 的 `isRunning` 状态
- 确认 `noteContainer` 和 `notePrefab` 已设置
- 检查音符是否在屏幕范围内

**问题：判定区域不显示**

- 检查 `showInEditor` 或 `showInGame` 设置
- 确认 `CanvasGroup` 的 alpha 值
- 在 Scene 视图中查看 Gizmos

**问题：判定结果不准确**

- 调整 `perfectWindow` 和 `normalWindow` 参数
- 检查 BPM 设置是否正确
- 确认音频延迟补偿（audio offset）设置

---

## 文件结构

```
Assets/Scripts/MiniGames/Rhythm/
├── BeatEvent.cs              # 节拍事件系统
├── BeatmapConfig.cs          # 谱面配置
├── JudgmentZone.cs           # 判定区域组件
├── RhythmFeedback.cs         # 判定反馈显示
├── RhythmInputHandler.cs     # 输入处理器
├── RhythmJudgmentSystem.cs   # 核心判定系统
├── RhythmNoteVisualizer.cs   # 音符可视化器
├── RhythmRewardConfig.cs     # 奖励配置（可选）
└── RhythmUIManager.cs        # UI 管理器
```

---

## 相关文档

- [节奏游戏设置指南](RHYTHM_GAME_SETUP.md)
- [判定区域设置指南](RHYTHM_JUDGMENT_AREA_SETUP.md)
- [音符可视化器设置指南](RHYTHM_NOTE_VISUALIZER_SETUP.md)
- [节拍编辑器窗口指南](RHYTHM_BEATMAP_EDITOR.md)
