using UnityEngine;

/// <summary>
/// 音频管理器 - 音效和背景音乐
/// 适配微信小游戏，支持音频开关控制
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    
    [Header("音频源")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;
    
    [Header("音量")]
    public float bgmVolume = 0.5f;
    public float sfxVolume = 0.8f;
    public bool musicEnabled = true;
    public bool soundEnabled = true;
    
    [Header("音效列表")]
    public AudioClip moveSound;
    public AudioClip attackSound;
    public AudioClip hitSound;
    public AudioClip destroySound;
    public AudioClip buttonClickSound;
    public AudioClip victorySound;
    public AudioClip defeatSound;
    public AudioClip upgradeSound;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadSettings();
            ApplyVolumeSettings();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 加载保存的音量设置
    /// </summary>
    void LoadSettings()
    {
        musicEnabled = PlayerPrefs.GetInt("MusicEnabled", 1) == 1;
        soundEnabled = PlayerPrefs.GetInt("SoundEnabled", 1) == 1;
        bgmVolume = PlayerPrefs.GetFloat("BGMVolume", 0.5f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0.8f);
    }

    /// <summary>
    /// 应用音量设置
    /// </summary>
    void ApplyVolumeSettings()
    {
        if (bgmSource != null)
        {
            bgmSource.volume = musicEnabled ? bgmVolume : 0f;
        }
        if (sfxSource != null)
        {
            sfxSource.volume = soundEnabled ? sfxVolume : 0f;
        }
    }

    /// <summary>
    /// 保存设置
    /// </summary>
    void SaveSettings()
    {
        PlayerPrefs.SetInt("MusicEnabled", musicEnabled ? 1 : 0);
        PlayerPrefs.SetInt("SoundEnabled", soundEnabled ? 1 : 0);
        PlayerPrefs.SetFloat("BGMVolume", bgmVolume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// 播放背景音乐
    /// </summary>
    public void PlayBGM(AudioClip clip)
    {
        if (!musicEnabled || bgmSource == null) return;
        
        bgmSource.clip = clip;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    /// <summary>
    /// 播放移动音效
    /// </summary>
    public void PlayMove()
    {
        PlaySFX(moveSound);
    }

    /// <summary>
    /// 播放攻击音效
    /// </summary>
    public void PlayAttack()
    {
        PlaySFX(attackSound);
    }

    /// <summary>
    /// 播放击中音效
    /// </summary>
    public void PlayHit()
    {
        PlaySFX(hitSound);
    }

    /// <summary>
    /// 播放单位摧毁音效
    /// </summary>
    public void PlayDestroy()
    {
        PlaySFX(destroySound);
    }

    /// <summary>
    /// 播放按钮点击
    /// </summary>
    public void PlayButtonClick()
    {
        PlaySFX(buttonClickSound);
    }

    /// <summary>
    /// 播放胜利
    /// </summary>
    public void PlayVictory()
    {
        PlaySFX(victorySound);
    }

    /// <summary>
    /// 播放失败
    /// </summary>
    public void PlayDefeat()
    {
        PlaySFX(defeatSound);
    }

    /// <summary>
    /// 播放升级
    /// </summary>
    public void PlayUpgrade()
    {
        PlaySFX(upgradeSound);
    }

    /// <summary>
    /// 通用播放音效
    /// </summary>
    void PlaySFX(AudioClip clip)
    {
        if (!soundEnabled || sfxSource == null || clip == null) return;
        
        sfxSource.PlayOneShot(clip);
    }

    /// <summary>
    /// 切换音乐开关
    /// </summary>
    public void ToggleMusic()
    {
        musicEnabled = !musicEnabled;
        ApplyVolumeSettings();
        SaveSettings();
    }

    /// <summary>
    /// 切换音效开关
    /// </summary>
    public void ToggleSound()
    {
        soundEnabled = !soundEnabled;
        ApplyVolumeSettings();
        SaveSettings();
    }

    /// <summary>
    /// 设置BGM音量
    /// </summary>
    public void SetBGMVolume(float volume)
    {
        bgmVolume = volume;
        ApplyVolumeSettings();
        SaveSettings();
    }

    /// <summary>
    /// 设置SFX音量
    /// </summary>
    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        ApplyVolumeSettings();
        SaveSettings();
    }

    /// <summary>
    /// 暂停/继续背景音乐（微信进入后台）
    /// </summary>
    public void PauseBGM()
    {
        if (bgmSource != null && bgmSource.isPlaying)
            bgmSource.Pause();
    }

    public void ResumeBGM()
    {
        if (bgmSource != null && musicEnabled)
            bgmSource.Play();
    }
}
