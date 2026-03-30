using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 新手教程面板 - 第一次游戏显示
/// </summary>
public class TutorialPanel : MonoBehaviour
{
    public GameObject tutorialPanel;
    public Button closeButton;
    public Toggle dontShowAgainToggle;
    
    private void Start()
    {
        // 检查是否已经看过教程
        bool showTutorial = PlayerPrefs.GetInt("ShowTutorial", 1) == 1;
        
        if (showTutorial)
        {
            ShowTutorial();
        }
        else
        {
            tutorialPanel.SetActive(false);
        }
        
        closeButton.onClick.AddListener(OnCloseClicked);
    }

    /// <summary>
    /// 显示教程
    /// </summary>
    void ShowTutorial()
    {
        tutorialPanel.SetActive(true);
        Time.timeScale = 0;
    }

    /// <summary>
    /// 关闭按钮点击
    /// </summary>
    void OnCloseClicked()
    {
        // 如果勾选了不再显示，保存设置
        if (dontShowAgainToggle.isOn)
        {
            PlayerPrefs.SetInt("ShowTutorial", 0);
            PlayerPrefs.Save();
        }
        
        tutorialPanel.SetActive(false);
        Time.timeScale = 1;
    }

    /// <summary>
    /// 强制显示教程
    /// </summary>
    public void ForceShow()
    {
        ShowTutorial();
    }
}
