using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 暂停菜单 - 微信小游戏必备
/// </summary>
public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Instance;
    
    [Header("面板")]
    public GameObject pausePanel;
    public bool isPaused = false;
    
    [Header("按钮事件绑定")]
    public UnityEngine.Events.UnityEvent OnResume;
    public UnityEngine.Events.UnityEvent OnRestart;
    public UnityEngine.Events.UnityEvent OnQuit;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        pausePanel.SetActive(false);
    }

    private void Update()
    {
        // 安卓返回键处理
        if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.Escape))
        {
            TogglePause();
        }
    }

    /// <summary>
    /// 切换暂停状态
    /// </summary>
    public void TogglePause()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    /// <summary>
    /// 暂停游戏
    /// </summary>
    public void PauseGame()
    {
        isPaused = true;
        pausePanel.SetActive(true);
        Time.timeScale = 0;
        AudioManager.Instance?.PauseBGM();
    }

    /// <summary>
    /// 继续游戏
    /// </summary>
    public void ResumeGame()
    {
        isPaused = false;
        pausePanel.SetActive(false);
        Time.timeScale = 1;
        AudioManager.Instance?.ResumeBGM();
        OnResume?.Invoke();
    }

    /// <summary>
    /// 重新开始当前关卡
    /// </summary>
    public void RestartCurrentLevel()
    {
        Time.timeScale = 1;
        isPaused = false;
        OnRestart?.Invoke();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// 返回主菜单（如果有）
    /// </summary>
    public void QuitToMenu()
    {
        Time.timeScale = 1;
        isPaused = false;
        OnQuit?.Invoke();
        // 如果有主菜单场景，加载它
        // SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// 打开设置面板
    /// </summary>
    public void OpenSettings()
    {
        // 这里可以打开音量设置等
        SettingsPanel.Instance?.ShowSettings();
    }

    /// <summary>
    /// 分享游戏
    /// </summary>
    public void ShareGame()
    {
        GameUIManager.Instance?.ShareToWeChat();
    }
}
