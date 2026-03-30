using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 分数管理器 - 计算关卡得分，统计表现
/// </summary>
public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    
    [Header("当前分数")]
    public int currentScore;
    public int totalScore;
    
    [Header("UI")]
    public Text scoreText;
    public Text totalScoreText;
    
    [Header("得分设置")]
    public int scorePerEnemyKill = 100;
    public int scorePerBuildingAlive = 50;
    public int bonusFullHealth = 20;
    public int bonusRemainingTurns = 10;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        LoadTotalScore();
        NewLevel();
    }

    /// <summary>
    /// 新关卡开始
    /// </summary>
    public void NewLevel()
    {
        currentScore = 0;
        UpdateScoreUI();
    }

    /// <summary>
    /// 击杀敌人加分
    /// </summary>
    public void AddKillScore()
    {
        currentScore += scorePerEnemyKill;
        UpdateScoreUI();
    }

    /// <summary>
    /// 关卡结束计算最终得分
    /// </summary>
    public int CalculateFinalScore(int enemyKilled, int buildingsAlive, int mechsAlive, int maxTurns)
    {
        int finalScore = enemyKilled * scorePerEnemyKill;
        finalScore += buildingsAlive * scorePerBuildingAlive;
        
        // 机甲满血奖励
        foreach (PlayerMech mech in FindObjectOfType<TurnManager>().playerMechs)
        {
            if (mech.IsAlive() && mech.currentHealth == mech.maxHealth)
            {
                finalScore += bonusFullHealth;
            }
        }
        
        totalScore += finalScore;
        SaveTotalScore();
        UpdateScoreUI();
        
        return finalScore;
    }

    /// <summary>
    /// 获取评价星级
    /// </summary>
    public int GetStarRating(int totalPossible, int actual)
    {
        float ratio = (float)actual / totalPossible;
        
        if (ratio >= 0.9f) return 3;
        if (ratio >= 0.6f) return 2;
        if (ratio >= 0.3f) return 1;
        return 0;
    }

    /// <summary>
    /// 更新UI
    /// </summary>
    void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = $"得分: {currentScore}";
        
        if (totalScoreText != null)
            totalScoreText.text = $"总分: {totalScore}";
    }

    /// <summary>
    /// 保存总分
    /// </summary>
    void SaveTotalScore()
    {
        PlayerPrefs.SetInt("TotalScore", totalScore);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// 加载总分
    /// </summary>
    void LoadTotalScore()
    {
        totalScore = PlayerPrefs.GetInt("TotalScore", 0);
    }

    /// <summary>
    /// 重置分数
    /// </summary>
    public void ResetAllScores()
    {
        totalScore = 0;
        currentScore = 0;
        SaveTotalScore();
        UpdateScoreUI();
    }
}
