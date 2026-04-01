using UnityEngine;

namespace HaChiMiOhNameruDo.Managers
{
    /// <summary>
    /// UI 管理器 - 单例模式
    /// 负责管理所有 UI 界面的显示和隐藏
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("UI 面板引用")]
        [SerializeField] private GameObject idleUIPanel;      // IDLE 状态 UI（道具按钮等）
        [SerializeField] private GameObject furBallGameUIPanel; // 毛球小游戏 UI
        [SerializeField] private GameObject tissueGameUIPanel;  // 纸巾筒小游戏 UI
        [SerializeField] private GameObject commonUIPanel;      // 通用 UI（返回按钮、计时器等）

        [Header("计时器文本")]
        [SerializeField] private UnityEngine.UI.Text timerText;

        private void Awake()
        {
            // 单例模式
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
            // 初始化 UI 状态
            HideAllUI();
            ShowIdleUI();
        }

        /// <summary>
        /// 隐藏所有 UI
        /// </summary>
        public void HideAllUI()
        {
            HideIdleUI();
            HideFurBallGameUI();
            HideTissueGameUI();
            HideCommonUI();
        }

        #region IDLE UI

        public void ShowIdleUI()
        {
            if (idleUIPanel != null)
                idleUIPanel.SetActive(true);
        }

        public void HideIdleUI()
        {
            if (idleUIPanel != null)
                idleUIPanel.SetActive(false);
        }

        #endregion

        #region 毛球小游戏 UI

        public void ShowFurBallGameUI()
        {
            if (furBallGameUIPanel != null)
                furBallGameUIPanel.SetActive(true);
            ShowCommonUI();
        }

        public void HideFurBallGameUI()
        {
            if (furBallGameUIPanel != null)
                furBallGameUIPanel.SetActive(false);
            HideCommonUI();
        }

        #endregion

        #region 纸巾筒小游戏 UI

        public void ShowTissueGameUI()
        {
            if (tissueGameUIPanel != null)
                tissueGameUIPanel.SetActive(true);
            ShowCommonUI();
        }

        public void HideTissueGameUI()
        {
            if (tissueGameUIPanel != null)
                tissueGameUIPanel.SetActive(false);
            HideCommonUI();
        }

        #endregion

        #region 通用 UI

        public void ShowCommonUI()
        {
            if (commonUIPanel != null)
                commonUIPanel.SetActive(true);
        }

        public void HideCommonUI()
        {
            if (commonUIPanel != null)
                commonUIPanel.SetActive(false);
        }

        #endregion

        #region 计时器

        /// <summary>
        /// 更新计时器显示
        /// </summary>
        public void UpdateTimer(float remainingTime)
        {
            if (timerText != null)
            {
                int minutes = Mathf.FloorToInt(remainingTime / 60);
                int seconds = Mathf.FloorToInt(remainingTime % 60);
                timerText.text = $"{minutes:00}:{seconds:00}";
            }
        }

        #endregion

        /// <summary>
        /// 返回按钮点击事件
        /// </summary>
        public void OnReturnButtonClicked()
        {
            GameManager.Instance?.ReturnToIdle();
        }
    }
}
