# 节奏游戏快速开始指南

本文档提供节奏游戏系统的快速配置步骤，适合初次设置的开发者。

## 前置条件

- Unity 2022 或更高版本
- 项目已包含 `RhythmManager.cs` 核心管理器
- 已准备好背景音乐（BGM）音频文件

---

## 步骤 1：创建基础场景结构

### 1.1 创建 Canvas
```
Hierarchy 右键 → UI → Canvas
重命名为 "GameCanvas"
```

**Canvas 设置：**
- Canvas Renderer 组件（自动添加）
- Canvas 组件：
  - Render Mode: Screen Space - Overlay
  - Pixel Perfect: 勾选

### 1.2 创建判定区域容器
```
在 GameCanvas 下创建空 GameObject
重命名为 "JudgmentArea"
添加 RectTransform 组件（如果还没有）
```

**JudgmentArea 位置设置（示例）：**
```
Anchors: 底部居中
Pos X: 0
Pos Y: 100
Width: 800
Height: 200
```

---

## 步骤 2：配置核心组件

### 2.1 添加判定区域组件
```
选中 "JudgmentArea" GameObject
Add Component → JudgmentZone
```

**JudgmentZone 参数设置：**
| 参数 | 值 | 说明 |
|------|-----|------|
| Zone Rect | (自动) | 自动获取 RectTransform |
| Perfect Window | 0.1 | Perfect 判定时间窗口（秒） |
| Normal Window | 0.2 | Normal 判定时间窗口（秒） |
| Show In Editor | ✓ | 编辑器中显示区域边框 |
| Show In Game | ✗ | 游戏中隐藏区域（可选） |

### 2.2 创建判定系统
```
Hierarchy 创建空 GameObject
重命名为 "RhythmJudgmentSystem"
Add Component → RhythmJudgmentSystem
```

**RhythmJudgmentSystem 参数设置：**
| 参数 | 值 |
|------|-----|
| Judgment Zones | [添加 JudgmentArea] |
| Rhythm Manager | [拖入 RhythmManager] |
| Note Visualizer | (稍后设置) |
| Rhythm Feedback | (稍后设置) |

### 2.3 创建音符可视化器
```
在 GameCanvas 下创建空 GameObject
重命名为 "NoteContainer"
```

**创建音符预制体：**
1. 在 NoteContainer 下创建 UI Image
2. 重命名为 "RhythmNote"
3. 设置大小：80x80
4. 添加 Button 组件
5. 设置颜色（白色）
6. 拖入 Project 窗口创建预制体

**添加 RhythmNoteVisualizer 组件：**
```
Hierarchy 创建空 GameObject
重命名为 "RhythmNoteVisualizer"
Add Component → RhythmNoteVisualizer
```

**参数设置：**
| 参数 | 值 |
|------|-----|
| Note Container | [拖入 NoteContainer] |
| Note Prefab | [拖入 RhythmNote 预制体] |
| Perfect Window | 0.1 |
| Note Speed | 0 (自动计算) |
| Rhythm Manager | [拖入 RhythmManager] |
| Beatmap Config | [拖入 BeatmapConfig] |

### 2.4 创建输入处理器
```
Hierarchy 创建空 GameObject
重命名为 "RhythmInputHandler"
Add Component → RhythmInputHandler
```

**参数设置：**
| 参数 | 值 |
|------|-----|
| Enable Mouse Input | ✓ |
| Enable Touch Input | ✓ |
| Enable Keyboard Input | ✓ |
| Rhythm Manager | [拖入 RhythmManager] |
| Note Visualizer | [拖入 RhythmNoteVisualizer] |
| Judgment System | [拖入 RhythmJudgmentSystem] |

### 2.5 创建判定反馈 UI
```
在 GameCanvas 下创建空 GameObject
重命名为 "RhythmFeedback"
Add Component → RhythmFeedback
```

**创建子对象：**
1. 创建 Text - TextMeshPro 或 Legacy Text
2. 重命名为 "JudgmentText"
3. 设置字体大小：48
4. 设置对齐：居中
5. 创建 "ComboText" 同理

**RhythmFeedback 参数设置：**
| 参数 | 值 |
|------|-----|
| Judgment Text | [拖入 JudgmentText] |
| Combo Text | [拖入 ComboText] |
| Rhythm Manager | [拖入 RhythmManager] |

---

## 步骤 3：配置管理器和谱面

### 3.1 RhythmManager 设置
```
找到场景中的 RhythmManager GameObject
```

