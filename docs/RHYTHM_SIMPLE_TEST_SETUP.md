# 节奏测试简易设置指南

## 当前已有组件
- RhythmManager
- RhythmInputHandler
- RhythmFeedback
- RhythmTestController

## 设置步骤

### 步骤 1：创建 RhythmSystem 对象
1. 在 Hierarchy 中右键 → `Create Empty`
2. 命名为 "RhythmSystem"

### 步骤 2：添加组件
在 RhythmSystem 上依次添加以下组件（Add Component）：
1. `RhythmManager`
2. `RhythmInputHandler`
3. `RhythmTestController`

### 步骤 3：配置 RhythmManager
在 Inspector 中设置：
```
BPM: [您的音乐 BPM，例如 110]
Beats Per Measure: 4
Audio Offset: 0
Start Delay: 1
Perfect Window: 0.05
Good Window: 0.1
Normal Window: 0.2
```

### 步骤 4：配置 RhythmInputHandler
在 Inspector 中设置：
```
Enable Mouse Input: ✓ (勾选)
Enable Touch Input: ✓ (勾选)
Enable Keyboard Input: ✓ (勾选)
Input Area: null (留空，表示全屏输入)
```

### 步骤 5：配置 RhythmTestController
在 Inspector 中设置：
```
Music Clip: [拖入您的 30 秒音乐文件]
Beatmap Config: [拖入您创建的 Beatmap Config 文件]
Auto Start Music: ✓ (勾选)
Start Delay: 1
```

### 步骤 6：创建 UI（用于显示判定）

#### 6.1 创建 Canvas
1. 在 Hierarchy 中右键 → `UI` → `Canvas`
2. 如果提示需要 EventSystem，点击"OK"

#### 6.2 创建判定文本
1. 在 Canvas 下右键 → `UI` → `Text - TextMeshPro`（或 Legacy → Text）
2. 命名为 "JudgmentText"
3. 设置 Text 组件：
   - Font Size: 60
   - Alignment: Center
   - Color: 金色 (R:255, G:204, B:0)

#### 6.3 创建连击文本
1. 在 Canvas 下再创建一个 Text
2. 命名为 "ComboText"
3. 设置 Text 组件：
   - Font Size: 40
   - Alignment: Center
   - Color: 白色

#### 6.4 创建空对象作为连击容器
1. 在 Canvas 下创建空 GameObject
2. 命名为 "ComboContainer"
3. 将 ComboText 拖入 ComboContainer 作为子对象

### 步骤 7：配置 RhythmFeedback
1. 在 RhythmSystem 上添加 `RhythmFeedback` 组件
2. 在 Inspector 中分配 UI 引用：
   - Judgment Text: [拖入 JudgmentText]
   - Combo Text: [拖入 ComboText]
   - Combo Container: [拖入 ComboContainer]

### 步骤 8：运行测试
1. 点击 Unity 播放按钮
2. 音乐将在 1 秒后自动开始
3. 按 **空格键** 或 **点击鼠标左键** 进行输入
4. 观察控制台输出和 UI 反馈

## 控制台输出说明

运行后您将看到类似以下的输出：

```
[Rhythm] 音乐开始 - BPM: 110, Beat Duration: 0.545s
[RhythmTest] 音乐开始播放
[RhythmTest] 音乐时长：30.00 秒
[RhythmTest] === 小节 0 开始 ===
[RhythmTest] 强拍 - 小节 0, 节拍 0
[RhythmTest] 玩家输入 - 音乐时间：0.543s, 节拍：1.00
[Rhythm] 判定：Perfect, 连击：1, 倍率：1.0x
```

## UI 显示说明

- **判定文本**：显示 PERFECT/GOOD/NORMAL/MISS
- **连击文本**：显示当前连击数
- **颜色**：
  - Perfect: 金色
  - Good: 绿色
  - Normal: 蓝色
  - Miss: 红色

## 快捷键

| 按键 | 功能 |
|------|------|
| 空格 | 触发节奏输入 |
| M | 开始/暂停音乐 |
| R | 重置测试 |

## 常见问题

### Q: 没有声音？
A: 检查 Music Clip 是否已分配，AudioSource 组件是否存在。

### Q: 输入没有反应？
A: 检查 RhythmInputHandler 的 Enable Mouse/Keyboard Input 是否勾选。

### Q: UI 不显示判定？
A: 检查 RhythmFeedback 组件的 Judgment Text 和 Combo Text 是否已分配。

### Q: 判定不准确？
A: 调整 RhythmManager 中的 Perfect/Good/Normal Window 值，或设置 Audio Offset 进行延迟补偿。

## 下一步

当您成功运行测试后，可以：
1. 在谱面配置器中手动添加音符
2. 调整 BPM 和判定窗口
3. 集成到现有的小游戏中
