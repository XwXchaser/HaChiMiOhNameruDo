# Unity 脚本命名空间规范

## 问题描述

在 Unity 项目中，当脚本使用命名空间（Namespace）时，如果其他脚本需要引用这些类，必须添加正确的 `using` 语句，否则会出现编译错误：

```
error CS0103: The name 'ClassName' does not exist in the current context
```

## 项目命名空间结构

本项目使用以下命名空间结构：

```
HaChiMiOhNameruDo/
├── Managers/
│   ├── GameManager.cs          → namespace HaChiMiOhNameruDo.Managers
│   ├── UIManager.cs            → namespace HaChiMiOhNameruDo.Managers
│   ├── HapticManager.cs        → namespace HaChiMiOhNameruDo.Managers
│   └── AudioManager.cs         → namespace HaChiMiOhNameruDo.Managers
├── Gameplay/
│   ├── CatController.cs        → namespace HaChiMiOhNameruDo.Gameplay
│   └── InputHandler.cs         → namespace HaChiMiOhNameruDo.Gameplay
├── MiniGames/
│   ├── FurBallGame/
│   │   ├── FurBallGameManager.cs → namespace HaChiMiOhNameruDo.MiniGames.FurBallGame
│   │   └── FurBall.cs            → namespace HaChiMiOhNameruDo.MiniGames.FurBallGame
│   └── TissueGame/
│       ├── TissueGameManager.cs  → namespace HaChiMiOhNameruDo.MiniGames.TissueGame
│       ├── TissueBox.cs          → namespace HaChiMiOhNameruDo.MiniGames.TissueGame
│       ├── TissuePaper.cs        → namespace HaChiMiOhNameruDo.MiniGames.TissueGame
│       └── TissueInputHandler.cs → namespace HaChiMiOhNameruDo.MiniGames.TissueGame
└── UI/
    └── IdleUI.cs               → namespace HaChiMiOhNameruDo.UI
```

## 规则：创建新脚本时的命名空间引用

### 1. 声明命名空间

每个新脚本必须根据其所在目录声明正确的命名空间：

```csharp
// 示例：Assets/Scripts/Managers/NewManager.cs
using UnityEngine;

namespace HaChiMiOhNameruDo.Managers  // 必须与目录结构匹配
{
    public class NewManager : MonoBehaviour
    {
        // ...
    }
}
```

### 2. 引用其他命名空间的类

当脚本需要引用其他命名空间的类时，**必须**在文件顶部添加对应的 `using` 语句：

```csharp
// 示例：GameManager.cs 需要引用 FurBallGameManager 和 TissueGameManager
using UnityEngine;
using HaChiMiOhNameruDo.MiniGames.FurBallGame;   // 引用毛球小游戏
using HaChiMiOhNameruDo.MiniGames.TissueGame;    // 引用纸巾筒小游戏

namespace HaChiMiOhNameruDo.Managers
{
    public class GameManager : MonoBehaviour
    {
        // 现在可以使用 FurBallGameManager 和 TissueGameManager
        FurBallGameManager.Instance?.StartGame();
    }
}
```

### 3. 命名空间引用速查表

| 要引用的类 | 需要添加的 using 语句 |
|-----------|----------------------|
| GameManager, UIManager, HapticManager, AudioManager | `using HaChiMiOhNameruDo.Managers;` |
| CatController, InputHandler | `using HaChiMiOhNameruDo.Gameplay;` |
| FurBallGameManager, FurBall | `using HaChiMiOhNameruDo.MiniGames.FurBallGame;` |
| TissueGameManager, TissueBox, TissuePaper, TissueInputHandler | `using HaChiMiOhNameruDo.MiniGames.TissueGame;` |
| IdleUI | `using HaChiMiOhNameruDo.UI;` |

### 4. 同一命名空间内不需要 using

如果两个类在同一个命名空间中，不需要添加 `using` 语句：

```csharp
// UIManager.cs 和 GameManager.cs 都在 HaChiMiOhNameruDo.Managers 命名空间
// 所以 UIManager.cs 可以直接使用 GameManager，无需 using

namespace HaChiMiOhNameruDo.Managers
{
    public class UIManager : MonoBehaviour
    {
        // 可以直接使用 GameManager，因为它们在同一命名空间
        GameManager.Instance?.ReturnToIdle();
    }
}
```

## 常见错误及修复

### 错误示例

```csharp
// ❌ 错误：GameManager.cs 中使用了 FurBallGameManager 但没有添加 using
using UnityEngine;

namespace HaChiMiOhNameruDo.Managers
{
    public class GameManager : MonoBehaviour
    {
        private void EnterFurBallGame()
        {
            FurBallGameManager.Instance?.StartGame();  // 错误：CS0103
        }
    }
}
```

### 修复方法

```csharp
// ✅ 正确：添加了正确的 using 语句
using UnityEngine;
using HaChiMiOhNameruDo.MiniGames.FurBallGame;

namespace HaChiMiOhNameruDo.Managers
{
    public class GameManager : MonoBehaviour
    {
        private void EnterFurBallGame()
        {
            FurBallGameManager.Instance?.StartGame();  // 正确
        }
    }
}
```

## 检查清单

创建新脚本时，请检查：

- [ ] 命名空间是否与目录结构匹配？
- [ ] 是否引用了其他命名空间的类？
- [ ] 如果引用了其他命名空间，是否添加了对应的 `using` 语句？
- [ ] 使用 `Ctrl+Shift+B` 或 Rider/Visual Studio 的编译功能检查是否有编译错误

## 工具支持

使用 JetBrains Rider 或 Visual Studio 等 IDE 时：

1. 当出现未识别的类名时，将光标放在类名上
2. 按 `Alt+Enter`（Rider）或 `Ctrl+.`（VS）
3. 选择 "Add using directive" 自动添加缺失的命名空间引用
