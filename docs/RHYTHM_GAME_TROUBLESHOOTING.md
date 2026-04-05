# 节奏游戏问题排查指南

本文档提供节奏游戏开发中常见问题的诊断和解决方案。

---

## 问题 1：只能看到一个音符

### 症状
- 点击"开始测试"后，无论谱面配置中添加了多少个音符，屏幕上只出现一个音符
- 或者音符每隔固定时间（如 2 秒）重复出现

### 可能原因及解决方案

#### 原因 A：谱面数据中所有音符的 `measure` 值相同（都是 0）

**诊断方法**：
1. 在 Unity 中打开谱面配置文件（`.asset` 文件）
2. 查看 `Notes` 列表
3. 检查每个音符的 `Measure` 字段

**如果所有音符的 Measure 都是 0**：
- 说明谱面数据是旧的，没有 `measure` 字段
- 需要重新创建谱面配置文件

**解决方案**：
1. 删除旧的谱面配置文件
2. 重新创建新的谱面配置：
   - 在 Project 窗口右键 → Create → Rhythm → Beatmap Config
   - 设置音乐文件、BPM、节拍数等
   - 点击"Auto Generate Simple Beatmap"按钮
   - 或者手动添加音符，确保每个音符的 `Measure` 和 `Beat In Measure` 不同

#### 原因 B：音符生成后没有正确显示

**诊断方法**：
1. 打开 Unity 控制台（Window → General → Console）
2. 点击"开始测试"
3. 查看是否有 `[NoteVisualizer]` 开头的日志

**预期日志**：
```
[NoteVisualizer] 开始生成音符...
[NoteVisualizer] 生成音符：拍数=0, 时间=0.00, 类型=Tap
[NoteVisualizer] 生成音符：拍数=1, 时间=0.50, 类型=Tap
...
```

**如果没有看到日志**：
- 检查 `RhythmNoteVisualizer` 组件是否启用
- 检查 `RhythmNoteVisualizer.beatmapConfig` 是否已设置

**如果日志显示生成但看不到音符**：
1. 检查 `RhythmNoteVisualizer.noteContainer` 是否设置
2. 检查 `RhythmNoteVisualizer.notePrefab` 是否设置
3. 在 Hierarchy 中展开 `NoteContainer`，查看是否有音符子对象

#### 原因 C：音符生成位置在屏幕外

**诊断方法**：
1. 在 Game 视图中，查看场景视图
2. 检查音符是否生成在屏幕上方可见区域外

**解决方案**：
- 调整 `RhythmNoteVisualizer.spawnYPosition` 值
- 确保 `spawnYPosition` 高于摄像机可见范围的上边界

---

## 问题 2：点击音符没有判定反应

### 症状
- 音符正常下落
- 点击音符时没有任何反应（没有判定提示、没有分数变化）

### 可能原因及解决方案

#### 原因 A：`RhythmInputHandler` 未启用鼠标输入

**诊断方法**：
1. 在 Hierarchy 中选择 `InputHandler` 或挂载 `RhythmInputHandler` 的 GameObject
2. 在 Inspector 中查看 `Enable Mouse Input` 复选框

**解决方案**：
- 勾选 `Enable Mouse Input`

#### 原因 B：`RhythmInputHandler.noteVisualizer` 引用未设置

**诊断方法**：
1. 选择 `RhythmInputHandler` 组件
2. 查看 `Note Visualizer` 字段是否为空

**解决方案**：
- 将挂载 `RhythmNoteVisualizer` 的 GameObject 拖拽到 `Note Visualizer` 字段

#### 原因 C：场景中缺少 `EventSystem`

**诊断方法**：
1. 在 Hierarchy 中查找 `EventSystem` GameObject
2. 如果不存在，UI 点击事件无法正常工作

**解决方案**：
- 右键 Hierarchy → UI → Event System
- 或者在菜单：GameObject → UI → Event System

#### 原因 D：音符预制体缺少 `Image` 组件

**诊断方法**：
1. 在 Project 窗口中选择音符预制体（如 `RhythmNote.prefab`）
2. 查看 Inspector 中是否有 `Image` 组件

**解决方案**：
- 确保音符预制体有 `Image` 组件
- `Image` 组件用于 Unity 的射线检测（Raycast）

#### 原因 E：音符不在 Perfect 判定区域内

**诊断方法**：
1. 观察音符下落过程
2. 在音符进入屏幕中心的判定区域时点击

**说明**：
- 判定只在音符进入 Perfect 区域时有效
- Perfect 区域高度 = 音符速度 × Perfect Window 时间
- 默认 Perfect Window 为 0.2 秒

