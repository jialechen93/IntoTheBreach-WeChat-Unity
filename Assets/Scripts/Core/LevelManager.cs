using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

/// <summary>
/// 关卡管理器 - 管理多关卡、难度、进度保存
/// </summary>
public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    
    [Header("当前关卡")]
    public int currentLevel = 1;
    public int maxLevel = 10;
    public LevelData currentLevelData;
    
    [Header("难度设置")]
    public AnimationCurve enemyCountCurve; // 敌人数随关卡增加
    public AnimationCurve enemyHealthCurve; // 敌人血量随关卡增加
    
    [Header("UI")]
    public Text levelText;
    public Text progressText;
    
    // 所有可用的岛屿/关卡
    private List<LevelData> availableLevels = new List<LevelData>();
    
    // 玩家升级点数
    public int totalUpgradePoints = 0;
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // 加载保存的进度
        LoadProgress();
        GenerateRandomLevel();
        UpdateLevelUI();
    }

    /// <summary>
    /// 生成随机关卡
    /// </summary>
    public void GenerateRandomLevel()
    {
        currentLevelData = new LevelData();
        currentLevelData.levelIndex = currentLevel;
        
        // 根据关卡计算敌人数量
        int enemyCount = Mathf.RoundToInt(enemyCountCurve.Evaluate(currentLevel));
        int buildingCount = 2 + currentLevel / 3; // 建筑数量随关卡增加
        
        currentLevelData.enemyCount = enemyCount;
        currentLevelData.buildingCount = buildingCount;
        
        // 生成随机地图
        MapGenerator generator = FindObjectOfType<MapGenerator>();
        if (generator != null)
        {
            generator.GenerateRandomMap(enemyCount, buildingCount);
        }
    }

    /// <summary>
    /// 完成当前关卡
    /// </summary>
    public void CompleteLevel()
    {
        // 奖励升级点数
        int points = 1 + (currentLevel % 3 == 0 ? 1 : 0); // 每3关多给1点
        totalUpgradePoints += points;
        
        // 保存进度
        if (currentLevel >= PlayerPrefs.GetInt("MaxLevelCleared", 1))
        {
            PlayerPrefs.SetInt("MaxLevelCleared", currentLevel + 1);
            PlayerPrefs.Save();
        }
        
        // 显示升级界面
        UpgradeUIManager.Instance.ShowUpgradeScreen(points);
    }

    /// <summary>
    /// 进入下一关
    /// </summary>
    public void NextLevel()
    {
        currentLevel++;
        if (currentLevel > maxLevel)
        {
            // 全部通关
            GameUIManager.Instance.ShowGameOver(true);
            Debug.Log("Game Complete! All levels cleared!");
            return;
        }
        
        GenerateRandomLevel();
        UpdateLevelUI();
        
        // 重新开始回合
        TurnManager turnManager = FindObjectOfType<TurnManager>();
        turnManager.CollectPlayerMechs();
        turnManager.CollectEnemies();
        turnManager.StartPlayerTurn();
    }

    /// <summary>
    /// 加载保存的进度
    /// </summary>
    void LoadProgress()
    {
        int savedLevel = PlayerPrefs.GetInt("MaxLevelCleared", 1);
        currentLevel = savedLevel;
        totalUpgradePoints = PlayerPrefs.GetInt("UpgradePoints", 0);
    }

    /// <summary>
    /// 保存升级点数
    /// </summary>
    public void SaveUpgradePoints()
    {
        PlayerPrefs.SetInt("UpgradePoints", totalUpgradePoints);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// 更新关卡UI
    /// </summary>
    void UpdateLevelUI()
    {
        if (levelText != null)
        {
            levelText.text = $"第 {currentLevel} 关";
        }
        
        if (progressText != null)
        {
            progressText.text = $"{currentLevel}/{maxLevel}";
        }
    }

    /// <summary>
    /// 使用一个升级点数
    /// </summary>
    public bool UseUpgradePoint()
    {
        if (totalUpgradePoints <= 0) return false;
        
        totalUpgradePoints--;
        SaveUpgradePoints();
        UpgradeUIManager.Instance.UpdateUpgradePointsDisplay();
        return true;
    }

    /// <summary>
    /// 获取当前敌人基础血量
    /// </summary>
    public int GetEnemyBaseHealth()
    {
        return Mathf.RoundToInt(enemyHealthCurve.Evaluate(currentLevel));
    }
}

/// <summary>
/// 关卡数据
/// </summary>
[System.Serializable]
public class LevelData
{
    public int levelIndex;
    public int enemyCount;
    public int buildingCount;
}
