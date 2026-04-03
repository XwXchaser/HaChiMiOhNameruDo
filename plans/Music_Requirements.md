# HaChiMiOhNameruDo 游戏音乐需求文档

## 项目概述

**项目名称**: HaChiMiOhNameruDo（ハチミオナメルド）  
**游戏类型**: 休闲解压小游戏 + 音乐节奏元素  
**游戏主题**: 搞怪猫咪的调皮日常  

---

## 一、音乐风格定位

### 1.1 核心风格关键词

| 关键词 | 描述 |
|--------|------|
| **搞怪** | 幽默、俏皮、不按常理出牌 |
| **轻快** | 节奏明快、活力十足 |
| **卡通感** | 类似猫和老鼠动画的配乐风格 |
| **可爱但不腻** | 猫咪形象是搞怪调皮路线，非传统可爱 |

### 1.2 参考风格

- **节奏天国系列** - 搞怪、魔性的节奏游戏配乐
- **超级马里奥** - 俏皮、跳跃感的音乐
- **猫和老鼠动画** - 幽默、夸张的卡通配乐
- **日本综艺节目** - 搞怪音效和短配乐

---

## 二、技术规格要求

### 2.1 基础规格

```
┌─────────────────────────────────────────────────────────┐
│ 音乐技术规格                                            │
├─────────────────────────────────────────────────────────┤
│ BPM: 130-150 (较快速度，增加活力感)                     │
│ 拍号：4/4 拍 (强 - 弱 - 次强 - 弱)                      │
│ 时长：30 秒 (可无缝循环)                                │
│ 风格：搞怪卡通 / 俏皮电子 / 幽默管弦                   │
│ 结构：Intro(4 小节) + Loop(8-12 小节)                   │
│ 格式：WAV (无损) 或 320kbps MP3                         │
│ 节拍强调：必须有清晰的鼓点/强拍，便于游戏节奏判定      │
└─────────────────────────────────────────────────────────┘
```

### 2.2 节拍要求（重要！）

由于游戏包含节奏判定系统，音乐必须满足：

| 要求 | 说明 |
|------|------|
| **清晰的强拍** | 第 1 拍必须有明显的鼓点或低音强调 |
| **稳定的节奏** | 节拍稳定，无明显变速或自由节奏 |
| **高频节奏元素** | 建议使用踩镲/响指等高频乐器演奏 8 分音符，便于玩家跟拍 |
| **循环点清晰** | 循环点必须无缝，无明显接缝感 |

---

## 三、乐器配置建议

### 3.1 推荐乐器组合

```
【主奏乐器】
├── 木琴/马林巴 - 卡通感、跳跃感
├── 小号 - 搞怪、夸张
├── 单簧管 - 俏皮、灵活
└── 合成器 Lead - 现代感、洗脑旋律

【节奏组】
├── 鼓组 - 轻快的 Kick + Snare
├── 踩镲 - 8 分音符节奏
├── 响指/拍手 - 增加搞怪感
└── 木鱼/邦戈 - 卡通打击乐

【低音】
└── 跳跃感的贝斯线（Staccato 断奏）

【特效音】（可选）
├── 弹簧声 (Boing)
├── 滑动声 (Slide)
├── 卡通跑步声
└── 猫咪叫声采样（搞怪处理）
```

### 3.2 不推荐的元素

| 元素 | 原因 |
|------|------|
| 过于抒情的弦乐 | 与搞怪风格不符 |
| 过于沉重的金属/摇滚 | 破坏休闲解压氛围 |
| 复杂的人声合唱 | 需要纯音乐，人声会分散注意力 |
| 过于 ambient/氛围音乐 | 需要清晰的节奏点 |

---

## 四、音乐结构详细要求

### 4.1 结构分解

```
【Intro 前奏】(4 小节，约 2 秒)
├── 目的：游戏准备阶段，让玩家进入状态
├── 建议：简单的鼓点引入，逐渐加入乐器
└── 情绪：期待感、即将开始的感觉

【Main Loop 主循环】(8-12 小节，约 28 秒)
├── 目的：游戏进行时的循环音乐
├── 建议：完整的乐器编排，搞怪旋律
├── 情绪：调皮、活泼、让人想跟着点头
└── 要求：可以无缝循环播放

【Loop 循环点】
├── 位置：第 12 小节末尾 → 第 5 小节开头（或第 8 小节末尾 → 第 5 小节）
└── 要求：循环点必须平滑，无明显接缝
```

