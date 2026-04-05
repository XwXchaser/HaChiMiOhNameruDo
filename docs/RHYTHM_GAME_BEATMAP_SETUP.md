# 节奏游戏谱面配置指南

## 问题修复说明

**修复日期**: 2026-04-05

### 问题 1：音符不生成或重叠
**原因**: `NoteData` 类缺少 `measure`（小节编号）字段，导致所有音符都被认为是在第 0 小节。

### 问题 2：音符重复生成
**原因**: 已生成的音符离开屏幕被销毁后，系统无法记住它已生成过，导致每 2 秒重复生成一次。

**修复内容**:
1. 在 `NoteData` 类中添加了 `measure` 字段（小节编号，从 0 开始）
2. 修复了 `GetNoteTime()` 和 `GetNoteBeat()` 方法，使用 `measure * beatsPerMeasure + beatInMeasure` 计算总拍数
3. 修复了 `RhythmNoteVisualizer` 中的音符生成逻辑，使用 `HashSet` 记录所有已生成的音符索引

---

## 谱面配置步骤

### 1. 创建谱面配置文件

1. 在 Unity 编辑器中，右键点击 Project 窗口
2. 选择 `Create > HaChiMiOhNameruDo > Rhythm > Beatmap Config`
3. 命名文件（例如：`TestBeatmap.asset`）

### 2. 配置基础信息

在 Inspector 面板中设置：

| 字段 | 说明 | 示例值 |
|------|------|--------|
| Beatmap Name（谱面名称） | 谱面的显示名称 | "测试歌曲" |
| Music Clip（音乐剪辑） | 对应的音频文件 | 从 Project 窗口拖入音频文件 |
| Music Duration（音乐时长） | 音乐时长（秒） | 30 |
| BPM（每分钟节拍数） | 音乐的速度 | 110 |
| Beats Per Measure（每小节拍数） | 每小节的拍数 | 4（表示 4/4 拍） |

### 3. 添加音符数据

**重要**: 音符数据使用 `Measure`（小节）和 `Beat In Measure`（拍数）来定位。

#### 在 Unity Inspector 中的配置表

展开 `Notes` 数组，配置如下：

