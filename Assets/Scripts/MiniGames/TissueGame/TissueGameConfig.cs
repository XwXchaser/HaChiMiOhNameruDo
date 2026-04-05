using UnityEngine;

namespace HaChiMiOhNameruDo.MiniGames.TissueGame
{
    /// <summary>
    /// 纸巾筒游戏配置
    /// ScriptableObject 用于在 Inspector 中配置游戏参数
    /// </summary>
    [CreateAssetMenu(fileName = "TissueGameConfig", menuName = "TissueGame/Config")]
    public class TissueGameConfig : ScriptableObject
    {
        [Header("扒拉次数设置")]
        [Tooltip("纸巾堆 L2 出现所需的扒拉次数")]
        public int pileL2Threshold = 5;

        [Tooltip("纸巾堆 L1 出现所需的扒拉次数（上限）")]
        public int pileL1Threshold = 10;

        [Header("划动阈值设置")]
        [Tooltip("向下划动触发阈值（像素）")]
        public float pullThreshold = 50f;

        [Tooltip("横向划动触发阈值（像素）")]
        public float swipeThreshold = 50f;

        [Tooltip("清理一层纸巾堆需要的划动次数")]
        public int swipesPerClear = 3;

        [Header("清除动画设置")]
        [Tooltip("清除动画时长（秒）")]
        public float clearDuration = 0.3f;

        [Header("装填动画设置")]
        [Tooltip("清空弹仓动画时长（秒）")]
        public float clearChamberDuration = 0.3f;

        [Tooltip("装填新纸卷动画时长（秒）")]
        public float reloadDuration = 0.5f;

        [Tooltip("纸巾耗尽时纸卷消失时长（秒）")]
        public float disappearDuration = 1f;

        [Header("抽取动画设置")]
        [Tooltip("抽取动画播放速度（秒/帧）")]
        public float pullAnimationSpeed = 0.1f;

        [Header("纸巾延伸设置")]
        [Tooltip("切换到长纸巾所需的扒拉次数")]
        public int longTissueThreshold = 5;

        [Header("得分设置")]
        [Tooltip("理想纸巾长度（扒拉次数）")]
        public int idealTissueLength = 8;
    }
}
