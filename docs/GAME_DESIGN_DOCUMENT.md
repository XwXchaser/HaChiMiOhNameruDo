# HaChiMiOhNameruDo 游戏系统策划案

> **文档版本**: v1.2
> **最后更新**: 2026-04-04
> **目标读者**: AI 开发者、系统设计师
> **新增功能**: 触摸互动玩法（双指抓取 + 陀螺仪摇晃）、小鱼干奖励系统

---

## 目录

1. [项目概述](#1-项目概述)
2. [游戏核心概念](#2-游戏核心概念)
3. [系统架构](#3-系统架构)
4. [游戏状态管理](#4-游戏状态管理)
5. [触摸互动系统](#5-触摸互动系统)
6. [奖励系统](#6-奖励系统)
7. [小游戏系统](#7-小游戏系统)
8. [节奏系统](#8-节奏系统)
9. [UI 系统](#9-ui-系统)
10. [音频系统](#10-音频系统)
11. [输入系统](#11-输入系统)
12. [数据结构和 API](#12-数据结构和-api)
13. [开发优先级](#13-开发优先级)

---

## 1. 项目概述

### 1.1 基本信息

| 属性 | 值 |
|------|-----|
| **项目名称** | HaChiMiOhNameruDo（ハチミオナメルド） |
| **游戏类型** | 休闲解压小游戏 + 音乐节奏元素 |
| **目标平台** | Unity 2D (移动端优先) |
| **开发引擎** | Unity 2022+ |
| **编程语言** | C# |
| **命名空间** | `HaChiMiOhNameruDo` |

### 1.2 游戏主题

**主题**: 搞怪猫咪的调皮日常

玩家通过与一只调皮的猫咪互动，体验各种解压小游戏。猫咪会做出各种搞怪动作，玩家可以在休闲模式下随意游玩，或者挑战节奏玩法获得高分。

### 1.3 设计理念

```
┌─────────────────────────────────────────────────────────┐
│ 核心设计理念                                            │
├─────────────────────────────────────────────────────────┤
│ 1. 休闲友好：不强制玩家按节奏交互                       │
│ 2. 奖励驱动：踩准节拍会有额外奖励                       │
│ 3. 清晰反馈：视觉 + 听觉 + 触觉三重反馈                 │
│ 4. 渐进难度：通过 BPM 和判定窗口调整难度                │
│ 5. 重复可玩性：音乐与玩法的结合提升沉浸感               │
└─────────────────────────────────────────────────────────┘
```

---

## 2. 游戏核心概念

### 2.1 核心循环

```
┌──────────────────────────────────────────────────────────┐
│                     游戏主循环                           │
│                                                          │
│   ┌─────────┐    选择     ┌─────────┐                   │
│   │  IDLE   │ ─────────> │ 小游戏  │                   │
│   │  状态   │            │  进行   │                   │
│   └────┬────┘            └────┬────┘                   │
│        │                      │                         │
│        │   返回/结束          │  计时器归零              │
│        │ <─────────────────── │                         │
│        │                      │                         │
│        │                      v                         │
│        │               ┌─────────────┐                  │
│        └────────────── │  结算/返回  │                  │
│                        └─────────────┘                  │
└──────────────────────────────────────────────────────────┘
```

### 2.2 游戏模式

| 模式 | 描述 | 时长 |
|------|------|------|
| **IDLE 模式** | 主界面，显示猫咪和道具按钮，支持触摸互动 | 无限 |
| **毛球游戏** | 拍打毛球，猫咪会做出反应 | 30 秒 |
| **纸巾筒游戏** | 抽纸巾并切碎，体验解压快感 | 30 秒 |

### 2.3 触摸互动玩法

**名称**: 举起猫咪（双指抓取 + 陀螺仪摇晃）

**核心玩法**:
- 玩家在 IDLE 状态下双指按住猫咪"双肩"判定点
- 持续按住约 0.5 秒后触发"举起"判定
- 启用手机陀螺仪，猫咪下半身随陀螺仪左右摆动
- 摆动逻辑：陀螺仪摆动速度 → 摆动角度，速度归零后角度慢慢归零
- 松开手指后猫咪落下/回到原位

**技术规格**:

| 要素 | 描述 |
|------|------|
| **触发条件** | 双指同时按住猫咪双肩判定点（位置可配置） |
| **判定时间** | 持续按住约 0.5 秒 |
| **交互方式** | 双指抓取 + 陀螺仪倾斜 |
| **摆动逻辑** | 陀螺仪摆动速度 → 摆动角度，随后慢慢归零 |
| **结束条件** | 松开手指 |
| **节奏元素** | 无（与小游戏区分） |

**流程图**:

```
┌─────────────────────────────────────────────────────────┐
│ 举起猫咪互动流程                                        │
├─────────────────────────────────────────────────────────┤
│                                                         │
│  1. IDLE 状态 - 猫咪坐姿面朝玩家                        │
│     │                                                   │
│     v                                                   │
│  2. 玩家双指按住"双肩"判定点（位置可配置）              │
│     │                                                   │
│     v                                                   │
│  3. 持续按住 0.5 秒                                      │
│     │                                                   │
│     v                                                   │
│  4. 触发"举起"判定                                      │
│     │                                                   │
│     v                                                   │
│  5. 启用手机陀螺仪                                      │
│     │                                                   │
│     v                                                   │
│  6. 猫咪下半身随陀螺仪摆动：                           │
│     - 摆动速度越大 → 摆动角度越大                       │
│     - 速度归零 → 摆动角度慢慢归零                       │
│     │                                                   │
│     v                                                   │
│  7. 松开手指 → 猫咪落下/回到原位                        │
│                                                         │
└─────────────────────────────────────────────────────────┘
```

### 2.4 猫咪角色设定

- **性格**: 搞怪、调皮、不按常理出牌
- **行为**: 眨眼、呼吸、拍击、转身、被抓起摇晃
- **动画状态**:
  - `Idle`:  idle 状态，面向前方
  - `Blink`: 随机或节拍触发眨眼
  - `Breath`: 呼吸动画（2 拍一次）
  - `Paw`: 拍击动作（玩家输入时触发）
  - `Turn`: 转身（切换小游戏时）
  - `Grabbed`: 被双指抓起（触摸互动）
  - `Swing`: 随陀螺仪摆动（触摸互动）

---

## 3. 系统架构

### 3.1 整体架构图

```
┌─────────────────────────────────────────────────────────────────┐
│                        游戏系统架构                             │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │                    核心管理层                            │   │
│  │  ┌──────────┐ ┌──────────┐ ┌──────────┐ ┌──────────┐  │   │
│  │  │ GameManager │ │ UIManager │ │ AudioManager │ │ HapticManager │ │
│  │  └──────────┘ └──────────┘ └──────────┘ └──────────┘  │   │
│  └─────────────────────────────────────────────────────────┘   │
│                              │                                  │
│                              v                                  │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │                    游戏逻辑层                            │   │
│  │  ┌──────────────────┐      ┌──────────────────┐        │   │
│  │  │  FurBallGame     │      │  TissueGame      │        │   │
│  │  │  - FurBall       │      │  - TissueBox     │        │   │
│  │  │  - FurBallManager│      │  - TissuePaper   │        │   │
│  │  └──────────────────┘      └──────────────────┘        │   │
│  └─────────────────────────────────────────────────────────┘   │
│                              │                                  │
│                              v                                  │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │                    表现层                                │   │
│  │  ┌──────────┐ ┌──────────┐ ┌──────────┐ ┌──────────┐  │   │
│  │  │CatController│ │InputHandler│ │  Effects   │ │   UI     │  │   │
│  │  └──────────┘ └──────────┘ └──────────┘ └──────────┘  │   │
│  └─────────────────────────────────────────────────────────┘   │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

### 3.2 命名空间结构

```
HaChiMiOhNameruDo
├── Managers
│   ├── GameManager          # 游戏状态管理
│   ├── UIManager            # UI 界面管理
│   ├── AudioManager         # 音频管理
│   └── HapticManager        # 触觉反馈管理
│
├── Gameplay
│   ├── CatController        # 猫咪控制
│   ├── InputHandler         # 基础输入处理
│   └── TouchInteractionHandler  # 触摸互动处理
│
├── MiniGames
│   ├── FurBallGame
│   │   ├── FurBall          # 毛球组件
│   │   └── FurBallGameManager  # 毛球游戏管理
│   └── TissueGame
│       ├── TissueBox        # 纸巾筒组件
│       ├── TissuePaper      # 厕纸组件
│       ├── TissueInputHandler  # 纸巾游戏输入
│       └── TissueGameManager   # 纸巾游戏管理
│
├── Rewards
│   ├── TreatCounter         # 小鱼干计数器
│   ├── TreatRewardSystem    # 奖励发放系统
│   └── UnlockSystem         # 解锁系统
│
└── UI
    ├── IdleUI               # IDLE 状态 UI
    └── TreatShopUI          # 小鱼干商店 UI
```

### 3.3 文件结构

```
Assets/
├── Scripts/
│   ├── Managers/
│   │   ├── GameManager.cs
│   │   ├── UIManager.cs
│   │   ├── AudioManager.cs
│   │   └── HapticManager.cs
│   ├── Gameplay/
│   │   ├── CatController.cs
│   │   ├── InputHandler.cs
│   │   └── TouchInteractionHandler.cs    # 触摸互动处理
│   ├── MiniGames/
│   │   ├── FurBallGame/
│   │   │   ├── FurBall.cs
│   │   │   └── FurBallGameManager.cs
│   │   └── TissueGame/
│   │       ├── TissueBox.cs
│   │       ├── TissuePaper.cs
│   │       ├── TissueInputHandler.cs
│   │       └── TissueGameManager.cs
│   ├── Rewards/
│   │   ├── TreatCounter.cs              # 小鱼干计数器
│   │   ├── TreatRewardSystem.cs         # 奖励发放系统
│   │   └── UnlockSystem.cs              # 解锁系统
│   └── UI/
│       ├── IdleUI.cs
│       └── TreatShopUI.cs               # 小鱼干商店 UI
├── Prefabs/
│   ├── FurBall.prefab
│   ├── TissueBox.prefab
│   └── TissuePaper.prefab
├── Animations/
│   └── Cat/
│       ├── CatBlink.anim
│       ├── CatBreath.anim
│       ├── CatGrabbed.anim
│       ├── CatSwing.anim
│       └── CatController.controller
├── Sprites/
│   └── 猫咪相关图片
└── Scenes/
    └── SampleScene.unity
```

---

## 4. 游戏状态管理

### 4.1 游戏状态枚举

```csharp
public enum GameState
{
    Idle,           // IDLE 状态 - 主界面
    FurBallGame,    // 小游戏 1 - 毛球
    TissueGame      // 小游戏 2 - 纸巾筒
}
```

### 4.2 GameManager 职责

| 职责 | 描述 |
|------|------|
| **状态切换** | 管理游戏状态的进入和退出 |
| **计时器** | 控制小游戏时长（默认 30 秒） |
| **退出延迟** | 小游戏结束后延迟返回 IDLE（默认 2 秒） |
| **单例模式** | 全局唯一实例，跨场景持久化 |

### 4.3 状态切换流程

```
┌─────────────────────────────────────────────────────────┐
│ 状态切换流程                                            │
├─────────────────────────────────────────────────────────┤
│                                                         │
│  Idle ──[StartFurBallGame]──> FurBallGame              │
│    ▲                            │                       │
│    │                            │                       │
│    │   [ExitMiniGame + 延迟]     │                       │
│    └────────────────────────────┘                       │
│                                                         │
│  Idle ──[StartTissueGame]──> TissueGame                │
│    ▲                            │                       │
│    │                            │                       │
│    │   [ExitMiniGame + 延迟]     │                       │
│    └────────────────────────────┘                       │
│                                                         │
└─────────────────────────────────────────────────────────┘
```

### 4.4 核心 API

```csharp
// 设置游戏状态
GameManager.Instance.SetGameState(GameState newState);

// 开始毛球游戏
GameManager.Instance.StartFurBallGame();

// 开始纸巾游戏
GameManager.Instance.StartTissueGame();

// 退出小游戏（带延迟）
GameManager.Instance.ExitMiniGame();

// 强制返回 IDLE
GameManager.Instance.ReturnToIdle();
```

---

## 5. 触摸互动系统

### 5.1 举起猫咪互动

#### 5.1.1 游戏机制

| 元素 | 描述 |
|------|------|
| **目标** | 与猫咪进行桌面宠物式的互动 |
| **操作** | 双指按住双肩 → 持续 0.5 秒 → 举起 → 陀螺仪摇晃 |
| **反馈** | 猫咪被举起 → 下半身随陀螺仪摆动 |
| **时长** | 无限（IDLE 状态下随时可触发） |

#### 5.1.2 互动状态机

```
┌─────────────────────────────────────────────────────────┐
│ 触摸互动状态机                                          │
├─────────────────────────────────────────────────────────┤
│                                                         │
│   ┌──────┐  双指按住  ┌──────────┐  0.5 秒   ┌─────────┐ │
│   │ Idle │ ─────────> │ Grabbing  │ ────────> │ Grabbed │ │
│   └──────┘  双肩判定点 └──────────┘  持续按住 └────┬────┘ │
│      ▲                                              │     │
│      │                                              │     │
│      │   松开手指                                   │     │
│      └──────────────────────────────────────────────┘     │
│                                                             │
│   状态说明：                                                │
│   - Idle: 空闲状态，猫咪坐姿面朝玩家                        │
│   - Grabbing: 双指按住双肩，计时 0.5 秒                      │
│   - Grabbed: 举起猫咪，启用陀螺仪，下半身随摆动摇晃         │
│                                                         │
│   摆动物理：                                                │
│   - 陀螺仪摆动速度 → 摆动角度                              │
│   - 速度归零 → 角度慢慢归零（阻尼效果）                    │
│                                                         │
└─────────────────────────────────────────────────────────┘
```

#### 5.1.3 核心组件

```csharp
// TouchInteractionHandler.cs - 触摸互动处理器
public class TouchInteractionHandler : MonoBehaviour
{
    [Header("判定点设置")]
    [SerializeField] Vector2 leftShoulderPos;    // 左肩判定位置（可配置）
    [SerializeField] Vector2 rightShoulderPos;   // 右肩判定位置（可配置）
    [SerializeField] float grabRadius = 0.5f;    // 判定半径
    
    [Header("判定时间")]
    [SerializeField] float grabHoldTime = 0.5f;  // 需要持续按住的时间
    
    [Header("陀螺仪设置")]
    [SerializeField] float maxSwingAngle = 30f;  // 最大摆动角度
    [SerializeField] float dampingFactor = 0.95f; // 阻尼系数（归零速度）
    
    // 状态
    private bool isGrabbing;                     // 是否正在抓取
    private bool isGrabbed;                      // 是否已抓起
    private float grabTimer;                     // 抓取计时器
    private float currentSwingAngle;             // 当前摆动角度
    private float swingVelocity;                 // 摆动速度
    
    // 事件
    public event Action OnGrabStarted;           // 抓取开始事件
    public event Action OnGrabReleased;          // 抓取释放事件
    
    // 核心方法
    public void Update();                        // 每帧检测双指输入和陀螺仪
    private bool CheckTwoFingerGrab();           // 检测双指按住双肩
    private void HandleGyroscope();              // 处理陀螺仪输入
    private void UpdateSwingPhysics();           // 更新摆动物理
}

// CatController.cs - 猫咪控制（扩展）
public class CatController : MonoBehaviour
{
    // 新增方法
    public void SetGrabbed(bool grabbed);        // 设置被抓状态
    public void SetSwingAngle(float angle);      // 设置摆动角度
    public void OnGrabReleased();                // 被抓释放时的回调
}
```

#### 5.1.4 双肩判定点配置

```
┌─────────────────────────────────────────────────────────┐
│ 双肩判定点配置示意图                                    │
├─────────────────────────────────────────────────────────┤
│                                                         │
│        左手判定点          右手判定点                   │
│            ●───────────────────●                       │
│            │                   │                       │
│            │     猫咪头部      │                       │
│            │        ◉          │                       │
│            │       /│\         │                       │
│            │      / │ \        │                       │
│            │     /  │  \       │                       │
│            │    /   │   \      │                       │
│            │   /    │    \     │                       │
│            │  /     │     \    │                       │
│            │ /      │      \   │                       │
│            │/       │       \  │                       │
│            ●───────┌─┴─┐──────●                       │
│           左肩     │身体│     右肩                     │
│           判定点   └───┘     判定点                   │
│                                                         │
│  配置参数（示例）：                                     │
│  - leftShoulderPos: (-0.5, 0.5)  // 相对于猫咪中心     │
│  - rightShoulderPos: (0.5, 0.5)  // 相对于猫咪中心     │
│  - grabRadius: 0.3               // 判定半径           │
│                                                         │
└─────────────────────────────────────────────────────────┘
```

#### 5.1.5 陀螺仪摆动逻辑

```csharp
// 伪代码示例
void HandleGyroscope()
{
    // 读取陀螺仪数据（Unity Input.gyro）
    Vector3 gyroRotation = Input.gyro.rotationRate;
    
    // 计算摆动速度（取 X 轴或 Z 轴，取决于设备方向）
    float targetVelocity = gyroRotation.x * sensitivity;
    
    // 更新摆动速度
    swingVelocity = Mathf.Lerp(swingVelocity, targetVelocity, 0.1f);
    
    // 应用阻尼，使摆动慢慢归零
    swingVelocity *= dampingFactor;
    
    // 计算摆动角度
    currentSwingAngle = swingVelocity * maxSwingAngle;
    currentSwingAngle = Mathf.Clamp(currentSwingAngle, -maxSwingAngle, maxSwingAngle);
    
    // 应用到猫咪
    catController.SetSwingAngle(currentSwingAngle);
}
```

---

## 6. 小游戏系统

### 6.1 毛球小游戏

#### 5.1.1 游戏机制

| 元素 | 描述 |
|------|------|
| **目标** | 点击毛球，猫咪会拍击毛球 |
| **操作** | 点击/触摸毛球 |
| **反馈** | 毛球弹起 → 停留 → 落下 → 回到原位 |
| **时长** | 30 秒 |

#### 5.1.2 毛球状态机

```
┌─────────────────────────────────────────────────────────┐
│ 毛球状态机                                              │
├─────────────────────────────────────────────────────────┤
│                                                         │
│   ┌──────┐   点击   ┌─────────┐   到达    ┌─────────┐ │
│   │ Idle │ ─────> │ Launching │ ─────> │ StayingOut │ │
│   └──────┘        └─────────┘         └────┬────┘   │
│      ▲                                      │        │
│      │                                      │        │
│      │   回到原位                           │ 时间到   │
│      └─────────────────────────────────────┘        │
│                                                         │
│   状态说明：                                            │
│   - Idle: 静止状态，等待玩家点击                        │
│   - Launching: 上升到最高点（使用 EaseOut 缓动）        │
│   - StayingOut: 在屏幕外停留（1.5 秒）                  │
│   - Falling: 下落回原位（使用 EaseIn 缓动，模拟重力）   │
│                                                         │
└─────────────────────────────────────────────────────────┘
```

#### 5.1.3 核心组件

```csharp
// FurBall.cs - 毛球组件
public class FurBall : MonoBehaviour
{
    // 运动参数
    [SerializeField] float launchHeight = 5f;       // 上升高度
    [SerializeField] float launchDuration = 0.5f;   // 上升时间
    [SerializeField] float fallDuration = 1.0f;     // 下落时间
    [SerializeField] float stayOutDuration = 1.5f;  // 停留时间
    
    // 核心方法
    public void PawBall();              // 执行拍击
    public void EnableInteraction();    // 启用交互
    public void DisableInteraction();   // 禁用交互
    public void ResetBall();            // 重置位置
}

// FurBallGameManager.cs - 游戏管理
public class FurBallGameManager : MonoBehaviour
{
    // 单例
    public static FurBallGameManager Instance { get; private set; }
    
    // 游戏流程
    public void StartGame();            // 开始游戏
    public void EndGame();              // 结束游戏
    public void OnBallPawed();          // 毛球被拍击时调用
    public void OnBallReturned();       // 毛球回到原位时调用
}
```

---

### 5.2 纸巾筒小游戏

#### 5.2.1 游戏机制

| 元素 | 描述 |
|------|------|
| **目标** | 下拉抽取纸巾，然后横向划动切碎 |
| **操作** | 下拉（抽纸）→ 横向划动（切碎） |
| **反馈** | 纸巾延伸 → 切碎粒子效果 |
| **时长** | 30 秒 |

#### 5.2.2 游戏流程

```
┌─────────────────────────────────────────────────────────┐
│ 纸巾筒游戏流程                                          │
├─────────────────────────────────────────────────────────┤
│                                                         │
│  1. 游戏开始                                            │
│     │                                                   │
│     v                                                   │
│  2. 玩家下拉输入 ──> 计算延伸长度 ──> 生成厕纸段        │
│     │                                                   │
│     v                                                   │
│  3. 重复步骤 2，累积纸巾长度                            │
│     │                                                   │
│     v                                                   │
│  4. 玩家横向划动 ──> 切碎厕纸 ──> 粒子特效             │
│     │                                                   │
│     v                                                   │
│  5. 计时器归零 → 游戏结束                               │
│                                                         │
└─────────────────────────────────────────────────────────┘
```

#### 5.2.3 核心组件

```csharp
// TissuePaper.cs - 厕纸组件
public class TissuePaper : MonoBehaviour
{
    // 厕纸参数
    [SerializeField] float segmentHeight = 0.1f;   // 每段高度
    [SerializeField] float extendSpeed = 0.5f;     // 延伸速度
    
    // 核心方法
    public void ExtendPaper(int newLength);   // 延伸厕纸
    public void CutPaper();                   // 切碎厕纸
    public void ResetPaper();                 // 重置厕纸
}

// TissueBox.cs - 纸巾筒组件
public class TissueBox : MonoBehaviour
{
    // 核心方法
    public void OnPullInput();    // 处理下拉输入
    public void OnCutInput();     // 处理切碎输入
}

// TissueGameManager.cs - 游戏管理
public class TissueGameManager : MonoBehaviour
{
    // 单例
    public static TissueGameManager Instance { get; private set; }
    
    // 游戏流程
    public void StartGame();            // 开始游戏
    public void EndGame();              // 结束游戏
    public void OnPaperPulled();        // 纸巾被抽出
    public void OnPaperCut();           // 纸巾被切碎
}
```

#### 5.2.4 输入检测

```csharp
// TissueInputHandler.cs - 输入处理
public class TissueInputHandler : MonoBehaviour
{
    [SerializeField] float swipeThreshold = 0.5f;    // 划动阈值
    [SerializeField] float pullThreshold = 0.3f;     // 下拉阈值
    
    // 输入事件
    public event Action<Vector2> OnPull;      // 下拉事件
    public event Action<Vector2> OnCut;       // 切碎事件
}
```

---

## 6. 节奏系统

### 6.1 系统概述

节奏系统是本游戏的核心特色，允许玩家在休闲模式下随意游玩，同时为想挑战高分的玩家提供节奏玩法。

### 6.2 节奏管理器设计

```csharp
// RhythmManager.cs - 节奏核心（待实现）
public class RhythmManager : MonoBehaviour
{
    // 音乐配置
    [SerializeField] float bpm = 110f;           // 节拍速度
    [SerializeField] int beatsPerMeasure = 4;    // 每小节拍数
    [SerializeField] float audioOffset = 0f;     // 音频延迟补偿
    
    // 判定窗口（秒）
    [SerializeField] float perfectWindow = 0.05f;  // ±50ms
    [SerializeField] float goodWindow = 0.10f;     // ±100ms
    [SerializeField] float normalWindow = 0.20f;   // ±200ms
    
    // 事件
    public event Action<int, int> OnBeatHit;        // 节拍命中事件
    public event Action<int> OnMeasureStart;        // 新小节开始
    public event Action<RhythmJudgment, int> OnInputJudged; // 输入判定
    
    // 连击系统
    private int currentCombo;
    private int maxCombo;
    private float comboMultiplier = 1f;
    
    // 核心方法
    public RhythmJudgment JudgeInput(float inputTime, out float timingOffset);
    public void ProcessRhythmInput(float inputTime);
    public int GetBeatInMeasure();
    public bool IsStrongBeat();
}
```

### 6.3 节奏判定

| 判定 | 时间窗口 | 分数倍率 | 特效 |
|------|----------|----------|------|
| **Perfect** | ±50ms | 1.5x | 金色闪光 + 粒子爆发 |
| **Good** | ±100ms | 1.2x | 绿色闪光 |
| **Normal** | ±200ms | 1.0x | 无特效 |
| **Miss** | >200ms | 0.5x | 连击重置 |

### 6.4 连击系统

```
┌─────────────────────────────────────────────────────────┐
│ 连击奖励表                                              │
├─────────────────────────────────────────────────────────┤
│                                                         │
│  连击数      倍率奖励      模式                         │
│  ─────────────────────────────────                      │
│  0-4        +0%          普通                          │
│  5-9        +10%         连击开始                      │
│  10-19      +25%         连击持续                      │
│  20-29      +50%         Fever 预备                    │
│  30+        +100%        Fever 模式！                  │
│                                                         │
└─────────────────────────────────────────────────────────┘
```

### 6.5 节奏同步

```
┌─────────────────────────────────────────────────────────┐
│ 节奏同步时序图                                          │
├─────────────────────────────────────────────────────────┤
│                                                         │
│  Player    Input    Rhythm    Audio    Anim    Game    │
│    │         │         │         │        │       │    │
│    │ 触摸    │         │         │        │       │    │
│    │────────>│         │         │        │       │    │
│    │         │ 输入时间│         │        │       │    │
│    │         │────────>│         │        │       │    │
│    │         │         │ 计算判定│        │       │    │
│    │         │         │────────>│        │       │    │
│    │         │         │         │ 播放音效│       │    │
│    │         │         │─────────────────>│       │    │
│    │         │         │                  │ 触发动画│    │
│    │         │         │─────────────────────────>│    │
│    │         │         │                         │ 更新分数│
│    │         │         │──────────────────────────────>│
│                                                         │
└─────────────────────────────────────────────────────────┘
```

---

## 7. 奖励系统

### 7.1 分数计算

```
基础分数 = 100 分

最终分数 = 基础分数 × 判定倍率 × 连击倍率

示例:
- Perfect 判定 + 15 连击 = 100 × 1.5 × 1.25 = 187.5 分
- Good 判定 + 5 连击 = 100 × 1.2 × 1.1 = 132 分
- Normal 判定 + 0 连击 = 100 × 1.0 × 1.0 = 100 分
```

### 7.2 小游戏特定奖励

#### 毛球游戏

| 行为 | 基础分 | 说明 |
|------|--------|------|
| 拍击毛球 | 100 | 基础分 |
| 重拍时拍击 | +50% | 节奏奖励 |
| 连续拍击 | 连击加成 | 连击系统 |

#### 纸巾筒游戏

| 行为 | 基础分 | 说明 |
|------|--------|------|
| 抽纸 | 50/段 | 每段厕纸 |
| 重拍时抽纸 | +100% 长度 | 节奏奖励 |
| 切碎 | 100 | 基础分 |
| 重拍时切碎 | +200% 碎片 | 节奏奖励 |

---

## 8. 奖励系统

### 8.1 小鱼干奖励系统

#### 8.1.1 系统概述

**名称**: 小鱼干奖励系统（Treat Reward System）

**核心概念**: 玩家通过游戏行为获得小鱼干奖励，收集的小鱼干可用于解锁新的小游戏玩法。

#### 8.1.2 奖励获取途径

| 来源 | 条件 | 奖励数量 | 说明 |
|------|------|----------|------|
| **小游戏 - 节奏玩法** | Perfect 判定 | 2 分 | 踩准节拍 |
| **小游戏 - 节奏玩法** | Normal 判定 | 1 分 | 判定宽松，减轻压力 |
| **触摸互动 - 甩猫咪** | 甩两下猫咪 | 5 分 | 有现实时间冷却 |

**判定说明**:
- Normal 判定窗口较为宽松，减轻玩家压力
- 无连击奖励，保持休闲体验

#### 8.1.3 小鱼干用途

| 用途 | 消耗 | 说明 |
|------|------|------|
| **解锁新小游戏** | 50 个 | 每个新小游戏需要 50 小鱼干 |

#### 8.1.4 冷却机制

| 来源 | 冷却类型 | 冷却时长 |
|------|----------|----------|
| **甩猫咪奖励** | 现实世界时间 | 可配置（默认待设定） |

#### 8.1.5 核心组件

```csharp
// TreatCounter.cs - 小鱼干计数器
public class TreatCounter : MonoBehaviour
{
    // 单例
    public static TreatCounter Instance { get; private set; }
    
    // 当前小鱼干数量
    private int currentTreats;
    
    // 事件
    public event Action<int> OnTreatsChanged;    // 数量变化事件
    public event Action<int> OnTreatsEarned;     // 获得事件
    public event Action<int> OnTreatsSpent;      // 消耗事件
    
    // 核心方法
    public int GetCurrentTreats();               // 获取当前数量
    public void AddTreats(int amount);           // 增加小鱼干
    public void SpendTreats(int amount);         // 消费小鱼干
    public bool CanAfford(int cost);             // 检查是否买得起
}

// TreatRewardSystem.cs - 奖励发放系统
public class TreatRewardSystem : MonoBehaviour
{
    // 单例
    public static TreatRewardSystem Instance { get; private set; }
    
    // 奖励配置
    [SerializeField] int perfectReward = 2;      // Perfect 奖励
    [SerializeField] int normalReward = 1;       // Normal 奖励
    [SerializeField] int swingReward = 5;        // 甩猫咪奖励
    [SerializeField] float swingCooldown = 60f;  // 甩猫咪冷却（秒）
    
    // 核心方法
    public void AwardForPerfect();               // Perfect 判定奖励
    public void AwardForNormal();                // Normal 判定奖励
    public void AwardForSwing();                 // 甩猫咪奖励（带冷却检查）
}

// UnlockSystem.cs - 解锁系统
public class UnlockSystem : MonoBehaviour
{
    // 单例
    public static UnlockSystem Instance { get; private set; }
    
    // 解锁项目
    public enum UnlockableType
    {
        MiniGame_FurBall,      // 毛球游戏（默认解锁）
        MiniGame_Tissue,       // 纸巾游戏（默认解锁）
        MiniGame_New1,         // 新小游戏 1（待解锁）
        MiniGame_New2,         // 新小游戏 2（待解锁）
    }
    
    // 解锁条件
    public class UnlockableItem
    {
        public UnlockableType type;
        public string name;
        public int treatCost;          // 需要的小鱼干数量
        public bool isUnlocked;        // 是否已解锁
    }
    
    // 核心方法
    public void UnlockMiniGame(UnlockableType type);  // 解锁小游戏
    public bool IsUnlocked(UnlockableType type);      // 检查是否已解锁
    public int GetUnlockCost(UnlockableType type);    // 获取解锁价格
}
```

#### 8.1.6 UI 显示

```
┌─────────────────────────────────────────────────────────┐
│ 小鱼干 UI 布局                                          │
├─────────────────────────────────────────────────────────┤
│                                                         │
│  主界面 (IDLE 状态):                                    │
│  ┌───────────────────────────────────────────────┐     │
│  │                                               │     │
│  │    🐟 x 15              [猫咪显示区域]         │     │
│  │    当前小鱼干                                  │     │
│  │                                               │     │
│  └───────────────────────────────────────────────┘     │
│                                                         │
│  解锁商店界面:                                          │
│  ┌───────────────────────────────────────────────┐     │
│  │  🐟 可用小鱼干：15                            │     │
│  │                                               │     │
│  │  ┌─────────────┐  ┌─────────────┐            │     │
│  │  │ 新小游戏 1   │  │ 新小游戏 2   │            │     │
│  │  │ 🔒 已解锁   │  │ 🔒 50 🐟    │            │     │
│  │  │             │  │  [解锁]     │            │     │
│  │  └─────────────┘  └─────────────┘            │     │
│  └───────────────────────────────────────────────┘     │
│                                                         │
└─────────────────────────────────────────────────────────┘
```

---

## 9. UI 系统

### 9.1 UI 管理器

```csharp
// UIManager.cs
public class UIManager : MonoBehaviour
{
    // 单例
    public static UIManager Instance { get; private set; }
    
    // UI 面板引用
    [SerializeField] GameObject idleUIPanel;       // IDLE UI
    [SerializeField] GameObject furBallGameUIPanel; // 毛球游戏 UI
    [SerializeField] GameObject tissueGameUIPanel;  // 纸巾游戏 UI
    [SerializeField] GameObject commonUIPanel;      // 通用 UI
    [SerializeField] GameObject treatShopUIPanel;   // 小鱼干商店 UI（新增）
    
    // 核心方法
    public void ShowIdleUI();
    public void HideIdleUI();
    public void ShowFurBallGameUI();
    public void HideFurBallGameUI();
    public void ShowTissueGameUI();
    public void HideTissueGameUI();
    public void ShowTreatShopUI();         // 新增：显示商店
    public void HideTreatShopUI();         // 新增：隐藏商店
    public void UpdateTimer(float remainingTime);
    public void OnReturnButtonClicked();
}
```

### 8.2 UI 布局

```
┌─────────────────────────────────────────────────────────┐
│ IDLE 状态 UI 布局                                       │
├─────────────────────────────────────────────────────────┤
│                                                         │
│   ┌───────────────────────────────────────────────┐    │
│   │                                               │    │
│   │              [猫咪显示区域]                    │    │
│   │                                               │    │
│   └───────────────────────────────────────────────┘    │
│                                                         │
│   ┌─────────────┐              ┌─────────────┐        │
│   │  毛球游戏   │              │  纸巾筒游戏  │        │
│   │   按钮      │              │    按钮      │        │
│   └─────────────┘              └─────────────┘        │
│                                                         │
└─────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────┐
│ 小游戏 UI 布局                                          │
├─────────────────────────────────────────────────────────┤
│                                                         │
│   ┌─────────────────┐  ┌─────────────────┐            │
│   │   返回按钮      │  │   计时器        │            │
│   │   (左上)        │  │   00:30         │            │
│   └─────────────────┘  └─────────────────┘            │
│                                                         │
│   ┌───────────────────────────────────────────────┐    │
│   │                                               │    │
│   │              [游戏区域]                        │    │
│   │                                               │    │
│   └───────────────────────────────────────────────┘    │
│                                                         │
│   ┌───────────────────────────────────────────────┐    │
│   │   Score: 12,500    Combo: x5    Perfect!     │    │
│   └───────────────────────────────────────────────┘    │
│                                                         │
└─────────────────────────────────────────────────────────┘
```

---

## 9. 音频系统

### 9.1 音频管理器

```csharp
// AudioManager.cs
public class AudioManager : MonoBehaviour
{
    // 单例
    public static AudioManager Instance { get; private set; }
    
    // 音效剪辑
    [SerializeField] AudioClip cutSound;         // 碎纸声
    [SerializeField] AudioClip pawSound;         // 拍击声
    [SerializeField] AudioClip swipeSound;       // 划动声
    [SerializeField] AudioClip gameStartSound;   // 游戏开始音效
    [SerializeField] AudioClip gameEndSound;     // 游戏结束音效
    
    // 背景音乐
    [SerializeField] AudioClip backgroundMusic;
    [SerializeField] float musicVolume = 0.5f;
    [SerializeField] float sfxVolume = 0.8f;
    
    // 核心方法
    public void PlayCutSound();
    public void PlayPawSound();
    public void PlaySwipeSound();
    public void PlayGameStartSound();
    public void PlayGameEndSound();
    public void PlayMusic();
    public void StopMusic();
    public void PauseMusic();
    public void ResumeMusic();
}
```

### 9.2 音乐规格

| 属性 | 值 |
|------|-----|
| **BPM** | 130-150 (活力感) |
| **拍号** | 4/4 拍 |
| **时长** | 30 秒 (可无缝循环) |
| **风格** | 搞怪卡通 / 俏皮电子 |
| **格式** | WAV 或 320kbps MP3 |

### 9.3 音乐需求

详细音乐需求请参考 [`plans/Music_Requirements.md`](../plans/Music_Requirements.md)

---

## 10. 输入系统

### 10.1 输入类型

| 输入类型 | 描述 | 使用场景 |
|----------|------|----------|
| **点击** | 点击/触摸 | 毛球拍击 |
| **下拉** | 向下滑动 | 抽取纸巾 |
| **横向划动** | 左右滑动 | 切碎纸巾 |

### 10.2 输入处理

```csharp
// InputHandler.cs - 基础输入处理
public class InputHandler : MonoBehaviour
{
    // 输入事件
    public event Action<Vector2> OnTap;           // 点击事件
    public event Action<Vector2, SwipeDirection> OnSwipe; // 划动事件
    
    // 输入检测
    [SerializeField] float swipeThreshold = 0.3f;  // 划动阈值
    [SerializeField] float minSwipeTime = 0.1f;    // 最小划动时间
    
    private Vector2 touchStartPosition;
    private float touchStartTime;
}
```

### 10.3 输入流程

```
┌─────────────────────────────────────────────────────────┐
│ 输入处理流程                                            │
├─────────────────────────────────────────────────────────┤
│                                                         │
│  1. 检测触摸开始                                        │
│     │                                                   │
│     v                                                   │
│  2. 记录起始位置和时间                                  │
│     │                                                   │
│     v                                                   │
│  3. 检测触摸结束                                        │
│     │                                                   │
│     v                                                   │
│  4. 计算位移和时间                                      │
│     │                                                   │
│     +─── 位移 < 阈值 ───> 判定为点击                    │
│     │                                                   │
│     +─── 位移 >= 阈值 ───> 判定为划动                   │
│             │                                           │
│             v                                           │
│         分析划动方向                                    │
│             │                                           │
│             +─── 垂直向下 ──> 下拉输入                  │
│             │                                           │
│             +─── 水平方向 ──> 切碎输入                  │
│                                                         │
└─────────────────────────────────────────────────────────┘
```

---

## 11. 数据结构和 API

### 11.1 核心枚举

```csharp
// 游戏状态
public enum GameState
{
    Idle,
    FurBallGame,
    TissueGame
}

// 节奏判定
public enum RhythmJudgment
{
    Perfect,
    Good,
    Normal,
    Miss
}

// 毛球状态
public enum BallState
{
    Idle,
    Launching,
    StayingOut,
    Falling
}

// 划动方向
public enum SwipeDirection
{
    Up,
    Down,
    Left,
    Right
}
```

### 11.2 核心数据结构

```csharp
// 节奏事件数据
public class RhythmHitEvent
{
    public float beatPosition;      // 节拍位置
    public RhythmJudgment judgment; // 判定结果
    public float timingOffset;      // 时间偏移 (ms)
    public int combo;               // 连击数
}

// 游戏统计数据
public class GameStats
{
    public int totalScore;          // 总分
    public int maxCombo;            // 最大连击
    public int perfectCount;        // Perfect 次数
    public int goodCount;           // Good 次数
    public int normalCount;         // Normal 次数
    public int missCount;           // Miss 次数
}
```

### 11.3 全局 API 参考

#### GameManager

```csharp
// 获取实例
GameManager.Instance

// 状态管理
GameManager.Instance.SetGameState(GameState newState);
GameManager.Instance.StartFurBallGame();
GameManager.Instance.StartTissueGame();
GameManager.Instance.ExitMiniGame();
GameManager.Instance.ReturnToIdle();

// 属性
GameManager.Instance.CurrentState;      // 当前状态
GameManager.Instance.MiniGameDuration;  // 小游戏时长
GameManager.Instance.ExitDelay;         // 退出延迟
```

#### UIManager

```csharp
// 获取实例
UIManager.Instance

// UI 显示/隐藏
UIManager.Instance.ShowIdleUI();
UIManager.Instance.HideIdleUI();
UIManager.Instance.ShowFurBallGameUI();
UIManager.Instance.HideFurBallGameUI();
UIManager.Instance.ShowTissueGameUI();
UIManager.Instance.HideTissueGameUI();
UIManager.Instance.HideAllUI();

// 计时器
UIManager.Instance.UpdateTimer(float remainingTime);

// 返回按钮
UIManager.Instance.OnReturnButtonClicked();
```

#### AudioManager

```csharp
// 获取实例
AudioManager.Instance

// 音效播放
AudioManager.Instance.PlayCutSound();
AudioManager.Instance.PlayPawSound();
AudioManager.Instance.PlaySwipeSound();
AudioManager.Instance.PlayGameStartSound();
AudioManager.Instance.PlayGameEndSound();

// 背景音乐
AudioManager.Instance.PlayMusic();
AudioManager.Instance.StopMusic();
AudioManager.Instance.PauseMusic();
AudioManager.Instance.ResumeMusic();
AudioManager.Instance.SetMusicVolume(float volume);
AudioManager.Instance.SetSFXVolume(float volume);
```

---

## 13. 开发优先级

### 13.1 Phase 1: 核心系统（已完成）

- [x] GameManager 基础框架
- [x] UIManager 基础框架
- [x] AudioManager 基础框架
- [x] HapticManager 基础框架
- [x] CatController 基础动画
- [x] InputHandler 基础输入
- [x] FurBallGame 基础玩法
- [x] TissueGame 基础玩法

### 13.2 Phase 2: 小鱼干奖励系统（待实现）

- [ ] 创建 TreatCounter 单例（小鱼干计数器）
- [ ] 实现 TreatRewardSystem 奖励发放逻辑
- [ ] 实现 Perfect/Normal 判定奖励（2 分/1 分）
- [ ] 实现甩猫咪奖励（5 分，带冷却）
- [ ] 创建 UnlockSystem 解锁系统
- [ ] 实现小游戏解锁功能（50 小鱼干）
- [ ] 创建 TreatShopUI 商店界面

### 13.3 Phase 3: 触摸互动系统（待实现）

- [ ] 创建 TouchInteractionHandler 组件
- [ ] 实现双指触摸检测逻辑
- [ ] 实现抓取判定计时器（0.5 秒）
- [ ] 集成陀螺仪输入处理
- [ ] 实现摆动物理逻辑（速度→角度，阻尼归零）
- [ ] 添加 CatController 被抓状态和摆动方法
- [ ] 创建被抓动画（CatGrabbed.anim）
- [ ] 创建摆动动画（CatSwing.anim）
- [ ] 配置双肩判定点位置（可配置参数）
- [ ] 集成甩猫咪奖励检测

### 13.4 Phase 4: 节奏系统（待实现）

- [ ] 创建 RhythmManager 单例
- [ ] 实现节拍追踪算法
- [ ] 创建节奏判定系统
- [ ] 添加连击和奖励倍率系统
- [ ] 创建通用节奏 UI 组件
- [ ] 集成小鱼干奖励发放

### 13.5 Phase 5: 小游戏节奏化（待实现）

- [ ] 创建 FurBallRhythmController
- [ ] 实现重拍自动弹起机制
- [ ] 添加节拍可视化提示
- [ ] 集成节奏奖励系统
- [ ] 添加 Perfect/Good 特效
- [ ] 创建 TissueRhythmController
- [ ] 实现纸巾筒发光提示
- [ ] 调整抽纸长度计算
- [ ] 集成切碎节奏奖励

### 13.6 Phase 6: 内容扩展（待实现）

- [ ] 新小游戏设计
- [ ] 更多猫咪动画
- [ ] 成就系统
- [ ] 排行榜系统

---

## 附录 A: 相关文档

- [Unity 设置指南](../README_UNITY_SETUP.md)
- [场景设置指南](../README_SCENE_SETUP.md)
- [预制体设置指南](../README_PREFAB_SETUP.md)
- [UI 按钮设置指南](../README_UI_BUTTON_SETUP.md)
- [退出按钮设置指南](../README_EXIT_BUTTON_SETUP.md)
- [猫咪动画设置指南](../README_CAT_ANIMATOR_SETUP.md)
- [猫咪眨眼动画设置指南](../README_CAT_ANIMATION_SETUP.md)
- [音乐需求文档](../plans/Music_Requirements.md)
- [节奏天国设计文档](../plans/Rhythm_Tengoku_Design_Document.md)
- [节奏游戏开发计划](../plans/Rhythm_Game_Development_Plan.md)

---

## 附录 B: 快速参考

### 命名空间速查

```csharp
using HaChiMiOhNameruDo.Managers;      // 管理器
using HaChiMiOhNameruDo.Gameplay;      // 游戏逻辑
using HaChiMiOhNameruDo.MiniGames;     // 小游戏
using HaChiMiOhNameruDo.UI;            // UI
```

### 单例访问速查

```csharp
GameManager.Instance
UIManager.Instance
AudioManager.Instance
HapticManager.Instance
FurBallGameManager.Instance
TissueGameManager.Instance
```

### 关键参数速查

#### 游戏管理

| 参数 | 默认值 | 说明 |
|------|--------|------|
| `miniGameDuration` | 30f | 小游戏时长（秒） |
| `exitDelay` | 2f | 退出延迟（秒） |

#### 节奏系统

| 参数 | 默认值 | 说明 |
|------|--------|------|
| `bpm` | 110f | 音乐节拍速度 |
| `perfectWindow` | 0.05f | Perfect 判定窗口（秒） |
| `goodWindow` | 0.10f | Good 判定窗口（秒） |
| `normalWindow` | 0.20f | Normal 判定窗口（秒） |

#### 触摸互动

| 参数 | 默认值 | 说明 |
|------|--------|------|
| `grabHoldTime` | 0.5f | 抓取判定时间（秒） |
| `maxSwingAngle` | 30f | 最大摆动角度（度） |
| `dampingFactor` | 0.95f | 摆动阻尼系数 |

#### 小鱼干奖励

| 参数 | 默认值 | 说明 |
|------|--------|------|
| `perfectReward` | 2 | Perfect 判定奖励 |
| `normalReward` | 1 | Normal 判定奖励 |
| `swingReward` | 5 | 甩猫咪奖励 |
| `swingCooldown` | 60f | 甩猫咪冷却（秒） |
| `miniGameUnlockCost` | 50 | 小游戏解锁价格 |

#### 新增单例访问

```csharp
TreatCounter.Instance           // 小鱼干计数器
TreatRewardSystem.Instance      // 奖励发放系统
UnlockSystem.Instance           // 解锁系统
```

---

*文档结束*
