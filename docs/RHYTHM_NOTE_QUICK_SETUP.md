# 节奏游戏音符可视化快速设置指南

## 问题：看不到音符预制体

如果你按照之前的文档设置了音符可视化器，但在游戏中看不到音符，请按照以下步骤检查和配置。

---

## 方法一：使用编辑器工具自动设置（推荐）

### 步骤 1：打开设置工具

在 Unity 编辑器顶部菜单栏中，选择：
```
Tools > Rhythm Game > Setup Scene
```

### 步骤 2：点击"设置场景"按钮

在打开的窗口中，点击 **"设置场景"** 按钮。

这将自动创建：
- `Canvas/RhythmUI/NoteContainer` - UI 层级结构
- `RhythmSystem` - 包含所有节奏游戏组件的空对象

### 步骤 3：手动检查引用

虽然工具会自动设置引用，但建议手动检查：

1. 选择 `RhythmSystem` 对象
2. 检查 `RhythmNoteVisualizer` 组件：
   - **Note Container**: 应引用 `Canvas/RhythmUI/NoteContainer`
   - **Note Prefab**: 应引用 `Assets/Prefabs/RhythmNote.prefab`
   - **Perfect Window**: 设置为 `0.1`（或根据需要调整）
   - **Note Speed**: 设置为 `0`（自动根据 BPM 计算）
   - **Rhythm Manager**: 应引用 `RhythmSystem` 上的 `RhythmManager`
   - **Beatmap Config**: 应引用你的谱面配置文件

3. 检查 `RhythmInputHandler` 组件：
   - **Note Visualizer**: 应引用 `RhythmSystem` 上的 `RhythmNoteVisualizer`

---

## 方法二：手动设置

### 步骤 1：创建 UI 层级结构

1. 在 Hierarchy 中找到 `Canvas` 对象（如果没有，创建一个新的）
2. 在 `Canvas` 下创建空对象，命名为 `RhythmUI`
3. 在 `RhythmUI` 下创建空对象，命名为 `NoteContainer`

### 步骤 2：配置 NoteContainer

1. 选择 `NoteContainer` 对象
2. 在 Inspector 中设置 RectTransform：
   - **Anchor Preset**: 选择 `Stretch-Stretch`（按住 Shift 点击）
   - **Left**: `0`
   - **Right**: `0`
   - **Top**: `0`
   - **Bottom**: `0`
   - **Width**: `0`
   - **Height**: `0`

### 步骤 3：创建 RhythmSystem

1. 在 Hierarchy 中创建空对象，命名为 `RhythmSystem`
2. 添加以下组件：
   - `RhythmManager`
   - `RhythmNoteVisualizer`
   - `RhythmInputHandler`
   - `RhythmFeedback`（可选）

### 步骤 4：配置 RhythmNoteVisualizer

在 Inspector 中设置 `RhythmNoteVisualizer` 组件：

| 字段 | 值 |
|------|-----|
| Note Container | 拖入 `Canvas/RhythmUI/NoteContainer` |
| Note Prefab | 拖入 `Assets/Prefabs/RhythmNote.prefab` |
| Perfect Window | `0.1` |
| Note Speed | `0`（自动计算） |
| Rhythm Manager | 拖入 `RhythmSystem` 上的 `RhythmManager` |
| Beatmap Config | 拖入你的谱面配置文件 |

### 步骤 5：配置 RhythmInputHandler

在 Inspector 中设置 `RhythmInputHandler` 组件：

| 字段 | 值 |
|------|-----|
| Note Visualizer | 拖入 `RhythmSystem` 上的 `RhythmNoteVisualizer` |
| Enable Mouse Input | ✓（编辑器测试用） |
| Enable Touch Input | ✓（手机平台用） |
| Enable Keyboard Input | ✓（测试用） |

---

## 验证设置

### 检查清单

