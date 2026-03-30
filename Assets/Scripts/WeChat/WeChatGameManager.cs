using UnityEngine;

#if UNITY_WECHAT_GAME
using WeChatWASM;
#endif

/// <summary>
/// 微信小游戏管理器
/// 处理微信登录、分享、广告等功能
/// </summary>
public class WeChatGameManager : MonoBehaviour
{
    public static WeChatGameManager Instance;
    
    [Header("微信设置")]
    public bool autoLogin = true;
    public bool enableShare = true;
    
    [Header("用户信息")]
    public string openId;
    public string nickName;
    public bool isLoggedIn = false;

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
        // 适配屏幕
        AdaptScreen();
        
        #if UNITY_WECHAT_GAME
        if (autoLogin)
        {
            WeChatLogin();
        }
        // 设置分享回调
        if (enableShare)
        {
            WX.OnShareAppMessage((res) =>
            {
                var shareMsg = new ShareAppMessageResponse
                {
                    title = "机甲防线 - 类似陷阵之志的回合策略游戏",
                    imageUrl = ""
                };
                return shareMsg;
            });
        }
        #endif
    }

    /// <summary>
    /// 适配微信小游戏屏幕
    /// </summary>
    void AdaptScreen()
    {
        #if UNITY_WECHAT_GAME
        var info = WX.GetSystemInfoSync();
        float designWidth = 1080;
        float designHeight = 1920;
        
        // 计算适配比例，让刘海屏、状态栏不影响游戏显示
        float safeTop = info.statusBarHeight;
        float screenWidth = info.windowWidth;
        float screenHeight = info.windowHeight;
        
        // 通知相机自适应
        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            InputManager input = FindObjectOfType<InputManager>();
            if (input != null)
            {
                input.AdaptToScreen();
            }
        }
        #endif
    }

    /// <summary>
    /// 微信登录
    /// </summary>
    void WeChatLogin()
    {
        #if UNITY_WECHAT_GAME
        WX.Login(new LoginOption
        {
            success = (res) =>
            {
                Debug.Log("微信登录成功 code: " + res.code);
                openId = res.code;
                isLoggedIn = true;
                
                // 这里可以把 code 发到你的服务器换取 openid
                GetUserInfo();
            },
            fail = (res) =>
            {
                Debug.LogError("微信登录失败: " + JsonUtility.ToJson(res));
            }
        });
        #endif
    }

    /// <summary>
    /// 获取用户信息
    /// </summary>
    void GetUserInfo()
    {
        #if UNITY_WECHAT_GAME
        WX.GetUserInfo(new GetUserInfoOption
        {
            success = (res) =>
            {
                nickName = res.nickName;
                Debug.Log("获取用户信息成功: " + nickName);
            }
        });
        #endif
    }

    /// <summary>
    /// 创建图片分享（截图分享）
    /// </summary>
    public void CaptureAndShare()
    {
        #if UNITY_WECHAT_GAME
        // 先截图
        ScreenCapture.CaptureScreenshot("screenshot.png");
        // 然后分享
        WX.ShareAppMessage(new ShareAppMessageOption
        {
            title = "我在机甲防线打到了第 " + FindObjectOfType<TurnManager>().currentTurn + " 回合",
            imageUrl = "screenshot.png"
        });
        #endif
    }

    /// <summary>
    /// 显示分享到群菜单
    /// </summary>
    public void ShareToTimeline()
    {
        #if UNITY_WECHAT_GAME
        WX.ShareTimeline(new ShareTimelineOption
        {
            title = "机甲防线 - 好玩的回合策略小游戏",
            query = ""
        });
        #endif
    }

    /// <summary>
    /// 微信小游戏进入后台
    /// </summary>
    private void OnApplicationPause(bool pause)
    {
        #if UNITY_WECHAT_GAME
        if (pause)
        {
            // 暂停游戏
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
        #endif
    }
}
