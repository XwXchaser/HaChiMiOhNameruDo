using UnityEngine;
using System.Collections;

namespace HaChiMiOhNameruDo.Gameplay
{
    /// <summary>
    /// 猫咪控制器 - 简化版
    /// 只需在 Inspector 中拖入精灵图片，即可自动播放动画
    /// </summary>
    public class CatController : MonoBehaviour
    {
        [Header("动画组件")]
        [SerializeField] private SpriteRenderer spriteRenderer;

        [Header("动画帧率")]
        [Tooltip("动画播放速度（每秒帧数）")]
        [SerializeField] private float frameRate = 8f;

        [Header("毛球游戏 - 待机动画")]
        [Tooltip("毛球游戏待机时的循环动画帧")]
        [SerializeField] private Sprite[] furBallIdleFrames;

        [Header("毛球游戏 - 拍击动画")]
        [Tooltip("玩家点击屏幕时播放的拍击动画帧")]
        [SerializeField] private Sprite[] furBallSlapFrames;

        [Header("纸巾游戏 - 待机动画")]
        [Tooltip("纸巾游戏待机时的循环动画帧")]
        [SerializeField] private Sprite[] tissueIdleFrames;

        [Header("纸巾游戏 - 扒拉动画")]
        [Tooltip("玩家扒拉纸巾时播放的动画帧")]
        [SerializeField] private Sprite[] tissuePullFrames;

        [Header("纸巾游戏 - 切断动画")]
        [Tooltip("玩家切断纸巾时播放的动画帧")]
        [SerializeField] private Sprite[] tissueCutFrames;

        // 当前状态
        private Coroutine currentAnimation;
        private float frameDuration;
        private bool isPlayingLoop = false;

        private void Awake()
        {
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();

            frameDuration = 1f / frameRate;
        }

        private void Start()
        {
            // 默认播放毛球待机动画
            if (furBallIdleFrames != null && furBallIdleFrames.Length > 0)
            {
                PlayLoopAnimation(furBallIdleFrames);
            }
        }

        #region 公共方法 - 直接调用播放动画

        /// <summary>
        /// 播放毛球待机动画
        /// </summary>
        public void PlayFurBallIdle()
        {
            if (furBallIdleFrames != null && furBallIdleFrames.Length > 0)
            {
                PlayLoopAnimation(furBallIdleFrames);
            }
        }

        /// <summary>
        /// 播放毛球拍击动画（播放一次后返回待机）
        /// </summary>
        public void PlayFurBallSlap()
        {
            if (furBallSlapFrames != null && furBallSlapFrames.Length > 0)
            {
                PlayOnceAnimation(furBallSlapFrames, () => PlayFurBallIdle());
            }
        }

        /// <summary>
        /// 播放纸巾待机动画
        /// </summary>
        public void PlayTissueIdle()
        {
            if (tissueIdleFrames != null && tissueIdleFrames.Length > 0)
            {
                PlayLoopAnimation(tissueIdleFrames);
            }
        }

        /// <summary>
        /// 播放纸巾扒拉动画（播放一次后返回待机）
        /// </summary>
        public void PlayTissuePull()
        {
            if (tissuePullFrames != null && tissuePullFrames.Length > 0)
            {
                PlayOnceAnimation(tissuePullFrames, () => PlayTissueIdle());
            }
        }

        /// <summary>
        /// 播放纸巾切断动画（播放一次后返回待机）
        /// </summary>
        public void PlayTissueCut()
        {
            if (tissueCutFrames != null && tissueCutFrames.Length > 0)
            {
                PlayOnceAnimation(tissueCutFrames, () => PlayTissueIdle());
            }
        }

        #endregion

        #region 动画播放核心方法

        /// <summary>
        /// 播放循环动画
        /// </summary>
        private void PlayLoopAnimation(Sprite[] frames)
        {
            StopCurrentAnimation();
            isPlayingLoop = true;
            currentAnimation = StartCoroutine(AnimateLoop(frames));
        }

        /// <summary>
        /// 播放一次动画，完成后执行回调
        /// </summary>
        private void PlayOnceAnimation(Sprite[] frames, System.Action onComplete)
        {
            StopCurrentAnimation();
            isPlayingLoop = false;
            currentAnimation = StartCoroutine(AnimateOnce(frames, onComplete));
        }

        /// <summary>
        /// 停止当前动画
        /// </summary>
        private void StopCurrentAnimation()
        {
            if (currentAnimation != null)
            {
                StopCoroutine(currentAnimation);
                currentAnimation = null;
            }
        }

        /// <summary>
        /// 循环动画协程
        /// </summary>
        private IEnumerator AnimateLoop(Sprite[] frames)
        {
            if (frames == null || frames.Length == 0) yield break;

            int index = 0;
            while (true)
            {
                spriteRenderer.sprite = frames[index];
                index = (index + 1) % frames.Length;
                yield return new WaitForSeconds(frameDuration);
            }
        }

        /// <summary>
        /// 播放一次动画协程
        /// </summary>
        private IEnumerator AnimateOnce(Sprite[] frames, System.Action onComplete)
        {
            if (frames == null || frames.Length == 0)
            {
                onComplete?.Invoke();
                yield break;
            }

            foreach (var frame in frames)
            {
                spriteRenderer.sprite = frame;
                yield return new WaitForSeconds(frameDuration);
            }

            onComplete?.Invoke();
        }

        #endregion

        #region 编辑器调试方法

        /// <summary>
        /// 在编辑器中测试动画
        /// </summary>
        [ContextMenu("测试：毛球待机")]
        private void TestFurBallIdle() => PlayFurBallIdle();

        [ContextMenu("测试：毛球拍击")]
        private void TestFurBallSlap() => PlayFurBallSlap();

        [ContextMenu("测试：纸巾待机")]
        private void TestTissueIdle() => PlayTissueIdle();

        [ContextMenu("测试：纸巾扒拉")]
        private void TestTissuePull() => PlayTissuePull();

        [ContextMenu("测试：纸巾切断")]
        private void TestTissueCut() => PlayTissueCut();

        #endregion
    }
}
