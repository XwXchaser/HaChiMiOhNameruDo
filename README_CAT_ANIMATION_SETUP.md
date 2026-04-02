# 猫咪呼吸和眨眼动画配置指南（Unity Animator 版）

本文档将指导您如何使用 Unity 自带的 Animator 组件配置猫咪的呼吸和眨眼动画。

## 核心机制说明

### 呼吸动画
- 使用 Animation Clip 中的 Scale 关键帧
- 锚点在底部中心，向上起伏
- X 方向幅度：0.8%，Y 方向幅度：1.2%（模拟胸腔起伏）
- 周期：约 1.1 秒（频率 1.8Hz）

### 眨眼动画
- **使用 Sprite 序列帧动画**，直接在 Animation Clip 中播放三张图片
- 眨眼间隔：3.5~5.5 秒随机
- 单次眨眼时长：约 200ms
  - 闭眼：60ms（睁眼 → 半闭眼 → 全闭眼）
  - 保持：80ms
  - 睁眼：60ms（全闭眼 → 半闭眼 → 睁眼）
- 三帧动画：
  - 帧 0：睁眼
  - 帧 1：半闭眼
  - 帧 2：全闭眼

## 步骤一：准备精灵图片

### 1. 导入猫咪三帧动画图片

**步骤：**
1. 在 `Assets/Sprites` 文件夹中放入三张猫咪图片：
   - `cat_eyes_open.png`（睁眼）
   - `cat_eyes_half.png`（半闭眼）
   - `cat_eyes_closed.png`（全闭眼）

2. 在 Unity 中选择这些图片，在 Inspector 中设置：
   - **Texture Type**: Sprite (2D and UI)
   - **Sprite Mode**: Single
   - **Pixels Per Unit**: 根据图片分辨率设置（如 100）
   - **Filter Mode**: Bilinear
   - **Compression**: High Quality

### 2. 设置精灵 Pivot（锚点）

**步骤：**
1. 在 Project 视图中选择所有猫咪精灵
2. 点击 **Sprite Editor** 按钮
3. 将 **Pivot** 设置为 **Bottom Center**
4. 点击 **Apply**

## 步骤二：创建呼吸动画 Clip

### 1. 创建 Animation Clip

**步骤：**
1. 在层级视图中选择猫咪对象
2. 在菜单栏选择 **Window > Animation > Animation**（或按 Ctrl+6）
3. 在 Animation 窗口中点击 **Create** 按钮
4. 在弹出的保存对话框中：
   - 路径：`Assets/Animations/Cat/`
   - 文件名：`CatBreath.anim`
   - 点击 **Save**

### 2. 添加呼吸关键帧

**步骤：**
1. 在 Animation 窗口中，确保录制按钮（红色圆点）已启用
2. 在层级视图中选择猫咪对象（有 SpriteRenderer 的对象）
3. 在 Animation 窗口中：
   - 点击 **Add Property** → **Transform** → **Scale**
4. 添加以下关键帧：

| 时间 | Scale X | Scale Y | Scale Z |
|------|---------|---------|---------|
| 0:00 | 1.000   | 1.000   | 1.0     |
| 0:14 | 1.008   | 1.012   | 1.0     |
| 0:28 | 1.000   | 1.000   | 1.0     |

5. 设置动画长度为 **0:28**（0.28 秒）
6. 在 Animation 窗口顶部设置 **Loop Time** 为勾选状态

### 3. 调整曲线（可选）

为了让呼吸更自然，可以使用正弦曲线：

**步骤：**
1. 在 Animation 窗口中点击 **Curves** 视图
2. 右键点击关键帧 → **Flat** 或 **Auto** 平滑曲线
3. 或者手动调整切线手柄，使曲线呈正弦波形状

## 步骤三：创建眨眼动画 Clip（Sprite 序列帧）

### 1. 创建 Sprite 序列动画

**步骤：**
1. 在 Project 视图中选中三张精灵图片（按顺序：睁眼、半闭眼、全闭眼）
2. 将它们直接拖到 **Animation 窗口** 中（不是层级视图）
3. Unity 会自动创建一个 Sprite 序列动画
4. 在弹出的保存对话框中：
   - 路径：`Assets/Animations/Cat/`
   - 文件名：`CatBlink.anim`
   - 点击 **Save**

### 2. 配置眨眼动画

