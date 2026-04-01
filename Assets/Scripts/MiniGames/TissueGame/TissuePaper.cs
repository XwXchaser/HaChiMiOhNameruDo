using UnityEngine;
using System.Collections.Generic;

namespace HaChiMiOhNameruDo.MiniGames.TissueGame
{
    /// <summary>
    /// 厕纸组件
    /// 负责厕纸的延伸、滚动和切碎效果
    /// </summary>
    public class TissuePaper : MonoBehaviour
    {
        [Header("厕纸设置")]
        [SerializeField] private float segmentHeight = 0.1f;    // 每段厕纸的高度
        [SerializeField] private float extendSpeed = 0.5f;      // 延伸速度

        [Header("组件引用")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Collider2D paperCollider;
        [SerializeField] private Transform anchorPoint;         // 厕纸固定点（纸巾筒底部）

        [Header("切碎效果")]
        [SerializeField] private GameObject cutParticlePrefab;  // 切碎粒子预制体
        [SerializeField] private int cutParticleCount = 10;     // 粒子数量

        // 当前厕纸长度（段数）
        private int currentLength;
        private List<GameObject> paperSegments;  // 厕纸段列表

        private void Awake()
        {
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();
            if (paperCollider == null)
                paperCollider = GetComponent<Collider2D>();
            
            paperSegments = new List<GameObject>();
        }

        private void Start()
        {
            Hide();
        }

        /// <summary>
        /// 重置厕纸
        /// </summary>
        public void ResetPaper()
        {
            StopAllCoroutines();
            currentLength = 0;

            // 清除所有厕纸段
            foreach (var segment in paperSegments)
            {
                if (segment != null)
                    Destroy(segment);
            }
            paperSegments.Clear();

            Show();
        }

        /// <summary>
        /// 显示厕纸
        /// </summary>
        public void Show()
        {
            if (spriteRenderer != null)
                spriteRenderer.enabled = true;
            if (paperCollider != null)
                paperCollider.enabled = true;
        }

        /// <summary>
        /// 隐藏厕纸
        /// </summary>
        public void Hide()
        {
            if (spriteRenderer != null)
                spriteRenderer.enabled = false;
            if (paperCollider != null)
                paperCollider.enabled = false;
        }

        /// <summary>
        /// 延伸厕纸
        /// </summary>
        /// <param name="newLength">新的厕纸长度（段数）</param>
        public void ExtendPaper(int newLength)
        {
            if (newLength <= currentLength) return;

            // 添加新的厕纸段
            while (currentLength < newLength)
            {
                CreatePaperSegment(currentLength);
                currentLength++;
            }
        }

        /// <summary>
        /// 创建一段厕纸
        /// </summary>
        private void CreatePaperSegment(int segmentIndex)
        {
            // 计算位置（从锚点向下）
            Vector3 segmentPosition = anchorPoint.position + Vector3.down * (segmentIndex * segmentHeight);
            
            // 创建厕纸段 GameObject
            GameObject segment = new GameObject($"TissueSegment_{segmentIndex}");
            segment.transform.position = segmentPosition;
            segment.transform.SetParent(transform);

            // 添加 SpriteRenderer
            SpriteRenderer sr = segment.AddComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                sr.sprite = spriteRenderer.sprite;
                sr.color = spriteRenderer.color;
                sr.sortingOrder = spriteRenderer.sortingOrder - segmentIndex;
            }

            // 添加 Collider
            BoxCollider2D collider = segment.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(0.5f, segmentHeight * 0.9f);

            paperSegments.Add(segment);
        }

        /// <summary>
        /// 切碎厕纸
        /// </summary>
        public void CutPaper()
        {
            Debug.Log("[TissuePaper] 切碎厕纸！");

            // 生成切碎粒子效果
            SpawnCutParticles();

            // 移除所有厕纸段
            foreach (var segment in paperSegments)
            {
                if (segment != null)
                    Destroy(segment);
            }
            paperSegments.Clear();

            currentLength = 0;
        }

        /// <summary>
        /// 生成切碎粒子效果
        /// </summary>
        private void SpawnCutParticles()
        {
            if (cutParticlePrefab == null) return;

            for (int i = 0; i < cutParticleCount; i++)
            {
                // 在厕纸区域随机位置生成粒子
                Vector3 spawnPos = anchorPoint.position + 
                    new Vector3(
                        Random.Range(-0.5f, 0.5f),
                        Random.Range(-currentLength * segmentHeight * 0.5f, 0),
                        0
                    );

                GameObject particle = Instantiate(cutParticlePrefab, spawnPos, Quaternion.identity);
                Destroy(particle, 2f);  // 2 秒后销毁
            }
        }
    }
}
