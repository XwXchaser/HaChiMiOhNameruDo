# UI 按钮配置指南

本文档将指导您如何在 Unity 编辑器中配置 UI 按钮。

## IdleUIPanel 按钮配置

### 1. 创建毛球道具按钮

**步骤：**
1. 在层级视图中选择 `Canvas/IdleUIPanel`
2. 右键 → UI → Button - TextMeshPro（或 Button - Legacy）
3. 命名为 `FurBallButton`
4. 在 Inspector 中调整 RectTransform：
   - Pos X: -150
   - Pos Y: 0
   - Width: 200
   - Height: 200
5. 在 Button 组件中，将 `FurBallButton` 拖到 `IdleUI` 脚本的 `Fur Ball Button` 字段

### 2. 创建纸巾筒道具按钮

**步骤：**
1. 在层级视图中选择 `Canvas/IdleUIPanel`
2. 右键 → UI → Button - TextMeshPro（或 Button - Legacy）
3. 命名为 `TissueBoxButton`
4. 在 Inspector 中调整 RectTransform：
   - Pos X: 150
   - Pos Y: 0
   - Width: 200
   - Height: 200
5. 在 Button 组件中，将 `TissueBoxButton` 拖到 `IdleUI` 脚本的 `Tissue Box Button` 字段

### 3. 配置 IdleUI 脚本引用

**步骤：**
1. 在层级视图中选择 `IdleUIPanel`
2. 在 Inspector 中添加 `IdleUI` 组件（如果还没有）
3. 配置引用：
   - **Fur Ball Button**: 拖入 `FurBallButton` 对象
   - **Tissue Box Button**: 拖入 `TissueBoxButton` 对象
   - **Game Manager**: 拖入场景中的 `GameManager` 对象

## 按钮样式自定义（可选）

### 修改按钮颜色

1. 选择按钮对象
2. 在 Inspector 中找到 `Button` 组件
3. 展开 `Colors` 部分
4. 修改以下颜色：
   - **Normal**: 正常状态颜色
   - **Highlighted**: 鼠标悬停颜色
   - **Pressed**: 点击状态颜色
   - **Selected**: 选中状态颜色
   - **Disabled**: 禁用状态颜色

### 添加按钮文本

1. 选择按钮对象
2. 展开按钮的子对象 `Text`（或 `Text (Legacy)`）
3. 在 Text 组件中修改：
   - **Text**: 按钮文字（如"毛球"、"纸巾筒"）
   - **Font Size**: 字体大小
   - **Color**: 文字颜色

### 添加按钮图标（可选）

1. 选择按钮对象
2. 展开按钮的子对象 `Image`
3. 在 Image 组件中：
   - **Source Image**: 拖入精灵图片
   - **Color**: 调整颜色

## 完整层级结构

```
Canvas
└── IdleUIPanel
    ├── FurBallButton
    │   └── Text (Legacy)
    └── TissueBoxButton
        └── Text (Legacy)
```

## 验证配置

1. 确保 `IdleUIPanel` 上有 `IdleUI` 脚本组件
2. 确保所有引用字段都已正确填充
3. 确保按钮有 `Button` 组件
4. 运行场景，点击按钮，查看控制台是否输出日志

## 常见问题

### Q: 按钮点击没有反应
A: 检查以下几点：
- 按钮是否有 Button 组件
- IdleUI 脚本中的引用是否正确
- GameManager 是否存在于场景中

### Q: 按钮位置不对
A: 调整 RectTransform 的 Pos X、Pos Y、Width、Height 属性

### Q: 按钮文字不显示
A: 检查 Text 组件是否启用，文字内容是否为空
