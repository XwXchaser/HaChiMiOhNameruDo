# 节奏游戏系统设置指南

## 已创建的组件

### 1. RhythmManager（节奏核心）
**路径**: [`Assets/Scripts/Managers/RhythmManager.cs`](Assets/Scripts/Managers/RhythmManager.cs)

**功能**:
- 音乐节拍追踪（BPM、拍号、小节）
- 判定窗口计算（Perfect/Good/Normal/Miss）
- 连击系统和分数倍率
- 音乐播放控制（开始/停止/暂停/恢复）

**使用方式**:
```csharp
// 获取实例
var rhythmManager = RhythmManager.Instance;

// 开始音乐
rhythmManager.StartMusic();

// 处理玩家输入
rhythmManager.ProcessInput();

// 订阅事件
rhythmManager.OnInputJudged += (judgment, combo) => {
    Debug.Log($"判定：{judgment}, 连击：{combo}");
};
```

**配置参数**:
| 参数 | 默认值 | 说明 |
|------|--------|------|
| BPM | 110 | 每分钟节拍数 |
| beatsPerMeasure | 4 | 每小节拍数 |
| audioOffset | 0 | 音频延迟补偿 |
| perfectWindow | 0.05s | Perfect 判定窗口（±50ms） |
| goodWindow | 0.10s | Good 判定窗口（±100ms） |
| normalWindow | 0.20s | Normal 判定窗口（±200ms） |

---

### 2. BeatmapConfig（谱面配置器）
**路径**: [`Assets/Scripts/MiniGames/Rhythm/BeatmapConfig.cs`](Assets/Scripts/MiniGames/Rhythm/BeatmapConfig.cs)

**功能**:
- ScriptableObject 谱面配置
- 音符数据管理（Tap/Hold/Swipe/DoubleTap）
- 自动生成简单谱面
- JSON 导入导出

**创建谱面**:
1. 在 Unity 中右键点击 Project 窗口
2. 选择 `HaChiMiOhNameruDo → Rhythm → Beatmap Config`
3. 配置 BPM、音乐时长、音符列表

**音符类型**:
| 类型 | 说明 | 对应操作 |
|------|------|----------|
| None | 无音符 | - |
| Tap | 单点音符 | 拍打毛球 |
| Hold | 长按音符 | 按住毛球 |
| Swipe | 滑动音符 | 抽纸巾 |
| DoubleTap | 双点音符 | 同时拍打 |

---

### 3. RhythmInputHandler（节奏输入处理器）
**路径**: [`Assets/Scripts/MiniGames/Rhythm/RhythmInputHandler.cs`](Assets/Scripts/MiniGames/Rhythm/RhythmInputHandler.cs)

**功能**:
- 支持鼠标点击（编辑器测试）
- 支持触摸输入（手机平台）
- 支持键盘空格键（测试用）
- 输入区域判定

**使用方式**:
```csharp
// 添加到场景中的 GameObject
var inputHandler = gameObject.AddComponent<RhythmInputHandler>();

// 配置输入方式
inputHandler.SetMouseInputEnabled(true);    // 编辑器测试
inputHandler.SetTouchInputEnabled(true);    // 手机触摸
inputHandler.SetKeyboardInputEnabled(true); // 键盘测试

// 订阅输入事件
inputHandler.OnRhythmInput += () => {
    Debug.Log("玩家输入了！");
};
```

**输入方式**:
| 输入 | 编辑器 | 手机 |
|------|--------|------|
| 单点 | 鼠标左键 | 单指触摸 |
| 滑动 | 鼠标右键 | 滑动触摸 |
| 测试 | 空格键/D 键/F 键 | - |

---

### 4. RhythmFeedback（判定 UI 显示）
**路径**: [`Assets/Scripts/MiniGames/Rhythm/RhythmFeedback.cs`](Assets/Scripts/MiniGames/Rhythm/RhythmFeedback.cs)

**功能**:
- 显示 Perfect/Good/Normal/Miss 判定
- 连击数显示和动画
- 判定颜色区分
- 缩放动画效果