```
┌─────────────────────────────────────────────────────────────────┐
│ Notes（音符列表）      Size: 16                                 │
├─────────────────────────────────────────────────────────────────┤
│ ▶ Element 0                                                     │
│   ├── Measure（小节）:         0                                │
│   ├── Beat In Measure（拍数）: 0                                │
│   ├── Note Type（音符类型）:   Tap（单点）                      │
│   ├── Intensity（强度）:       1                                │
│   └── Is Strong Beat（强拍）:  ☑                               │
├─────────────────────────────────────────────────────────────────┤
│ ▶ Element 1                                                     │
│   ├── Measure（小节）:         0                                │
│   ├── Beat In Measure（拍数）: 1                                │
│   ├── Note Type（音符类型）:   Tap（单点）                      │
│   ├── Intensity（强度）:       1                                │
│   └── Is Strong Beat（强拍）:  ☐                               │
├─────────────────────────────────────────────────────────────────┤
│ ▶ Element 2                                                     │
│   ├── Measure（小节）:         0                                │
│   ├── Beat In Measure（拍数）: 2                                │
│   ├── Note Type（音符类型）:   Tap（单点）                      │
│   ├── Intensity（强度）:       1                                │
│   └── Is Strong Beat（强拍）:  ☐                               │
├─────────────────────────────────────────────────────────────────┤
│ ▶ Element 3                                                     │
│   ├── Measure（小节）:         0                                │
│   ├── Beat In Measure（拍数）: 3                                │
│   ├── Note Type（音符类型）:   Tap（单点）                      │
│   ├── Intensity（强度）:       1                                │
│   └── Is Strong Beat（强拍）:  ☐                               │
├─────────────────────────────────────────────────────────────────┤
│ ▶ Element 4                                                     │
│   ├── Measure（小节）:         1                                │
│   ├── Beat In Measure（拍数）: 0                                │
│   ├── Note Type（音符类型）:   Tap（单点）                      │
│   ├── Intensity（强度）:       1                                │
│   └── Is Strong Beat（强拍）:  ☑                               │
├─────────────────────────────────────────────────────────────────┤
│ ▶ Element 5                                                     │
│   ├── Measure（小节）:         1                                │
│   ├── Beat In Measure（拍数）: 1                                │
│   ├── Note Type（音符类型）:   Tap（单点）                      │
│   └── ...                                                       │
├─────────────────────────────────────────────────────────────────┤
│ ▶ Element 6                                                     │
│   ├── Measure（小节）:         1                                │
│   ├── Beat In Measure（拍数）: 2                                │
│   └── ...                                                       │
├─────────────────────────────────────────────────────────────────┤
│ ▶ Element 7                                                     │
│   ├── Measure（小节）:         1                                │
│   ├── Beat In Measure（拍数）: 3                                │
│   └── ...                                                       │
├─────────────────────────────────────────────────────────────────┤
│ ▶ Element 8                                                     │
│   ├── Measure（小节）:         2                                │
│   ├── Beat In Measure（拍数）: 0                                │
│   └── ...                                                       │
├─────────────────────────────────────────────────────────────────┤
│ ▶ Element 9                                                     │
│   ├── Measure（小节）:         2                                │
│   ├── Beat In Measure（拍数）: 1                                │
│   └── ...                                                       │
├─────────────────────────────────────────────────────────────────┤
│ ▶ Element 10                                                    │
│   ├── Measure（小节）:         2                                │
│   ├── Beat In Measure（拍数）: 2                                │
│   └── ...                                                       │
├─────────────────────────────────────────────────────────────────┤
│ ▶ Element 11                                                    │
│   ├── Measure（小节）:         2                                │
│   ├── Beat In Measure（拍数）: 3                                │
│   └── ...                                                       │
├─────────────────────────────────────────────────────────────────┤
│ ▶ Element 12                                                    │
│   ├── Measure（小节）:         3                                │
│   ├── Beat In Measure（拍数）: 0                                │
│   └── ...                                                       │
├─────────────────────────────────────────────────────────────────┤
│ ▶ Element 13                                                    │
│   ├── Measure（小节）:         3                                │
│   ├── Beat In Measure（拍数）: 1                                │
│   └── ...                                                       │
├─────────────────────────────────────────────────────────────────┤
│ ▶ Element 14                                                    │
│   ├── Measure（小节）:         3                                │
│   ├── Beat In Measure（拍数）: 2                                │
│   └── ...                                                       │
└─────────────────────────────────────────────────────────────────┘
│ ▶ Element 15                                                    │
│   ├── Measure（小节）:         3                                │
│   ├── Beat In Measure（拍数）: 3                                │
│   └── ...                                                       │
└─────────────────────────────────────────────────────────────────┘
```

#### 音符类型说明

| 类型（Note Type） | 中文名称 | 对应操作 |
|------------------|----------|----------|
| None | 无 | - |
| Tap | 单点音符 | 点击/拍打毛球 |
| Hold | 长按音符 | 按住毛球 |
| Swipe | 滑动音符 | 抽纸巾 |
| DoubleTap | 双点音符 | 同时拍打 |

#### 配置表示例（BPM = 110，4/4 拍）

每小节 4 拍，每拍时长 = 60/110 ≈ 0.545 秒

