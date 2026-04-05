# 节奏游戏音符可视化测试指南

## 快速开始测试

### 步骤 1：使用编辑器工具设置场景

1. 在 Unity 顶部菜单栏选择：**Tools > Rhythm Game > Setup Scene**
2. 在打开的窗口中点击 **"设置场景"** 按钮
3. 等待提示"设置完成"

### 步骤 2：配置 RhythmManager

1. 在 Hierarchy 中选择 `RhythmSystem` 对象
2. 在 Inspector 中找到 `RhythmManager` 组件
3. 配置以下字段：

| 字段 | 说明 | 示例值 |
|------|------|--------|
| BPM | 音乐节拍速度 | 110 |
| Beats Per Measure | 每小节拍数 | 4 |

### 步骤 3：配置 BeatmapConfig

1. 在 Project 窗口中右键：**Create > HaChiMiOhNameruDo > Beatmap Config**
2. 命名为 `TestBeatmap`
3. 配置以下字段：

| 字段 | 说明 | 示例值 |
|------|------|--------|
| Beatmap Name | 谱面名称 | TestBeatmap |
| Music Duration | 音乐时长（秒） | 60 |
| BPM | 节拍速度（与 RhythmManager 一致） | 110 |
| Beats Per Measure | 每小节拍数 | 4 |
| Auto Generate Notes | 自动生成谱面 | ✓ |
| Notes Per Measure | 每小节音符数 | 4 |

4. 点击 **"生成谱面"** 按钮自动生成测试谱面

### 步骤 4：配置 RhythmNoteVisualizer

1. 在 Hierarchy 中选择 `RhythmSystem` 对象
2. 在 Inspector 中找到 `RhythmNoteVisualizer` 组件
3. 配置以下字段：

| 字段 | 说明 | 值 |
|------|------|-----|
| Note Container | 拖入 `Canvas/RhythmUI/NoteContainer` | Canvas/RhythmUI/NoteContainer |
| Note Prefab | 拖入 `Assets/Prefabs/RhythmNote.prefab` | Assets/Prefabs/RhythmNote.prefab |
| Perfect Window | Perfect 判定容错时间（秒） | 0.1 |
| Note Speed | 音符下落速度（0=自动） | 0 |
| Rhythm Manager | 拖入 `RhythmSystem` 上的 RhythmManager | RhythmSystem |
| Beatmap Config | 拖入刚才创建的谱面配置 | TestBeatmap |

### 步骤 5：配置 RhythmInputHandler

1. 在 Inspector 中找到 `RhythmInputHandler` 组件
2. 配置以下字段：

| 字段 | 说明 | 值 |
|------|------|-----|
| Note Visualizer | 拖入 `RhythmSystem` 上的 RhythmNoteVisualizer | RhythmSystem |
| Enable Mouse Input | 启用鼠标输入（编辑器测试） | ✓ |
| Enable Touch Input | 启用触摸输入（手机平台） | ✓ |
| Enable Keyboard Input | 启用键盘输入（测试用） | ✓ |

### 步骤 6：开始测试（使用谱面编辑器）

1. 点击 Unity 编辑器顶部的 **Play** 按钮
2. 菜单栏选择：**HaChiMiOhNameruDo > 谱面配置器**
3. 在谱面配置器窗口中，选择刚才创建的 `TestBeatmap`
4. 点击 **"开始测试"** 按钮
5. 音符应从屏幕上方下落
6. 点击音符触发判定

---

## 测试方法

### 方法一：使用谱面编辑器（推荐）

创建一个测试脚本 `RhythmTestController.cs`：

```csharp
using UnityEngine;
using HaChiMiOhNameruDo.Managers;
using HaChiMiOhNameruDo.MiniGames.Rhythm;

public class RhythmTestController : MonoBehaviour
{
    [SerializeField] private RhythmManager rhythmManager;
    [SerializeField] private BeatmapConfig beatmapConfig;
    
    private void OnGUI()
    {
        if (GUILayout.Button("开始音乐", GUILayout.Height(30)))
        {
            rhythmManager.StartMusic(beatmapConfig);
        }
        
        if (GUILayout.Button("停止音乐", GUILayout.Height(30)))
        {
            rhythmManager.StopMusic();
        }
    }
}
```

### 方法二：使用谱面编辑器

1. 菜单栏选择：**HaChiMiOhNameruDo > 谱面配置器**
2. 选择或创建谱面配置
3. 点击 **"开始测试"** 按钮