- [ ] `Canvas` 存在且正确配置
- [ ] `RhythmUI` 是 `Canvas` 的子对象
- [ ] `NoteContainer` 是 `RhythmUI` 的子对象
- [ ] `NoteContainer` 的 RectTransform 锚点为 `Stretch-Stretch`
- [ ] `RhythmSystem` 对象存在
- [ ] `RhythmNoteVisualizer` 组件已添加并正确配置
- [ ] `RhythmInputHandler` 组件已添加并正确配置
- [ ] `Note Prefab` 引用了 `Assets/Prefabs/RhythmNote.prefab`
- [ ] `Beatmap Config` 引用了有效的谱面配置文件

### 测试步骤

1. 确保 `RhythmManager` 配置了正确的 BPM 和音乐片段
2. 确保 `BeatmapConfig` 中有音符数据
3. 点击 Play 按钮
4. 播放音乐后，音符应从屏幕上方下落
5. 点击音符应触发判定并销毁音符

---

## 常见问题

### 问题 1：音符不生成

**可能原因**：
- `NoteContainer` 或 `Note Prefab` 未设置
- `BeatmapConfig` 为空或没有音符数据
- `RhythmManager` 未播放音乐

**解决方法**：
1. 检查 `RhythmNoteVisualizer` 的引用是否完整
2. 检查 `BeatmapConfig` 是否有音符数据
3. 确保音乐正在播放

### 问题 2：音符生成但看不到

**可能原因**：
- `NoteContainer` 的 RectTransform 设置不正确
- 音符颜色与背景相同
- Canvas 的 Sorting Order 问题

**解决方法**：
1. 检查 `NoteContainer` 的锚点是否为 `Stretch-Stretch`
2. 检查 `RhythmNote.prefab` 的 Image 颜色（应为白色）
3. 调整 Canvas 或 `NoteContainer` 的 Sorting Order

### 问题 3：点击音符没有反应

**可能原因**：
- `RhythmInputHandler` 的 `Note Visualizer` 未设置
- Canvas 的 `Graphic Raycaster` 组件缺失
- 音符的 `Raycast Target` 未启用

**解决方法**：
1. 检查 `RhythmInputHandler` 的引用
2. 确保 Canvas 有 `Graphic Raycaster` 组件
3. 检查 `RhythmNote.prefab` 的 Image 组件，确保 `Raycast Target` 已勾选

---

## 音符预制体说明

`Assets/Prefabs/RhythmNote.prefab` 的配置：

| 组件 | 设置 |
|------|------|
| RectTransform | 50x20 像素，中心锚点 |
| Image | 白色，Raycast Target 已启用 |
| CanvasRenderer | 默认设置 |

如果需要自定义音符外观：
1. 复制 `RhythmNote.prefab` 创建新预制体
2. 修改 Image 的 Sprite 或颜色
3. 在 `RhythmNoteVisualizer` 中引用新的预制体

---

## 判定区域说明

```
屏幕顶部
    │
    ▼
    ┌─────────────────┐
    │                 │
    │   音符生成区    │
    │                 │
    └─────────────────┘
              │
              ▼ 下落
    ┌─────────────────┐
    │   Perfect 区    │ ← 中心区域，容错时间内
    └─────────────────┘
              │
    ┌─────────────────┐
    │   Normal 区     │ ← 其余区域
    └─────────────────┘
              │
              ▼
    ┌─────────────────┐
    │   销毁区        │ ← 离开屏幕后销毁
    └─────────────────┘
屏幕底部
```

**Perfect 区域高度** = 音符速度 × Perfect Window

例如：
- 音符速度：540 px/s（1080p 屏幕，2 节拍下落）
- Perfect Window：0.1 秒
- Perfect 区域高度：54 像素

---

## 相关文件

- [`RhythmNoteVisualizer.cs`](../Assets/Scripts/MiniGames/Rhythm/RhythmNoteVisualizer.cs) - 音符可视化器脚本
- [`RhythmInputHandler.cs`](../Assets/Scripts/MiniGames/Rhythm/RhythmInputHandler.cs) - 输入处理器脚本
- [`RhythmManager.cs`](../Assets/Scripts/Managers/RhythmManager.cs) - 节奏管理器脚本
- [`RhythmNote.prefab`](../Assets/Prefabs/RhythmNote.prefab) - 音符预制体
- [`RhythmSceneSetupTool.cs`](../Assets/Scripts/Editor/RhythmSceneSetupTool.cs) - 编辑器设置工具
