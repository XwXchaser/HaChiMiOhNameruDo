# 纸巾筒游戏设置指南

## 概述

纸巾筒游戏（Tissue Game）是一个模拟抽取纸巾的小游戏，包含抽取、切断、清理、装填等机制。

**核心玩法：**
- 向下划动抽取纸巾（每次划动算 1 次扒拉）
- 纸巾会根据扒拉次数切换 Short/Long 状态
- 向左/右划动切断纸巾
- 切断后纸巾堆积计数增加
- 堆积 5 次后出现 L2 纸巾堆，10 次后叠加 L1
- 在堆积区域横向划动清理（每 3 次划动清理一层）
- 纸巾耗尽后纸卷慢慢消失，只剩 Holder
- 向右划动清空弹仓，再向左划动装填新纸卷

## 游戏机制

### 基本流程

1. **抽取纸巾**：在纸巾筒区域向下划动（每次划动算 1 次扒拉）
2. **纸巾延伸**：纸巾根据扒拉次数切换 Short/Long 状态
3. **切断纸巾**：在纸巾筒区域向左/右划动
4. **堆积计数**：切断后扒拉次数累计到纸巾堆
5. **纸巾堆显示**：5 次后显示 L2，10 次后显示 L2+L1
6. **清理纸巾**：在堆积区域向左/右划动（每 3 次划动清除一层）
7. **纸巾耗尽**：纸卷慢慢消失，只显示 Holder
8. **清空弹仓**：纸巾耗尽后，向右划动清空弹仓
9. **装填新卷**：从屏幕右侧向中心划动装填新纸卷

### 状态机

#### 纸巾筒状态（TissueBoxState）

| 状态 | 说明 | 可执行操作 |
|------|------|-----------|
| Idle | 空闲 | 抽取、切断 |
| Pulling | 抽取中（播放 Roll_1/2 动画） | - |
| Empty | 耗尽（纸卷消失中） | 清空弹仓 |
| ClearingChamber | 清空弹仓中 | - |
| Reloading | 装填中 | - |

#### 纸巾状态（TissuePaperState）

| 状态 | 说明 |
|------|------|
| Connected | 连接中（延伸） |
| Cut | 已切断 |
| Retracting | 收回中 |
| Retracted | 已收回 |

#### 堆积纸巾状态（TissuePileState）

| 状态 | 说明 |
|------|------|
| Idle | 空闲 |
| Clearing | 清除中 |

#### 游戏状态（TissueGameState）

| 状态 | 说明 |
|------|------|
| Idle | 空闲 |
| Playing | 游戏中 |
| Pulling | 抽取中 |
| Cutting | 切断中 |
| Empty | 纸巾耗尽 |
| ClearingChamber | 清空弹仓 |
| Reloading | 装填中 |
| GameOver | 游戏结束（堆积达到 10 次） |

## 组件设置

### 1. TissueGameManager

游戏主管理器，负责整体状态管理。

**组件引用：**
- `Tissue Box`: TissueBox 组件
- `Tissue Paper`: TissuePaper 组件
- `Pile Manager`: TissuePileManager 组件
- `Input Handler`: TissueInputHandler 组件
- `Config`: TissueGameConfig 资源

### 2. TissueBox

纸巾筒组件，负责纸巾筒的状态管理和动画。

**Inspector 设置：**
```
Holder Renderer: 纸巾筒架子（始终显示）
Roll Renderer: 纸巾卷精灵渲染器
Box Collider: 纸巾筒碰撞体（BoxCollider2D）

美术素材引用:
  - Roll Idle: TissueGame_TissueRoll_0（默认状态）
  - Roll Pull 1: TissueGame_TissueRoll_1（抽取动画帧 1）
  - Roll Pull 2: TissueGame_TissueRoll_2（抽取动画帧 2）

动画设置:
  - Pull Animation Speed: 0.1 (抽取动画播放速度，秒/帧)
  - Disappear Duration: 1.0 (纸巾耗尽时纸卷消失时长)
```

