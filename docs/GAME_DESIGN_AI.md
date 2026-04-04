# HaChiMiOhNameruDo AI 开发指南 - 完整版

> **文档版本**: v2.0  
> **最后更新**: 2026-04-04  
> **目标读者**: AI 开发者  
> **任务范围**: 谱面生成 + 小鱼干奖励 + 触摸互动  

---

## 目录

1. [项目概述](#1-项目概述)
2. [游戏系统总览](#2-游戏系统总览)
3. [谱面系统](#3-谱面系统)
4. [小鱼干奖励系统](#4-小鱼干奖励系统)
5. [触摸互动系统](#5-触摸互动系统)
6. [节奏系统](#6-节奏系统)
7. [数据结构总览](#7-数据结构总览)
8. [AI 任务清单](#8-ai-任务清单)

---

## 1. 项目概述

### 1.1 游戏信息

| 属性 | 值 |
|------|-----|
| **项目名称** | HaChiMiOhNameruDo（ハチミオナメルド） |
| **游戏类型** | 休闲解压小游戏 + 音乐节奏 + 桌面宠物互动 |
| **目标平台** | Unity 2D (移动端优先) |
| **开发引擎** | Unity 2022+ |
| **编程语言** | C# |
| **命名空间** | `HaChiMiOhNameruDo` |

### 1.2 核心玩法

```
┌─────────────────────────────────────────────────────────┐
│ 游戏循环                                                │
├─────────────────────────────────────────────────────────┤
│                                                         │
│  ┌─────────┐     选择      ┌─────────┐                 │
│  │  IDLE   │ ────────────> │ 小游戏  │                 │
│  │  状态   │  小游戏按钮   │  进行   │                 │
│  └────┬────┘              └────┬────┘                 │
│       │                        │                       │
│       │ 返回/结束              │ 计时器归零            │
│       │ <───────────────────── │                       │
│       │                        │                       │
│       │    ┌──────────────┐    │                       │
│       └────│  触摸互动    │<───┘                       │
│            │ (双指抓取)   │                            │
│            └──────────────┘                            │
│                                                         │
└─────────────────────────────────────────────────────────┘
```

### 1.3 你的任务（AI 开发者）

作为 AI 开发者，你需要实现以下系统：

| 系统 | 任务 | 优先级 |
|------|------|--------|
| **谱面系统** | 分析音乐、检测节拍、生成谱面配置 | 高 |
| **小鱼干奖励** | 实现奖励获取、消费、解锁逻辑 | 高 |
| **触摸互动** | 实现双指抓取、陀螺仪摇晃、奖励检测 | 中 |
| **节奏系统** | 实现节拍追踪、判定、连击系统 | 高 |

---

## 2. 游戏系统总览

### 2.1 系统架构图

```
┌─────────────────────────────────────────────────────────────────┐
│                        游戏系统架构                             │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │                    核心管理层                            │   │
│  │  ┌──────────┐ ┌──────────┐ ┌──────────┐ ┌──────────┐  │   │
│  │  │ GameManager │ │ UIManager │ │ AudioManager │ │ HapticManager │ │
│  │  └──────────┘ └──────────┘ └──────────┘ └──────────┘  │   │
│  └─────────────────────────────────────────────────────────┘   │
│                              │                                  │
│                              v                                  │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │                    游戏逻辑层                            │   │
│  │  ┌──────────────────┐      ┌──────────────────┐        │   │
│  │  │  FurBallGame     │      │  TissueGame      │        │   │
│  │  │  (谱面 + 奖励)    │      │  (谱面 + 奖励)    │        │   │
│  │  └──────────────────┘      └──────────────────┘        │   │
│  └─────────────────────────────────────────────────────────┘   │
│                              │                                  │
│                              v                                  │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │                    互动与奖励层                          │   │
│  │  ┌──────────────────┐      ┌──────────────────┐        │   │
│  │  │ TouchInteraction │      │ TreatReward      │        │   │
│  │  │ (双指 + 陀螺仪)    │      │ (小鱼干系统)      │        │   │
│  │  └──────────────────┘      └──────────────────┘        │   │
│  └─────────────────────────────────────────────────────────┘   │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

### 2.2 命名空间结构

```
HaChiMiOhNameruDo
├── Managers
│   ├── GameManager          # 游戏状态管理
│   ├── UIManager            # UI 界面管理
│   ├── AudioManager         # 音频管理
│   └── HapticManager        # 触觉反馈管理
│
├── Gameplay
│   ├── CatController        # 猫咪控制
│   ├── InputHandler         # 基础输入处理
│   └── TouchInteractionHandler  # 触摸互动处理
│
├── MiniGames
│   ├── FurBallGame
│   │   ├── FurBall          # 毛球组件
│   │   └── FurBallGameManager  # 毛球游戏管理
│   └── TissueGame
│       ├── TissueBox        # 纸巾筒组件
│       ├── TissuePaper      # 厕纸组件
│       ├── TissueInputHandler  # 纸巾游戏输入
│       └── TissueGameManager   # 纸巾游戏管理
│
├── Rhythm
│   ├── RhythmManager        # 节奏核心
│   ├── ChartLoader          # 谱面加载器
│   └── ChartData            # 谱面数据结构
│
├── Rewards
│   ├── TreatCounter         # 小鱼干计数器
│   ├── TreatRewardSystem    # 奖励发放系统
│   └── UnlockSystem         # 解锁系统
│
└── UI
    ├── IdleUI               # IDLE 状态 UI
    └── TreatShopUI          # 小鱼干商店 UI
```

---

## 3. 谱面系统

### 3.1 任务说明

你需要实现一个**谱面生成工具**，能够：
1. 读取音乐文件（.wav, .mp3, .ogg）
2. 自动检测音乐的节拍点和 BPM
3. 生成游戏可用的谱面配置文件
4. 为不同小游戏配置相应的谱面事件

### 3.2 音乐技术规格

```
┌─────────────────────────────────────────────────────────┐
│ 音乐技术规格                                            │
├─────────────────────────────────────────────────────────┤
│ BPM 范围：80-150（休闲向）                              │
│ 拍号：4/4 拍（强 - 弱 - 次强 - 弱）                     │
│ 时长：30-60 秒（每个小游戏）                            │
│ 格式：.wav, .mp3, .ogg                                  │
│ 采样率：44.1kHz 或 48kHz                                │
└─────────────────────────────────────────────────────────┘
```

### 3.3 谱面数据结构

```csharp
namespace HaChiMiOhNameruDo.Rhythm
{
    /// <summary>
    /// 谱面元数据
    /// </summary>
    [System.Serializable]
    public class ChartMetadata
    {
        public string songName;           // 歌曲名
        public string artist;             // 艺术家
        public float bpm;                 // 节拍速度
        public int timeSignature;         // 拍号（如 4 表示 4/4 拍）
        public float audioOffset;         // 音频延迟补偿（秒）
        public float duration;            // 音乐时长（秒）
    }

    /// <summary>
    /// 谱面事件类型
    /// </summary>
    public enum ChartEventType
    {
        Beat,           // 普通节拍
        StrongBeat,     // 强拍（第 1 拍）
        Paw,            // 拍击（毛球游戏）
        Pull,           // 抽纸（纸巾筒游戏）
        Cut             // 切碎（纸巾筒游戏）
    }

    /// <summary>
    /// 谱面事件
    /// </summary>
    [System.Serializable]
    public class ChartEvent
    {
        public float time;              // 事件时间（秒，相对于音乐开始）
        public ChartEventType type;     // 事件类型
        public int beatIndex;           // 节拍编号（从 0 开始）
        public int measureIndex;        // 小节编号（从 0 开始）
        public int beatInMeasure;       // 在小节中的位置（0-3 对于 4/4 拍）

        public ChartEvent(float time, ChartEventType type, int beatIndex, int measureIndex, int beatInMeasure)
        {
            this.time = time;
            this.type = type;
            this.beatIndex = beatIndex;
            this.measureIndex = measureIndex;
            this.beatInMeasure = beatInMeasure;
        }
    }

    /// <summary>
    /// 谱面数据
    /// </summary>
    [System.Serializable]
    public class ChartData
    {
        public ChartMetadata metadata;  // 元数据
        public List<ChartEvent> events; // 事件列表

        public ChartData()
        {
            events = new List<ChartEvent>();
        }
    }
}
```

### 3.4 JSON 谱面配置格式

```json
{
  "metadata": {
    "songName": "Quirky Cat Theme",
    "artist": "AI Generated",
    "bpm": 120.0,
    "timeSignature": 4,
    "audioOffset": 0.0,
    "duration": 30.0
  },
  "events": [
    { "time": 0.0, "type": "StrongBeat", "beatIndex": 0, "measureIndex": 0, "beatInMeasure": 0 },
    { "time": 0.5, "type": "Beat", "beatIndex": 1, "measureIndex": 0, "beatInMeasure": 1 },
    { "time": 1.0, "type": "Beat", "beatIndex": 2, "measureIndex": 0, "beatInMeasure": 2 },
    { "time": 1.5, "type": "Beat", "beatIndex": 3, "measureIndex": 0, "beatInMeasure": 3 },
    { "time": 2.0, "type": "StrongBeat", "beatIndex": 4, "measureIndex": 1, "beatInMeasure": 0 }
  ]
}
```

### 3.5 节拍检测算法

#### 方法 1：基于能量检测

```csharp
public class BeatDetector
{
    /// <summary>
    /// 检测音频中的瞬态（节拍点）
    /// </summary>
    public static List<int> DetectTransients(float[] samples, int sampleRate, float threshold = 0.5f)
    {
        // 1. 计算包络
        int windowSize = sampleRate / 100;  // 10ms 窗口
        float[] envelope = new float[samples.Length / windowSize];

        for (int i = 0; i < envelope.Length; i++)
        {
            float max = 0;
            for (int j = 0; j < windowSize; j++)
            {
                float abs = Mathf.Abs(samples[i * windowSize + j]);
                if (abs > max) max = abs;
            }
            envelope[i] = max;
        }

        // 2. 检测峰值
        List<int> transients = new List<int>();
        float avg = envelope.Average();
        float adaptiveThreshold = avg * threshold;

        for (int i = 1; i < envelope.Length - 1; i++)
        {
            if (envelope[i] > envelope[i - 1] &&
                envelope[i] > envelope[i + 1] &&
                envelope[i] > adaptiveThreshold)
            {
                transients.Add(i);
            }
        }

        return transients;
    }

    /// <summary>
    /// 估算 BPM
    /// </summary>
    public static float EstimateBPM(List<int> transients, int sampleRate, float minBPM = 80f, float maxBPM = 150f)
    {
        // 计算间隔直方图
        Dictionary<int, int> histogram = new Dictionary<int, int>();
        for (int i = 1; i < transients.Count; i++)
        {
            int interval = transients[i] - transients[i - 1];
            if (interval > 0 && interval < sampleRate * 2)
            {
                if (!histogram.ContainsKey(interval))
                    histogram[interval] = 0;
                histogram[interval]++;
            }
        }

        // 找到最常见的间隔
        int maxCount = 0;
        int mostCommonInterval = 0;
        foreach (var kvp in histogram)
        {
            if (kvp.Value > maxCount)
            {
                maxCount = kvp.Value;
                mostCommonInterval = kvp.Key;
            }
        }

        // 转换为 BPM
        if (mostCommonInterval > 0)
        {
            float bpm = 60.0f * sampleRate / mostCommonInterval;

            // 调整到合理范围
            while (bpm < minBPM) bpm *= 2;
            while (bpm > maxBPM) bpm /= 2;

            return bpm;
        }

        return 120f;  // 默认
    }

    /// <summary>
    /// 对齐节拍到网格
    /// </summary>
    public static List<float> AlignBeats(List<int> transients, float bpm, int sampleRate, float duration)
    {
        List<float> beatTimes = new List<float>();
        float beatInterval = 60.0f / bpm;
        float startTime = transients.Count > 0 ? transients[0] / (float)sampleRate : 0;

        // 量化到最近的节拍网格
        float offset = Mathf.Round(startTime / beatInterval) * beatInterval;

        while (offset < duration)
        {
            beatTimes.Add(offset);
            offset += beatInterval;
        }

        return beatTimes;
    }

    /// <summary>
    /// 检测强拍（每 4 拍的第 1 拍）
    /// </summary>
    public static List<int> DetectStrongBeats(int beatCount, int beatsPerMeasure = 4)
    {
        List<int> strongBeats = new List<int>();
        for (int i = 0; i < beatCount; i += beatsPerMeasure)
        {
            strongBeats.Add(i);
        }
        return strongBeats;
    }
}
```

### 3.6 谱面生成器（完整实现）

```csharp
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace HaChiMiOhNameruDo.Tools
{
    /// <summary>
    /// AI 谱面生成器
    /// 分析音频文件并自动生成节奏游戏谱面
    /// </summary>
    public class AIChartGenerator : MonoBehaviour
    {
        [Header("输入设置")]
        [SerializeField] private string inputAudioPath;
        [SerializeField] private string outputJsonPath;

        [Header("分析参数")]
        [SerializeField] private float detectionThreshold = 0.5f;
        [SerializeField] private float minBPM = 80f;
        [SerializeField] private float maxBPM = 150f;

        /// <summary>
        /// 生成谱面（主入口）
        /// </summary>
        public void GenerateChart()
        {
            // 1. 加载音频
            AudioClip clip = LoadAudio(inputAudioPath);
            if (clip == null)
            {
                Debug.LogError("Failed to load audio!");
                return;
            }

            // 2. 分析音频
            ChartAnalysisResult analysis = AnalyzeAudio(clip);

            // 3. 生成谱面数据
            Rhythm.ChartData chart = CreateChartData(clip, analysis);

            // 4. 保存谱面
            SaveChart(chart, outputJsonPath);

            Debug.Log($"Chart generated: {chart.events.Count} events, BPM: {chart.metadata.bpm}");
        }

        private AudioClip LoadAudio(string path)
        {
            // 使用 Unity 的 AudioImporter 加载
            // 或者使用 WWW/UnityWebRequest 加载
            return null;  // 需要根据实际情况实现
        }

        private ChartAnalysisResult AnalyzeAudio(AudioClip clip)
        {
            ChartAnalysisResult result = new ChartAnalysisResult();

            // 获取音频数据
            float[] samples = GetAudioSamples(clip);

            // 检测瞬态
            result.transients = BeatDetector.DetectTransients(samples, clip.frequency, detectionThreshold);

            // 估算 BPM
            result.bpm = BeatDetector.EstimateBPM(result.transients, clip.frequency, minBPM, maxBPM);

            // 对齐节拍
            result.beatTimes = BeatDetector.AlignBeats(result.transients, result.bpm, clip.frequency, clip.length);

            // 检测强拍
            result.strongBeatIndices = BeatDetector.DetectStrongBeats(result.beatTimes.Count);

            return result;
        }

        private float[] GetAudioSamples(AudioClip clip)
        {
            float[] samples = new float[clip.samples * clip.channels];
            clip.GetData(samples, 0);

            // 转换为单声道
            if (clip.channels > 1)
            {
                return DownmixToMono(samples, clip.channels);
            }
            return samples;
        }

        private float[] DownmixToMono(float[] samples, int channels)
        {
            float[] mono = new float[samples.Length / channels];
            for (int i = 0; i < mono.Length; i++)
            {
                float sum = 0;
                for (int c = 0; c < channels; c++)
                {
                    sum += samples[i * channels + c];
                }
                mono[i] = sum / channels;
            }
            return mono;
        }

        private Rhythm.ChartData CreateChartData(AudioClip clip, ChartAnalysisResult analysis)
        {
            Rhythm.ChartData chart = new Rhythm.ChartData
            {
                metadata = new Rhythm.ChartMetadata
                {
                    songName = clip.name,
                    artist = "AI Generated",
                    bpm = analysis.bpm,
                    timeSignature = 4,
                    audioOffset = 0,
                    duration = clip.length
                },
                events = new List<Rhythm.ChartEvent>()
            };

            // 添加节拍事件
            for (int i = 0; i < analysis.beatTimes.Count; i++)
            {
                bool isStrongBeat = analysis.strongBeatIndices.Contains(i);
                Rhythm.ChartEventType type = isStrongBeat ?
                    Rhythm.ChartEventType.StrongBeat : Rhythm.ChartEventType.Beat;

                int measureIndex = i / 4;
                int beatInMeasure = i % 4;

                chart.events.Add(new Rhythm.ChartEvent(
                    analysis.beatTimes[i],
                    type,
                    i,
                    measureIndex,
                    beatInMeasure
                ));
            }

            return chart;
        }

        private void SaveChart(Rhythm.ChartData chart, string path)
        {
            string json = JsonConvert.SerializeObject(chart, Formatting.Indented);
            
            // 确保目录存在
            string directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            
            File.WriteAllText(path, json);
            Debug.Log($"Chart saved to: {path}");
        }
    }

    /// <summary>
    /// 音频分析结果
    /// </summary>
    public class ChartAnalysisResult
    {
        public List<int> transients;
        public float bpm;
        public List<float> beatTimes;
        public List<int> strongBeatIndices;
    }
}
```

### 3.7 谱面加载器

```csharp
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace HaChiMiOhNameruDo.Rhythm
{
    /// <summary>
    /// 谱面加载器
    /// 从 JSON 文件加载谱面数据
    /// </summary>
    public class ChartLoader : MonoBehaviour
    {
        public static ChartLoader Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// 从 JSON 文件加载谱面
        /// </summary>
        public ChartData LoadChart(string jsonPath)
        {
            if (!File.Exists(jsonPath))
            {
                Debug.LogError($"Chart file not found: {jsonPath}");
                return null;
            }

            string json = File.ReadAllText(jsonPath);
            ChartData chart = JsonConvert.DeserializeObject<ChartData>(json);

            Debug.Log($"Loaded chart: {chart.metadata.songName}, BPM: {chart.metadata.bpm}, Events: {chart.events.Count}");

            return chart;
        }
    }
}
```

---

## 4. 小鱼干奖励系统

### 4.1 系统概述

**名称**: 小鱼干奖励系统（Treat Reward System）

**核心概念**: 玩家通过游戏行为获得小鱼干奖励，收集的小鱼干可用于解锁新的小游戏玩法。

### 4.2 奖励获取途径

| 来源 | 条件 | 奖励数量 | 说明 |
|------|------|----------|------|
| **小游戏 - 节奏玩法** | Perfect 判定 | 2 分 | 踩准节拍 |
| **小游戏 - 节奏玩法** | Normal 判定 | 1 分 | 判定宽松，减轻压力 |
| **触摸互动 - 甩猫咪** | 甩两下猫咪 | 5 分 | 有现实时间冷却 |

**判定说明**:
- Normal 判定窗口较为宽松，减轻玩家压力
- 无连击奖励，保持休闲体验

### 4.3 小鱼干用途

| 用途 | 消耗 | 说明 |
|------|------|------|
| **解锁新小游戏** | 50 个 | 每个新小游戏需要 50 小鱼干 |

### 4.4 冷却机制

| 来源 | 冷却类型 | 冷却时长 |
|------|----------|----------|
| **甩猫咪奖励** | 现实世界时间 | 可配置（默认 60 秒） |

### 4.5 核心数据结构

```csharp
namespace HaChiMiOhNameruDo.Rewards
{
    /// <summary>
    /// 小鱼干计数器 - 单例
    /// 管理玩家的小鱼干数量
    /// </summary>
    public class TreatCounter : MonoBehaviour
    {
        public static TreatCounter Instance { get; private set; }

        [Header("初始设置")]
        [SerializeField] private int initialTreats = 0;

        private int currentTreats;      // 当前小鱼干数量
        private int totalEarned;        // 累计获得数量
        private int totalSpent;         // 累计消耗数量

        // 事件
        public event Action<int> OnTreatsChanged;    // 数量变化事件
        public event Action<int> OnTreatsEarned;     // 获得事件
        public event Action<int> OnTreatsSpent;      // 消耗事件

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            currentTreats = initialTreats;
        }

        /// <summary>
        /// 获取当前小鱼干数量
        /// </summary>
        public int GetCurrentTreats()
        {
            return currentTreats;
        }

        /// <summary>
        /// 增加小鱼干
        /// </summary>
        public void AddTreats(int amount)
        {
            if (amount <= 0) return;

            currentTreats += amount;
            totalEarned += amount;

            OnTreatsChanged?.Invoke(currentTreats);
            OnTreatsEarned?.Invoke(amount);

            Debug.Log($"[TreatCounter] 获得 {amount} 小鱼干，当前：{currentTreats}");
        }

        /// <summary>
        /// 消费小鱼干
        /// </summary>
        public bool SpendTreats(int amount)
        {
            if (amount <= 0) return false;
            if (currentTreats < amount) return false;

            currentTreats -= amount;
            totalSpent += amount;

            OnTreatsChanged?.Invoke(currentTreats);
            OnTreatsSpent?.Invoke(amount);

            Debug.Log($"[TreatCounter] 消费 {amount} 小鱼干，当前：{currentTreats}");
            return true;
        }

        /// <summary>
        /// 检查是否买得起
        /// </summary>
        public bool CanAfford(int cost)
        {
            return currentTreats >= cost;
        }
    }

    /// <summary>
    /// 奖励发放系统
    /// </summary>
    public class TreatRewardSystem : MonoBehaviour
    {
        public static TreatRewardSystem Instance { get; private set; }

        [Header("奖励配置")]
        [SerializeField] private int perfectReward = 2;      // Perfect 奖励
        [SerializeField] private int normalReward = 1;       // Normal 奖励
        [SerializeField] private int swingReward = 5;        // 甩猫咪奖励
        [SerializeField] private float swingCooldown = 60f;  // 甩猫咪冷却（秒）

        private float lastSwingRewardTime = -999f;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Perfect 判定奖励
        /// </summary>
        public void AwardForPerfect()
        {
            TreatCounter.Instance?.AddTreats(perfectReward);
        }

        /// <summary>
        /// Normal 判定奖励
        /// </summary>
        public void AwardForNormal()
        {
            TreatCounter.Instance?.AddTreats(normalReward);
        }

        /// <summary>
        /// 甩猫咪奖励（带冷却检查）
        /// </summary>
        public bool AwardForSwing()
        {
            float currentTime = Time.time;
            if (currentTime - lastSwingRewardTime >= swingCooldown)
            {
                TreatCounter.Instance?.AddTreats(swingReward);
                lastSwingRewardTime = currentTime;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获取甩猫咪冷却剩余时间
        /// </summary>
        public float GetSwingCooldownRemaining()
        {
            float elapsed = Time.time - lastSwingRewardTime;
            return Mathf.Max(0, swingCooldown - elapsed);
        }
    }

    /// <summary>
    /// 解锁系统
    /// </summary>
    public class UnlockSystem : MonoBehaviour
    {
        public static UnlockSystem Instance { get; private set; }

        /// <summary>
        /// 可解锁项目类型
        /// </summary>
        public enum UnlockableType
        {
            MiniGame_FurBall,      // 毛球游戏（默认解锁）
            MiniGame_Tissue,       // 纸巾游戏（默认解锁）
            MiniGame_New1,         // 新小游戏 1（待解锁）
            MiniGame_New2,         // 新小游戏 2（待解锁）
        }

        /// <summary>
        /// 解锁项目配置
        /// </summary>
        [System.Serializable]
        public class UnlockableItem
        {
            public UnlockableType type;
            public string name;
            public int treatCost;          // 需要的小鱼干数量
            public bool isUnlocked;        // 是否已解锁
        }

        [Header("解锁配置")]
        [SerializeField] private List<UnlockableItem> unlockables = new List<UnlockableItem>();
        [SerializeField] private int defaultMiniGameCost = 50;  // 默认小游戏解锁价格

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            // 初始化默认解锁项目
            InitializeDefaultUnlockables();
        }

        private void InitializeDefaultUnlockables()
        {
            // 默认解锁毛球和纸巾游戏
            unlockables.Add(new UnlockableItem
            {
                type = UnlockableType.MiniGame_FurBall,
                name = "毛球游戏",
                treatCost = 0,
                isUnlocked = true
            });

            unlockables.Add(new UnlockableItem
            {
                type = UnlockableType.MiniGame_Tissue,
                name = "纸巾筒游戏",
                treatCost = 0,
                isUnlocked = true
            });
        }

        /// <summary>
        /// 解锁小游戏
        /// </summary>
        public bool UnlockMiniGame(UnlockableType type)
        {
            var item = GetUnlockableItem(type);
            if (item == null || item.isUnlocked) return false;

            if (TreatCounter.Instance?.SpendTreats(item.treatCost) == true)
            {
                item.isUnlocked = true;
                Debug.Log($"[UnlockSystem] 已解锁：{item.name}");
                return true;
            }

            Debug.Log($"[UnlockSystem] 小鱼干不足，无法解锁：{item.name}");
            return false;
        }

        /// <summary>
        /// 检查是否已解锁
        /// </summary>
        public bool IsUnlocked(UnlockableType type)
        {
            var item = GetUnlockableItem(type);
            return item != null && item.isUnlocked;
        }

        /// <summary>
        /// 获取解锁价格
        /// </summary>
        public int GetUnlockCost(UnlockableType type)
        {
            var item = GetUnlockableItem(type);
            return item != null ? item.treatCost : defaultMiniGameCost;
        }

        private UnlockableItem GetUnlockableItem(UnlockableType type)
        {
            return unlockables.Find(x => x.type == type);
        }
    }
}
```

### 4.6 UI 显示

```csharp
using UnityEngine;
using UnityEngine.UI;

namespace HaChiMiOhNameruDo.UI
{
    /// <summary>
    /// 小鱼干计数器 UI
    /// </summary>
    public class TreatCounterUI : MonoBehaviour
    {
        [Header("UI 引用")]
        [SerializeField] private Text treatCountText;
        [SerializeField] private GameObject treatIcon;

        private void OnEnable()
        {
            Rewards.TreatCounter.Instance?.OnTreatsChanged?.Invoke(UpdateDisplay);
            UpdateDisplay(Rewards.TreatCounter.Instance?.GetCurrentTreats() ?? 0);
        }

        private void UpdateDisplay(int count)
        {
            if (treatCountText != null)
            {
                treatCountText.text = $"x {count}";
            }
        }
    }

    /// <summary>
    /// 小鱼干商店 UI
    /// </summary>
    public class TreatShopUI : MonoBehaviour
    {
        [Header("UI 引用")]
        [SerializeField] private Text currentTreatsText;
        [SerializeField] private Transform unlockablesContainer;
        [SerializeField] private GameObject unlockableItemPrefab;

        private void OnEnable()
        {
            Rewards.TreatCounter.Instance?.OnTreatsChanged?.Invoke(UpdateCurrentTreats);
            UpdateCurrentTreats(Rewards.TreatCounter.Instance?.GetCurrentTreats() ?? 0);
            PopulateUnlockables();
        }

        private void UpdateCurrentTreats(int count)
        {
            if (currentTreatsText != null)
            {
                currentTreatsText.text = $"🐟 {count}";
            }
        }

        private void PopulateUnlockables()
        {
            // 清理现有项目
            foreach (Transform child in unlockablesContainer)
            {
                Destroy(child.gameObject);
            }

            // 添加解锁项目
            var unlockSystem = Rewards.UnlockSystem.Instance;
            if (unlockSystem == null) return;

            // 这里需要根据实际的解锁项目来生成 UI
            // 示例代码略
        }

        public void OnUnlockButtonClicked(Rewards.UnlockSystem.UnlockableType type)
        {
            if (unlockSystem.UnlockMiniGame(type))
            {
                // 解锁成功，刷新 UI
                PopulateUnlockables();
            }
        }
    }
}
```

---

## 5. 触摸互动系统

### 5.1 系统概述

**名称**: 举起猫咪互动（双指抓取 + 陀螺仪摇晃）

**核心概念**: 玩家在 IDLE 状态下双指按住猫咪双肩，持续 0.5 秒后举起猫咪，通过陀螺仪控制猫咪下半身摆动。

### 5.2 游戏流程

```
┌─────────────────────────────────────────────────────────┐
│ 举起猫咪互动流程                                        │
├─────────────────────────────────────────────────────────┤
│                                                         │
│  1. IDLE 状态 - 猫咪坐姿面朝玩家                        │
│     │                                                   │
│     v                                                   │
│  2. 玩家双指按住"双肩"判定点（位置可配置）              │
│     │                                                   │
│     v                                                   │
│  3. 持续按住 0.5 秒                                      │
│     │                                                   │
│     v                                                   │
│  4. 触发"举起"判定                                      │
│     │                                                   │
│     v                                                   │
│  5. 启用手机陀螺仪                                      │
│     │                                                   │
│     v                                                   │
│  6. 猫咪下半身随陀螺仪摆动：                           │
│     - 摆动速度越大 → 摆动角度越大                       │
│     - 速度归零 → 摆动角度慢慢归零                       │
│     │                                                   │
│     v                                                   │
│  7. 检测到"甩两下" → 奖励 5 小鱼干（有冷却）             │
│     │                                                   │
│     v                                                   │
│  8. 松开手指 → 猫咪落下/回到原位                        │
│                                                         │
└─────────────────────────────────────────────────────────┘
```

### 5.3 技术规格

| 要素 | 描述 |
|------|------|
| **触发条件** | 双指同时按住猫咪双肩判定点（位置可配置） |
| **判定时间** | 持续按住约 0.5 秒 |
| **交互方式** | 双指抓取 + 陀螺仪倾斜 |
| **摆动逻辑** | 陀螺仪摆动速度 → 摆动角度，随后慢慢归零 |
| **结束条件** | 松开手指 |
| **奖励** | 甩两下奖励 5 小鱼干（冷却 60 秒） |

### 5.4 核心组件

```csharp
using UnityEngine;
using System;

namespace HaChiMiOhNameruDo.Gameplay
{
    /// <summary>
    /// 触摸互动处理器
    /// 处理双指抓取和陀螺仪摇晃逻辑
    /// </summary>
    public class TouchInteractionHandler : MonoBehaviour
    {
        [Header("判定点设置")]
        [SerializeField] private Vector2 leftShoulderPos;    // 左肩判定位置（可配置）
        [SerializeField] private Vector2 rightShoulderPos;   // 右肩判定位置（可配置）
        [SerializeField] private float grabRadius = 0.5f;    // 判定半径

        [Header("判定时间")]
        [SerializeField] private float grabHoldTime = 0.5f;  // 需要持续按住的时间

        [Header("陀螺仪设置")]
        [SerializeField] private float maxSwingAngle = 30f;  // 最大摆动角度
        [SerializeField] private float dampingFactor = 0.95f; // 阻尼系数（归零速度）
        [SerializeField] private float sensitivity = 2f;     // 陀螺仪灵敏度

        [Header("组件引用")]
        [SerializeField] private CatController catController;

        // 状态
        private bool isGrabbing;                     // 是否正在抓取
        private bool isGrabbed;                      // 是否已抓起
        private float grabTimer;                     // 抓取计时器
        private float currentSwingAngle;             // 当前摆动角度
        private float swingVelocity;                 // 摆动速度
        private int swingCount;                      // 摆动次数计数
        private float lastSwingDirection;            // 上次摆动方向

        // 事件
        public event Action OnGrabStarted;           // 抓取开始事件
        public event Action OnGrabReleased;          // 抓取释放事件

        private void Update()
        {
            if (isGrabbed)
            {
                HandleGyroscope();
                UpdateSwingPhysics();
                CheckSwingReward();
            }
            else
            {
                CheckTwoFingerGrab();
            }
        }

        /// <summary>
        /// 检测双指抓取
        /// </summary>
        private void CheckTwoFingerGrab()
        {
            if (Input.touchCount != 2)
            {
                isGrabbing = false;
                grabTimer = 0;
                return;
            }

            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            // 检查双指是否在双肩判定点附近
            bool leftTouchOnShoulder = IsTouchOnShoulder(touch0.position, leftShoulderPos) ||
                                       IsTouchOnShoulder(touch1.position, leftShoulderPos);
            bool rightTouchOnShoulder = IsTouchOnShoulder(touch0.position, rightShoulderPos) ||
                                        IsTouchOnShoulder(touch1.position, rightShoulderPos);

            if (leftTouchOnShoulder && rightTouchOnShoulder)
            {
                if (!isGrabbing)
                {
                    isGrabbing = true;
                    grabTimer = 0;
                    OnGrabStarted?.Invoke();
                }

                grabTimer += Time.deltaTime;
                if (grabTimer >= grabHoldTime)
                {
                    StartGrab();
                }
            }
            else
            {
                isGrabbing = false;
                grabTimer = 0;
            }
        }

        private bool IsTouchOnShoulder(Vector2 touchPos, Vector2 shoulderPos)
        {
            // 将屏幕坐标转换为世界坐标（简化版）
            float distance = Vector2.Distance(touchPos, shoulderPos);
            return distance <= grabRadius;
        }

        /// <summary>
        /// 开始抓取
        /// </summary>
        private void StartGrab()
        {
            isGrabbed = true;
            isGrabbing = false;
            grabTimer = 0;

            // 通知猫咪进入被抓状态
            catController?.SetGrabbed(true);

            // 启用陀螺仪
            Input.gyro.enabled = true;

            Debug.Log("[TouchInteraction] 猫咪被举起！");
        }

        /// <summary>
        /// 处理陀螺仪输入
        /// </summary>
        private void HandleGyroscope()
        {
            // 读取陀螺仪数据
            Vector3 gyroRotation = Input.gyro.rotationRate;

            // 计算摆动速度（取 X 轴或 Z 轴，取决于设备方向）
            float targetVelocity = gyroRotation.x * sensitivity;

            // 更新摆动速度
            swingVelocity = Mathf.Lerp(swingVelocity, targetVelocity, 0.1f);
        }

        /// <summary>
        /// 更新摆动物理
        /// </summary>
        private void UpdateSwingPhysics()
        {
            // 应用阻尼，使摆动慢慢归零
            swingVelocity *= dampingFactor;

            // 计算摆动角度
            currentSwingAngle = swingVelocity * maxSwingAngle;
            currentSwingAngle = Mathf.Clamp(currentSwingAngle, -maxSwingAngle, maxSwingAngle);

            // 应用到猫咪
            catController?.SetSwingAngle(currentSwingAngle);
        }

        /// <summary>
        /// 检查摆动奖励（甩两下）
        /// </summary>
        private void CheckSwingReward()
        {
            // 检测摆动方向变化
            float currentDirection = Mathf.Sign(currentSwingAngle);
            
            if (currentDirection != 0 && currentDirection != lastSwingDirection)
            {
                swingCount++;
                lastSwingDirection = currentDirection;

                // 检测是否甩了两下（方向变化 2 次）
                if (swingCount >= 2)
                {
                    AwardSwingReward();
                    swingCount = 0;
                }
            }
        }

        private void AwardSwingReward()
        {
            var rewardSystem = Rewards.TreatRewardSystem.Instance;
            if (rewardSystem != null)
            {
                if (rewardSystem.AwardForSwing())
                {
                    Debug.Log("[TouchInteraction] 甩猫咪奖励！");
                }
                else
                {
                    float cooldown = rewardSystem.GetSwingCooldownRemaining();
                    Debug.Log($"[TouchInteraction] 甩猫咪冷却中：{cooldown:F1}秒");
                }
            }
        }

        /// <summary>
        /// 释放抓取
        /// </summary>
        public void ReleaseGrab()
        {
            isGrabbed = false;
            isGrabbing = false;
            grabTimer = 0;
            swingCount = 0;
            swingVelocity = 0;
            currentSwingAngle = 0;

            // 通知猫咪回到正常状态
            catController?.SetGrabbed(false);
            catController?.SetSwingAngle(0);

            // 禁用陀螺仪
            Input.gyro.enabled = false;

            OnGrabReleased?.Invoke();
            Debug.Log("[TouchInteraction] 猫咪被释放");
        }
    }

    /// <summary>
    /// 猫咪控制器（扩展）
    /// </summary>
    public partial class CatController : MonoBehaviour
    {
        [Header("触摸互动设置")]
        [SerializeField] private Transform lowerBodyPivot;  // 下半身旋转轴心

        private bool isGrabbed;
        private float currentSwingAngle;

        /// <summary>
        /// 设置被抓状态
        /// </summary>
        public void SetGrabbed(bool grabbed)
        {
            isGrabbed = grabbed;
            // 可以在这里触发被抓动画
        }

        /// <summary>
        /// 设置摆动角度
        /// </summary>
        public void SetSwingAngle(float angle)
        {
            currentSwingAngle = angle;
            if (lowerBodyPivot != null)
            {
                // 绕 Z 轴旋转（2D 游戏）
                lowerBodyPivot.localRotation = Quaternion.Euler(0, 0, angle);
            }
        }

        /// <summary>
        /// 被抓释放时的回调
        /// </summary>
        public void OnGrabReleased()
        {
            // 播放落下动画或音效
        }
    }
}
```

### 5.5 双肩判定点配置

```
┌─────────────────────────────────────────────────────────┐
│ 双肩判定点配置示意图                                    │
├─────────────────────────────────────────────────────────┤
│                                                         │
│        左手判定点          右手判定点                   │
│            ●───────────────────●                       │
│            │                   │                       │
│            │     猫咪头部      │                       │
│            │        ◉          │                       │
│            │       /│\         │                       │
│            │      / │ \        │                       │
│            │     /  │  \       │                       │
│            │    /   │   \      │                       │
│            │   /    │    \     │                       │
│            │  /     │     \    │                       │
│            │ /      │      \   │                       │
│            │/       │       \  │                       │
│            ●───────┌─┴─┐──────●                       │
│           左肩     │身体│     右肩                     │
│           判定点   └───┘     判定点                   │
│                                                         │
│  配置参数（示例，需要在 Unity 编辑器中调整）：          │
│  - leftShoulderPos: (-2.5, 3.0)  // 屏幕坐标           │
│  - rightShoulderPos: (2.5, 3.0)  // 屏幕坐标           │
│  - grabRadius: 1.0               // 判定半径           │
│                                                         │
└─────────────────────────────────────────────────────────┘
```

---

## 6. 节奏系统

### 6.1 系统概述

节奏系统是本游戏的核心特色，允许玩家在休闲模式下随意游玩，同时为想挑战高分的玩家提供节奏玩法。

### 6.2 节奏管理器

```csharp
using UnityEngine;
using System;

namespace HaChiMiOhNameruDo.Rhythm
{
    /// <summary>
    /// 节奏判定结果
    /// </summary>
    public enum RhythmJudgment
    {
        Perfect,    // 完美
        Good,       // 良好
        Normal,     // 普通
        Miss        // 未命中
    }

    /// <summary>
    /// 节奏管理器 - 单例
    /// 负责追踪音乐节拍、判定输入时机、触发节拍事件
    /// </summary>
    public class RhythmManager : MonoBehaviour
    {
        public static RhythmManager Instance { get; private set; }

        [Header("音乐设置")]
        [SerializeField] private float bpm = 110f;           // 节拍速度
        [SerializeField] private int beatsPerMeasure = 4;    // 每小节拍数
        [SerializeField] private float audioOffset = 0f;     // 音频延迟补偿

        [Header("判定窗口 (秒)")]
        [SerializeField] private float perfectWindow = 0.05f;  // ±50ms
        [SerializeField] private float goodWindow = 0.10f;     // ±100ms
        [SerializeField] private float normalWindow = 0.20f;   // ±200ms

        [Header("谱面数据")]
        [SerializeField] private ChartData chartData;        // 谱面数据

        // 当前状态
        private float currentBeatTime;         // 当前节拍时间
        private int currentBeat;               // 当前节拍编号
        private int currentMeasure;            // 当前小节编号
        private float beatDuration;            // 每拍时长
        private bool isPlaying;                // 是否正在播放

        // 事件
        public event Action<int, int> OnBeatHit;        // 节拍命中事件 (beat, measure)
        public event Action<int> OnMeasureStart;        // 新小节开始
        public event Action<RhythmJudgment, int> OnInputJudged; // 输入判定事件

        // 连击系统
        private int currentCombo;
        private int maxCombo;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        private void Start()
        {
            beatDuration = 60f / bpm;
            currentBeatTime = -audioOffset;
        }

        private void Update()
        {
            if (!isPlaying) return;

            // 更新节拍追踪
            currentBeatTime += Time.deltaTime;

            // 检测是否到达新节拍
            if (currentBeatTime >= beatDuration)
            {
                currentBeat++;
                currentBeatTime -= beatDuration;

                // 检查新小节
                if (currentBeat % beatsPerMeasure == 0)
                {
                    currentMeasure++;
                    OnMeasureStart?.Invoke(currentMeasure);
                }

                // 触发节拍事件
                OnBeatHit?.Invoke(currentBeat, currentMeasure);
            }
        }

        /// <summary>
        /// 开始节奏追踪
        /// </summary>
        public void StartRhythm(ChartData chart)
        {
            chartData = chart;
            bpm = chart.metadata.bpm;
            beatDuration = 60f / bpm;
            currentBeatTime = 0;
            currentBeat = 0;
            currentMeasure = 0;
            currentCombo = 0;
            maxCombo = 0;
            isPlaying = true;
        }

        /// <summary>
        /// 停止节奏追踪
        /// </summary>
        public void StopRhythm()
        {
            isPlaying = false;
        }

        /// <summary>
        /// 判定玩家输入
        /// </summary>
        public RhythmJudgment JudgeInput(float inputTime, out float timingOffset)
        {
            timingOffset = currentBeatTime - inputTime;
            float absOffset = Mathf.Abs(timingOffset);

            if (absOffset <= perfectWindow)
                return RhythmJudgment.Perfect;
            if (absOffset <= goodWindow)
                return RhythmJudgment.Good;
            if (absOffset <= normalWindow)
                return RhythmJudgment.Normal;
            return RhythmJudgment.Miss;
        }

        /// <summary>
        /// 处理输入并计算奖励
        /// </summary>
        public void ProcessRhythmInput(float inputTime)
        {
            var judgment = JudgeInput(inputTime, out float offset);

            if (judgment != RhythmJudgment.Miss)
            {
                currentCombo++;
                if (currentCombo > maxCombo) maxCombo = currentCombo;

                // 发放小鱼干奖励
                if (judgment == RhythmJudgment.Perfect)
                {
                    Rewards.TreatRewardSystem.Instance?.AwardForPerfect();
                }
                else if (judgment == RhythmJudgment.Normal)
                {
                    Rewards.TreatRewardSystem.Instance?.AwardForNormal();
                }
            }
            else
            {
                currentCombo = 0;
            }

            OnInputJudged?.Invoke(judgment, currentCombo);
        }

        /// <summary>
        /// 获取当前节拍在小节中的位置 (0-3)
        /// </summary>
        public int GetBeatInMeasure() => currentBeat % beatsPerMeasure;

        /// <summary>
        /// 检查是否是强拍 (第 1 拍)
        /// </summary>
        public bool IsStrongBeat() => GetBeatInMeasure() == 0;

        /// <summary>
        /// 获取当前连击数
        /// </summary>
        public int GetCurrentCombo() => currentCombo;

        /// <summary>
        /// 获取最大连击数
        /// </summary>
        public int GetMaxCombo() => maxCombo;
    }
}
```

---

## 7. 数据结构总览

### 7.1 核心枚举

```csharp
// 游戏状态
public enum GameState
{
    Idle,
    FurBallGame,
    TissueGame
}

// 节奏判定
public enum RhythmJudgment
{
    Perfect,
    Good,
    Normal,
    Miss
}

// 谱面事件类型
public enum ChartEventType
{
    Beat,
    StrongBeat,
    Paw,
    Pull,
    Cut
}

// 划动方向
public enum SwipeDirection
{
    Up,
    Down,
    Left,
    Right
}
```

### 7.2 单例访问速查

```csharp
// 核心管理
GameManager.Instance
UIManager.Instance
AudioManager.Instance
HapticManager.Instance

// 游戏逻辑
FurBallGameManager.Instance
TissueGameManager.Instance

// 节奏系统
RhythmManager.Instance
ChartLoader.Instance

// 奖励系统
TreatCounter.Instance
TreatRewardSystem.Instance
UnlockSystem.Instance
```

### 7.3 关键参数速查

#### 游戏管理

| 参数 | 默认值 | 说明 |
|------|--------|------|
| `miniGameDuration` | 30f | 小游戏时长（秒） |
| `exitDelay` | 2f | 退出延迟（秒） |

#### 节奏系统

| 参数 | 默认值 | 说明 |
|------|--------|------|
| `bpm` | 110f | 音乐节拍速度 |
| `perfectWindow` | 0.05f | Perfect 判定窗口（秒） |
| `goodWindow` | 0.10f | Good 判定窗口（秒） |
| `normalWindow` | 0.20f | Normal 判定窗口（秒） |

#### 触摸互动

| 参数 | 默认值 | 说明 |
|------|--------|------|
| `grabHoldTime` | 0.5f | 抓取判定时间（秒） |
| `maxSwingAngle` | 30f | 最大摆动角度（度） |
| `dampingFactor` | 0.95f | 摆动阻尼系数 |
| `sensitivity` | 2f | 陀螺仪灵敏度 |

#### 小鱼干奖励

| 参数 | 默认值 | 说明 |
|------|--------|------|
| `perfectReward` | 2 | Perfect 判定奖励 |
| `normalReward` | 1 | Normal 判定奖励 |
| `swingReward` | 5 | 甩猫咪奖励 |
| `swingCooldown` | 60f | 甩猫咪冷却（秒） |
| `miniGameUnlockCost` | 50 | 小游戏解锁价格 |

---

## 8. AI 任务清单

### 8.1 谱面系统

- [ ] 实现 `BeatDetector` 类（节拍检测算法）
- [ ] 实现 `AIChartGenerator` 类（谱面生成器）
- [ ] 实现 `ChartLoader` 类（谱面加载器）
- [ ] 创建谱面 JSON 配置格式
- [ ] 测试 BPM 检测准确性（误差 < ±5 BPM）
- [ ] 测试节拍位置准确性（误差 < ±50ms）

### 8.2 小鱼干奖励系统

- [ ] 实现 `TreatCounter` 类（小鱼干计数器）
- [ ] 实现 `TreatRewardSystem` 类（奖励发放）
- [ ] 实现 `UnlockSystem` 类（解锁系统）
- [ ] 创建 `TreatCounterUI`（计数器 UI）
- [ ] 创建 `TreatShopUI`（商店 UI）
- [ ] 集成到节奏系统（Perfect/Normal 奖励）
- [ ] 集成到触摸互动（甩猫咪奖励）

### 8.3 触摸互动系统

- [ ] 实现 `TouchInteractionHandler` 类
- [ ] 实现双指触摸检测逻辑
- [ ] 实现抓取判定计时器（0.5 秒）
- [ ] 集成陀螺仪输入处理
- [ ] 实现摆动物理逻辑（速度→角度，阻尼归零）
- [ ] 扩展 `CatController` 添加被抓状态和摆动方法
- [ ] 创建被抓动画（CatGrabbed.anim）
- [ ] 创建摆动动画（CatSwing.anim）
- [ ] 配置双肩判定点位置
- [ ] 集成甩猫咪奖励检测

### 8.4 节奏系统

- [ ] 实现 `RhythmManager` 类
- [ ] 实现节拍追踪算法
- [ ] 实现节奏判定系统
- [ ] 集成小鱼干奖励发放
- [ ] 创建通用节奏 UI 组件（Perfect/Good 显示）

### 8.5 集成测试

- [ ] 测试谱面加载和节拍同步
- [ ] 测试节奏判定和奖励发放
- [ ] 测试触摸互动和奖励检测
- [ ] 测试小鱼干解锁小游戏流程
- [ ] 整体游戏流程测试

---

## 附录 A: 相关文档

- [主游戏设计文档](./GAME_DESIGN_DOCUMENT.md)
- [音乐需求文档](../plans/Music_Requirements.md)
- [节奏天国设计文档](../plans/Rhythm_Tengoku_Design_Document.md)
- [节奏游戏开发计划](../plans/Rhythm_Game_Development_Plan.md)

## 附录 B: 参考资源

- [Essentia 文档](https://essentia.upf.edu/)
- [Librosa 文档](https://librosa.org/doc/)
- [Aubio 文档](https://aubio.org/)
- [Unity Audio 文档](https://docs.unity3d.com/Manual/AudioSection.html)
- [Unity 输入系统文档](https://docs.unity3d.com/Manual/InputSystem.html)
- [Unity 陀螺仪文档](https://docs.unity3d.com/ScriptReference/Gyroscope.html)

---

*文档结束*