### 4.2 小节结构示例

```
4/4 拍，BPM 140

Intro (4 小节):
| 1 - 2 - 3 - 4 | 1 - 2 - 3 - 4 | 1 - 2 - 3 - 4 | 1 - 2 - 3 - 4 |
  [鼓点引入]      [加入贝斯]      [加入主奏]      [准备进入 Loop]

Main Loop (8 小节，可循环):
| 1 - 2 - 3 - 4 | 1 - 2 - 3 - 4 | 1 - 2 - 3 - 4 | 1 - 2 - 3 - 4 |
  [完整编排]      [变化]          [高潮]          [过渡]
| 1 - 2 - 3 - 4 | 1 - 2 - 3 - 4 | 1 - 2 - 3 - 4 | 1 - 2 - 3 - 4 |
  [完整编排]      [变化]          [高潮]          [循环回第 1 小节]
```

---

## 五、AI 音乐生成提示词 (Prompt)

### 5.1 英文提示词（推荐用于 AI 工具）

```
Create a quirky, playful background music for a casual mobile game about a mischievous cat.

Style: Cartoon comedy, upbeat, humorous, similar to "Rhythm Heaven" game series or "Tom and Jerry" animation soundtrack.

Technical specifications:
- Tempo: 140 BPM
- Time signature: 4/4
- Duration: 30 seconds (seamless loopable)
- Format: WAV or high-quality MP3
- Instrumental only (no vocals)

Instrumentation:
- Lead: Xylophone, marimba, trumpet, or clarinet for cartoon feel
- Rhythm: Light drums, finger snaps, wood blocks
- Bass: Bouncy, staccato bassline
- FX: Optional cartoon sound effects (spring sounds, slides)

Important:
- Must have clear, prominent drum beats on beat 1 (strong beat) for rhythm game mechanics
- Hi-hats playing eighth notes for rhythm guidance
- Catchy, memorable melody that feels mischievous and fun
- Seamless loop point

Mood keywords: Quirky, playful, mischievous, comedic, lighthearted, energetic
```

### 5.2 中文提示词（用于支持中文的 AI 工具）

```
请生成一首搞怪俏皮的游戏背景音乐，主题是一只调皮的猫咪在捣蛋。

风格参考：
- 节奏天国游戏的搞怪配乐
- 猫和老鼠动画的幽默音乐
- 日本综艺节目的搞笑音效

技术规格：
- 速度：140 BPM
- 拍号：4/4 拍
- 时长：30 秒（可无缝循环）
- 格式：WAV 或高质量 MP3
- 纯音乐（无人声）

乐器建议：
- 主奏：木琴、马林巴、小号、单簧管等卡通感乐器
- 节奏：轻快鼓点、响指、木鱼
- 低音：跳跃感的断奏贝斯
- 特效：可加入弹簧声、滑动声等卡通音效

重要要求：
- 第 1 拍必须有清晰的强拍鼓点（用于游戏节奏判定）
- 踩镲演奏 8 分音符提供节奏指引
- 旋律要洗脑、让人想跟着点头
- 循环点必须无缝

情绪关键词：搞怪、俏皮、调皮、幽默、轻松、活力
```

### 5.3 简短版提示词（用于有字数限制的工具）

```
Quirky cartoon comedy music, 140 BPM, 4/4, 30s loop, xylophone/trumpet lead, 
bouncy bass, clear drum beats, mischievous cat theme, Rhythm Heaven style, 
instrumental, seamless loop, eighth-note hi-hats for rhythm guidance
```

---

## 六、推荐 AI 音乐工具

