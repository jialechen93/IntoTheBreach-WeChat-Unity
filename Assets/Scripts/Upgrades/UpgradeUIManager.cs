using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// 升级界面管理器 - 显示升级选项，处理玩家选择
/// </summary>
public class UpgradeUIManager : MonoBehaviour
{
    public static UpgradeUIManager Instance;
    
    [Header("升级面板")]
    public GameObject upgradePanel;
    public Button[] upgradeButtons;
    public Text[] upgradeNameTexts;
    public Text[] upgradeDescTexts;
    public Text pointsText;
    
    [Header("信息")]
    public Text availablePointsText;
    
    private List<UpgradeOption> currentOptions;
    private int pendingPoints;
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        upgradePanel.SetActive(false);
    }

    private void Start()
    {
        UpdateUpgradePointsDisplay();
    }

    /// <summary>
    /// 显示升级界面
    /// </summary>
    public void ShowUpgradeScreen(int pointsAwarded)
    {
        pendingPoints = pointsAwarded;
        
        // 如果没有点数了，直接进下一关
        if (LevelManager.Instance.totalUpgradePoints <= 0)
        {
            LevelManager.Instance.NextLevel();
            return;
        }
        
        // 获取随机选项
        currentOptions = UpgradeManager.Instance.GetRandomUpgrades(3);
        
        // 更新按钮
        for (int i = 0; i < upgradeButtons.Length; i++)
        {
            if (i < currentOptions.Count)
            {
                upgradeNameTexts[i].text = currentOptions[i].name;
                upgradeDescTexts[i].text = $"{currentOptions[i].description}\n消耗: {currentOptions[i].cost} 点";
                upgradeButtons[i].gameObject.SetActive(true);
                
                // 绑定点击事件
                int index = i;
                upgradeButtons[i].onClick.RemoveAllListeners();
                upgradeButtons[i].onClick.AddListener(() => OnUpgradeSelected(index));
                
                // 检查是否够点数
                upgradeButtons[i].interactable = LevelManager.Instance.totalUpgradePoints >= currentOptions[i].cost;
            }
            else
            {
                upgradeButtons[i].gameObject.SetActive(false);
            }
        }
        
        UpdateUpgradePointsDisplay();
        upgradePanel.SetActive(true);
        Time.timeScale = 0; // 暂停游戏
    }

    /// <summary>
    /// 玩家选择了一个升级
    /// </summary>
    void OnUpgradeSelected(int optionIndex)
    {
        UpgradeOption option = currentOptions[optionIndex];
        
        // 检查点数够不够
        if (LevelManager.Instance.totalUpgradePoints < option.cost)
        {
            return;
        }
        
        // 扣除点数
        for (int i = 0; i < option.cost; i++)
        {
            if (!LevelManager.Instance.UseUpgradePoint())
                break;
        }
        
        // 应用升级（给哪个机甲？这里简化，玩家选择一个机甲后应用）
        // 简化版本：直接让玩家选完就生效到对应机甲，这里弹出选择机甲窗口
        // 实际中可以做一个选择机甲的界面，这里先简化为直接选择后继续
        
        // 这里我们默认选择一个机甲，或者可以扩展让玩家选
        // 简化处理：默认给第一个机甲，实际游戏可以加UI选择
        UpgradeManager.Instance.ApplyUpgrade(0, option.type);
        UpgradeManager.Instance.SaveUpgrades();
        
        UpdateUpgradePointsDisplay();
        
        // 如果还有点数，继续选，否则关闭
        if (LevelManager.Instance.totalUpgradePoints <= 0)
        {
            CloseUpgradeScreen();
            LevelManager.Instance.NextLevel();
        }
        else
        {
            // 刷新选项
            ShowUpgradeScreen(0);
        }
    }

    /// <summary>
    /// 跳过升级（不升级，直接保留点数以后用）
    /// </summary>
    public void OnSkipClicked()
    {
        CloseUpgradeScreen();
        LevelManager.Instance.NextLevel();
    }

    /// <summary>
    /// 关闭升级界面
    /// </summary>
    void CloseUpgradeScreen()
    {
        upgradePanel.SetActive(false);
        Time.timeScale = 1;
    }

    /// <summary>
    /// 更新可用升级点数显示
    /// </summary>
    public void UpdateUpgradePointsDisplay()
    {
        if (availablePointsText != null && LevelManager.Instance != null)
        {
            availablePointsText.text = $"可用点数: {LevelManager.Instance.totalUpgradePoints}";
        }
    }

    /// <summary>
    /// 选择机甲界面（完整版需要这个）
    /// 这里预留接口，后续扩展
    /// </summary>
    public void ShowMechSelection(UpgradeOption option)
    {
        // TODO: 显示三个机甲选择按钮
        // 玩家选择哪个机甲应用这个升级
    }
}
