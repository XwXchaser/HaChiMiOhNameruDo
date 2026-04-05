using UnityEngine;
using HaChiMiOhNameruDo.Managers;

namespace HaChiMiOhNameruDo.MiniGames.Rhythm
{
    /// <summary>
    /// 节奏判定奖励配置
    /// 用于 ScriptableObject 资产，配置不同判定等级的小鱼干奖励
    /// </summary>
    [CreateAssetMenu(fileName = "NewRhythmRewardConfig", menuName = "HaChiMiOhNameruDo/Rhythm/Reward Config")]
    public class RhythmRewardConfig : ScriptableObject
    {
        [Header("小鱼干奖励")]
        [Tooltip("Perfect 判定奖励的小鱼干数量")]
        public int perfectReward = 10;
        
        [Tooltip("Normal 判定奖励的小鱼干数量")]
        public int normalReward = 5;
        
        [Tooltip("Miss 判定奖励的小鱼干数量")]
        public int missReward = 0;
        
        [Header("连击倍率")]
        [Tooltip("Perfect 判定的连击加成")]
        public float perfectComboBonus = 1f;
        
        [Tooltip("Normal 判定的连击加成")]
        public float normalComboBonus = 0.5f;
        
        /// <summary>
        /// 根据判定等级获取奖励
        /// </summary>
        public int GetReward(RhythmJudgment judgment)
        {
            return judgment switch
            {
                RhythmJudgment.Perfect => perfectReward,
                RhythmJudgment.Normal => normalReward,
                _ => missReward
            };
        }
        
        /// <summary>
        /// 根据判定等级获取连击加成
        /// </summary>
        public float GetComboBonus(RhythmJudgment judgment)
        {
            return judgment switch
            {
                RhythmJudgment.Perfect => perfectComboBonus,
                RhythmJudgment.Normal => normalComboBonus,
                _ => 0f
            };
        }
    }
}