| 工具 | 网址 | 特点 | 适用场景 |
|------|------|------|----------|
| **Suno AI** | suno.ai | AI 歌曲生成，风格多样，质量高 | 主要推荐 |
| **Udio** | udio.com | 高质量 AI 音乐，卡通风格支持好 | 主要推荐 |
| **AIVA** | aiva.ai | 可导出 MIDI，便于后期编辑 | 需要 MIDI 时 |
| **Soundraw** | soundraw.io | 可自定义 BPM、风格、时长 | 精确控制参数 |
| **Stable Audio** | stableaudio.com | Stability AI 出品，生成快速 | 快速迭代 |
| **Beatoven** | beatoven.ai | 情绪驱动，可分段导出 | 需要分段时 |

---

## 七、与 AI 工具沟通的技巧

### 7.1 迭代式生成

1. **第一次生成**：使用完整提示词生成基础版本
2. **评估结果**：检查是否符合风格、节奏、乐器要求
3. **调整提示词**：根据结果调整，例如：
   - "节奏再快一点，BPM 提高到 145"
   - "加入更多搞怪的打击乐"
   - "让旋律更简单、更洗脑"
   - "强拍鼓点再明显一些"

### 7.2 提供参考

如果 AI 工具支持参考曲目，可以上传或描述：
- "类似超级马里奥兄弟的地下关卡音乐"
- "类似节奏天国的'恋のマシンガン'风格"
- "类似猫和老鼠中 Tom 猫偷吃东西时的配乐"

### 7.3 技术检查清单

生成后请检查：
- [ ] BPM 是否在 130-150 范围内
- [ ] 是否有清晰的强拍（第 1 拍）
- [ ] 循环点是否无缝
- [ ] 时长是否约 30 秒
- [ ] 风格是否符合搞怪调皮的定位
- [ ] 是否为纯音乐（无人声）

---

## 八、音乐使用场景

### 8.1 游戏内使用

| 场景 | 音乐需求 | 当前计划 |
|------|----------|----------|
| 主界面 (Idle) | 轻松、不抢戏 | 使用本需求生成的音乐 |
| 毛球小游戏 | 活力、节奏感强 | 使用本需求生成的音乐 |
| 纸巾筒小游戏 | 搞怪、有趣 | 使用本需求生成的音乐 |
| 结算画面 | 短促、成就感 | 后续补充需求 |

### 8.2 音频集成

音乐将集成到 Unity 项目的 [`AudioManager`](../Assets/Scripts/Managers/AudioManager.cs:9) 中，通过 [`RhythmManager`](plans/Rhythm_Game_Development_Plan.md:16) 进行节奏同步。

---

## 九、后续需求

### 9.1 可能需要补充的音乐

- [ ] 结算/成功音效（短促、欢快）
- [ ] 失败/Miss 音效（搞怪但不沮丧）
- [ ] UI 点击音效（轻微、不抢戏）
- [ ] 连击/Fever 模式特殊音乐层（可选）

### 9.2 音效需求（待补充）

后续将创建单独的 `SFX_Requirements.md` 文档，详细描述游戏音效需求。

---

## 十、文档版本

| 版本 | 日期 | 更新内容 |
|------|------|----------|
| v1.0 | 2026-04-03 | 初始版本，基础音乐需求 |

---

## 附录：快速复制的 Prompt 模板

### 英文完整版
```
Create a quirky, playful background music for a casual mobile game about a mischievous cat. Style: Cartoon comedy, upbeat, humorous, similar to "Rhythm Heaven" game series. Tempo: 140 BPM, 4/4, 30 seconds seamless loop. Instruments: xylophone/marimba lead, light drums, finger snaps, bouncy bass. Must have clear drum beats on beat 1 for rhythm game. Hi-hats on eighth notes. Catchy, mischievous melody. Instrumental only.
```

### 中文完整版
```
请生成一首搞怪俏皮的游戏背景音乐，主题是一只调皮的猫咪。风格参考节奏天国游戏和猫和老鼠动画。速度 140 BPM，4/4 拍，30 秒无缝循环。乐器：木琴/马林巴主奏、轻快鼓点、响指、跳跃贝斯。第 1 拍必须有清晰强拍用于节奏游戏。踩镲演奏 8 分音符。旋律洗脑调皮。纯音乐。
```