**位置设置：**
- 纸巾筒应放置在场景上方
- 建议 Y 坐标：2.0 ~ 3.0（根据相机大小调整）
- Collider2D 应覆盖整个纸巾筒可见区域

### 3. TissuePaper

纸巾组件，负责纸巾的 Short/Long 状态切换。

**Inspector 设置：**
```
Sprite Renderer: 纸巾精灵渲染器

美术素材引用:
  - Tissue Short: TissueGame_Tissue_Short（初次扒拉或切断后）
  - Tissue Long: TissueGame_Tissue_Long（持续扒拉不切断）

延伸设置:
  - Long Threshold: 5 (切换到长纸巾所需的扒拉次数)
```

**位置设置：**
- 纸巾筒出口位置应与 TissueBox 的出口对齐
- 建议位置：纸巾筒底部出口处

### 4. TissuePileManager

堆积纸巾管理器，负责管理 L1/L2 两层纸巾堆。

**Inspector 设置：**
```
美术素材引用:
  - Pile L2: TissueGame_PileOfTissue_L2（5 次后显示）
  - Pile L1: TissueGame_PileOfTissue_L1（10 次后叠加）

组件引用:
  - L2 Renderer: L2 精灵渲染器
  - L1 Renderer: L1 精灵渲染器
  - Config: TissueGameConfig 资源
```

**位置设置：**
- TissuePileManager 应放置在场景下方
- 建议 Y 坐标：-2.0 ~ -3.0
- L1 和 L2 的 SpriteRenderer 应重叠放置

### 5. TissueInputHandler

输入处理器，支持多判定区域。

**Inspector 设置：**
```
Tissue Box Zone: 纸巾筒判定区域（Collider2D）
Tissue Pile Zone: 纸巾堆积判定区域（Collider2D）
Config: TissueGameConfig 资源
```

### 6. TissueGameConfig

游戏配置 ScriptableObject。

**创建方式：**
1. 在 Project 窗口右键
2. 选择 `Create > TissueGame > Config`
3. 命名为 `TissueGameConfig`
4. 配置参数

**参数说明：**
```
扒拉次数设置:
  - Pile L2 Threshold: 5 (纸巾堆 L2 出现所需的扒拉次数)
  - Pile L1 Threshold: 10 (纸巾堆 L1 出现所需的扒拉次数，上限)

划动阈值设置:
  - Pull Threshold: 50 (向下划动触发阈值，像素)
  - Swipe Threshold: 50 (横向划动触发阈值，像素)
  - Swipes Per Clear: 3 (清理一层纸巾堆需要的划动次数)

清除动画设置:
  - Clear Duration: 0.3 (清除动画时长，秒)

装填动画设置:
  - Clear Chamber Duration: 0.3 (清空弹仓动画时长，秒)
  - Reload Duration: 0.5 (装填新纸卷动画时长，秒)
  - Disappear Duration: 1.0 (纸巾耗尽时纸卷消失时长，秒)

抽取动画设置:
  - Pull Animation Speed: 0.1 (抽取动画播放速度，秒/帧)

纸巾延伸设置:
  - Long Tissue Threshold: 5 (切换到长纸巾所需的扒拉次数)

得分设置:
  - Ideal Tissue Length: 8 (理想纸巾长度，扒拉次数)
```

## 场景设置步骤

### 步骤 1：创建游戏对象

1. 在 Hierarchy 创建空对象 `TissueGame`
2. 添加 `TissueGameManager` 组件

### 步骤 2：创建纸巾筒

1. 在 Hierarchy 创建空对象 `TissueBox`
2. 添加 `SpriteRenderer`（Holder）和 `SpriteRenderer`（Roll）两个组件
3. 添加 `BoxCollider2D`
4. 添加 `TissueBox` 组件
5. 设置精灵和碰撞体

**Collider2D 设置：**
```
Size: 根据精灵大小调整（例如：X=2, Y=2）
Center: 根据精灵中心调整
```

