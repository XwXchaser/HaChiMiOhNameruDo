# 预制体创建指南

本文档将指导您如何创建预制体（Prefabs）。

## 什么是预制体

预制体是 Unity 中的一种资源类型，允许您创建可重复使用的 GameObject 配置。预制体可以在多个场景中实例化，修改预制体会影响所有实例。

## 创建预制体文件夹结构

首先在 `Assets/Prefabs` 下创建以下文件夹结构：

```
Assets/Prefabs/
├── Managers/
├── MiniGames/
│   ├── FurBallGame/
│   └── TissueGame/
│   └── UI/
```

## 创建管理器预制体

### 1. GameManager 预制体

**步骤：**
1. 在层级视图中选择 `GameManager` 对象
2. 将其拖到 `Assets/Prefabs/Managers/` 文件夹中
3. 命名为 `GameManager.prefab`

**配置检查：**
- 确保 `GameManager` 脚本已附加
- 检查参数：
  - Current State: Idle
  - Mini Game Duration: 30

### 2. UIManager 预制体

**步骤：**
1. 在层级视图中选择 `UIManager` 对象
2. 将其拖到 `Assets/Prefabs/Managers/` 文件夹中
3. 命名为 `UIManager.prefab`

**配置检查：**
- 确保 `UIManager` 脚本已附加
- 检查 UI 面板引用是否已设置

### 3. AudioManager 预制体

**步骤：**
1. 在层级视图中选择 `AudioManager` 对象
2. 将其拖到 `Assets/Prefabs/Managers/` 文件夹中
3. 命名为 `AudioManager.prefab`

**配置检查：**
- 确保 `AudioManager` 脚本已附加
- 添加音频剪辑引用（可选）

### 4. HapticManager 预制体

**步骤：**
1. 在层级视图中选择 `HapticManager` 对象
2. 将其拖到 `Assets/Prefabs/Managers/` 文件夹中
3. 命名为 `HapticManager.prefab`

**配置检查：**
- 确保 `HapticManager` 脚本已附加
- 检查震动强度设置

## 创建小游戏预制体

### 5. FurBall 预制体

**步骤：**
1. 在层级视图中选择 `FurBall` 对象
2. 将其拖到 `Assets/Prefabs/MiniGames/FurBallGame/` 文件夹中
3. 命名为 `FurBall.prefab`

**配置检查：**
- 确保 `FurBall` 脚本已附加
- 确保 `SpriteRenderer` 组件已配置
- 确保 `CircleCollider2D` 组件已配置
- 检查参数：
  - Launch Height: 5
  - Launch Duration: 0.5
  - Fall Duration: 1
  - Stay Out Duration: 1.5

### 6. TissueBox 预制体

**步骤：**
1. 在层级视图中选择 `TissueBox` 对象
2. 将其拖到 `Assets/Prefabs/MiniGames/TissueGame/` 文件夹中
3. 命名为 `TissueBox.prefab`

**配置检查：**
- 确保 `TissueBox` 脚本已附加
- 确保 `SpriteRenderer` 组件已配置
- 确保 `BoxCollider2D` 组件已配置
- 检查参数：
  - Move Up Duration: 1
  - Move Up Distance: 3

### 7. TissuePaper 预制体

**步骤：**
1. 在层级视图中选择 `TissuePaper` 对象
2. 将其拖到 `Assets/Prefabs/MiniGames/TissueGame/` 文件夹中
3. 命名为 `TissuePaper.prefab`

**配置检查：**
- 确保 `TissuePaper` 脚本已附加
- 确保 `TissueInputHandler` 脚本已附加
- 确保 `SpriteRenderer` 组件已配置
- 确保 `BoxCollider2D` 组件已配置
- 检查参数：
  - Segment Height: 0.1
  - Extend Speed: 0.5
  - Swipe Threshold: 50

## 使用预制体

### 在场景中实例化预制体

1. 从 `Assets/Prefabs` 文件夹中将预制体拖到层级视图中
2. 或者在代码中使用 `Instantiate()` 方法：

```csharp
// 示例：实例化 FurBall 预制体
public FurBall furBallPrefab;

void SpawnFurBall()
{
    FurBall newBall = Instantiate(furBallPrefab, spawnPosition, Quaternion.identity);
}
```

### 修改预制体

1. 在层级视图中选择预制体实例
2. 进行修改
3. 点击 Inspector 顶部的 "Apply" 按钮保存更改到预制体
4. 或者点击 "Revert" 撤销更改

### 创建预制体变体

如果您需要基于现有预制体创建略有不同的版本：

1. 右键点击预制体
2. 选择 "Create" → "Prefab Variant"
3. 修改变体的参数

## 预制体引用设置

### FurBallGameManager 引用设置

1. 选择 `FurBallGameManager` 对象
2. 在 Inspector 中设置引用：
   - **Fur Ball**: 拖入 `FurBall` 预制体或场景中的 FurBall 对象
   - **Cat Controller**: 拖入 `CatController` 对象

### TissueGameManager 引用设置

1. 选择 `TissueGameManager` 对象
2. 在 Inspector 中设置引用：
   - **Tissue Box**: 拖入 `TissueBox` 预制体或场景中的 TissueBox 对象
   - **Tissue Paper**: 拖入 `TissuePaper` 预制体或场景中的 TissuePaper 对象
   - **Cat Controller**: 拖入 `CatController` 对象

## 常见问题

### Q: 预制体显示为灰色
A: 这是正常的，表示这是一个预制体资源。

### Q: 修改预制体后场景中的实例没有更新
A: 确保点击了 "Apply" 按钮保存更改。

### Q: 预制体引用丢失
A: 重新将对象从层级视图或项目视图拖到引用字段中。

### Q: 如何删除预制体
A: 在项目视图中右键点击预制体，选择 "Delete"。注意：这将删除预制体资源，但不会影响已实例化的对象。

## 下一步

完成预制体创建后，您可以：
1. 添加精灵图片替换默认形状
2. 添加音频剪辑到 AudioManager
3. 调整游戏参数
4. 测试游戏功能