| 元素编号 | 小节 | 拍数 | 音符类型 | 强度 | 强拍 | 出现时间 |
|:--------:|:----:|:----:|:--------:|:----:|:----:|:--------:|
| 0 | 0 | 0 | Tap（单点） | 1.5 | ☑ | 0.00s |
| 1 | 0 | 1 | Tap（单点） | 1.0 | ☐ | 0.55s |
| 2 | 0 | 2 | Tap（单点） | 1.0 | ☐ | 1.09s |
| 3 | 0 | 3 | Tap（单点） | 1.0 | ☐ | 1.64s |
| 4 | 1 | 0 | Tap（单点） | 1.5 | ☑ | 2.18s |
| 5 | 1 | 1 | Tap（单点） | 1.0 | ☐ | 2.73s |
| 6 | 1 | 2 | Tap（单点） | 1.0 | ☐ | 3.27s |
| 7 | 1 | 3 | Tap（单点） | 1.0 | ☐ | 3.82s |
| 8 | 2 | 0 | Tap（单点） | 1.5 | ☑ | 4.36s |
| 9 | 2 | 1 | Tap（单点） | 1.0 | ☐ | 4.91s |
| 10 | 2 | 2 | Tap（单点） | 1.0 | ☐ | 5.45s |
| 11 | 2 | 3 | Tap（单点） | 1.0 | ☐ | 6.00s |
| 12 | 3 | 0 | Tap（单点） | 1.5 | ☑ | 6.55s |
| 13 | 3 | 1 | Tap（单点） | 1.0 | ☐ | 7.09s |
| 14 | 3 | 2 | Tap（单点） | 1.0 | ☐ | 7.64s |
| 15 | 3 | 3 | Tap（单点） | 1.0 | ☐ | 8.18s |

**时间计算公式**:
```
音符出现时间 (秒) = (小节编号 × 每小节拍数 + 拍数) × (60 ÷ BPM)

例如：第 2 小节第 3 拍，BPM=110
时间 = (2 × 4 + 3) × (60 ÷ 110) = 11 × 0.545 = 6.00 秒
```

---

## 测试步骤

### 方法 1：使用谱面编辑器

1. 打开谱面编辑器：`HaChiMiOhNameruDo > 谱面配置器`
2. 选择谱面配置文件
3. 点击 **"开始测试"**
4. 此时音乐和音符会同时开始

**预期控制台输出**:
```
[谱面编辑器] 找到 RhythmManager 组件，挂载在 GameObject: RhythmSystem 上
[谱面编辑器] 已设置 RhythmManager.bpm = 110
[谱面编辑器] 找到 RhythmNoteVisualizer 组件，挂载在 GameObject: RhythmSystem 上
[谱面编辑器] 已设置 BeatmapConfig
[谱面编辑器] 手动触发音符生成...
[NoteVisualizer] StartMusicInternal 被调用
[NoteVisualizer] 已设置 RhythmManager BPM: 110
[NoteVisualizer] 音符速度：540.00 px/s, Perfect 区域高度：54.00 px
[NoteVisualizer] 生成音符：小节 0, 拍 0, 类型 Tap, 时间 0.00s
[NoteVisualizer] 生成音符：小节 0, 拍 1, 类型 Tap, 时间 0.55s
[NoteVisualizer] 生成音符：小节 0, 拍 2, 类型 Tap, 时间 1.09s
[NoteVisualizer] 生成音符：小节 0, 拍 3, 类型 Tap, 时间 1.64s
[NoteVisualizer] 生成音符：小节 1, 拍 0, 类型 Tap, 时间 2.18s
[NoteVisualizer] 音符可视化已手动启动
[Rhythm] 播放音频：TestSong, 时长：30s
[Rhythm] 音乐开始 - BPM: 110, Beat Duration: 0.545s
```

### 方法 2：在场景中运行

1. 确保场景中有 `RhythmSystem` GameObject（包含 `RhythmManager`、`RhythmNoteVisualizer`、`RhythmInputHandler` 组件）
2. 在 `RhythmNoteVisualizer` 组件中设置 `Beatmap Config`
3. 运行场景
4. 音乐开始后音符会自动生成

---

## 点击判定测试

