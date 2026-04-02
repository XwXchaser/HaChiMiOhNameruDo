# 猫咪动画配置指南（代码驱动 - 呼吸 + 眨眼）

本文档指导如何配置猫咪的代码驱动呼吸和眨眼动画。

## 动画实现方案

### 呼吸动画 - 代码驱动
使用 `CatBreathController.cs` 脚本，通过 `sin()` 函数实时计算缩放值：
- **频率**: 1.8Hz（周期约 0.56 秒）
- **幅度**: X 轴 0.8%, Y 轴 1.2%（Y 略大于 X 模拟胸腔起伏）
- **公式**: `scale = 1.0 + sin(breathTime) * amplitude`

### 眨眼动画 - 四态状态机
使用 `CatBlinkController.cs` 脚本，实现四态状态机：

```
Open ──(倒计时 3.5~5.5s 到)──► Closing ──(60ms)──► Closed ──(80ms)──► Opening ──(60ms)──► Open
 帧 0                            帧 1                  帧 2                帧 1                 帧 0
```

整个眨一次眼约 200ms，符合真实猫咪的眨眼节奏。

## 步骤一：添加 CatBreathController 组件

1. 选择猫咪对象
2. 在 Inspector 中点击 **Add Component**
3. 搜索并添加 `CatBreathController`

**默认参数：**
| 参数 | 默认值 | 说明 |
|------|--------|------|
| Breath Frequency | 1.8 | 呼吸频率（Hz） |
| X Amplitude | 0.008 | X 轴缩放幅度（0.8%） |
| Y Amplitude | 0.012 | Y 轴缩放幅度（1.2%） |
| Base Scale | (1, 1, 1) | 基础缩放 |

## 步骤二：添加 CatBlinkController 组件

1. 选择猫咪对象
2. 在 Inspector 中点击 **Add Component**
3. 搜索并添加 `CatBlinkController`

**参数配置：**

### 眨眼间隔参数
| 参数 | 默认值 | 说明 |
|------|--------|------|
| Min Blink Interval | 3.5 | 最小眨眼间隔（秒） |
| Max Blink Interval | 5.5 | 最大眨眼间隔（秒） |

### 眨眼状态时长（毫秒）
| 参数 | 默认值 | 说明 |
|------|--------|------|
| Closing Duration Ms | 60 | 闭眼过程时长 |
| Closed Duration Ms | 80 | 完全闭眼时长 |
| Opening Duration Ms | 60 | 睁眼过程时长 |

### 眼睛精灵（需要分配）
| 参数 | 说明 | 精灵 GUID |
|------|------|-----------|
| Open Sprite | 睁眼状态 | `26298525bd517e6459edf5c1230a36ca` |
| Half Sprite | 半闭眼状态 | `316779de87eb8ed44b49f16f163650fc` |
| Closed Sprite | 完全闭眼 | `efdcbb12a30139e49859b8e19419e7e2` |

**分配精灵步骤：**
1. 在 Project 视图中找到眼睛精灵图片
2. 将睁眼精灵拖到 `Open Sprite` 字段
3. 将半闭眼精灵拖到 `Half Sprite` 字段
4. 将完全闭眼精灵拖到 `Closed Sprite` 字段

## 步骤三：移除不需要的组件

如果猫咪对象上有以下组件，请移除：
- `CatBreath.anim` 和 `CatBlink.anim` Animation Clips
- `CatController.controller` Animator Controller
- `Animator` 组件（如果不再使用）

## 最终组件结构

```
Cat (GameObject)
├── Transform
├── SpriteRenderer
├── CatController (状态管理)
├── CatBreathController (呼吸动画)
└── CatBlinkController (眨眼动画)
```

## 检查清单

- [ ] CatBreathController 已添加
- [ ] CatBlinkController 已添加
- [ ] CatBlinkController 的三个精灵已分配（Open/Half/Closed）
- [ ] 移除了旧的 Animator 相关组件
- [ ] 运行游戏，观察呼吸和眨眼效果

## 调整参数

### 调整呼吸频率
```csharp
// 在代码中调用
catBreathController.SetBreathFrequency(2.0f); // 2Hz
```

### 调整呼吸幅度
```csharp
// 在代码中调用
catBreathController.SetBreathAmplitude(0.01f, 0.015f); // X: 1%, Y: 1.5%
```

### 调整眨眼间隔
```csharp
// 在代码中调用
catBlinkController.SetBlinkInterval(4.0f, 6.0f); // 4~6 秒随机
```

### 调整眨眼状态时长
```csharp
// 在代码中调用
catBlinkController.SetBlinkDurations(50f, 100f, 50f); // Closing: 50ms, Closed: 100ms, Opening: 50ms
```

## 常见问题

### Q: 呼吸效果不明显？
A: 增加 `X Amplitude` 和 `Y Amplitude` 参数值。

### Q: 眨眼太快/太慢？
A: 调整 `Min Blink Interval` 和 `Max Blink Interval` 参数。

### Q: 眨眼动画不流畅？
A: 确保三个精灵（Open/Half/Closed）已正确分配，且图片尺寸一致。

### Q: 猫咪不呼吸/不眨眼？
A: 确保 `CatBreathController` 和 `CatBlinkController` 组件已启用（勾选）。

## 动画时序参考

### 呼吸动画
- **周期**: 0.56 秒（1.8Hz）
- **公式**: `scale = 1.0 + sin(t * 2π * 1.8) * amplitude`
- **X 轴范围**: 0.992 ~ 1.008
- **Y 轴范围**: 0.988 ~ 1.012

### 眨眼动画
```
时间轴（毫秒）:
  0ms ──────────────────────► 3500~5500ms (Open 状态等待)
  3500~5500ms ──► 60ms (Closing 状态)
  60ms ──► 80ms (Closed 状态)
  80ms ──► 60ms (Opening 状态)
  ──► 回到 Open 状态，重新计时 3.5~5.5 秒

总眨眼时长：60 + 80 + 60 = 200ms
```

## 方案对比

### 代码驱动 vs Unity Animation Clip

| 指标 | 代码驱动 | Animation Clip |
|------|----------|----------------|
| 包体大小 | ~7.5KB（脚本） | ~17KB（.anim 文件） |
| CPU 开销 | 极低（简单数学运算） | 低（Unity 优化） |
| 灵活性 | 高（参数可调） | 中（需重新编辑） |
| 美术友好度 | 低 | 高 |
| 可维护性 | 高（逻辑清晰） | 中（文件较多） |

**适用场景：**
- 代码驱动：简单动画（呼吸、待机微动）、程序化动画
- Animation Clip：复杂动画（行走、攻击连招）、美术主导动画
