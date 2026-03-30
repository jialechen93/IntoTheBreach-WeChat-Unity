using UnityEngine;

#if UNITY_WECHAT_GAME
using WeChatWASM;
#endif

/// <summary>
/// 微信小游戏性能优化专家
/// 处理包体积、内存、渲染性能
/// </summary>
public class WeChatPerformanceOptimizer : MonoBehaviour
{
    public static WeChatPerformanceOptimizer Instance;
    
    [Header("性能设置")]
    public int targetFrameRate = 60;
    public bool useDynamicBatching = true;
    public bool enableGPUInstancing = true;
    public int maxMeshVertexBytes = 500000;
    
    [Header("内存管理")]
    public bool autoCollectGCOnLevelLoad = true;
    public float gcCollectInterval = 60f; // 60秒自动回收一次
    
    private float lastGCTime;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        ApplyPerformanceSettings();
        
        #if UNITY_WECHAT_GAME
        // 监听微信剪贴板（可选）
        WX.OnMemoryWarning((res) =>
        {
            Debug.Log("微信低内存警告，执行GC");
            ForceGC();
        });
        #endif
    }

    /// <summary>
    /// 应用性能设置
    /// </summary>
    void ApplyPerformanceSettings()
    {
        // 目标帧率
        Application.targetFrameRate = targetFrameRate;
        
        // 降低帧率当在后台
        Application.backgroundLoadingPriority = ThreadPriority.Low;
        
        // 禁用日志发布版本
        #if UNITY_WEBGL && !UNITY_EDITOR
        Debug.unityLogger.logEnabled = false;
        #endif
    }

    private void Update()
    {
        // 自动GC
        if (autoCollectGCOnLevelLoad && Time.time > lastGCTime + gcCollectInterval)
        {
            ForceGC();
            lastGCTime = Time.time;
        }
    }

    /// <summary>
    /// 强制垃圾回收
    /// </summary>
    public void ForceGC()
    {
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
        lastGCTime = Time.time;
    }

    /// <summary>
    /// 关卡加载完成后调用
    /// </summary>
    public void OnLevelLoaded()
    {
        if (autoCollectGCOnLevelLoad)
        {
            ForceGC();
        }
    }

    /// <summary>
    /// 获取微信小游戏环境信息
    /// </summary>
    public void LogWeChatEnvInfo()
    {
        #if UNITY_WECHAT_GAME
        var info = WX.GetSystemInfoSync();
        Debug.Log($"微信环境: 屏幕={info.windowWidth}x{info.windowHeight}, SDKVersion={info.SDKVersion}");
        #endif
    }

    /// <summary>
    /// 预加载建议：对于微信小游戏，把常用纹理压缩成ETC2格式
    /// 包体积优化建议：
    /// 1. 使用 Sprite Atlas 打包精灵
    /// 2. 音频压缩成 MP3 低比特率
    /// 3. 清除未使用的资源
    /// 4. 如果超过4MB，使用微信分包
    /// </summary>
}