### 方法三：手动触发

在 Play 模式下，通过 Console 执行以下代码：

```csharp
// 获取 RhythmManager
var rm = FindObjectOfType<RhythmManager>();
// 获取 BeatmapConfig
var bc = FindObjectOfType<BeatmapConfig>();
// 开始音乐
rm.StartMusic(bc);
```

---

## 验证测试

### 预期行为

1. **音符生成**：音乐开始后，音符应从屏幕上方出现
2. **音符下落**：音符应按设定的速度匀速下落
3. **点击判定**：点击音符应触发判定并销毁音符
4. **判定反馈**：Console 应显示判定日志（Perfect/Normal）

### 检查清单

- [ ] 音符从屏幕上方生成
- [ ] 音符匀速下落
- [ ] 点击音符后音符消失
- [ ] Console 显示判定日志
- [ ] 判定结果与点击时机相符

---

## 常见问题排查

### 问题 1：音符不生成

**检查项**：
1. `RhythmNoteVisualizer` 的 `Note Container` 是否设置
2. `Note Prefab` 是否引用了 `RhythmNote.prefab`
3. `Beatmap Config` 是否有音符数据
4. 音乐是否正在播放

**解决方法**：
```csharp
// 在 Console 中检查
var visualizer = FindObjectOfType<RhythmNoteVisualizer>();
Debug.Log($"Note Container: {visualizer.GetType().GetField("noteContainer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(visualizer)}");
Debug.Log($"Note Prefab: {visualizer.GetType().GetField("notePrefab", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(visualizer)}");
Debug.Log($"Beatmap Config: {visualizer.GetType().GetField("beatmapConfig", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(visualizer)}");
```

### 问题 2：音符生成但看不到

**可能原因**：
1. 音符颜色与背景相同
2. Canvas 的 Sorting Order 问题
3. NoteContainer 的 RectTransform 设置不正确

**解决方法**：
1. 检查 `RhythmNote.prefab` 的 Image 颜色（应为白色）
2. 调整 Canvas 的 Sorting Order 为更高的值
3. 确保 NoteContainer 的锚点为 `Stretch-Stretch`

### 问题 3：点击音符没有反应

**检查项**：
1. `RhythmInputHandler` 的 `Note Visualizer` 是否设置
2. Canvas 是否有 `Graphic Raycaster` 组件
3. `RhythmNote.prefab` 的 Image 是否启用了 `Raycast Target`

**解决方法**：
1. 在 `RhythmInputHandler` 中设置 `Note Visualizer` 引用
2. 确保 Canvas 有 `Graphic Raycaster` 组件
3. 检查 `RhythmNote.prefab` 的 Image 组件，勾选 `Raycast Target`

### 问题 4：判定结果不正确

**调整方法**：
1. 修改 `RhythmNoteVisualizer` 的 `Perfect Window` 字段
2. 值越大，Perfect 区域越大，越容易触发 Perfect
3. 推荐值：0.1~0.2 秒

---

## 高级配置

### 调整音符速度

在 `RhythmNoteVisualizer` 中：
- `Note Speed = 0`：自动根据 BPM 计算
- `Note Speed > 0`：使用自定义速度（像素/秒）

### 调整判定区域

Perfect 区域高度 = 音符速度 × Perfect Window

例如：
- 音符速度：540 px/s
- Perfect Window：0.1 秒
- Perfect 区域高度：54 像素

### 自定义音符外观

1. 复制 `RhythmNote.prefab` 创建新预制体
2. 修改 Image 的 Sprite 或颜色
3. 在 `RhythmNoteVisualizer` 中引用新的预制体

---

## 相关文件

- [`RhythmNoteVisualizer.cs`](../Assets/Scripts/MiniGames/Rhythm/RhythmNoteVisualizer.cs) - 音符可视化器
- [`RhythmInputHandler.cs`](../Assets/Scripts/MiniGames/Rhythm/RhythmInputHandler.cs) - 输入处理器
- [`RhythmManager.cs`](../Assets/Scripts/Managers/RhythmManager.cs) - 节奏管理器
- [`BeatmapConfig.cs`](../Assets/Scripts/MiniGames/Rhythm/BeatmapConfig.cs) - 谱面配置
- [`RhythmNote.prefab`](../Assets/Prefabs/RhythmNote.prefab) - 音符预制体
- [`RhythmSceneSetupTool.cs`](../Assets/Scripts/Editor/RhythmSceneSetupTool.cs) - 编辑器设置工具
