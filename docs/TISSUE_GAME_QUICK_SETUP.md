# 纸巾游戏快速配置指南

## 简化后的配置方式

现在纸巾游戏使用与毛球游戏相同的简化配置方式，所有精灵都通过 **Unity Inspector** 直接配置。

---

## 1. TissueBox 配置

### 在 Hierarchy 中选择 `TissueBox` 对象

### Inspector 配置面板

```
┌─ 组件引用 ─────────────────────┐
│ Sprite Renderer  │ [拖拽 SpriteRenderer] │
│ Box Collider     │ [拖拽 BoxCollider2D]  │
└─────────────────────────────────┘

┌─ 美术素材引用 ─────────────────┐
│ Sprite Idle    │ [默认状态精灵]       │
│ Sprite Pull 1  │ [抽取动画帧 1]       │
│ Sprite Pull 2  │ [抽取动画帧 2]       │
└─────────────────────────────────┘

┌─ 动画设置 ─────────────────────┐
│ Pull Animation Speed  │ 0.1    │
│ Disappear Duration    │ 1.0    │
└─────────────────────────────────┘
```

### 配置步骤

1. **添加组件**
   - 选中 `TissueBox` 对象
   - 添加 `Sprite Renderer` 组件
   - 添加 `Box Collider 2D` 组件
   - 添加 `TissueBox` 脚本组件

2. **配置组件引用**
   - `Sprite Renderer`: 将刚添加的 SpriteRenderer 组件拖拽到字段
   - `Box Collider`: 将刚添加的 BoxCollider2D 组件拖拽到字段

3. **配置精灵**
   - `Sprite Idle`: 从 Project 窗口拖拽默认状态精灵（如 `TissueGame_Box_0`）
   - `Sprite Pull 1`: 拖拽抽取动画帧 1（如 `TissueGame_Box_1`）
   - `Sprite Pull 2`: 拖拽抽取动画帧 2（如 `TissueGame_Box_2`）

4. **调整 Collider**
   - 点击 BoxCollider2D 的 "Edit Collider" 按钮
   - 调整绿色边框覆盖整个精灵

---

## 2. TissuePaper 配置（纸巾条）

### 在 Hierarchy 中选择 `TissuePaper` 对象

TissuePaper 负责管理从纸巾筒拉出的纸巾条，根据扒拉次数显示 Short 或 Long 状态。

### Inspector 配置面板

```
┌─ 组件引用 ─────────────────────┐
│ Sprite Renderer  │ [拖拽 SpriteRenderer] │
└─────────────────────────────────┘

┌─ 美术素材引用 ─────────────────┐
│ Tissue Short     │ [短纸巾精灵]         │
│ Tissue Long      │ [长纸巾精灵]         │
└─────────────────────────────────┘

┌─ 延伸设置 ─────────────────────┐
│ Long Threshold   │ 5              │
└─────────────────────────────────┘
```

### 配置步骤

1. **添加组件**
   - 选中 `TissuePaper` 对象
   - 添加 `Sprite Renderer` 组件
   - 添加 `TissuePaper` 脚本组件

2. **配置组件引用**
   - `Sprite Renderer`: 将 SpriteRenderer 组件拖拽到字段

3. **配置精灵**
   - `Tissue Short`: 拖拽短纸巾精灵（初次被扒拉或切断后显示）
   - `Tissue Long`: 拖拽长纸巾精灵（扒拉 5 次后显示）

4. **设置阈值**
   - `Long Threshold`: 设置为 5，表示扒拉 5 次后切换到长纸巾

### 工作原理

- **0 次扒拉**: 显示短纸巾（默认）
- **1-4 次扒拉**: 显示短纸巾
- **5+ 次扒拉**: 显示长纸巾
- **切断后**: 重置为短纸巾

---

## 3. TissuePileManager 配置（纸巾堆）

### 在 Hierarchy 中选择 `TissuePileManager` 对象

TissuePileManager 负责管理地面上堆积的纸巾，分为 L1 和 L2 两层显示。

### Inspector 配置面板

```
┌─ 组件引用 ─────────────────────┐
│ L2 Renderer      │ [L2 精灵渲染器]      │
│ L1 Renderer      │ [L1 精灵渲染器]      │
└─────────────────────────────────┘

┌─ 美术素材引用 ─────────────────┐
│ Pile L2          │ [纸巾堆 L2 精灵]      │
│ Pile L1          │ [纸巾堆 L1 精灵]      │
└─────────────────────────────────┘

┌─ 配置引用 ─────────────────────┐
│ Config           │ [TissueGameConfig]   │
└─────────────────────────────────┘
```

### 配置步骤

1. **创建子对象**
   - 在 `TissuePileManager` 下创建两个空子对象：`PileL2` 和 `PileL1`
   - 为每个子对象添加 `Sprite Renderer` 组件

