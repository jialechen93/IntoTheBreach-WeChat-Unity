using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 游戏UI管理器
/// </summary>
public class GameUIManager : MonoBehaviour
{
    public static GameUIManager Instance;
    
    [Header("游戏结束面板")]
    public GameObject gameOverPanel;
    public Text gameOverTitleText;
    public Text gameOverDescText;
    public Button restartButton;
    public Button backToMenuButton;
    public Button shareButton;
    
    [Header("微信分享")]
    public string shareTitle = "我在「机甲防线」守住了城市！";
    public string shareDesc = "类似陷阵之志的回合制机甲策略游戏，来挑战一下吧！";

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        gameOverPanel.SetActive(false);
        
        // 绑定按钮事件
        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);
        if (backToMenuButton != null)
            backToMenuButton.onClick.AddListener(BackToMenu);
        if (shareButton != null)
            shareButton.onClick.AddListener(ShareToWeChat);
    }

    /// <summary>
    /// 显示游戏结束面板
    /// </summary>
    public void ShowGameOver(bool isVictory)
    {
        gameOverPanel.SetActive(true);
        
        if (isVictory)
        {
            gameOverTitleText.text = "胜利！";
            gameOverDescText.text = "你成功击退了所有敌人，保护了城市！";
        }
        else
        {
            gameOverTitleText.text = "失败";
            gameOverDescText.text = "城市被敌人攻破了，再试一次吧！";
        }
        
        // 微信小游戏环境显示分享按钮
        #if UNITY_WECHAT_GAME
        shareButton.gameObject.SetActive(true);
        #else
        if (shareButton != null)
            shareButton.gameObject.SetActive(false);
        #endif
    }

    /// <summary>
    /// 重新开始游戏
    /// </summary>
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// 返回主菜单
    /// </summary>
    public void BackToMenu()
    {
        // 如果有主菜单场景，加载它
        // SceneManager.LoadScene("Menu");
        // 这里先做重新开始
        RestartGame();
    }

    /// <summary>
    /// 分享到微信
    /// </summary>
    public void ShareToWeChat()
    {
        #if UNITY_WECHAT_GAME
        WeChatWASM.WX.ShareAppMessage(
            new WeChatWASM.ShareAppMessageOption
            {
                title = shareTitle,
                imageUrl = "",
                query = ""
            }
        );
        #else
        Debug.Log("点击了分享按钮，当前非微信小游戏环境");
        #endif
    }

    /// <summary>
    /// 更新分数显示（可选）
    /// </summary>
    public void UpdateScore(int score)
    {
        // 如果需要显示分数，可以在这里实现
    }
}
