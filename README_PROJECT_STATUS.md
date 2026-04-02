# 项目进度报告 - HaChiMiOhNameruDo

**最后更新**: 2026-04-02  
**Unity 版本**: 团结 1.8.5 (基于 Unity 2022.3.62t7)  
**项目类型**: 2D 休闲小游戏合集  
**当前分支**: feature/code-driven-animation（代码驱动动画）

---

## 项目概述

《哈奇米哦捏路多》（HaChiMiOhNameruDo）是一款以猫咪为主题的 2D 休闲小游戏合集，包含多个互动小游戏。

### 游戏特色
- 可爱的猫咪互动系统
- 多个小游戏模式
- 简洁的 UI 界面
- 适配移动端的操作

---

## 当前分支

### master 分支（Unity 组件驱动）
- **最新提交**: fad916b
- **动画方案**: Unity Animation Clip + Animator Controller
- **特点**: 使用 Unity 原生动画系统，美术友好

### feature/code-driven-animation 分支（代码驱动）【当前分支】
- **最新提交**: 8e1de53
- **动画方案**: 代码驱动（CatBreathController + CatBlinkController 四态状态机）
- **特点**: 包体更小（~7.5KB vs ~17KB），灵活可调

---

## 已完成功能

### 1. 核心系统

#### GameManager (单例)
- **文件**: `Assets/Scripts/Managers/GameManager.cs`
- **功能**:
  - 游戏状态管理（Idle, FurBallGame, TissueGame）
  - 小游戏时长控制（默认 30 秒）
  - 退出延迟功能（2 秒延迟返回 IDLE）
  - 状态切换时的 UI 显示/隐藏

#### UIManager
- **文件**: `Assets/Scripts/Managers/UIManager.cs`
- **功能**:
  - 主界面 UI 显示/隐藏
  - 小游戏 UI 显示/隐藏
  - 退出按钮处理

#### AudioManager
- **文件**: `Assets/Scripts/Managers/AudioManager.cs`
- **功能**: 音频管理（待完善）

#### HapticManager
- **文件**: `Assets/Scripts/Managers/HapticManager.cs`
- **功能**: 触觉反馈管理（移动端震动）

---

### 2. 小游戏

#### 小游戏 1: 毛球游戏 (FurBallGame)
- **管理器**: `FurBallGameManager.cs`
- **毛球脚本**: `FurBall.cs`
- **玩法**: 点击毛球，猫咪进行拍击互动
- **状态**: 基本功能完成

#### 小游戏 2: 纸巾筒游戏 (TissueGame)
- **管理器**: `TissueGameManager.cs`
- **纸巾筒脚本**: `TissueBox.cs`
- **纸巾脚本**: `TissuePaper.cs`
- **输入处理**: `TissueInputHandler.cs`
- **玩法**: 从纸巾筒中抽取纸巾
- **状态**: 基本功能完成

---

### 3. 猫咪系统（代码驱动版本）

#### CatController
- **文件**: `Assets/Scripts/Gameplay/CatController.cs`
- **功能**:
  - 状态管理（IDLE、准备拍击、背对玩家）
  - 朝向控制（SpriteRenderer.flipX）
  - 毛球/纸巾筒生成点配置
  - DoPaws() 方法供毛球游戏调用

#### 猫咪动画（代码驱动）

**CatBreathController.cs**:
- 使用 sin() 函数实时计算呼吸缩放
- 频率：1.8Hz（周期约 0.56 秒）
- 幅度：X 轴 0.8%, Y 轴 1.2%
- 公式：`scale = 1.0 + sin(breathTime) * amplitude`

**CatBlinkController.cs**:
- 四态状态机：Open → Closing → Closed → Opening
- 时序：60ms + 80ms + 60ms = 200ms 总眨眼时长
- 间隔：3.5~5.5 秒随机触发
- 需要分配三个精灵：Open/Half/Closed

---

### 4. UI 系统

#### IdleUI
- **文件**: `Assets/Scripts/UI/IdleUI.cs`
- **功能**: 主界面 UI 管理

#### UI 按钮配置
- 开始游戏按钮（毛球）
- 开始游戏按钮（纸巾筒）
- 退出按钮（带延迟返回 IDLE）

---

### 5. 场景配置

#### SampleScene
- **文件**: `Assets/Scenes/SampleScene.unity`
- **内容**:
  - 猫咪对象（含 CatController/CatBreathController/CatBlinkController）
  - GameManager 空对象
  - UI Canvas
  - 小游戏预制体生成点

---

### 6. 预制体

- `FurBall.prefab`: 毛球预制体
- `TissueBox.prefab`: 纸巾筒预制体
- `TissuePaper.prefab`: 纸巾预制体

---

### 7. 美术资源

#### 猫咪精灵
- `cat_base.png`: 猫咪基础图片
- `edited_cat_blink_closed_*.png`: 闭眼精灵
- `edited_cat_blink_half_*.png`: 半闭眼精灵

---

## 配置指南文档