**步骤：**
1. 在 Animation 窗口中选择 `CatBlink` 动画
2. 确认三帧精灵顺序正确
3. 设置每帧的持续时间：
   - 帧 0（睁眼→半闭眼）：0.06 秒
   - 帧 1（半闭眼→全闭眼）：0.06 秒
   - 帧 2（全闭眼）：0.08 秒（保持）
   - 帧 1（全闭眼→半闭眼）：0.06 秒
   - 帧 0（半闭眼→睁眼）：0.06 秒

**注意：** 由于眨眼动画需要来回播放，我们使用以下方法：
- 在 Animation 窗口中设置 **Loop Time** 为 **不勾选**
- 设置 **Wrap Mode** 为 **Clamp Forever**

### 3. 调整帧率

**步骤：**
1. 在 Animation 窗口中，点击齿轮图标
2. 设置 **Sample Rate** 为 60（或更高以确保流畅）

## 步骤四：创建 Animator Controller

### 1. 创建 Animator Controller

**步骤：**
1. 在 Project 视图中右键 → **Create** → **Animator Controller**
2. 命名为 `CatAnimator`
3. 双击打开 Animator 窗口

### 2. 创建状态机

**步骤：**
1. 删除默认的 `Any State` 连接
2. 右键 → **Create State** → **Empty**，创建以下状态：
   - `Idle`（默认状态，橙色边框）
   - `Blink`

### 3. 配置 Idle 状态

**步骤：**
1. 将 `CatBreath` 动画 Clip 拖到 `Idle` 状态上
2. 在 Inspector 中设置：
   - **Speed**: 1.0
   - **Loop**: 勾选

### 4. 配置 Blink 状态

**步骤：**
1. 将 `CatBlink` 动画 Clip 拖到 `Blink` 状态上
2. 在 Inspector 中设置：
   - **Speed**: 1.0
   - **Loop**: 不勾选
   - **Speed Parameter**: 不勾选

### 5. 创建过渡

**步骤：**
1. 右键 `Idle` 状态 → **Make Transition**，拖到 `Blink` 状态
2. 右键 `Blink` 状态 → **Make Transition**，拖回 `Idle` 状态
3. 选择 `Idle → Blink` 过渡，在 Inspector 中设置：
   - **Has Exit Time**: 不勾选
   - **Transition Duration**: 0
   - 点击 **+** 添加条件：
     - Parameter: `Trigger`
     - Name: `BlinkTrigger`
4. 选择 `Blink → Idle` 过渡，在 Inspector 中设置：
   - **Has Exit Time**: 勾选
   - **Exit Time**: 1（播放完成后返回）

### 6. 创建触发器参数

**步骤：**
1. 在 Animator 窗口左侧 **Parameters** 标签页
2. 点击 **+** → **Trigger**
3. 命名为 `BlinkTrigger`

## 步骤五：创建眨眼计时器脚本

由于 Unity Animator 不支持自动随机间隔触发，需要一个小脚本来控制眨眼触发时机：

### 1. 创建 CatBlinkController 脚本

**步骤：**
1. 在 `Assets/Scripts/Gameplay` 文件夹中创建新脚本
2. 命名为 `CatBlinkController.cs`
3. 粘贴以下代码：

```csharp
using UnityEngine;

namespace HaChiMiOhNameruDo.Gameplay
{
    /// <summary>
    /// 猫咪眨眼控制器
    /// 使用 Unity Animator 控制眨眼动画
    /// </summary>
    public class CatBlinkController : MonoBehaviour
    {
        [Header("Animator 组件")]
        [SerializeField] private Animator animator;

        [Header("眨眼参数")]
        [Range(2f, 6f)]
        [SerializeField] private float minBlinkInterval = 3.5f;  // 最小眨眼间隔
        [Range(2f, 8f)]
        [SerializeField] private float maxBlinkInterval = 5.5f;  // 最大眨眼间隔

        private float nextBlinkTime;

        private void Awake()
        {
            if (animator == null)
                animator = GetComponent<Animator>();
        }

        private void Start()
        {
            ScheduleNextBlink();
        }

        private void Update()
        {
            if (Time.time >= nextBlinkTime)
            {
                TriggerBlink();
                ScheduleNextBlink();
            }
        }

        /// <summary>
        /// 触发眨眼动画
        /// </summary>
        public void TriggerBlink()
        {
            if (animator != null)
            {
                animator.SetTrigger("BlinkTrigger");
            }
        }

        /// <summary>
        /// 安排下次眨眼
        /// </summary>
        private void ScheduleNextBlink()
        {
            nextBlinkTime = Time.time + Random.Range(minBlinkInterval, maxBlinkInterval);
        }

        /// <summary>
        /// 设置眨眼间隔范围
        /// </summary>
        public void SetBlinkInterval(float min, float max)
        {
            minBlinkInterval = min;
            maxBlinkInterval = max;
            ScheduleNextBlink();
        }
    }
}
```

