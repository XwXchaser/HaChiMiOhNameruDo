# HaChiMiOhNameruDo 游戏系统策划案

> **文档版本**: v2.0  
> **最后更新**: 2026-04-05  
> **项目名称**: HaChiMiOhNameruDo（ハチミオナメルド）  
> **游戏类型**: 休闲解压小游戏 + 音乐节奏元素  

---

## 目录

1. [项目概述](#1-项目概述)
2. [游戏核心概念](#2-游戏核心概念)
3. [系统架构](#3-系统架构)
4. [游戏模式](#4-游戏模式)
5. [小游戏系统](#5-小游戏系统)
6. [触摸互动系统](#6-触摸互动系统)
7. [奖励系统](#7-奖励系统)
8. [UI 系统](#8-ui-系统)
9. [音频系统](#9-音频系统)
10. [输入系统](#10-输入系统)
11. [数据结构](#11-数据结构)
12. [技术实现](#12-技术实现)

---

## 1. 项目概述

### 1.1 基本信息

| 属性 | 值 |
|------|-----|
| **项目名称** | HaChiMiOhNameruDo（ハチミオナメルド） |
| **游戏类型** | 休闲解压小游戏 + 音乐节奏元素 |
| **目标平台** | Android / iOS |
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
│       ├── TissuePileManager   # 堆积纸巾管理
│       ├── TissueGameManager   # 纸巾游戏管理
│       └── TissueGameConfig    # 游戏配置
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
│   │   └── TouchInteractionHandler.cs
│   ├── MiniGames/
│   │   ├── FurBallGame/
│   │   │   ├── FurBall.cs
│   │   │   └── FurBallGameManager.cs
│   │   └── TissueGame/
│   │       ├── TissueBox.cs
│   │       ├── TissuePaper.cs
│   │       ├── TissueInputHandler.cs
│   │       ├── TissuePileManager.cs
│   │       ├── TissueGameManager.cs
│   │       └── TissueGameConfig.cs
│   ├── Rewards/
│   │   ├── TreatCounter.cs
│   │   ├── TreatRewardSystem.cs
│   │   └── UnlockSystem.cs
│   └── UI/
│       ├── IdleUI.cs
│       └── TreatShopUI.cs
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

## 4. 游戏模式

### 4.1 IDLE 模式

**描述**: 游戏主界面，玩家可以看到猫咪并与之互动。

**功能**:
- 显示猫咪（呼吸、眨眼动画）
- 显示道具按钮（毛球、纸巾筒）
- 支持触摸互动（双指抓取猫咪）
- 显示小鱼干数量

**状态转换**:
- 点击毛球按钮 → 进入毛球游戏
- 点击纸巾筒按钮 → 进入纸巾筒游戏
- 双指抓取猫咪 → 触发触摸互动

### 4.2 毛球游戏模式

**描述**: 玩家拍打毛球，猫咪会做出反应。

**游戏时长**: 30 秒

**核心玩法**:
- 点击/拍打毛球
- 毛球会移动、反弹
- 猫咪会注视、拍打毛球
- 根据互动频率获得小鱼干奖励

**状态转换**:
- 游戏开始 → 毛球游戏模式
- 时间到 → 返回 IDLE 模式，显示结算

### 4.3 纸巾筒游戏模式

**描述**: 玩家从纸巾筒抽取纸巾并切碎。

**游戏时长**: 30 秒

**核心玩法**:
- 向下划动抽取纸巾
- 横向划动切断纸巾
- 在堆积区域划动清理纸巾
- 纸巾耗尽后装填新纸卷

**状态转换**:
- 游戏开始 → 纸巾筒游戏模式
- 时间到/堆积上限 → 返回 IDLE 模式，显示结算

---

## 5. 小游戏系统

### 5.1 毛球游戏系统

#### 5.1.1 组件结构

```
FurBallGameManager (主管理器)
└── FurBall (毛球组件)
```

#### 5.1.2 毛球状态机

| 状态 | 说明 | 触发条件 |
|------|------|----------|
| Idle | 静止 | 初始状态 |
| Moving | 移动中 | 被拍打 |
| Bouncing | 反弹 | 碰到边界 |

#### 5.1.3 游戏参数

| 参数 | 默认值 | 说明 |
|------|--------|------|
| moveSpeed | 5.0 | 毛球移动速度 |
| bounceForce | 10.0 | 反弹力度 |
| friction | 0.98 | 摩擦力 |

### 5.2 纸巾筒游戏系统

#### 5.2.1 组件结构

```
TissueGameManager (主管理器)
├── TissueBox (纸巾筒)
├── TissuePaper (纸巾)
├── TissuePileManager (堆积管理)
└── TissueInputHandler (输入处理)
```

#### 5.2.2 游戏状态机

| 状态 | 说明 | 触发条件 |
|------|------|----------|
| Idle | 空闲 | 初始状态 |
| Playing | 游戏中 | StartGame() |
| Pulling | 抽取中 | 向下划动 |
| Cutting | 切断中 | 横向划动 |
| Empty | 纸巾耗尽 | 纸巾用完 |
| ClearingChamber | 清空弹仓 | 向右划动 |
| Reloading | 装填中 | 向左划动 |
| GameOver | 游戏结束 | 堆积上限 |

#### 5.2.3 操作方式

| 操作 | 输入方式 | 判定区域 | 效果 |
|------|----------|----------|------|
| 抽取 | 向下划动 | 纸巾筒区域 | 纸巾延伸 |
| 切断 | 向左/右划动 | 纸巾筒区域 | 纸巾切断并堆积 |
| 清理 | 向左/右划动 | 堆积区域 | 累计 3 次清除一个纸巾 |
| 清空 | 向右划动 | 纸巾筒区域 | 清空弹仓（耗尽后） |
| 装填 | 向左划动 | 纸巾筒区域 | 装填新纸卷（清空后） |

#### 5.2.4 判定区域配置

| 区域 | 组件类型 | 推荐尺寸 | 位置 |
|------|----------|----------|------|
| TissueBoxZone | BoxCollider2D | X=2.5, Y=2.5 | 纸巾筒位置 |
| TissuePileZone | BoxCollider2D | X=4, Y=5 | 场景下方 |

#### 5.2.5 游戏参数

| 参数 | 默认值 | 说明 |
|------|--------|------|
| maxTissueCount | 15 | 纸巾堆积上限 |
| pullThreshold | 50 | 向下划动触发阈值 |
| swipeThreshold | 50 | 横向划动触发阈值 |
| swipesPerClear | 3 | 清理一个纸巾需要的划动次数 |
| idealTissueLength | 10 | 理想纸巾长度（段数） |

#### 5.2.6 得分计算

| 条件 | 得分 |
|------|------|
| 长度 = 理想长度 | 100 分（完美） |
| \|长度 - 理想长度\| ≤ 2 | 50 分（很好） |
| \|长度 - 理想长度\| ≤ 5 | 25 分（一般） |
| \|长度 - 理想长度\| > 5 | 10 分（较差） |

---

## 6. 触摸互动系统

### 6.1 举起猫咪玩法

**核心玩法**:
- 玩家在 IDLE 状态下双指按住猫咪"双肩"判定点
- 持续按住约 0.5 秒后触发"举起"判定
- 启用手机陀螺仪，猫咪下半身随陀螺仪左右摆动
- 松开手指后猫咪落下/回到原位

### 6.2 技术规格

| 要素 | 描述 |
|------|------|
| **触发条件** | 双指同时按住猫咪双肩判定点 |
| **判定时间** | 持续按住约 0.5 秒 |
| **交互方式** | 双指抓取 + 陀螺仪倾斜 |
| **摆动逻辑** | 陀螺仪摆动速度 → 摆动角度，随后慢慢归零 |
| **结束条件** | 松开手指 |

### 6.3 猫咪动画状态

| 状态 | 说明 | 触发条件 |
|------|------|----------|
| Idle |  idle 状态，面向前方 | 初始状态 |
| Blink | 随机或节拍触发眨眼 | 随机/节拍 |
| Breath | 呼吸动画（2 拍一次） | 持续 |
| Paw | 拍击动作 | 玩家输入 |
| Turn | 转身 | 切换小游戏 |
| Grabbed | 被双指抓起 | 触摸互动 |
| Swing | 随陀螺仪摆动 | 触摸互动 |

---

## 7. 奖励系统

### 7.1 小鱼干系统

**用途**:
- 游戏内货币
- 解锁新内容
- 购买道具

### 7.2 奖励获取

| 行为 | 奖励数量 |
|------|----------|
| 毛球游戏互动 | 1-5 个/次 |
| 纸巾筒游戏完美切断 | 10 个/次 |
| 纸巾筒游戏良好切断 | 5 个/次 |
| 纸巾筒游戏一般切断 | 2 个/次 |
| 触摸互动 | 1 个/次 |

### 7.3 解锁系统

**可解锁内容**:
- 新猫咪皮肤
- 新背景音乐
- 新小游戏

---

## 8. UI 系统

### 8.1 UI 层级结构

```
Canvas
├── IdleUI
│   ├── CatContainer
│   ├── PropButtons
│   │   ├── FurBallButton
│   │   └── TissueBoxButton
│   └── TreatCounter
├── FurBallGameUI
│   ├── Timer
│   └── Score
├── TissueGameUI
│   ├── Timer
│   ├── Score
│   └── TissueCount
└── TreatShopUI
    ├── ShopItems
    └── BuyButtons
```

### 8.2 UI 管理器

**UIManager 职责**:
- 管理 UI 面板显示/隐藏
- 更新 UI 数据
- 处理 UI 事件

---

## 9. 音频系统

### 9.1 音频管理器

**AudioManager 职责**:
- 播放背景音乐
- 播放音效
- 音量控制

### 9.2 音频类型

| 类型 | 说明 | 示例 |
|------|------|------|
| BGM | 背景音乐 | 菜单音乐、游戏音乐 |
| SFX | 音效 | 点击音、切断音、得分音 |
| Voice | 语音 | 猫咪叫声 |

### 9.3 音频触发

| 事件 | 音频 |
|------|------|
| 点击按钮 | UI 点击音 |
| 拍打毛球 | 拍打音 |
| 切断纸巾 | 切断音 |
| 完美切断 | 得分音 |
| 触摸互动 | 猫咪叫声 |

---

## 10. 输入系统

### 10.1 输入类型

| 类型 | 平台 | 说明 |
|------|------|------|
| 鼠标 | PC/编辑器 | 点击、拖动 |
| 触摸 | 移动端 | 单指点击、多指触摸 |
| 陀螺仪 | 移动端 | 设备倾斜 |

### 10.2 输入管理器

**InputHandler 职责**:
- 统一处理鼠标和触摸输入
- 检测划动方向和距离
- 触发相应事件

### 10.3 触摸互动输入

**TouchInteractionHandler 职责**:
- 检测双指触摸
- 计算触摸点位置
- 读取陀螺仪数据
- 控制猫咪摆动

---

## 11. 数据结构

### 11.1 游戏状态枚举

```csharp
public enum GameState
{
    Idle,           // IDLE 状态
    FurBallGame,    // 毛球游戏
    TissueGame      // 纸巾筒游戏
}
```

### 11.2 纸巾筒游戏状态

```csharp
public enum TissueGameState
{
    Idle,           // 空闲
    Playing,        // 游戏中
    Pulling,        // 抽取中
    Cutting,        // 切断中
    Empty,          // 纸巾耗尽
    ClearingChamber,// 清空弹仓
    Reloading,      // 装填中
    GameOver        // 游戏结束
}
```

### 11.3 配置数据结构

```csharp
[CreateAssetMenu(fileName = "TissueGameConfig", menuName = "TissueGame/Config")]
public class TissueGameConfig : ScriptableObject
{
    // 数量设置
    public int maxTissueCount = 15;
    public int maxPaperLength = 20;
    
    // 输入设置
    public float pullThreshold = 50f;
    public float swipeThreshold = 50f;
    public int swipesPerClear = 3;
    
    // 动画设置
    public float clearMoveDistance = 1000f;
    public float clearMoveDuration = 0.5f;
    public float clearChamberDuration = 0.3f;
    public float reloadDuration = 0.5f;
    
    // 延伸设置
    public int segmentsPerPull = 1;
    public float segmentHeight = 0.1f;
    
    // 得分设置
    public int idealTissueLength = 10;
}
```

---

## 12. 技术实现

### 12.1 关键技术点

1. **多判定区域输入处理**
   - 使用 Collider2D.OverlapPoint() 检测点击位置
   - 支持鼠标和触摸统一处理

2. **状态机管理**
   - 每个组件独立状态机
   - 主管理器协调全局状态

3. **事件驱动架构**
   - 组件间通过事件通信
   - 松耦合设计

4. **ScriptableObject 配置**
   - 游戏参数可配置化
   - 便于平衡调整

5. **单例模式**
   - GameManager、UIManager、AudioManager 使用单例
   - 方便全局访问

### 12.2 性能优化

1. **对象池**
   - 纸巾段预制体复用
   - 减少 Instantiate/Destroy 开销

2. **协程动画**
   - 使用 StartCoroutine 处理时序动画
   - 避免 Update 中的复杂计算

3. **事件订阅管理**
   - OnEnable 订阅，OnDisable 取消订阅
   - 防止内存泄漏

### 12.3 扩展方向

1. **游戏性扩展**
   - 连击系统
   - 特殊纸巾
   - 道具系统

2. **视觉扩展**
   - 猫咪皮肤系统
   - 特效系统
   - UI 动画优化

3. **音频扩展**
   - 不同操作的音效
   - 背景音乐
   - 语音反馈

---

## 附录：相关文件

### 脚本文件
- `Assets/Scripts/Managers/GameManager.cs`
- `Assets/Scripts/Managers/UIManager.cs`
- `Assets/Scripts/Managers/AudioManager.cs`
- `Assets/Scripts/Managers/HapticManager.cs`
- `Assets/Scripts/Gameplay/CatController.cs`
- `Assets/Scripts/Gameplay/InputHandler.cs`
- `Assets/Scripts/MiniGames/FurBallGame/FurBall.cs`
- `Assets/Scripts/MiniGames/FurBallGame/FurBallGameManager.cs`
- `Assets/Scripts/MiniGames/TissueGame/TissueBox.cs`
- `Assets/Scripts/MiniGames/TissueGame/TissuePaper.cs`
- `Assets/Scripts/MiniGames/TissueGame/TissueInputHandler.cs`
- `Assets/Scripts/MiniGames/TissueGame/TissuePileManager.cs`
- `Assets/Scripts/MiniGames/TissueGame/TissueGameManager.cs`
- `Assets/Scripts/MiniGames/TissueGame/TissueGameConfig.cs`

### 文档文件
- `docs/GAME_SYSTEM_DESIGN.md` - 系统策划案（本文档）
- `docs/GAME_DESIGN_DOCUMENT.md` - 游戏设计文档
- `docs/GAME_DESIGN_AI.md` - AI 设计文档
- `docs/TISSUE_GAME_DESIGN.md` - 纸巾筒游戏设计
- `docs/TISSUE_GAME_SETUP.md` - 纸巾筒游戏设置指南
- `docs/RHYTHM_GAME_SETUP.md` - 节奏游戏设置指南

---

## 版本历史

| 版本 | 日期 | 变更内容 |
|------|------|----------|
| 1.0 | 2026-04-04 | 初始版本 |
| 2.0 | 2026-04-05 | 添加纸巾筒游戏完整系统设计 |
