using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 设置面板 - 音量控制等
/// </summary>
public class SettingsPanel : MonoBehaviour
{
    public static SettingsPanel Instance;
    
    [Header"面板")]
    public GameObject settingsPanel;
    public Slider bgmVolumeSlider;
    public Slider sfxVolumeSlider;
    public Toggle musicToggle;
    public Toggle soundToggle;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        settingsPanel.SetActive(false);
        
        // 初始化滑块
        if (AudioManager.Instance != null)
        {
            if (bgmVolumeSlider != null)
            {
                bgmVolumeSlider.value = AudioManager.Instance.bgmVolume;
                bgmVolumeSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
            }
            
            if (sfxVolumeSlider != null)
            {
                sfxVolumeSlider.value = AudioManager.Instance.sfxVolume;
                sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
            }
            
            if (musicToggle != null)
            {
                musicToggle.isOn = AudioManager.Instance.musicEnabled;
                musicToggle.onValueChanged.AddListener(OnMusicToggleChanged);
            }
            
            if (soundToggle != null)
            {
                soundToggle.isOn = AudioManager.Instance.soundEnabled;
                soundToggle.onValueChanged.AddListener(OnSoundToggleChanged);
            }
        }
    }

    /// <summary>
    /// 显示设置面板
    /// </summary>
    public void ShowSettings()
    {
        settingsPanel.SetActive(true);
    }

    /// <summary>
    /// 隐藏设置面板
    /// </summary>
    public void HideSettings()
    {
        settingsPanel.SetActive(false);
    }

    /// <summary>
    /// BGM音量改变
    /// </summary>
    void OnBGMVolumeChanged(float value)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetBGMVolume(value);
        }
    }

    /// <summary>
    /// SFX音量改变
    /// </summary>
    void OnSFXVolumeChanged(float value)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetSFXVolume(value);
        }
    }

    /// <summary>
    /// 音乐开关改变
    /// </summary>
    void OnMusicToggleChanged(bool enabled)
    {
        if (AudioManager.Instance != null)
        {
            if (enabled != AudioManager.Instance.musicEnabled)
            {
                AudioManager.Instance.ToggleMusic();
            }
        }
    }

    /// <summary>
    /// 音效开关改变
    /// </summary>
    void OnSoundToggleChanged(bool enabled)
    {
        if (AudioManager.Instance != null)
        {
            if (enabled != AudioManager.Instance.soundEnabled)
            {
                AudioManager.Instance.ToggleSound();
            }
        }
    }

    /// <summary>
    /// 清除所有存档
    /// </summary>
    public void ClearAllSaveData()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        
        Debug.Log("所有存档已清除");
    }
}
