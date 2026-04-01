using UnityEngine;
using HaChiMiOhNameruDo.Managers;

namespace HaChiMiOhNameruDo.UI
{
    /// <summary>
    /// IDLE 状态 UI 控制器
    /// 负责处理主界面的道具按钮点击事件
    /// </summary>
    public class IdleUI : MonoBehaviour
    {
        [Header("道具按钮")]
        [SerializeField] private GameObject furBallButton;      // 毛球道具按钮
        [SerializeField] private GameObject tissueBoxButton;    // 纸巾筒道具按钮

        [Header("管理器引用")]
        [SerializeField] private Managers.GameManager gameManager;

        private void Awake()
        {
            // 自动获取 GameManager
            if (gameManager == null)
                gameManager = FindObjectOfType<Managers.GameManager>();
        }

        private void Start()
        {
            // 绑定按钮点击事件
            BindButtonEvents();
        }

        private void BindButtonEvents()
        {
            // 毛球道具按钮
            if (furBallButton != null)
            {
                // 检查是否有 Button 组件
                var button = furBallButton.GetComponent<UnityEngine.UI.Button>();
                if (button != null)
                {
                    button.onClick.AddListener(OnFurBallButtonClicked);
                }
                else
                {
                    // 如果没有 Button 组件，添加点击检测
                    AddClickDetector(furBallButton, OnFurBallButtonClicked);
                }
            }

            // 纸巾筒道具按钮
            if (tissueBoxButton != null)
            {
                var button = tissueBoxButton.GetComponent<UnityEngine.UI.Button>();
                if (button != null)
                {
                    button.onClick.AddListener(OnTissueBoxButtonClicked);
                }
                else
                {
                    AddClickDetector(tissueBoxButton, OnTissueBoxButtonClicked);
                }
            }
        }

        /// <summary>
        /// 为 GameObject 添加点击检测器
        /// </summary>
        private void AddClickDetector(GameObject obj, UnityEngine.Events.UnityAction onClick)
        {
            // 添加 Collider 组件（如果还没有）
            if (obj.GetComponent<Collider2D>() == null && obj.GetComponent<Collider>() == null)
            {
                obj.AddComponent<BoxCollider2D>();
            }

            // 添加 Button 组件
            var button = obj.AddComponent<UnityEngine.UI.Button>();
            button.onClick.AddListener(onClick);
        }

        #region 按钮点击事件

        /// <summary>
        /// 毛球道具按钮点击
        /// </summary>
        public void OnFurBallButtonClicked()
        {
            Debug.Log("[IdleUI] 毛球道具按钮被点击");
            gameManager?.StartFurBallGame();
        }

        /// <summary>
        /// 纸巾筒道具按钮点击
        /// </summary>
        public void OnTissueBoxButtonClicked()
        {
            Debug.Log("[IdleUI] 纸巾筒道具按钮被点击");
            gameManager?.StartTissueGame();
        }

        #endregion
    }
}