| 文档 | 内容 |
|------|------|
| `README_SCENE_SETUP.md` | 场景配置指南 |
| `README_PREFAB_SETUP.md` | 预制体配置指南 |
| `README_UI_BUTTON_SETUP.md` | UI 按钮配置指南 |
| `README_EXIT_BUTTON_SETUP.md` | 退出按钮配置指南 |
| `README_CAT_ANIMATION_SETUP.md` | 猫咪动画配置指南 |
| `README_CAT_ANIMATOR_SETUP.md` | 代码驱动动画配置指南（本分支） |
| `README_UNITY_SETUP.md` | Unity 设置指南 |

---

## 开发规范

### 命名空间
- `HaChiMiOhNameruDo.Managers`: 管理器类
- `HaChiMiOhNameruDo.Gameplay`: 游戏玩法类
- `HaChiMiOhNameruDo.MiniGames.FurBallGame`: 毛球游戏
- `HaChiMiOhNameruDo.MiniGames.TissueGame`: 纸巾筒游戏
- `HaChiMiOhNameruDo.UI`: UI 类

### 代码风格
- 使用 `[SerializeField]` 暴露私有字段
- 使用 `[Header()]` 组织 Inspector 字段
- 使用 `[Tooltip()]` 添加字段说明
- 使用 `#region` 组织代码块

---

## 待办事项

### 高优先级
- [ ] 完善毛球游戏逻辑（生成、移动、拍击反馈）
- [ ] 完善纸巾筒游戏逻辑（抽取、物理效果）
- [ ] 添加音频资源和管理
- [ ] 完善 UI 界面（主界面、游戏界面）

### 中优先级
- [ ] 添加更多小游戏
- [ ] 添加计分系统
- [ ] 添加教程/引导
- [ ] 优化移动端适配

### 低优先级
- [ ] 添加更多猫咪动画（拍击、转身等）
- [ ] 添加猫咪表情系统
- [ ] 添加成就系统
- [ ] 添加音效和背景音乐

---

## 技术决策记录

### 2026-04-02: 动画方案选择

**背景**: 猫咪呼吸和眨眼动画的实现方式选择

**方案对比**:
| 指标 | 代码驱动 | Animation Clip |
|------|----------|----------------|
| 包体大小 | ~7.5KB | ~17KB |
| 灵活性 | 高 | 中 |
| 美术友好度 | 低 | 高 |
| CPU 开销 | 极低 | 低 |

**决定**: 
- master 分支使用 Animation Clip 方案（美术友好）
- feature/code-driven-animation 分支使用代码驱动方案（包体更小）
- 两个分支都包含 `.kilocodemodes` 配置，要求大范围改动前提供方案对比

---

## 给后续 AI 的说明

### 项目理解要点
1. **游戏结构**: 状态机驱动（Idle → 小游戏 → Idle）
2. **核心组件**: GameManager（状态管理）、UIManager（UI 管理）、CatController（猫咪控制）
3. **小游戏框架**: 每个小游戏有独立的 GameManager，由主 GameManager 统一调度
4. **动画方案**: 
   - master 分支：Animation Clip + Animator
   - feature 分支：代码驱动（CatBreathController + CatBlinkController）

### 开发建议
1. **状态切换**: 使用 `GameManager.SetGameState()` 统一处理
2. **UI 显示**: 使用 `UIManager` 的 Show/Hide 方法
3. **小游戏开发**: 遵循现有框架（GameManager + 输入处理 + 游戏对象）
4. **动画添加**: 
   - master 分支：创建 Animation Clip 并配置 Animator
   - feature 分支：创建代码控制器脚本

### 注意事项
1. **团结 1.8.5 兼容性**: 包版本需与团结 1.8.5 兼容
2. **URP 配置**: 项目使用 URP 14.x，修改渲染设置时需注意
3. **单例模式**: GameManager、UIManager 等使用单例，避免重复创建
4. **分支差异**: 注意两个分支的动画实现方式不同

---

## Git 分支说明

```bash
# 查看分支
git branch -a

# 切换到 Unity 组件驱动版本（master）
git checkout master

# 切换到代码驱动版本（当前分支）
git checkout feature/code-driven-animation

# 拉取最新代码
git pull origin master
git pull origin feature/code-driven-animation
```

---

## 项目结构

```
HaChiMiOhNameruDo/
├── Assets/
│   ├── Animations/          # 动画资源（master 分支）
│   │   └── Cat/
│   ├── Audio/               # 音频资源（待添加）
│   ├── Prefabs/             # 预制体
│   ├── Scenes/              # 场景
│   ├── Scripts/             # 脚本
│   │   ├── Gameplay/        # 游戏玩法
│   │   ├── Managers/        # 管理器
│   │   ├── MiniGames/       # 小游戏
│   │   └── UI/              # UI
│   ├── Settings/            # URP 设置
│   └── Sprites/             # 精灵图片
├── Packages/                # 包配置
├── ProjectSettings/         # 项目设置
├── README_*.md              # 配置指南文档
└── .kilocodemodes           # Code mode 自定义指令
```

---

**项目状态**: 开发中  
**下次更新**: 待完成小游戏核心逻辑后
