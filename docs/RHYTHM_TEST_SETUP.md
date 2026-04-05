# 节奏测试场景设置指南

## 完整设置步骤

### 步骤 1：创建场景结构

#### 1.1 创建 Canvas
1. 在 Hierarchy 中右键 → `UI` → `Canvas`
2. 将 Canvas 的 `Render Mode` 改为 `Screen Space - Camera`（根据之前的 UI 遮挡解决方案）
3. 设置 `Render Camera` 为主相机
4. 设置 `Plane Distance` 为 10

#### 1.2 创建 UI 元素

在 Canvas 下创建以下 UI 元素：

```
Canvas
├── JudgmentText (UI → Text)
│   └── 设置：字体大小 60，居中，金色
├── ComboContainer (空 GameObject)
│   └── ComboText (UI → Text)
│       └── 设置：字体大小 40，居中
├── BeatText (UI → Text)
│   └── 设置：字体大小 24，左上角
├── BPMText (UI → Text)
│   └── 设置：字体大小 20，右上角
├── ProgressSlider (UI → Slider)
│   └── 设置：宽度 80%，底部
├── JudgmentZones (空 GameObject)
│   ├── PerfectZone (UI → Image)
│   ├── GoodZone (UI → Image)
│   └── NormalZone (UI → Image)
└── NoteContainer (空 GameObject)
    └── (音符将在此生成)
```

### 步骤 2：创建音符预制体

#### 2.1 创建音符预制体
1. 在 Hierarchy 中右键 → `UI` → `Image`
2. 命名为 "NotePrefab"
3. 设置 RectTransform：
   - Width: 50
   - Height: 20
   - Anchor: 上中
   - Pivot: 上中 (0.5, 1)
4. 创建一个 Prefab 文件：
   - 在 Project 窗口创建 `Assets/Prefabs/Rhythm` 文件夹
   - 将 NotePrefab 拖入创建预制体

#### 2.2 创建判定线
1. 在 Hierarchy 中右键 → `UI` → `Image`
2. 命名为 "JudgmentLine"
3. 设置 RectTransform：
   - Width: 100
   - Height: 4
   - Anchor: 下中
   - Position Y: 100
4. 颜色设置为白色或亮色

### 步骤 3：创建 RhythmSystem

#### 3.1 创建系统对象
1. 在 Hierarchy 中创建空 GameObject，命名为 "RhythmSystem"
2. 添加以下组件：
   - `RhythmManager`
   - `RhythmInputHandler`
   - `RhythmTestController`
   - `AudioSource`（会自动添加）

#### 3.2 配置 RhythmManager
```
BPM: [您的音乐 BPM，如 110]
Beats Per Measure: 4
Audio Offset: 0
Perfect Window: 0.05
Good Window: 0.1
Normal Window: 0.2
```

#### 3.3 配置 RhythmInputHandler
```
Enable Mouse Input: ✓
Enable Touch Input: ✓
Enable Keyboard Input: ✓
Input Area: null (全屏)
```

#### 3.4 配置 RhythmTestController
```
Music Clip: [拖入您的 30 秒音乐]
Beatmap Config: [拖入您的谱面配置]
Auto Start Music: ✓
Start Delay: 1
```

### 步骤 4：配置可视化组件

#### 4.1 添加 RhythmNoteVisualizer
1. 在 RhythmSystem 上添加 `RhythmNoteVisualizer` 组件
2. 配置：
```
Note Container: [拖入 NoteContainer]
Note Prefab: [拖入音符预制体]
Judgment Line: [拖入 JudgmentLine]
Note Speed: 200
Judgment Line Y: 100
Spawn Ahead Time: 2
Destroy Delay Time: 1
```

#### 4.2 添加 RhythmUIManager
1. 在 RhythmSystem 上添加 `RhythmUIManager` 组件
2. 配置所有 UI 引用：
```
Judgment Text: [拖入 JudgmentText]
Combo Text: [拖入 ComboText]
Combo Container: [拖入 ComboContainer]
Progress Slider: [拖入 ProgressSlider]
BPM Text: [拖入 BPMText]
Beat Text: [拖入 BeatText]
Perfect Zone Image: [拖入 PerfectZone]
Good Zone Image: [拖入 GoodZone]
Normal Zone Image: [拖入 NormalZone]
```

#### 4.3 设置判定区域颜色
```
Perfect Zone Color: RGBA(255, 204, 0, 77)  // 金色半透明
Good Zone Color: RGBA(0, 255, 128, 77)     // 绿色半透明
Normal Zone Color: RGBA(128, 204, 255, 77) // 蓝色半透明
```

### 步骤 5：运行测试

1. 点击 Unity 播放按钮
2. 音乐将在 1 秒延迟后开始
3. 按空格键或点击鼠标进行输入
4. 观察：
   - 音符从顶部下落
   - 到达判定线时进行输入
   - Console 显示判定结果
   - UI 显示 Perfect/Good/Normal/Miss
   - 连击数和倍率更新

### 控制台输出示例

```
[RhythmTest] 音乐开始播放
[RhythmTest] 音乐时长：30.00 秒
[RhythmTest] BPM: 110
[RhythmTest] === 小节 0 开始 ===
[RhythmTest] 强拍 - 小节 0, 节拍 0
[RhythmTest] 玩家输入 - 音乐时间：0.543s, 节拍：1.00
[RhythmTest] 输入判定：PERFECT 连击：1 倍率：1.0x
[NoteVisualizer] 命中：Perfect, 偏移：12.5ms
```

### 快捷键

| 按键 | 功能 |
|------|------|
| 空格 | 触发输入 |
| M | 开始/暂停音乐 |
| R | 重置测试 |

### 判定区域说明

屏幕底部会显示三个半透明彩色区域：
- **金色区域**（中间）：Perfect 判定区（±50ms）
- **绿色区域**（外层）：Good 判定区（±100ms）
- **蓝色区域**（最外层）：Normal 判定区（±200ms）

当音符到达金色区域中心（判定线）时输入可获得 Perfect 判定。

### 故障排除

**问题 1：音符不显示**
- 检查 Note Prefab 是否分配
- 检查 Note Container 是否分配
- 确认音符预制体有 Image 组件

**问题 2：输入没有反应**
- 检查 RhythmInputHandler 的输入是否启用
- 确认 RhythmManager 正在播放（IsPlaying = true）

**问题 3：UI 不显示**
- 检查 Canvas 的 Render Mode
- 确认所有 UI 引用已正确分配
- 检查 Text 组件的 Font 是否有效
