using UnityEngine;

namespace HaChiMiOhNameruDo.Gameplay
{
    /// <summary>
    /// 猫咪控制器
    /// 负责控制猫咪的动画、状态和行为
    /// </summary>
    public class CatController : MonoBehaviour
    {
        [Header("动画组件")]
        [SerializeField] private Animator animator;
        [SerializeField] private SpriteRenderer spriteRenderer;

        [Header("动画参数")]
        private static readonly int IdleHash = Animator.StringToHash("Idle");
        private static readonly int JumpHash = Animator.StringToHash("Jump");
        private static readonly int PawsHash = Animator.StringToHash("Paws");
        private static readonly int BackHash = Animator.StringToHash("Back");

        [Header("毛球小游戏设置")]
        [SerializeField] private Transform furBallSpawnPoint;   // 毛球生成点（头顶）
        [SerializeField] private float furBallYOffset = 1.5f;   // 毛球相对头顶的 Y 轴偏移

        [Header("纸巾筒小游戏设置")]
        [SerializeField] private Transform tissueSpawnPoint;    // 纸巾筒生成点（头顶）

        // 当前状态
        private bool isBackToPlayer;  // 是否背对玩家

        public bool IsBackToPlayer => isBackToPlayer;
        public Transform FurBallSpawnPoint => furBallSpawnPoint;
        public Transform TissueSpawnPoint => tissueSpawnPoint;

        private void Awake()
        {
            // 如果没有手动分配，尝试自动获取组件
            if (animator == null)
                animator = GetComponent<Animator>();
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            // 初始化为 IDLE 状态
            SetIdle();
        }

        #region 状态控制

        /// <summary>
        /// 设置 IDLE 状态
        /// </summary>
        public void SetIdle()
        {
            isBackToPlayer = false;
            SetAnimation(IdleHash);
            Debug.Log("[Cat] 进入 IDLE 状态");
        }

        /// <summary>
        /// 准备拍击毛球（面向前方）
        /// </summary>
        public void PrepareForFurBall()
        {
            isBackToPlayer = false;
            SetAnimation(IdleHash);
        }

        /// <summary>
        /// 执行拍击动作
        /// </summary>
        public void DoPaws()
        {
            SetAnimation(PawsHash);
            Debug.Log("[Cat] 执行拍击动作");
        }

        /// <summary>
        /// 准备纸巾筒游戏（背对玩家）
        /// </summary>
        public void PrepareForTissue()
        {
            isBackToPlayer = true;
            SetAnimation(BackHash);
            Debug.Log("[Cat] 背对玩家，准备纸巾筒游戏");
        }

        #endregion

        #region 动画控制

        /// <summary>
        /// 设置动画状态
        /// </summary>
        private void SetAnimation(int animationHash)
        {
            if (animator != null)
            {
                animator.SetTrigger(animationHash);
            }
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
            // 可以在这里触发其他逻辑
        }

        #endregion
    }
}
