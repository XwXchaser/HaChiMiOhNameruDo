# 纸巾筒游戏判定区域设置指南

## 概述

判定区域（Zone）用于检测玩家点击/触摸的位置，从而决定触发哪种操作。

## 判定区域列表

| 区域名称 | 用途 | 操作 |
|----------|------|------|
| TissueBoxZone | 纸巾筒区域 | 向下划动：抽取<br>向左/右划动：切断 |
| TissuePileZone | 纸巾堆积区域 | 向左/右划动：清理（每 3 次清除一层） |

## 设置步骤

### 步骤 1：创建 TissueBoxZone（纸巾筒判定区域）

**目标：** 让纸巾筒可以响应抽取和切断操作

1. 在 Hierarchy 中选中 `TissueBox` 对象

2. 确保已添加 `BoxCollider2D` 组件
   - 如果没有，点击 `Add Component` → 搜索 `BoxCollider2D` → 添加

3. 调整 Collider2D 参数：
   ```
   Size: X=2.5, Y=2.5（根据精灵大小调整）
   Center: X=0, Y=0
   ```

4. 点击 Inspector 中的 `Edit Collider` 按钮
   - 调整绿色边框覆盖整个纸巾筒
   - 确保包含出口位置（纸巾延伸的起点）

5. 完成后再次点击 `Edit Collider` 退出编辑模式

### 步骤 2：创建 TissuePileZone（纸巾堆积判定区域）

**目标：** 让堆积区域可以响应清理操作

1. 在 Hierarchy 创建空对象
   - 右键 → `Create Empty`
   - 命名为 `TissuePileZone`

2. 设置位置
   ```
   Position: X=0, Y=-2.5, Z=0
   ```

3. 添加 BoxCollider2D 组件
   - `Add Component` → 搜索 `BoxCollider2D` → 添加

4. 调整 Collider2D 参数
   ```
   Size: X=4, Y=5
   Center: X=0, Y=0
   ```

5. （可选）可视化调试
   - 在 Scene 视图中，Collider2D 会以绿色线框显示
   - 确保覆盖预期的堆积区域

### 步骤 3：配置到 TissueInputHandler

1. 在 Hierarchy 中选中 `TissueInputHandler` 对象

2. 在 Inspector 中找到 `TissueInputHandler` 组件

3. 配置判定区域字段：

   | 字段 | 拖拽对象 | 说明 |
   |------|----------|------|
   | Tissue Box Zone | TissueBox 对象 | 自动获取其 BoxCollider2D |
   | Tissue Pile Zone | TissuePileZone 对象 | 自动获取其 BoxCollider2D |
   | Config | TissueGameConfig 资源 | 从 Project 窗口拖拽 |

4. 确认配置完成
   - Tissue Box Zone: 应显示 `TissueBox (BoxCollider2D)`
   - Tissue Pile Zone: 应显示 `TissuePileZone (BoxCollider2D)`

### 步骤 4：配置 TissuePileManager

1. 在 Hierarchy 中选中 `TissuePileManager` 对象

2. 在 Inspector 中找到 `TissuePileManager` 组件

3. 配置字段：

   | 字段 | 拖拽对象 | 说明 |
   |------|----------|------|
   | Pile L2 | TissueGame_PileOfTissue_L2 | 从 Project 窗口拖拽 Sprite |
   | Pile L1 | TissueGame_PileOfTissue_L1 | 从 Project 窗口拖拽 Sprite |
   | L2 Renderer | PileL2 对象的 SpriteRenderer | 从 Hierarchy 拖拽 |
   | L1 Renderer | PileL1 对象的 SpriteRenderer | 从 Hierarchy 拖拽 |
   | Config | TissueGameConfig 资源 | 从 Project 窗口拖拽 |

## 场景层级结构示例

```
Hierarchy:
├── TissueGame (GameManager)
│   ├── TissueBox
│   │   ├── Holder (SpriteRenderer)
│   │   ├── Roll (SpriteRenderer)
│   │   └── BoxCollider2D ← TissueBoxZone
│   ├── TissuePaper
│   │   └── SpriteRenderer
│   ├── TissuePileManager
│   │   ├── PileL2 (SpriteRenderer)
│   │   └── PileL1 (SpriteRenderer)
│   ├── TissueInputHandler
│   │   └── (配置引用上述 Collider2D)
│   └── TissuePileZone
│       └── BoxCollider2D ← TissuePileZone
```

## 可视化调试

### 方法 1：使用 Scene 视图

1. 选中带有 Collider2D 的对象
2. 在 Scene 视图中，Collider2D 会以绿色线框显示
3. 点击 `Edit Collider` 可以调整形状

### 方法 2：添加 Gizmos 脚本

在 `TissueInputHandler` 中添加以下脚本用于调试：

```csharp
private void OnDrawGizmos()
{
    // 绘制纸巾筒判定区域
    if (tissueBoxZone != null)
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(tissueBoxZone.bounds.center, tissueBoxZone.bounds.size);
    }
    
    // 绘制堆积判定区域
    if (tissuePileZone != null)
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(tissuePileZone.bounds.center, tissuePileZone.bounds.size);
    }
}
```

## 常见问题

### Q: 点击没有反应？
A: 检查：
1. Collider2D 是否正确添加
2. TissueInputHandler 的字段是否正确配置
3. 判定区域位置是否与预期位置对齐

### Q: 如何调整判定区域大小？
A: 
1. 选中带有 Collider2D 的对象
2. 在 Inspector 中调整 BoxCollider2D 的 Size 参数
3. 或者点击 `Edit Collider` 手动调整

### Q: 判定区域偏移？
A: 
1. 调整 BoxCollider2D 的 Center 参数
2. 或者调整对象的 Transform Position

### Q: 多个判定区域重叠怎么办？
A: 
- 代码会优先检测先判断的区域
- 确保 TissueBoxZone 和 TissuePileZone 不重叠
- 建议垂直排列，TissueBoxZone 在上，TissuePileZone 在下

## 推荐尺寸参考

### TissueBoxZone
```
Position: X=0, Y=2.5, Z=0
Size: X=2.5, Y=2.5
```

### TissuePileZone
```
Position: X=0, Y=-2.5, Z=0
Size: X=4, Y=5
```

## 配置检查清单

- [ ] TissueBox 已添加 BoxCollider2D
- [ ] TissueBoxZone 大小覆盖整个纸巾筒
- [ ] TissuePileZone 空对象已创建
- [ ] TissuePileZone 已添加 BoxCollider2D
- [ ] TissuePileZone 位置在场景下方
- [ ] TissueInputHandler 已配置 Tissue Box Zone
- [ ] TissueInputHandler 已配置 Tissue Pile Zone
- [ ] TissuePileManager 已配置 L2/L1 Renderer
- [ ] 所有组件已关联到 GameManager
