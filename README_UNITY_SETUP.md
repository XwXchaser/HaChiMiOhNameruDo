# Unity 项目设置指南

## 解决"无法找到脚本类"问题

本项目使用了命名空间（Namespace），在 Unity 编辑器中添加组件时需要注意以下几点：

### 方法 1：直接搜索类名（推荐）

1. 在 Unity 编辑器中选中一个 GameObject
2. 在 Inspector 面板点击 "Add Component"
3. 在搜索框中输入类名，例如：
   - `GameManager`
   - `UIManager`
   - `HapticManager`
   - `AudioManager`
   - `CatController`
   - `FurBallGameManager`
   - `TissueGameManager`
4. 从搜索结果中选择对应的脚本

### 方法 2：使用完整命名空间

如果方法 1 找不到，尝试输入完整命名空间类名：
- `HaChiMiOhNameruDo.Managers.GameManager`
- `HaChiMiOhNameruDo.Managers.UIManager`
- `HaChiMiOhNameruDo.Managers.HapticManager`
- `HaChiMiOhNameruDo.Managers.AudioManager`
- `HaChiMiOhNameruDo.Gameplay.CatController`
- `HaChiMiOhNameruDo.Gameplay.InputHandler`
- `HaChiMiOhNameruDo.MiniGames.FurBallGameManager`
- `HaChiMiOhNameruDo.MiniGames.TissueGameManager`

### 方法 3：等待 Unity 重新编译

1. 打开 Unity 编辑器
2. 查看底部状态栏，等待 "Compiling Scripts..." 完成
3. 查看 `Window` → `General` → `Console` 确认没有编译错误
4. 编译完成后，重新尝试添加组件

### 方法 4：重新导入脚本

1. 在 Project 窗口选中 `Assets/Scripts` 文件夹
2. 右键 → `Reimport`
3. 等待重新编译完成

### 方法 5：重启 Unity

如果以上方法都无效：
1. 保存当前场景
2. 关闭 Unity 编辑器
3. 重新打开项目

---

## 场景设置步骤

### 1. 创建管理器对象

1. 在 Hierarchy 窗口右键 → `Create Empty`
2. 重命名为 `GameManager`
3. 添加组件：`HaChiMiOhNameruDo.Managers.GameManager`
4. 重复步骤创建其他管理器：
   - `UIManager` → 添加 `HaChiMiOhNameruDo.Managers.UIManager`
   - `HapticManager` → 添加 `HaChiMiOhNameruDo.Managers.HapticManager`
   - `AudioManager` → 添加 `HaChiMiOhNameruDo.Managers.AudioManager`

### 2. 创建猫咪对象

1. 在 Hierarchy 窗口右键 → `2D Object` → `Sprites` → `Square`（临时占位）
2. 重命名为 `Cat`
3. 添加组件：`HaChiMiOhNameruDo.Gameplay.CatController`
4. 后续替换 Sprite 为猫咪美术资源

### 3. 创建毛球小游戏对象

1. 创建空对象 `FurBallGame`
2. 添加组件：`HaChiMiOhNameruDo.MiniGames.FurBallGameManager`
3. 创建毛球子对象，添加 `FurBall` 组件

### 4. 创建纸巾筒小游戏对象

1. 创建空对象 `TissueGame`
2. 添加组件：`HaChiMiOhNameruDo.MiniGames.TissueGameManager`
3. 创建纸巾筒和厕纸对象

### 5. 创建 UI

1. 在 Hierarchy 窗口右键 → `UI` → `Canvas`
2. 在 Canvas 下创建面板和按钮

---

## 命名空间结构

```
HaChiMiOhNameruDo.Managers
├── GameManager
├── UIManager
├── HapticManager
└── AudioManager

HaChiMiOhNameruDo.Gameplay
├── CatController
└── InputHandler

HaChiMiOhNameruDo.MiniGames
├── FurBallGameManager
├── FurBall
├── TissueGameManager
├── TissueBox
├── TissuePaper
└── TissueInputHandler

HaChiMiOhNameruDo.UI
└── IdleUI
```

---

## 常见问题

### Q: 添加组件时提示"无法找到脚本类"
**A:** 确保 Unity 已完成脚本编译，查看 Console 窗口是否有错误。

### Q: 脚本修改后 Unity 没有自动更新
**A:** 在 Project 窗口右键 → `Refresh` 或重新导入脚本。

### Q: 如何查看编译状态
**A:** 查看 Unity 底部状态栏，或打开 `Window` → `General` → `Console`。