---

## 问题 3：音乐不播放

### 症状
- 点击"开始测试"后，音符开始下落但没有音乐

### 解决方案

1. **检查 `RhythmTestController` 的引用**：
   - `beatmapConfig` 是否设置了音乐文件
   - `rhythmManager` 是否引用了 `RhythmManager` 组件

2. **检查 `RhythmManager` 组件**：
   - 确保场景中有 `RhythmManager` GameObject
   - 确保 `RhythmManager` 组件已挂载

3. **查看控制台错误**：
   - 是否有 `MissingReferenceException` 错误
   - 是否有 `AudioClip` 相关的警告

---

## 问题 4：音符时间与谱面配置不符

### 症状
- 音符出现的时间与预期的节拍不一致
- 音符下落速度看起来不对

### 解决方案

1. **检查 BPM 设置**：
   - BPM（Beats Per Minute）决定音乐节拍速度
   - BPM 越高，音符下落越快

2. **检查音符速度**：
   - `RhythmNoteVisualizer.noteSpeed` 控制音符下落速度（单位：像素/秒）
   - 调整此值使音符下落视觉效果与音乐匹配

3. **验证时间计算**：
   - 音符时间 = (小节 × 每小节拍数 + 拍数) × (60 / BPM)
   - 例如：BPM=120，第 0 小节第 2 拍的音符时间 = (0×4+2)×(60/120) = 1.0 秒

---

## 快速验证步骤

### 完整的测试流程

1. **准备谱面配置**：
   ```
   - 创建新的 BeatmapConfig
   - 设置 AudioClip（音乐文件）
   - 设置 BPM（如 120）
   - 设置 Beats Per Measure（如 4）
   - 设置 Music Duration（音乐时长，秒）
   - 点击"Auto Generate Simple Beatmap"
   ```

2. **设置场景组件**：
   ```
   - 确保场景中有 RhythmManager
   - 确保场景中有 RhythmNoteVisualizer
   - 确保场景中有 RhythmInputHandler
   - 确保场景中有 EventSystem
   - 确保场景中有 RhythmTestController
   ```

3. **配置 RhythmTestController**：
   ```
   - beatmapConfig: 拖入谱面配置文件
   - rhythmManager: 拖入 RhythmManager GameObject
   - noteVisualizer: 拖入 RhythmNoteVisualizer GameObject
   ```

4. **配置 RhythmNoteVisualizer**：
   ```
   - beatmapConfig: 拖入谱面配置文件
   - notePrefab: 拖入 RhythmNote.prefab
   - noteContainer: 创建一个空 GameObject 作为容器并拖入
   - noteSpeed: 设置合适的速度（如 300）
   ```

5. **配置 RhythmInputHandler**：
   ```
   - enableMouseInput: 勾选
   - noteVisualizer: 拖入 RhythmNoteVisualizer GameObject
   ```

6. **运行测试**：
   - 点击 Play
   - 在 RhythmTestController 的 UI 上点击"开始测试"
   - 观察音符生成和下落
   - 在音符进入判定区域时点击音符

---

## 调试技巧

### 启用详细日志

在 `RhythmNoteVisualizer.cs` 中，确保有以下日志：

```csharp
Debug.Log($"[NoteVisualizer] 开始生成音符...");
Debug.Log($"[NoteVisualizer] 生成音符：拍数={noteData.beatInMeasure}, 时间={GetNoteTime():F2}, 类型={noteData.noteType}");
```

### 使用 Gizmos 显示判定区域

在 `RhythmNoteVisualizer.cs` 的 `OnDrawGizmos()` 方法中：

```csharp
private void OnDrawGizmos()
{
    // 显示 Perfect 判定区域
    Gizmos.color = Color.green;
    Vector3 perfectZoneCenter = new Vector3(transform.position.x, perfectZoneY, 0);
    Vector3 perfectZoneSize = new Vector3(100, perfectZoneHeight, 0);
    Gizmos.DrawWireCube(perfectZoneCenter, perfectZoneSize);
}
```

### 检查音符生成数量

在 `RhythmNoteVisualizer.cs` 中添加：

```csharp
private void Update()
{
    Debug.Log($"[NoteVisualizer] 当前活跃音符数：{transform.childCount}");
}
```

---

## 联系支持

如果以上方法都无法解决问题，请提供以下信息：

1. Unity 控制台的完整日志输出
2. 谱面配置文件的截图（显示所有音符数据）
3. `RhythmNoteVisualizer` 组件 Inspector 截图
4. `RhythmInputHandler` 组件 Inspector 截图
5. Hierarchy 中相关 GameObject 的截图