1. 当音符下落到屏幕中心（Perfect 区域）时
2. 用鼠标点击音符（或触摸屏幕）
3. 查看控制台输出判定结果

**预期输出**:
```
[NoteVisualizer] 判定：Perfect, 距离：25.50
[Rhythm] 输入判定：Perfect, 连击 +1
```

---

## 常见问题

### Q1: 音符不生成？

**检查**:
1. `RhythmNoteVisualizer` 的 `noteContainer` 和 `notePrefab` 是否设置
2. `BeatmapConfig.notes` 数组是否有数据
3. 查看控制台是否有 `[NoteVisualizer] 音符预制体或容器未设置` 警告

### Q2: 所有音符重叠在一起？

**检查**:
1. 确保每个音符的 `Measure` 字段设置不同（递增）
2. 确保音符的 `Beat In Measure` 在 0-3 范围内（4/4 拍）
3. 查看控制台日志中每个音符的生成时间

### Q3: 音符重复出现（每 2 秒一次）？

这是之前版本的 bug，已修复。确保使用最新代码：
1. `RhythmNoteVisualizer` 中使用 `HashSet<int> spawnedNoteIndices` 记录已生成的音符
2. 每个音符使用唯一索引 `measure * 1000 + beatInMeasure`

### Q4: 点击音符没有反应？

**检查**:
1. `RhythmInputHandler` 的 `enableMouseInput` 是否启用
2. `RhythmInputHandler` 的 `noteVisualizer` 引用是否设置
3. 音符预制体是否有 `Image` 组件（用于射线检测）
4. 场景中是否有 `EventSystem` GameObject

### Q5: 音乐不播放？

**检查**:
1. `BeatmapConfig.musicClip` 是否设置音频文件
2. `RhythmSystem` GameObject 上是否有 `AudioSource` 组件（会自动创建）
3. 查看控制台是否有 `[Rhythm] 播放音频：XXX` 日志

---

## 调试技巧

### 查看音符生成日志

在 `RhythmNoteVisualizer.SpawnNote()` 中已添加调试日志：
```csharp
Debug.Log($"[NoteVisualizer] 生成音符：小节{noteData.measure}, 拍{noteData.beatInMeasure}, 类型{noteData.noteType}, 时间{beatmapConfig.GetNoteTime(noteData):F2}s");
```

### 手动计算音符时间

使用公式：`时间 (秒) = (measure * beatsPerMeasure + beatInMeasure) * (60 / BPM)`

例如：BPM=110, 小节=2, 拍=3
```
时间 = (2 * 4 + 3) * (60 / 110) = 11 * 0.545 = 6.00s
```

### 使用自动生成

在谱面配置中勾选 `Auto Generate Notes`，然后点击 `Auto Generate` 按钮，会根据 BPM 自动生成简单谱面。

---

## 文件结构

```
Assets/
├── Scripts/
│   ├── Managers/
│   │   └── RhythmManager.cs          # 节奏管理器（单例）
│   └── MiniGames/Rhythm/
│       ├── BeatmapConfig.cs          # 谱面配置 ScriptableObject
│       ├── RhythmNoteVisualizer.cs   # 音符可视化器
│       ├── RhythmInputHandler.cs     # 输入处理器
│       ├── RhythmFeedback.cs         # 判定反馈
│       └── RhythmUIManager.cs        # UI 管理器
├── Prefabs/
│   └── RhythmNote.prefab             # 音符预制体（50x20 像素）
└── Scenes/
    └── SampleScene.unity             # 测试场景
```

---

## 更新日志

### 2026-04-05 (更新)
- **修复**: 添加 `measure` 字段到 `NoteData` 类
- **修复**: 修复 `GetNoteTime()` 和 `GetNoteBeat()` 方法的时间计算
- **修复**: 修复 `RhythmNoteVisualizer` 的音符生成逻辑
- **修复**: 使用 `HashSet` 记录已生成的音符，防止重复生成
- **新增**: 添加音符生成调试日志
