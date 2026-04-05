# Unity UI Canvas 渲染顺序与遮挡问题解决方案

## 问题描述

当场景中的 2D/3D 对象（如玩家角色、小猫等）被 UI Canvas 遮挡时，即使调整了 Sorting Order 也无效。

## 问题原因

**Screen Space - Overlay 模式的特性**：
- Overlay 模式的 Canvas 会**始终渲染在所有场景物体的最上层**
- 这是 Unity 的设计特性，不是 bug
- Canvas 的所有子对象（包括背景 Image）都会继承这个特性
- 即使将 Canvas 的 Sorting Order 设为负值，Overlay 模式仍然会覆盖所有场景物体

## 解决方案

### 方案：使用 Screen Space - Camera 模式

要实现"UI 覆盖全屏"同时"不遮挡场景物体"，必须使用 `Screen Space - Camera` 模式。

### 操作步骤

#### 1. 修改 Canvas 渲染模式

```
选中 Canvas → Inspector → Canvas 组件
- Render Mode: Screen Space - Camera
- Render Camera: 拖入 Main Camera
- Plane Distance: 10（或更大值，确保 UI 在场景物体后面）
```

#### 2. 设置背景 Image

```
在 Canvas 下选中背景 Image
- Rect Transform:
  - Anchor Presets: Stretch（四个角都拉伸到边缘）
  - Position: (0, 0, 0)
  - Width/Height: 根据屏幕分辨率自动填充
- Image 组件:
  - Source Image: 选择背景图
  - Order in Layer: 0（最小值，作为最底层）
```

#### 3. 设置场景物体的渲染顺序

```
选中场景物体（如 CatController）
- Sprite Renderer 组件:
  - Order in Layer: 10（或比背景 Image 大的值）
```

#### 4. 设置 UI 按钮的顺序

```
在 Canvas 下选中按钮等 UI 元素
- Image/Button 组件:
  - Order in Layer: 15（或比场景物体大的值，确保在最前面）
```

### 渲染层级示例

| 对象 | Order in Layer | 说明 |
|------|---------------|------|
| 背景 Image | 0 | 最底层 |
| 场景物体（小猫） | 10 | 中间层 |
| UI 按钮 | 15 | 最上层（可点击） |

## 三种 Canvas 渲染模式对比

| 模式 | 特点 | 适用场景 |
|------|------|----------|
| Screen Space - Overlay | 始终在最上层，不受相机影响 | 纯 UI 界面，不需要场景物体遮挡 |
| Screen Space - Camera | 作为场景中的平面，可被遮挡 | 需要 UI 与场景物体有前后关系 |
| World Space | 完全 3D 化，可放置在任意位置 | VR/AR 应用，3D 空间中的 UI |

## 常见问题

### Q1: 改为 Screen Space - Camera 后 UI 变小/变大了？
**A**: 调整 Canvas 的 `Graphic Raycaster` 或相机的 `Field of View`，或调整 Canvas 的 `Plane Distance`。

### Q2: UI 变得模糊？
**A**: 确保 Canvas 的 `Additional Settings → Pixel Perfect` 已勾选，或调整 `Reference Pixels Per Unit`。

### Q3: 相机需要特殊设置吗？
**A**: 对于 2D 项目，确保相机是 `Orthographic` 模式，并调整 `Size` 使 UI 和场景物体都在视野内。

### Q4: 小猫仍然不可见？
**A**: 检查：
- 相机的 Near Clip Plane 和 Far Clip Plane 设置
- 小猫的 Z 轴位置是否在相机可视范围内
- 小猫的 Layer 是否在相机的 Culling Mask 中

## 快速检查清单

- [ ] Canvas Render Mode 已改为 Screen Space - Camera
- [ ] Render Camera 已设置为主相机
- [ ] Plane Distance 设置合理（5-20 之间）
- [ ] 背景 Image 的 Order in Layer 最小
- [ ] 场景物体的 Order in Layer 居中
- [ ] UI 按钮的 Order in Layer 最大
- [ ] 所有 UI 元素的 Anchor 设置为 Stretch（如需全屏）