**参数设置：**
| 参数 | 值 | 说明 |
|------|-----|------|
| BGM Audio Source | [拖入 AudioSource] | 播放 BGM |
| Beatmap Config | [拖入 BeatmapConfig] | 谱面数据 |
| BPM | 110 | 根据音乐设置 |

### 3.2 BeatmapConfig 设置
```
Project 窗口右键 → Create → BeatmapConfig
```

**基础设置：**
| 参数 | 值 |
|------|-----|
| BGM | [拖入音频文件] |
| BPM | 110 |
| Beats Per Measure | 4 |

**添加节拍事件：**
1. 展开 Beat Events 列表
2. 点击 "+" 添加事件
3. 设置事件类型、小节、拍数

---

## 步骤 4：连接所有引用

确保以下引用已正确设置：

### RhythmJudgmentSystem
- ✓ Judgment Zones 包含 JudgmentArea
- ✓ Rhythm Manager 已设置
- ✓ Rhythm Feedback 已设置

### RhythmNoteVisualizer
- ✓ Note Container 已设置
- ✓ Note Prefab 已设置
- ✓ Rhythm Manager 已设置
- ✓ Beatmap Config 已设置

### RhythmInputHandler
- ✓ Rhythm Manager 已设置
- ✓ Note Visualizer 已设置
- ✓ Judgment System 已设置

### RhythmFeedback
- ✓ Judgment Text 已设置
- ✓ Combo Text 已设置
- ✓ Rhythm Manager 已设置

---

## 步骤 5：测试运行

### 5.1 进入 Play 模式
1. 点击 Unity 编辑器 Play 按钮
2. 观察 Console 日志输出

### 5.2 预期日志输出
```
[RhythmManager] 初始化完成，BPM: 110
[RhythmJudgmentSystem] 初始化完成
[NoteVisualizer] 初始化完成，音符速度：500.00
[RhythmInputHandler] 初始化完成
```

### 5.3 测试音符点击
1. 等待音符生成并下落
2. 点击音符
3. 观察 JudgmentText 显示判定结果
4. 观察 ComboText 更新连击数

---

## 常见问题排查

### 问题 1：音符不生成
**检查项：**
- [ ] BeatmapConfig 中是否有节拍事件
- [ ] RhythmManager 是否开始播放音乐
- [ ] NoteContainer 是否存在

### 问题 2：点击音符无反应
**检查项：**
- [ ] RhythmNote 是否有 Button 组件
- [ ] RhythmInputHandler 是否启用鼠标/触摸输入
- [ ] JudgmentSystem 的 OnNoteClicked 事件是否订阅

### 问题 3：判定区域不显示
**检查项：**
- [ ] JudgmentZone 的 showInEditor 是否勾选（编辑器测试）
- [ ] RectTransform 位置是否正确
- [ ] Canvas 层级是否正确

### 问题 4：判定结果不准确
**检查项：**
- [ ] BPM 设置是否与音乐匹配
- [ ] Perfect Window 和 Normal Window 是否合理
- [ ] 音频延迟是否需要补偿

---

## 场景层级结构参考

```
Hierarchy
├── GameCanvas (Canvas)
│   ├── JudgmentArea (RectTransform + JudgmentZone)
│   ├── NoteContainer (RectTransform)
│   │   └── RhythmNote (实例化的音符)
│   └── RhythmFeedback
│       ├── JudgmentText (Text)
│       └── ComboText (Text)
├── RhythmManager
├── RhythmJudgmentSystem
├── RhythmNoteVisualizer
└── RhythmInputHandler
```

---

## 下一步

完成基础设置后，可以进一步配置：

1. **多判定区域**：添加更多 JudgmentZone 实现复杂玩法
2. **节拍事件**：使用 BeatEvent 系统触发特效和 BGM 变化
3. **奖励系统**：配置 RhythmRewardConfig 实现小鱼干奖励
4. **谱面编辑**：使用 BeatmapEditorWindow 可视化编辑谱面

---

## 相关文档

- [节奏判定系统指南](RHYTHM_JUDGMENT_SYSTEM_GUIDE.md) - 详细的 API 和使用说明
- [判定区域设置指南](RHYTHM_JUDGMENT_AREA_SETUP.md) - 判定区域高级配置
- [节拍编辑器窗口指南](RHYTHM_BEATMAP_EDITOR.md) - 谱面编辑工具使用
- [节奏游戏故障排除](RHYTHM_GAME_TROUBLESHOOTING.md) - 常见问题解决
