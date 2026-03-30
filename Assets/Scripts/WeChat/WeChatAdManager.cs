using UnityEngine;

#if UNITY_WECHAT_GAME
using WeChatWASM;
#endif

/// <summary>
/// 微信小游戏广告管理
/// 可以增加游戏变现功能
/// </summary>
public class WeChatAdManager : MonoBehaviour
{
    public static WeChatAdManager Instance;
    
    [Header("广告ID")]
    public string bannerAdUnitId;
    public string interstitialAdUnitId;
    public string videoAdUnitId;
    
    [Header("当前广告状态")]
    public bool bannerAdLoaded = false;
    public bool interstitialAdLoaded = false;
    public bool videoAdLoaded = false;
    
    private BannerAd bannerAd;
    private InterstitialAd interstitialAd;
    private RewardedVideoAd rewardedVideoAd;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        #if UNITY_WECHAT_GAME
        LoadBannerAd();
        LoadInterstitialAd();
        LoadRewardedVideoAd();
        #endif
    }

    #region 横幅广告
    void LoadBannerAd()
    {
        #if UNITY_WECHAT_GAME && !UNITY_EDITOR
        if (string.IsNullOrEmpty(bannerAdUnitId)) return;
        
        bannerAd = WX.CreateBannerAd(new CreateBannerAdOption
        {
            adUnitId = bannerAdUnitId,
            style = new BannerAdStyle
            {
                left = 0,
                top = 0,
                width = Screen.width,
                height = 100
            }
        });
        
        bannerAd.OnLoad(() =>
        {
            bannerAdLoaded = true;
            bannerAd.Show();
        });
        
        bannerAd.OnError((res) =>
        {
            bannerAdLoaded = false;
            Debug.LogError("横幅广告加载失败: " + res.errMsg);
        });
        #endif
    }

    public void ShowBannerAd()
    {
        #if UNITY_WECHAT_GAME
        if (bannerAdLoaded && bannerAd != null)
        {
            bannerAd.Show();
        }
        #endif
    }

    public void HideBannerAd()
    {
        #if UNITY_WECHAT_GAME
        if (bannerAd != null)
        {
            bannerAd.Hide();
        }
        #endif
    }
    #endregion

    #region 插屏广告
    void LoadInterstitialAd()
    {
        #if UNITY_WECHAT_GAME
        if (string.IsNullOrEmpty(interstitialAdUnitId)) return;
        
        interstitialAd = WX.CreateInterstitialAd(new CreateInterstitialAdOption
        {
            adUnitId = interstitialAdUnitId
        });
        
        interstitialAd.OnLoad(() =>
        {
            interstitialAdLoaded = true;
        });
        
        interstitialAd.OnError((res) =>
        {
            interstitialAdLoaded = false;
        });
        
        interstitialAd.OnClose(() =>
        {
            // 关闭后重新加载
            interstitialAdLoaded = false;
            LoadInterstitialAd();
        });
        #endif
    }

    public void ShowInterstitialAd()
    {
        #if UNITY_WECHAT_GAME
        if (interstitialAdLoaded && interstitialAd != null)
        {
            interstitialAd.Show();
        }
        else
        {
            LoadInterstitialAd();
        }
        #endif
    }
    #endregion

    #region 激励视频广告
    void LoadRewardedVideoAd()
    {
        #if UNITY_WECHAT_GAME
        if (string.IsNullOrEmpty(videoAdUnitId)) return;
        
        rewardedVideoAd = WX.CreateRewardedVideoAd(new CreateRewardedVideoAdOption
        {
            adUnitId = videoAdUnitId
        });
        
        rewardedVideoAd.OnLoad(() =>
        {
            videoAdLoaded = true;
        });
        
        rewardedVideoAd.OnError((res) =>
        {
            videoAdLoaded = false;
        });
        
        rewardedVideoAd.OnClose((res) =>
        {
            // 检查是否看完视频，给奖励
            if (res.isEnded)
            {
                GiveRewarded();
            }
            // 重新加载
            videoAdLoaded = false;
            LoadRewardedVideoAd();
        });
        #endif
    }

    public void ShowRewardedVideoAd()
    {
        #if UNITY_WECHAT_GAME
        if (videoAdLoaded && rewardedVideoAd != null)
        {
            rewardedVideoAd.Show();
        }
        else
        {
            LoadRewardedVideoAd();
        }
        #endif
    }

    /// <summary>
    /// 看完视频给奖励
    /// </summary>
    void GiveRewarded()
    {
        // 这里根据你的游戏逻辑给奖励，比如额外生命、重新开始等
        Debug.Log("激励视频播放完成，发放奖励");
        // 可以发送事件，让游戏处理
        SendMessage("OnRewardedVideoCompleted", SendMessageOptions.DontRequireReceiver);
    }
    #endregion

    private void OnDestroy()
    {
        #if UNITY_WECHAT_GAME
        if (bannerAd != null) bannerAd.Destroy();
        if (interstitialAd != null) interstitialAd.Destroy();
        if (rewardedVideoAd != null) rewardedVideoAd.Destroy();
        #endif
    }
}
