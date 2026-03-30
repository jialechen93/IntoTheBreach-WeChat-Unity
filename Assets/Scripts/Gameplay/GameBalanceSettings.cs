using UnityEngine;

/// <summary>
/// 游戏平衡设置 - 集中管理所有数值，方便调优
/// 由游戏设计师维护
/// </summary>
[CreateAssetMenu(fileName = "GameBalance", menuName = "IntoTheBreach/Game Balance Settings")]
public class GameBalanceSettings : ScriptableObject
{
    [Header("玩家设置")]
    public int startingMechCount = 3;
    public int baseMechHealth = 3;
    public int baseMechAttack = 1;
    public int baseMechMoveRange = 3;
    
    [Header("敌人数量曲线 - 随关卡")]
    public AnimationCurve enemyCountPerLevel = new AnimationCurve(
        new Keyframe(1, 3),
        new Keyframe(5, 5),
        new Keyframe(10, 8)
    );
    
    [Header("敌人血量曲线 - 随关卡")]
    public AnimationCurve enemyHealthPerLevel = new AnimationCurve(
        new Keyframe(1, 2),
        new Keyframe(5, 3),
        new Keyframe(10, 4)
    );
    
    [Header("建筑设置")]
    public int buildingHealth = 3;
    public int buildingsPerLevelBase = 2;
    public int buildingsPerLevelIncrement = 1;
    
    [Header("升级奖励")]
    public int upgradePointsPerLevel = 1;
    public int extraUpgradePointsEveryNLevels = 3;
    
    [Header("得分设置")]
    public int scorePerEnemyKill = 100;
    public int scorePerBuildingAlive = 50;
    public int scoreBonusFullHealth = 20;
    
    [Header("微信小游戏设置")]
    public bool enableAdvertOnGameOver = true;
    public int revivePerGame = 1; // 允许看广告复活一次
    
    /// <summary>
    /// 获取这关敌人数量
    /// </summary>
    public int GetEnemyCount(int level)
    {
        return Mathf.RoundToInt(enemyCountPerLevel.Evaluate(level));
    }
    
    /// <summary>
    /// 获取这关敌人基础血量
    /// </summary>
    public int GetEnemyBaseHealth(int level)
    {
        return Mathf.Max(1, Mathf.RoundToInt(enemyHealthPerLevel.Evaluate(level)));
    }
    
    /// <summary>
    /// 获取这关建筑数量
    /// </summary>
    public int GetBuildingCount(int level)
    {
        return buildingsPerLevelBase + (level - 1) / buildingsPerLevelIncrement;
    }
    
    /// <summary>
    /// 获取这关奖励升级点数
    /// </summary>
    public int GetUpgradePoints(int level)
    {
        int points = upgradePointsPerLevel;
        if (level % extraUpgradePointsEveryNLevels == 0)
        {
            points += 1;
        }
        return points;
    }
}
