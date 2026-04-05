using UnityEngine;
using System.Collections;

namespace HaChiMiOhNameruDo.Gameplay
{
    /// <summary>
    /// 猫咪控制器 - 纯代码版本
    /// 负责控制猫咪的动画、状态和行为
    /// 使用 Sprite 切换替代 Animator
    /// </summary>
    public class CatController : MonoBehaviour
    {
        [Header("动画组件")]
        [SerializeField] private SpriteRenderer spriteRenderer;

        [Header("毛球小游戏设置")]
        [SerializeField] private Transform furBallSpawnPoint;   // 毛球生成点（头顶）
        [SerializeField] private float furBallYOffset = 1.5f;   // 毛球相对头顶的 Y 轴偏移

        [Header("纸巾筒小游戏设置")]
        [SerializeField] private Transform tissueSpawnPoint;    // 纸巾筒生成点（头顶）

        [Header("精灵资源 - 毛球游戏")]
        [SerializeField] private Sprite furBallIdle;            // Cat_BallGame_idle
        [SerializeField] private Sprite furBallSit;             // Cat_BallGame_sit
        [SerializeField] private Sprite furBallSlap1;           // Cat_BallGame_slap1
        [SerializeField] private Sprite furBallSlap2;           // Cat_BallGame_slap2

        [Header("精灵资源 - 纸巾游戏")]
        [SerializeField] private Sprite tissueSit;              // Cat_TissueGame_sit
        [SerializeField] private Sprite tissueCut1;             // Cat_TissueGame_cut1
        [SerializeField] private Sprite tissueCut2;             // Cat_TissueGame_cut2
        [SerializeField] private Sprite tissuePull1;            // Cat_TissueGame_pull1
        [SerializeField] private Sprite tissuePull2;            // Cat_TissueGame_pull2

        [Header("动画设置")]
        [SerializeField] private float animationFrameRate = 10f; // 动画帧率（FPS）

        // 当前状态
        private CatAnimationState currentState = CatAnimationState.Idle;
        private bool isBackToPlayer;  // 是否背对玩家

        // 动画相关
        private Coroutine currentAnimationCoroutine;
        private float frameDuration;

        public bool IsBackToPlayer => isBackToPlayer;
        public Transform FurBallSpawnPoint => furBallSpawnPoint;
        public Transform TissueSpawnPoint => tissueSpawnPoint;
        public CatAnimationState CurrentState => currentState;

        /// <summary>
        /// 猫咪动画状态枚举
        /// </summary>
        public enum CatAnimationState
        {
            Idle,               // 主菜单待机
            FurBallGame_Sit,    // 毛球游戏待机
            FurBallGame_Slap,   // 毛球游戏拍击
            TissueGame_Sit,     // 纸巾游戏待机
            TissueGame_Cut,     // 纸巾游戏切断
            TissueGame_Pull     // 纸巾游戏扒拉
        }

        private void Awake()
        {
            // 自动获取组件
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();
            
            // 计算帧间隔
            frameDuration = 1f / animationFrameRate;

            // 设置 Sorting Order 确保小猫渲染在 UI 前面
            if (spriteRenderer != null)
            {
                spriteRenderer.sortingOrder = 1;
            }
        }

        private void Start()
        {
            // 初始化为 IDLE 状态
            SetIdle();
        }

        #region 状态控制

        /// <summary>
        /// 设置 IDLE 状态（主菜单待机）
        /// </summary>
        public void SetIdle()
        {
            StopCurrentAnimation();
            currentState = CatAnimationState.Idle;
            isBackToPlayer = false;
            SetSprite(furBallIdle);
            Debug.Log("[Cat] 进入 IDLE 状态");
        }

        /// <summary>
        /// 准备拍击毛球（面向前方）
        /// </summary>
        public void PrepareForFurBall()
        {
            StopCurrentAnimation();
            currentState = CatAnimationState.FurBallGame_Sit;
            isBackToPlayer = false;
            SetSprite(furBallSit);
            Debug.Log("[Cat] 准备拍击毛球");
        }

        /// <summary>
        /// 执行拍击动作（毛球小游戏）
        /// 玩家点击屏幕时触发
        /// </summary>
        public void DoPaws()
        {
            if (currentState != CatAnimationState.FurBallGame_Sit && 
                currentState != CatAnimationState.FurBallGame_Slap)
            {
                Debug.LogWarning("[Cat] 当前不在毛球游戏状态，无法执行拍击");
                return;
            }

            StopCurrentAnimation();
            currentState = CatAnimationState.FurBallGame_Slap;
            currentAnimationCoroutine = StartCoroutine(PlayLoopAnimation(
                new[] { furBallSlap1, furBallSlap2 },
                () => {
                    // 动画完成后返回待机状态
                    if (currentState == CatAnimationState.FurBallGame_Slap)
                    {
                        PrepareForFurBall();
                    }
                }
            ));
            Debug.Log("[Cat] 执行拍击动作");
        }

        /// <summary>
        /// 停止拍击动画，返回待机状态
        /// </summary>
        public void StopPaws()
        {
            if (currentState == CatAnimationState.FurBallGame_Slap)
            {
                PrepareForFurBall();
            }
        }

