# RhythmNoteVisualizer 设置指南（新版）

## 概述

`RhythmNoteVisualizer` 组件提供类似传统音游的音符下落可视化功能。玩家需要**点击音符本身**才能触发判定。

## 核心机制

1. **音符运动**：从屏幕上方按节拍速度下落 → 离开屏幕下方
2. **判定方式**：必须点击到音符本身才能触发判定
3. **判定逻辑**：只判定最接近中心的那个音符
4. **判定结果**：只有 Perfect/Normal，没有 Miss（100% 命中）

---

## UI 层级结构

在现有的 Canvas 下创建以下结构：

```
Canvas
├── RhythmSystem (空对象，挂载 RhythmManager 等组件)
├── RhythmUI (空对象，作为所有节奏 UI 的父容器)
│   └── NoteContainer (空对象，音符容器)
│       └── (音符实例化在这里)
└── FeedbackUI (已有的判定文本和连击显示)
    ├── JudgmentText
    └── ComboText
```

---

## 详细设置步骤

### 步骤 1：创建 RhythmUI 容器

1. 在 Canvas 下创建空对象，命名为 `RhythmUI`
2. 添加 `RectTransform` 组件（自动添加）
3. 设置锚点为 **stretch-stretch**（铺满整个屏幕）
4. 设置 `Left`, `Right`, `Top`, `Bottom` 都为 `0`

### 步骤 2：创建音符容器

1. 在 `RhythmUI` 下创建空对象，命名为 `NoteContainer`
2. 添加 `RectTransform` 组件
3. 设置锚点为 **stretch-stretch**
4. 设置 `Left`, `Right`, `Top`, `Bottom` 都为 `0`
5. 这个容器将作为音符实例化的父对象

### 步骤 3：创建音符预制体

1. 在 `Assets/Prefabs` 下创建新预制体：`GameObject > UI > Image`
2. 命名为 `NotePrefab`
3. 设置 `RectTransform`：
   - 锚点：**center-center**
   - Pivot: `0.5, 0.5`
   - Width: `50`
   - Height: `20`
4. 设置 Image 颜色：`R:255, G:255, B:255, A:255` (白色)
5. 保存为预制体到 `Assets/Prefabs/NotePrefab.prefab`

---

## 组件配置

### RhythmNoteVisualizer 组件

在场景中的 `RhythmSystem` 对象上添加 `RhythmNoteVisualizer` 组件，然后配置：

#### 容器设置
| 字段 | 类型 | 说明 | 配置值 |
|------|------|------|--------|
| Note Container | RectTransform | 音符容器 | 拖入 `NoteContainer` |
| Note Prefab | GameObject | 音符预制体 | 拖入 `NotePrefab` |

#### 判定设置
| 字段 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| Perfect Window | float | 0.1 | Perfect 判定容错时间（秒），例如 0.1 表示±100ms |
| Note Speed | float | 0 | 音符下落速度（像素/秒），如果为 0 则根据 BPM 自动计算 |

#### 引用
| 字段 | 类型 | 说明 |
|------|------|------|
| Rhythm Manager | RhythmManager | 拖入 `RhythmManager` 组件所在对象 |
| Beatmap Config | BeatmapConfig | 拖入谱面配置文件 |

---

## 判定区域说明

### Perfect 区域

- **大小**：自动计算，公式为 `Perfect 区域高度 = 音符速度 × Perfect Window`
- **位置**：屏幕中心（Y=0）
- **判定**：当音符中心点距离屏幕中心 ≤ Perfect 区域高度时，判定为 Perfect

### Normal 区域

- **位置**：Perfect 区域之外的范围
- **判定**：当音符被点击但不在 Perfect 区域内时，判定为 Normal

### 示意图