**UI 设置**:
1. 创建 Canvas
2. 添加 Text 组件用于显示判定
3. 添加 Text 组件用于显示连击
4. 将组件拖入 RhythmFeedback 的对应字段

**判定颜色**:
| 判定 | 颜色 |
|------|------|
| Perfect | 金色 |
| Good | 绿色 |
| Normal | 蓝色 |
| Miss | 红色 |

---

### 5. BeatmapEditorWindow（谱面编辑器）
**路径**: [`Assets/Scripts/Editor/BeatmapEditorWindow.cs`](Assets/Scripts/Editor/BeatmapEditorWindow.cs)

**功能**:
- 可视化谱面配置界面
- 音符列表编辑
- 自动生成谱面
- 测试模式

**打开方式**:
1. 在 Unity 菜单栏选择 `HaChiMiOhNameruDo → 谱面配置器`
2. 选择或创建谱面配置文件
3. 配置音符数据
4. 点击"保存谱面"

---

## 快速开始

### 步骤 1：创建谱面配置
1. 在 Project 窗口右键 → `HaChiMiOhNameruDo → Rhythm → Beatmap Config`
2. 命名谱面（如 "TestBeatmap"）
3. 设置 BPM（如 110）
4. 设置音乐时长（如 30 秒）
5. 勾选"自动生成谱面"并点击"生成谱面"

### 步骤 2：设置场景
1. 创建空 GameObject 命名为 "RhythmSystem"
2. 添加 `RhythmManager` 组件
3. 添加 `RhythmInputHandler` 组件
4. 配置输入方式（启用鼠标/触摸/键盘）

### 步骤 3：创建 UI
1. 创建 Canvas
2. 在 Canvas 下创建两个 Text：
   - "JudgmentText" - 显示判定
   - "ComboText" - 显示连击
3. 创建空 GameObject 命名为 "RhythmFeedback"
4. 添加 `RhythmFeedback` 组件
5. 将 Text 组件拖入对应字段

### 步骤 4：测试
1. 点击 Unity 播放按钮
2. 按空格键或点击鼠标进行测试
3. 观察判定和连击显示

---

## 判定标准

| 判定 | 时间窗口 | 分数倍率 |
|------|----------|----------|
| Perfect | ±50ms | 1.5x |
| Good | ±100ms | 1.2x |
| Normal | ±200ms | 1.0x |
| Miss | >200ms | 0.5x |

**连击倍率**:
| 连击数 | 倍率 |
|--------|------|
| 1-4 | 1.0x |
| 5-9 | 1.1x |
| 10-19 | 1.3x |
| 20-29 | 1.5x |
| 30+ | 2.0x (Fever) |

---

## 常见问题

### Q1: 判定不准确？
**A**: 调整 `RhythmManager` 中的判定窗口参数，或设置 `audioOffset` 进行音频延迟补偿。

### Q2: 触摸输入不灵敏？
**A**: 检查 `RhythmInputHandler` 中的 `enableTouchInput` 是否启用，确保输入区域设置正确。

### Q3: 如何在手机上测试？
**A**: 构建到手机后，`RhythmInputHandler` 会自动启用触摸输入。在编辑器中可以使用鼠标或键盘测试。

### Q4: 如何导入我的 30 秒音乐？
**A**: 
1. 将音乐文件放入 `Assets/Audio` 文件夹
2. 在 `BeatmapConfig` 中选择音乐文件
3. 设置正确的 BPM 和时长
4. 生成或手动配置谱面

---

## 下一步开发建议

1. **集成到现有小游戏**
   - 扩展 `FurBallGameManager` 加入节奏判定
   - 扩展 `TissueGameManager` 加入节奏判定

2. **添加视觉效果**
   - 创建音符下落动画
   - 添加命中特效（粒子系统）

3. **添加音频反馈**
   - 命中音效
   - 连击音效

4. **完善谱面编辑器**
   - 添加谱面预览功能
   - 支持谱面导入导出