### 步骤 3：创建纸巾

1. 在 Hierarchy 创建空对象 `TissuePaper`
2. 添加 `SpriteRenderer` 和 `TissuePaper` 组件
3. 设置 Short/Long 精灵引用

### 步骤 4：创建堆积管理器

1. 在 Hierarchy 创建两个空对象，分别命名为 `PileL2` 和 `PileL1`
2. 分别添加 `SpriteRenderer` 组件
3. 设置对应的精灵（L2 和 L1）
4. 创建空对象 `TissuePileManager`
5. 添加 `TissuePileManager` 组件
6. 将 L2 和 L1 的 SpriteRenderer 引用拖拽到对应字段

**判定区域创建（重要）：**

#### 使用 Collider2D（推荐）

1. 在 Hierarchy 创建空对象 `TissuePileZone`
2. 添加 `BoxCollider2D` 组件
3. 调整 Collider 大小覆盖堆积区域
4. 将 `TissuePileZone` 拖拽到 `TissuePileManager` 和 `TissueInputHandler` 的对应字段

**Collider2D 设置：**
```
Size: X=5, Y=3 (根据场景调整)
Center: X=0, Y=0
Is Trigger: 不勾选（仅用于检测点击位置）
```

### 步骤 5：创建输入处理器

1. 在 Hierarchy 创建空对象 `TissueInputHandler`
2. 添加 `TissueInputHandler` 组件
3. 设置两个判定区域

**判定区域配置：**

| 字段 | 说明 | 设置方法 |
|------|------|----------|
| Tissue Box Zone | 纸巾筒判定区域 | 拖拽 TissueBox 的 Collider2D |
| Tissue Pile Zone | 纸巾堆积判定区域 | 拖拽步骤 4 创建的 Collider2D |

### 步骤 6：创建配置资源

1. 在 Project 窗口右键
2. 选择 `Create > TissueGame > Config`
3. 命名为 `TissueGameConfig`
4. 配置参数

### 步骤 7：关联组件

在 `TissueGameManager` 组件中：
1. 拖拽 `TissueBox` 对象到 `Tissue Box` 字段
2. 拖拽 `TissuePaper` 对象到 `Tissue Paper` 字段
3. 拖拽 `TissuePileManager` 对象到 `Pile Manager` 字段
4. 拖拽 `TissueInputHandler` 对象到 `Input Handler` 字段
5. 拖拽 `TissueGameConfig` 资源到 `Config` 字段

## 判定区域详细配置

### 判定区域原理

游戏使用 `Collider2D.OverlapPoint()` 方法检测点击位置是否在判定区域内。

### 纸巾筒判定区域（TissueBoxZone）

**位置：** 与纸巾筒 Sprite 重合

**大小调整步骤：**
1. 选中 TissueBox 对象
2. 在 Inspector 中找到 BoxCollider2D
3. 点击 "Edit Collider" 按钮
4. 调整绿色边框覆盖整个纸巾筒
5. 确保包含出口位置（纸巾延伸的起点）

**推荐尺寸：**
```
Size: X=2.5, Y=2.5
Center: X=0, Y=0
```

### 纸巾堆积判定区域（TissuePileZone）

**位置：** 场景下方，堆积纸巾的区域

**大小调整步骤：**
1. 创建空对象 `TissuePileZone`
2. 添加 `BoxCollider2D`
3. 调整 Size 覆盖预期堆积区域

**推荐尺寸：**
```
Size: X=4, Y=5
Center: X=0, Y=0
Position: Y=-2.5 (根据相机调整)
```

## 美术素材使用

### 素材列表

