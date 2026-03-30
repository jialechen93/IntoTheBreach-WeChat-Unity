using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 复活系统 - 看广告复活，微信小游戏变现
/// </summary>
public class ReviveSystem : MonoBehaviour
{
    public static ReviveSystem Instance;
    
    [Header("设置")]
    public int maxRevivesPerGame = 1;
    public int currentRevivesUsed = 0;
    
    [Header("UI")]
    public GameObject revivePanel;
    public Button reviveButton;
    public Button skipReviveButton;
    public Text reviveText;
    
    private bool isGameOverPending = false;
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        if (revivePanel != null)
            revivePanel.SetActive(false);
        
        if (reviveButton != null)
            reviveButton.onClick.AddListener(OnReviveClicked);
        
        if (skipReviveButton != null)
            skipReviveButton.onClick.AddListener(OnSkipReviveClicked);
    }

    private void Start()
    {
        ResetRevives();
    }

    /// <summary>
    /// 游戏失败，询问是否复活
    /// </summary>
    public void AskForRevive()
    {
        if (currentRevivesUsed >= maxRevivesPerGame)
        {
            // 用完了复活次数，直接游戏结束
            ShowGameOver();
            return;
        }
        
        // 显示复活面板
        isGameOverPending = true;
        revivePanel.SetActive(true);
        Time.timeScale = 0;
        
        int remaining = maxRevivesPerGame - currentRevivesUsed;
        reviveText.text = $"游戏失败！还可以看广告复活 {remaining} 次\n是否复活继续？";
    }

    /// <summary>
    /// 玩家点击复活
    /// </summary>
    void OnReviveClicked()
    {
        // 播放激励广告
        WeChatAdManager adManager = FindObjectOfType<WeChatAdManager>();
        if (adManager != null)
        {
            adManager.ShowRewardedVideoAd();
        }
        
        // 广告看完会回调这里
        OnReviveGranted();
    }

    /// <summary>
    /// 复活授予
    /// </summary>
    public void OnReviveGranted()
    {
        currentRevivesUsed++;
        revivePanel.SetActive(false);
        Time.timeScale = 1;
        isGameOverPending = false;
        
        // 这里可以恢复一个机甲或者满血恢复
        ReviveOneMech();
    }

    /// <summary>
    /// 复活一个机甲（满血恢复第一个死亡的机甲）
    /// </summary>
    void ReviveOneMech()
    {
        TurnManager turnManager = FindObjectOfType<TurnManager>();
        if (turnManager == null) return;
        
        // 找到第一个死亡的机甲复活它
        foreach (PlayerMech mech in turnManager.playerMechs)
        {
            if (!mech.IsAlive())
            {
                // 这里简单复活，实际你可能需要实例化
                mech.currentHealth = mech.maxHealth;
                break;
            }
        }
        
        // 继续游戏
        GameManager.Instance.gameState = GameManager.GameState.Playing;
        Time.timeScale = 1;
    }

    /// <summary>
    /// 玩家跳过复活
    /// </summary>
    void OnSkipReviveClicked()
    {
        revivePanel.SetActive(false);
        isGameOverPending = false;
        Time.timeScale = 1;
        ShowGameOver();
    }

    /// <summary>
    /// 显示最终游戏结束
    /// </summary>
    void ShowGameOver()
    {
        isGameOverPending = false;
        GameUIManager.Instance.ShowGameOver(false);
        
        // 播放插屏广告
        WeChatAdManager adManager = FindObjectOfType<WeChatAdManager>();
        if (adManager != null && adManager.enableAdvertOnGameOver)
        {
            adManager.ShowInterstitialAd();
        }
    }

    /// <summary>
    /// 重置复活次数（新游戏）
    /// </summary>
    public void ResetRevives()
    {
        currentRevivesUsed = 0;
        if (revivePanel != null)
            revivePanel.SetActive(false);
        isGameOverPending = false;
    }

    /// <summary>
    /// 是否可以复活
    /// </summary>
    public bool CanRevive()
    {
        return currentRevivesUsed < maxRevivesPerGame;
    }
}
