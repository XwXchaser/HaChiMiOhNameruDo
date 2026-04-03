# 猫咪 Animator 配置指南（呼吸 + 眨眼）

本文档指导如何配置 Unity Animator Controller，仅包含呼吸和眨眼动画。

## 当前动画资源

- `CatBreath.anim` - 呼吸动画（循环）
- `CatBlink.anim` - 眨眼动画（一次）

## 步骤一：打开 Animator 窗口

1. 在 Project 视图中双击 `Assets/Animations/Cat/CatController.controller`
2. 或者选择猫咪对象，在 Inspector 中找到 Animator 组件，点击 `CatController` 旁边的圆圈打开

## 步骤二：添加 Parameters 参数

在 Animator 窗口左侧，点击 **Parameters** 标签页，然后点击 **+** 号添加：

### BlinkTrigger（Trigger 类型）
- 点击 **+** → 选择 **Trigger**
- 命名为 `BlinkTrigger`
- 用途：控制眨眼动画触发

**完成后的 Parameters 列表：**
```
Parameters
└── BlinkTrigger (Trigger)
```

## 步骤三：配置动画状态

### 1. 确认已有状态

确保 Animator 中有以下状态：
- **Idle**（橙色边框，表示默认状态）
- **Blink**

如果没有：
- 将 `CatBreath` 动画 Clip 拖入 Animator 窗口，自动创建为 `Idle` 状态
- 将 `CatBlink` 动画 Clip 拖入 Animator 窗口，自动创建为 `Blink` 状态

### 2. 设置默认状态

如果 `Idle` 不是默认状态（没有橙色边框）：
1. 右键点击 `Idle` 状态
2. 选择 **Set as Layer Default State**

### 3. 配置每个状态的设置

**Idle 状态（呼吸）：**
- 在 Inspector 中设置：
  - **Speed**: 1
  - **Loop**: ✓（勾选）
- Motion: `CatBreath`

**Blink 状态（眨眼）：**
- 在 Inspector 中设置：
  - **Speed**: 1
  - **Loop**: ☐（不勾选）
- Motion: `CatBlink`

## 步骤四：配置状态过渡（连线）

### 1. Idle → Blink 过渡

**创建过渡：**
1. 右键点击 **Idle** 状态
2. 选择 **Make Transition**
3. 点击 **Blink** 状态

**配置过渡：**
1. 点击过渡箭头（从 Idle 指向 Blink 的线）
2. 在 Inspector 中设置：
   - **Has Exit Time**: ☐（不勾选）
   - **Transition Duration**: 0
   - **Transition Offset**: 0
3. 点击 **Conditions** 下的 **+** 添加条件：
   - Parameter: `BlinkTrigger`

### 2. Blink → Idle 过渡

**创建过渡：**
1. 右键点击 **Blink** 状态
2. 选择 **Make Transition**
3. 点击 **Idle** 状态

**配置过渡：**
1. 点击过渡箭头
2. 在 Inspector 中设置：
   - **Has Exit Time**: ✓（勾选）
   - **Exit Time**: 1
   - **Transition Duration**: 0
   - **Transition Offset**: 0
3. **Conditions**: 留空（播放完成后自动返回）

## 步骤五：最终 Animator 结构

```
┌─────────────────────────────────────────────────────┐
│ Parameters:                                         │
│   └── BlinkTrigger (Trigger)                        │
└─────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────┐
│ States:                                             │
│                                                     │
│    ┌─────┐     BlinkTrigger      ┌─────┐           │
│    │ Idle│ ─────────────────────>│Blink│           │
│    └─────┘                        └─────┘           │
│       ▲                              │              │
│       │                              │              │
│       │         Exit Time=1          │              │
│       │                              │              │
│       └──────────────────────────────┘              │
│                                                     │
└─────────────────────────────────────────────────────┘
```

## 步骤六：在代码中触发

### CatBlinkController.cs 自动触发眨眼

`CatBlinkController.cs` 脚本已经实现了自动定时触发眨眼：

```csharp
private void Update()
{
    if (Time.time >= nextBlinkTime)
    {
        TriggerBlink();
        ScheduleNextBlink();
    }
}

public void TriggerBlink()
{
    if (animator != null)
    {
        animator.SetTrigger("BlinkTrigger");
    }
}
```

### 手动触发（测试用）

```csharp
// 在 CatController.cs 或其他脚本中
if (animator != null)
{
    animator.SetTrigger("BlinkTrigger");
}
```

## 检查清单

- [ ] Parameters 中有 `BlinkTrigger`（Trigger 类型）
- [ ] Idle 是默认状态（橙色边框）
- [ ] Idle 状态勾选 Loop
- [ ] Blink 状态不勾选 Loop
- [ ] Idle → Blink 过渡条件为 `BlinkTrigger`
- [ ] Blink → Idle 过渡使用 Exit Time = 1
- [ ] CatBlinkController 脚本已添加到猫咪对象
- [ ] CatBlinkController 的 Animator 引用已设置

## 常见问题

### Q: 眨眼后不返回 Idle？
A: 检查 Blink → Idle 的过渡是否设置了 **Has Exit Time: true** 和 **Exit Time: 1**。

### Q: 眨眼动画不播放？
A: 确保 `CatBlink` 动画 Clip 已正确分配到 Blink 状态的 Motion 字段。

### Q: 呼吸动画不循环？
A: 确保 Idle 状态的 **Loop** 已勾选。

### Q: 眨眼频率太快/太慢？
A: 调整 `CatBlinkController` 脚本中的 `minBlinkInterval` 和 `maxBlinkInterval` 参数。

## 呼吸动画关键帧配置（参考）

根据之前提供的代码案例：

| 时间 | Scale X | Scale Y |
|------|---------|---------|
| 0:00 | 1.000 | 1.000 |
| 0:14 | 1.008 | 1.012 |
| 0:28 | 1.000 | 1.000 |
| 0:42 | 1.008 | 1.012 |
| 0:56 | 1.000 | 1.000 |

- **动画长度**: 0:56（0.56 秒，频率 1.8Hz）
- **Loop**: 勾选

## 眨眼动画关键帧配置（参考）

| 时间 | Sprite |
|------|--------|
| 0:00 | cat_eyes_open |
| 0:03 | cat_eyes_half |
| 0:06 | cat_eyes_closed |
| 0:14 | cat_eyes_closed (保持) |
| 0:17 | cat_eyes_half |
| 0:20 | cat_eyes_open |

- **动画长度**: 0:20（0.20 秒，200ms）
- **Loop**: 不勾选