        /// <summary>
        /// 准备纸巾筒游戏（背对玩家）
        /// </summary>
        public void PrepareForTissue()
        {
            StopCurrentAnimation();
            currentState = CatAnimationState.TissueGame_Sit;
            isBackToPlayer = true;
            SetSprite(tissueSit);
            Debug.Log("[Cat] 背对玩家，准备纸巾筒游戏");
        }

        /// <summary>
        /// 执行扒拉动作（纸巾小游戏）
        /// 玩家划动纸巾时触发
        /// </summary>
        public void DoPull()
        {
            if (currentState != CatAnimationState.TissueGame_Sit && 
                currentState != CatAnimationState.TissueGame_Pull)
            {
                Debug.LogWarning("[Cat] 当前不在纸巾游戏状态，无法执行扒拉");
                return;
            }

            StopCurrentAnimation();
            currentState = CatAnimationState.TissueGame_Pull;
            currentAnimationCoroutine = StartCoroutine(PlayLoopAnimation(
                new[] { tissuePull1, tissuePull2 },
                () => {
                    // 动画完成后返回待机状态
                    if (currentState == CatAnimationState.TissueGame_Pull)
                    {
                        PrepareForTissue();
                    }
                }
            ));
            Debug.Log("[Cat] 执行扒拉动作");
        }

        /// <summary>
        /// 停止扒拉动画，返回待机状态
        /// </summary>
        public void StopPull()
        {
            if (currentState == CatAnimationState.TissueGame_Pull)
            {
                PrepareForTissue();
            }
        }

        /// <summary>
        /// 执行切断动作（纸巾小游戏）
        /// 玩家划动纸巾筒时触发
        /// </summary>
        public void DoCut()
        {
            if (currentState != CatAnimationState.TissueGame_Sit && 
                currentState != CatAnimationState.TissueGame_Cut)
            {
                Debug.LogWarning("[Cat] 当前不在纸巾游戏状态，无法执行切断");
                return;
            }

            StopCurrentAnimation();
            currentState = CatAnimationState.TissueGame_Cut;
            currentAnimationCoroutine = StartCoroutine(PlayLoopAnimation(
                new[] { tissueCut1, tissueCut2 },
                () => {
                    // 动画完成后返回待机状态
                    if (currentState == CatAnimationState.TissueGame_Cut)
                    {
                        PrepareForTissue();
                    }
                }
            ));
            Debug.Log("[Cat] 执行切断动作");
        }

        /// <summary>
        /// 停止切断动画，返回待机状态
        /// </summary>
        public void StopCut()
        {
            if (currentState == CatAnimationState.TissueGame_Cut)
            {
                PrepareForTissue();
            }
        }

        #endregion

        #region 动画控制

        /// <summary>
        /// 设置精灵
        /// </summary>
        private void SetSprite(Sprite sprite)
        {
            if (spriteRenderer != null && sprite != null)
            {
                spriteRenderer.sprite = sprite;
            }
        }

        /// <summary>
        /// 停止当前动画
        /// </summary>
        private void StopCurrentAnimation()
        {
            if (currentAnimationCoroutine != null)
            {
                StopCoroutine(currentAnimationCoroutine);
                currentAnimationCoroutine = null;
            }
        }

        /// <summary>
        /// 播放循环动画（两帧交替）
        /// </summary>
        /// <param name="frames">动画帧数组</param>
        /// <param name="onComplete">动画完成回调</param>
        private IEnumerator PlayLoopAnimation(Sprite[] frames, System.Action onComplete)
        {
            if (frames == null || frames.Length < 2) yield break;

            int frameIndex = 0;
            while (currentAnimationCoroutine != null) // 如果协程未被停止则继续
            {
                SetSprite(frames[frameIndex]);
                frameIndex = (frameIndex + 1) % frames.Length;
                yield return new WaitForSeconds(frameDuration);
            }

            onComplete?.Invoke();
        }

        /// <summary>
        /// 翻转猫咪朝向
        /// </summary>
        /// <param name="faceForward">true=面向玩家，false=背对玩家</param>
        public void SetFacingDirection(bool faceForward)
        {
            if (spriteRenderer != null)
            {
                // 根据朝向翻转 sprite
                spriteRenderer.flipX = !faceForward;
            }
            isBackToPlayer = !faceForward;
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 获取毛球生成位置
        /// </summary>
        public Vector3 GetFurBallSpawnPosition()
        {
            if (furBallSpawnPoint != null)
                return furBallSpawnPoint.position;

            // 如果没有分配 spawn point，使用头顶位置
            return transform.position + Vector3.up * furBallYOffset;
        }

        /// <summary>
        /// 获取纸巾筒生成位置
        /// </summary>
        public Vector3 GetTissueSpawnPosition()
        {
            if (tissueSpawnPoint != null)
                return tissueSpawnPoint.position;

            // 默认位置
            return transform.position + Vector3.up * 1.0f;
        }

        #endregion

        #region 动画事件回调

        /// <summary>
        /// 动画事件：拍击完成时调用
        /// </summary>
        public void OnPawsComplete()
        {
            Debug.Log("[Cat] 拍击动画完成");
        }

        #endregion
    }
}