```
屏幕上方 (Y > 0)
    ↓
[音符生成并下落]
    ↓
┌─────────────────┐
│                 │
│  Normal 区域     │ ← 点击这里 = Normal
│                 │
├─────────────────┤ ← Perfect 区域上边界 (Y = +PerfectZoneHeight)
│ ═══════════════ │
│ ═══════════════ │ ← Perfect 区域 (点击这里 = Perfect)
│ ═══════════════ │
├─────────────────┤ ← Perfect 区域下边界 (Y = -PerfectZoneHeight)
│                 │
│  Normal 区域     │ ← 点击这里 = Normal
│                 │
└─────────────────┘
    ↓
[音符离开屏幕下方 (Y < 0)]
```

---

## 音符速度计算

如果 `Note Speed` 设置为 0，系统会根据 BPM 自动计算：

```
音符速度 = 屏幕高度 / (节拍时长 × 2)
         = Screen.height / ((60 / BPM) × 2)
```

这意味着音符从屏幕顶部到底部需要 2 个节拍的时长。

---

## 音符类型颜色

代码中定义了不同音符类型的颜色：

| 音符类型 | 颜色 |
|----------|------|
| Tap | 白色 |
| Hold | 黄色 |
| Swipe | 青色 |
| DoubleTap | 品红色 |
| 默认 | 灰色 |

---

## 快速设置（最小配置）

如果只想快速测试，只需配置：

1. **Note Container** - 创建一个空对象作为容器
2. **Note Prefab** - 创建一个简单的 UI Image 作为预制体（50x20 像素）
3. **Rhythm Manager** - 场景中已有的 `RhythmManager`
4. **Beatmap Config** - 谱面配置文件

---

## RhythmInputHandler 配置

在 `RhythmInputHandler` 组件中，需要设置：

| 字段 | 类型 | 说明 |
|------|------|------|
| Note Visualizer | RhythmNoteVisualizer | 拖入 `RhythmNoteVisualizer` 组件所在对象 |

这样输入处理器会将点击事件传递给音符可视化器进行判定。

---

## 调试技巧

1. **调整 Perfect 容错**：修改 `Perfect Window` 字段（例如 0.05 表示±50ms，0.1 表示±100ms）

2. **调整音符速度**：如果自动计算的速度不合适，可以设置 `Note Speed` 为具体数值

3. **隐藏可视化**：在 `RhythmTestController` 中按 **V** 键可以切换音符可视化的显示/隐藏

4. **查看日志**：组件会输出以下日志：
   - `[NoteVisualizer] 音符速度：XXX px/s, Perfect 区域高度：XXX px` - 初始化时
   - `[NoteVisualizer] 音符可视化开始` - 当音乐开始时
   - `[NoteVisualizer] 音符可视化结束` - 当音乐结束时
   - `[NoteVisualizer] 判定：XXX, 距离：XXX` - 当音符被击中时

---

## 常见问题

### Q: 音符不显示？
A: 检查以下几点：
1. `Note Container` 是否正确设置
2. `Note Prefab` 是否正确设置
3. `Beatmap Config` 是否有音符数据
4. 音乐是否开始播放（音符只在音乐播放时生成）

### Q: 音符位置不对？
A: 检查 `RectTransform` 的锚点设置，确保：
1. `Note Container` 的锚点是 stretch-stretch
2. `Note Prefab` 的锚点是 center-center

### Q: 点击音符没有反应？
A: 检查：
1. `RhythmInputHandler` 中是否设置了 `Note Visualizer` 引用
2. 音符是否有 `Image` 组件（用于接收点击）
3. 音符是否有 `Button` 组件（自动添加）

### Q: 音符下落速度太快/太慢？
A: 调整 `Note Speed` 参数：
- 设置为 0 = 根据 BPM 自动计算
- 设置为具体数值 = 固定速度（像素/秒）

---

## 正式游戏集成

在正式游戏中，可以通过以下方式隐藏或禁用可视化：

```csharp
// 方法 1：禁用整个组件
noteVisualizer.enabled = false;

// 方法 2：隐藏 GameObject
noteVisualizer.gameObject.SetActive(false);

// 方法 3：使用 RhythmTestController 的方法
rhythmTestController.SetNoteVisualizerEnabled(false);
```

或者添加一个配置选项，让玩家可以选择是否显示音符。