## 步骤六：在 Unity 中配置

### 1. 分配 Animator Controller

**步骤：**
1. 在层级视图中选择猫咪对象
2. 确保有 **Animator** 组件
3. 将 `CatAnimator` Controller 拖到 Animator 组件的 **Controller** 字段

### 2. 添加 CatBlinkController 组件

**步骤：**
1. 在猫咪对象上点击 **Add Component**
2. 搜索并添加 `CatBlinkController`
3. 在 Inspector 中设置：
   - **Animator**: 拖入 Animator 组件
   - **Min Blink Interval**: 3.5
   - **Max Blink Interval**: 5.5

### 3. 配置 SpriteRenderer

**步骤：**
1. 确保猫咪对象有 **SpriteRenderer** 组件
2. 将默认的睁眼精灵拖到 **Sprite** 字段

## 步骤七：测试动画

**步骤：**
1. 点击 Unity 编辑器顶部的 **Play** 按钮
2. 观察猫咪对象：
   - 身体应该有轻微的呼吸缩放（循环播放）
   - 眼睛应该每隔 3.5~5.5 秒眨一次（播放眨眼动画序列）

## 扩展：添加更多动画状态

### 1. 创建其他动画 Clip

按照同样的方法创建：
- `CatJump.anim` - 跳跃动画
- `CatPaws.anim` - 拍击动画
- `CatBack.anim` - 背对动画

### 2. 更新 Animator Controller

**步骤：**
1. 在 Animator 窗口中创建新状态
2. 将对应的动画 Clip 拖入
3. 创建过渡和触发器

### 3. 在 CatController 中控制

```csharp
[SerializeField] private Animator animator;

private static readonly int IdleHash = Animator.StringToHash("Idle");
private static readonly int JumpHash = Animator.StringToHash("Jump");
private static readonly int PawsHash = Animator.StringToHash("Paws");
private static readonly int BackHash = Animator.StringToHash("Back");

public void SetIdle()
{
    animator.SetTrigger(IdleHash);
}

public void DoJump()
{
    animator.SetTrigger(JumpHash);
}

public void DoPaws()
{
    animator.SetTrigger(PawsHash);
}

public void SetBack()
{
    animator.SetTrigger(BackHash);
}
```

## 常见问题

### Q: 动画不流畅
A: 确保图片的 Pixels Per Unit 设置正确，Filter Mode 使用 Bilinear。

### Q: 呼吸幅度过大/过小
A: 在 Animation 窗口中调整 Scale 关键帧的值。

### Q: 眨眼频率不自然
A: 调整 CatBlinkController 中的 Min/Max Blink Interval 参数。

### Q: 锚点位置不对
A: 在 Sprite Editor 中重新设置 Pivot 为 Bottom Center。

### Q: 眨眼动画播放一次后不继续
A: 这是正常行为。眨眼动画由 CatBlinkController 脚本控制触发时机，每次触发播放一次后返回 Idle 状态。

## 文件结构

```
Assets/
├── Animations/
│   └── Cat/
│       ├── CatBreath.anim       (呼吸动画，循环播放)
│       ├── CatBlink.anim        (眨眼动画，Sprite 序列帧)
│       └── CatAnimator.controller
├── Scripts/
│   └── Gameplay/
│       └── CatBlinkController.cs
└── Sprites/
    ├── cat_eyes_open.png
    ├── cat_eyes_half.png
    └── cat_eyes_closed.png
```

## 性能优化说明

- **呼吸动画**：只有关键帧数据，无额外脚本开销
- **眨眼动画**：使用 Sprite 序列帧，在 Animation Clip 中播放，无需运行时切换 Sprite
- **CatBlinkController**：仅用于计时，每 3.5~5.5 秒触发一次 Animator Trigger，几乎无性能开销
- **所有动画**：由 Unity Animator 系统统一管理，高效且易于扩展
