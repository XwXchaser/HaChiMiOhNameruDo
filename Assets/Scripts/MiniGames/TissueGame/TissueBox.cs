using UnityEngine;
using System.Collections;

namespace HaChiMiOhNameruDo.MiniGames.TissueGame
{
    /// <summary>
    /// 纸巾筒组件
    /// 负责纸巾筒的显示、隐藏和移动动画
    /// </summary>
    public class TissueBox : MonoBehaviour
    {
        [Header("移动设置")]
        [SerializeField] private float moveUpDuration = 1f;     // 上移消失时间
        [SerializeField] private float moveUpDistance = 3f;     // 上移距离

        [Header("组件引用")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Collider2D boxCollider;

        private Vector3 startPosition;
        private Vector3 targetPosition;

        private void Awake()
        {
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();
            if (boxCollider == null)
                boxCollider = GetComponent<Collider2D>();
        }

        private void Start()
        {
            startPosition = transform.position;
            targetPosition = startPosition + Vector3.up * moveUpDistance;
        }

        /// <summary>
        /// 重置纸巾筒到初始位置
        /// </summary>
        public void ResetBox()
        {
            StopAllCoroutines();
            transform.position = startPosition;
            Show();
        }

        /// <summary>
        /// 显示纸巾筒
        /// </summary>
        public void Show()
        {
            if (spriteRenderer != null)
                spriteRenderer.enabled = true;
            if (boxCollider != null)
                boxCollider.enabled = true;
        }

        /// <summary>
        /// 隐藏纸巾筒
        /// </summary>
        public void Hide()
        {
            if (spriteRenderer != null)
                spriteRenderer.enabled = false;
            if (boxCollider != null)
                boxCollider.enabled = false;
        }

        /// <summary>
        /// 上移并消失
        /// </summary>
        public void MoveUpAndDisappear()
        {
            StartCoroutine(MoveUpCoroutine());
        }

        private IEnumerator MoveUpCoroutine()
        {
            float elapsed = 0f;

            while (elapsed < moveUpDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / moveUpDuration);
                
                // 使用 EaseIn 缓动
                t = t * t;
                transform.position = Vector3.Lerp(startPosition, targetPosition, t);
                
                yield return null;
            }

            // 确保到达目标位置
            transform.position = targetPosition;
            Hide();
        }
    }
}
