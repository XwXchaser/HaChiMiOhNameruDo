# 退出按钮配置指南

本文档将指导您如何在 Unity 编辑器中配置退出按钮。

## 退出按钮功能

退出按钮允许玩家在游戏过程中点击后立刻结束游戏，但会等待一定时间（默认 2 秒），让小游戏相关物体缓慢移出屏幕，然后再返回 IDLE 面板。

## 在 CommonUIPanel 中添加退出按钮

### 1. 创建退出按钮

**步骤：**
1. 在层级视图中选择 `Canvas/CommonUIPanel`
2. 右键 → UI → Button - TextMeshPro（或 Button - Legacy）
3. 命名为 `ExitButton`
4. 在 Inspector 中调整 RectTransform：
   - Pos X: 400（靠右）
   - Pos Y: 0
   - Width: 80
   - Height: 40

### 2. 配置按钮文本

**步骤：**
1. 展开 `ExitButton` 子对象
2. 选择 `Text`（或 `Text (Legacy)`）
3. 在 Text 组件中修改：
   - **Text**: "退出"
   - **Font Size**: 20
   - **Color**: 白色

### 3. 绑定按钮点击事件

**方法一：使用 Button 组件（推荐）**

1. 选择 `ExitButton`
2. 在 Inspector 中找到 `Button` 组件
3. 在 `On Click ()` 部分：
   - 点击 "+" 添加事件
   - 拖入场景中的 `UIManager` 对象
   - 选择函数：`UIManager.OnReturnButtonClicked()`

**方法二：使用 ExitButtonScript**

如果您想使用脚本方式，可以创建一个简单的脚本：

```csharp
using UnityEngine;
using UnityEngine.UI;
using HaChiMiOhNameruDo.Managers;

public class ExitButton : MonoBehaviour
{
    private void Start()
    {
        var button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OnExitClicked);
        }
    }

    private void OnExitClicked()
    {
        GameManager.Instance?.ReturnToIdle();
    }
}
```

## 调整退出延迟时间

### 在 GameManager 中设置

1. 选择场景中的 `GameManager` 对象
2. 在 Inspector 中找到 `GameManager` 组件
3. 修改 **Exit Delay** 参数（默认 2 秒）

## 完整层级结构

```
Canvas
├── IdleUIPanel
│   ├── FurBallButton
│   └── TissueBoxButton
├── FurBallGameUIPanel
├── TissueGameUIPanel
└── CommonUIPanel
    ├── TimerText
    └── ExitButton
```

## 工作流程

1. 玩家在小游戏过程中点击退出按钮
2. `GameManager.ExitMiniGame()` 被调用
3. 小游戏结束（物体开始缓慢移出）
4. 等待 `exitDelay` 秒（默认 2 秒）
5. 自动返回 IDLE 状态，显示主界面

## 验证配置

1. 确保 `CommonUIPanel` 在小游戏时显示
2. 点击退出按钮
3. 查看控制台是否输出日志：`[GameManager] 退出小游戏，2 秒后返回 IDLE`
4. 等待 2 秒后，确认返回 IDLE 状态

## 常见问题

### Q: 退出按钮没有反应
A: 检查 Button 组件的 On Click 事件是否正确绑定

### Q: 退出延迟时间不生效
A: 检查 GameManager 组件的 Exit Delay 参数是否设置

### Q: 物体没有缓慢移出
A: 确保小游戏管理器（FurBallGameManager/TissueGameManager）的 EndGame() 方法中有相应的动画逻辑
