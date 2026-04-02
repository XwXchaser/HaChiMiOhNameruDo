# 场景配置指南

本文档将指导您如何在团结 1.8.5 中重新配置场景和创建预制体。

## 目录结构

首先确保您的项目结构如下：

```
Assets/
├── Prefabs/                    # 预制体文件夹
│   ├── Managers/
│   ├── MiniGames/
│   │   ├── FurBallGame/
│   │   └── TissueGame/
│   └── UI/
├── Scenes/
│   └── SampleScene.unity       # 主场景
├── Scripts/
│   ├── Managers/
│   ├── Gameplay/
│   ├── MiniGames/
│   └── UI/
└── Settings/
```

## 第一步：创建管理器预制体

### 1.1 创建 GameManager

1. 在 Unity 中，右键点击 `Assets/Prefabs/Managers` 文件夹
2. 选择 `Create Empty` 创建空预制体，命名为 `GameManager`
3. 将 `GameManager` 脚本拖到预制体上
4. 设置参数：
   - **Game State**: Idle
   - **Mini Game Duration**: 30

### 1.2 创建 UIManager

1. 右键点击 `Assets/Prefabs/Managers` 文件夹
2. 创建空预制体，命名为 `UIManager`
3. 将 `UIManager` 脚本拖到预制体上

### 1.3 创建 AudioManager

1. 右键点击 `Assets/Prefabs/Managers` 文件夹
2. 创建空预制体，命名为 `AudioManager`
3. 将 `AudioManager` 脚本拖到预制体上
4. 设置音频剪辑引用

### 1.4 创建 HapticManager

1. 右键点击 `Assets/Prefabs/Managers` 文件夹
2. 创建空预制体，命名为 `HapticManager`
3. 将 `HapticManager` 脚本拖到预制体上

## 第二步：配置主场景 (SampleScene)

### 2.1 设置相机

1. 选择场景中的 `Main Camera`
2. 添加 `Universal Renderer 2D` 组件（如果还没有）
3. 设置相机属性：
   - **Projection**: Orthographic
   - **Size**: 5
   - **Background**: RGB(31, 77, 121) 或您喜欢的颜色

### 2.2 创建 GameManager 空对象

1. 在层级视图中右键 → `Create Empty`
2. 命名为 `GameManager`
3. 添加 `GameManager` 脚本组件
4. 设置参数：
   - **Current State**: Idle
   - **Mini Game Duration**: 30

### 2.3 创建 UI 管理器空对象

1. 在层级视图中右键 → `Create Empty`
2. 命名为 `UIManager`
3. 添加 `UIManager` 脚本组件
4. 添加 `IdleUI` 脚本组件

## 第三步：配置毛球小游戏

### 3.1 创建 FurBallGameManager 空对象

1. 在层级视图中右键 → `Create Empty`
2. 命名为 `FurBallGameManager`
3. 添加 `FurBallGameManager` 脚本组件
4. 设置参数：
   - **Game Duration**: 30
   - **Fur Ball**: (留空，稍后分配)
   - **Cat Controller**: (留空，稍后分配)

### 3.2 创建毛球 (FurBall)

1. 在层级视图中右键 → `2D Object` → `Sprites` → `Circle`
2. 命名为 `FurBall`
3. 添加 `FurBall` 脚本组件
4. 添加 `Rigidbody2D` 组件：
   - **Body Type**: Dynamic
   - **Gravity Scale**: 1
   - **Constraints**: Freeze Rotation Z
5. 添加 `Circle Collider 2D` 组件
6. 将 `FurBall` 拖到 `FurBallGameManager` 的 `Fur Ball` 字段

### 3.3 创建猫咪控制器空对象

1. 在层级视图中右键 → `Create Empty`
2. 命名为 `CatController`
3. 添加 `CatController` 脚本组件
4. 将 `CatController` 拖到 `FurBallGameManager` 的 `Cat Controller` 字段

## 第四步：配置纸巾筒小游戏

### 4.1 创建 TissueGameManager 空对象

1. 在层级视图中右键 → `Create Empty`
2. 命名为 `TissueGameManager`
3. 添加 `TissueGameManager` 脚本组件
4. 设置参数：
   - **Max Tissue Length**: 100
   - **Game Duration**: 30
   - **Tissue Box**: (留空)
   - **Tissue Paper**: (留空)
   - **Cat Controller**: (留空)

### 4.2 创建纸巾盒 (TissueBox)

1. 在层级视图中右键 → `2D Object` → `Sprites` → `Square`
2. 命名为 `TissueBox`
3. 添加 `TissueBox` 脚本组件
4. 调整 Transform：
   - **Position**: (0, 3, 0)
   - **Scale**: (2, 1, 1)
5. 添加 `Box Collider 2D` 组件
6. 将 `TissueBox` 拖到 `TissueGameManager` 的 `Tissue Box` 字段

### 4.3 创建纸巾 (TissuePaper)

1. 在层级视图中右键 → `2D Object` → `Sprites` → `Square`
2. 命名为 `TissuePaper`
3. 添加 `TissuePaper` 脚本组件
4. 添加 `TissueInputHandler` 脚本组件
5. 调整 Transform：
   - **Position**: (0, 2, 0)
   - **Scale**: (1.8, 0.5, 1)
6. 添加 `Box Collider 2D` 组件
7. 将 `TissuePaper` 拖到 `TissueGameManager` 的 `Tissue Paper` 字段

## 第五步：创建预制体

### 5.1 保存预制体

1. 在 `Assets/Prefabs` 下创建相应子文件夹
2. 将配置好的对象拖到对应的文件夹中：
   - `Assets/Prefabs/Managers/GameManager.prefab`
   - `Assets/Prefabs/Managers/UIManager.prefab`
   - `Assets/Prefabs/Managers/AudioManager.prefab`
   - `Assets/Prefabs/Managers/HapticManager.prefab`
   - `Assets/Prefabs/MiniGames/FurBallGame/FurBall.prefab`
   - `Assets/Prefabs/MiniGames/TissueGame/TissueBox.prefab`
   - `Assets/Prefabs/MiniGames/TissueGame/TissuePaper.prefab`

## 第六步：验证配置

### 6.1 检查引用

确保所有脚本的引用都已正确设置：

1. **GameManager**: 不需要额外引用
2. **FurBallGameManager**: 
   - Fur Ball → FurBall 对象
   - Cat Controller → CatController 对象
3. **TissueGameManager**:
   - Tissue Box → TissueBox 对象
   - Tissue Paper → TissuePaper 对象
   - Cat Controller → CatController 对象

### 6.2 测试运行

1. 点击 Unity 的播放按钮
2. 检查控制台是否有错误
3. 测试状态切换是否正常

## 常见问题

### Q: 脚本引用丢失
A: 重新将脚本从 `Assets/Scripts` 拖到组件上

### Q: 预制体无法保存
A: 确保 `Assets/Prefabs` 文件夹存在

### Q: 相机不显示 2D 视图
A: 将相机的 Projection 设置为 Orthographic

## 下一步

完成基础配置后，您可以：
1. 添加精灵图片替换默认形状
2. 添加音频剪辑
3. 调整游戏参数
4. 添加更多小游戏