| 素材名称 | 用途 | 设置位置 |
|----------|------|----------|
| TissueGame_Holder | 纸巾筒架子 | TissueBox.Holder Renderer |
| TissueGame_TissueRoll_0 | 纸巾筒默认状态 | TissueBox.Roll Idle |
| TissueGame_TissueRoll_1 | 抽取动画帧 1 | TissueBox.Roll Pull 1 |
| TissueGame_TissueRoll_2 | 抽取动画帧 2 | TissueBox.Roll Pull 2 |
| TissueGame_Tissue_Short | 短纸巾 | TissuePaper.Tissue Short |
| TissueGame_Tissue_Long | 长纸巾 | TissuePaper.Tissue Long |
| TissueGame_PileOfTissue_L2 | 纸巾堆 L2（5 次） | TissuePileManager.Pile L2 |
| TissueGame_PileOfTissue_L1 | 纸巾堆 L1（10 次） | TissuePileManager.Pile L1 |

### 动画说明

#### 抽取动画（Pulling）
- Roll_1 和 Roll_2 循环播放
- 播放速度由 `Pull Animation Speed` 控制

#### 纸巾延伸
- 扒拉次数 < LongThreshold：显示 Short
- 扒拉次数 >= LongThreshold：显示 Long

#### 纸巾堆显示
- 扒拉次数 < 5：不显示
- 5 <= 扒拉次数 < 10：显示 L2
- 扒拉次数 >= 10：显示 L2 + L1（叠加）

#### 清理动画
- 先清理 L1（淡出），扒拉次数回到 5
- 再清理 L2（淡出），扒拉次数归零

#### 耗尽动画
- 纸卷（Roll）淡出消失，时长由 `Disappear Duration` 控制
- Holder 始终显示

## 输入方式

### 编辑器测试
- 按住鼠标左键拖动

### 安卓手机版
- 手指滑动触摸

## 判定区域

游戏有两个独立的判定区域：

1. **纸巾筒区域（TissueBoxZone）**
   - 向下划动：抽取纸巾（增加扒拉次数）
   - 向左/右划动：切断纸巾

2. **纸巾堆积区域（TissuePileZone）**
   - 向左/右划动：清理纸巾（每 3 次划动清除一层）

## 得分系统

根据切断纸巾时的扒拉次数计算得分：

| 条件 | 得分 |
|------|------|
| 扒拉次数 = 理想长度 | 100 分（完美） |
| 扒拉次数差 ≤ 2 | 50 分（很好） |
| 扒拉次数差 ≤ 5 | 25 分（一般） |
| 扒拉次数差 > 5 | 10 分（较差） |

## 游戏结束条件

当扒拉次数达到 10 次（L2+L1 叠加）时，游戏结束。

## 常见问题

### Q: 划动不响应？
A: 检查阈值设置，确保划动距离超过 `Pull Threshold` 或 `Swipe Threshold`

### Q: 纸巾不切换 Long 状态？
A: 检查 `Long Tissue Threshold` 设置，确保扒拉次数达到阈值

### Q: 纸巾堆不显示？
A: 检查：
   1. L1/L2 SpriteRenderer 是否正确设置
   2. Pile L2/L1 精灵是否拖拽
   3. 扒拉次数是否达到 5/10 次

### Q: 清理不生效？
A: 确保在堆积区域划动，且划动方向为横向（左/右）

### Q: 装填流程无法进行？
A: 按照顺序操作：
   1. 纸巾耗尽后，纸卷会慢慢消失
   2. 向右划动清空弹仓
   3. 等待清空动画完成
   4. 从屏幕右侧向中心划动装填新纸卷

### Q: 判定区域不工作？
A: 检查：
   1. Collider2D 是否正确添加
   2. 判定区域位置是否与预期位置对齐
   3. 使用 Gizmos 可视化检查判定区域范围

## 相关文件

- `Assets/Scripts/MiniGames/TissueGame/TissueGameManager.cs`
- `Assets/Scripts/MiniGames/TissueGame/TissueBox.cs`
- `Assets/Scripts/MiniGames/TissueGame/TissuePaper.cs`
- `Assets/Scripts/MiniGames/TissueGame/TissuePileManager.cs`
- `Assets/Scripts/MiniGames/TissueGame/TissueInputHandler.cs`
- `Assets/Scripts/MiniGames/TissueGame/TissueGameConfig.cs`