2. **添加组件**
   - 选中 `TissuePileManager` 对象
   - 添加 `TissuePileManager` 脚本组件

3. **配置组件引用**
   - `L2 Renderer`: 将 PileL2 的 SpriteRenderer 拖拽到字段
   - `L1 Renderer`: 将 PileL1 的 SpriteRenderer 拖拽到字段
   - `Config`: 拖拽 TissueGameConfig 资源（如果有）

4. **配置精灵**
   - `Pile L2`: 拖拽纸巾堆 L2 精灵（5 次扒拉后显示）
   - `Pile L1`: 拖拽纸巾堆 L1 精灵（10 次扒拉后叠加显示）

### 工作原理

| 扒拉次数 | L2 显示 | L1 显示 |
|---------|--------|--------|
| 0-4 次   | ❌     | ❌     |
| 5-9 次   | ✅     | ❌     |
| 10 次    | ✅     | ✅     |

### 清理机制

- 每 3 次横向划动清理一层
- 先清理 L1（10 次层），再清理 L2（5 次层）
- 清理时播放淡出动画

---

## 4. TissueInputHandler 配置

### 在 Hierarchy 中选择 `TissueInputHandler` 对象

### Inspector 配置面板

```
┌─ 判定区域 ─────────────────────┐
│ Tissue Box Zone  │ [纸巾筒 Collider2D]  │
│ Tissue Pile Zone │ [纸巾堆 Collider2D]  │
└─────────────────────────────────┘
```

---

## 5. TissueGameManager 配置

### 在 Hierarchy 中选择 `TissueGame` 对象（GameManager）

### Inspector 配置面板

```
┌─ 组件引用 ─────────────────────┐
│ Tissue Box       │ [拖拽 TissueBox 对象]     │
│ Tissue Paper     │ [拖拽 TissuePaper 对象]   │
│ Pile Manager     │ [拖拽 TissuePileManager]  │
│ Input Handler    │ [拖拽 TissueInputHandler] │
│ Config           │ [TissueGameConfig 资源]   │
└─────────────────────────────────┘

┌─ 猫咪控制器 ───────────────────┐
│ Cat Controller   │ [拖拽 CatController]    │
└─────────────────────────────────┘
```

---

## 精灵资源位置

假设你的精灵资源在 `Assets/Sprites/` 目录下：

```
Assets/Sprites/
├── TissueGame/
│   ├── TissueGame_Box_0.png      → TissueBox.Sprite Idle
│   ├── TissueGame_Box_1.png      → TissueBox.Sprite Pull 1
│   ├── TissueGame_Box_2.png      → TissueBox.Sprite Pull 2
│   ├── TissueGame_Tissue_Short.png → TissuePaper.Tissue Short
│   ├── TissueGame_Tissue_Long.png  → TissuePaper.Tissue Long
│   ├── TissueGame_Pile_L2.png    → TissuePileManager.Pile L2
│   └── TissueGame_Pile_L1.png    → TissuePileManager.Pile L1
```

---

## 快速配置步骤

1. **创建 TissueBox**
   - Hierarchy 右键 → 2D Object → Sprites → Square
   - 重命名为 `TissueBox`
   - 添加 `TissueBox` 脚本
   - 配置精灵和 Collider

2. **创建 TissuePaper**
   - Hierarchy 右键 → 2D Object → Sprites → Square
   - 重命名为 `TissuePaper`
   - 添加 `TissuePaper` 脚本
   - 配置精灵

3. **创建 TissuePileManager**
   - Hierarchy 右键 → Create Empty
   - 重命名为 `TissuePileManager`
   - 添加 `TissuePileManager` 脚本
   - 创建两个子对象 `PileL2` 和 `PileL1`，分别添加 SpriteRenderer

4. **创建 TissueInputHandler**
   - Hierarchy 右键 → Create Empty
   - 重命名为 `TissueInputHandler`
   - 添加 `TissueInputHandler` 脚本
   - 创建两个空子对象作为判定区域，添加 Collider2D

5. **创建 TissueGameManager**
   - Hierarchy 右键 → Create Empty
   - 重命名为 `TissueGame`
   - 添加 `TissueGameManager` 脚本
   - 关联所有组件引用

---

## 注意事项

1. **SpriteRenderer 设置**
   - 确保 `Order in Layer` 设置正确，避免渲染层级问题
   - TissuePaper 应在 TissueBox 前方渲染

2. **Collider2D 设置**
   - 确保 Collider 覆盖整个精灵
   - 使用 "Edit Collider" 按钮手动调整

3. **排序层级（Sorting Layer）**
   - 建议创建专门的排序层级用于小游戏
   - 确保各组件渲染顺序正确
