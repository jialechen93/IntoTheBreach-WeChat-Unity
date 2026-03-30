using UnityEngine;

/// <summary>
/// 游戏管理器 - 根节点，管理整个游戏生命周期
/// 遵循Unity单例模式，适配微信小游戏生命周期
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    [Header("模块引用")]
    public GridManager gridManager;
    public TurnManager turnManager;
    public LevelManager levelManager;
    public MapGenerator mapGenerator;
    public InputManager inputManager;
    public VisualEffects visualEffects;
    public UpgradeManager upgradeManager;
    public AudioManager audioManager;
    public ScoreManager scoreManager;
    
    [Header("游戏状态")]
    public GameState gameState = GameState.Initializing;
    
    public enum GameState
    {
        Initializing,
        Loading,
        Playing,
        Paused,
        GameOver,
        LevelComplete
    }

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
        
        // 初始化顺序：模块引用 → 加载 → 生成地图 → 开始游戏
        Application.targetFrameRate = 60; // 微信小游戏稳定60帧
        gameState = GameState.Initializing;
    }

    private void Start()
    {
        gameState = GameState.Loading;
        StartGame();
    }

    /// <summary>
    /// 开始新游戏
    /// </summary>
    public void StartGame()
    {
        // 加载存档
        upgradeManager.LoadUpgrades();
        levelManager.LoadProgress();
        
        // 生成第一关地图
        mapGenerator.GenerateRandomMap(
            levelManager.currentLevelData.enemyCount,
            levelManager.currentLevelData.buildingCount
        );
        
        // 开始第一回合
        turnManager.StartPlayerTurn();
        scoreManager.NewLevel();
        
        gameState = GameState.Playing;
        
        // 播放背景音乐
        if (audioManager != null && audioManager.bgmSource.clip != null)
        {
            audioManager.PlayBGM(audioManager.bgmSource.clip);
        }
    }

    /// <summary>
    /// 重新开始当前关卡
    /// </summary>
    public void RestartCurrentLevel()
    {
        // 重置所有模块
        levelManager.GenerateRandomLevel();
        mapGenerator.GenerateRandomMap(
            levelManager.currentLevelData.enemyCount,
            levelManager.currentLevelData.buildingCount
        );
        turnManager.CollectPlayerMechs();
        turnManager.CollectEnemies();
        turnManager.StartPlayerTurn();
        scoreManager.NewLevel();
        
        gameState = GameState.Playing;
    }

    /// <summary>
    /// 暂停游戏
    /// </summary>
    public void PauseGame()
    {
        if (gameState == GameState.Playing)
        {
            gameState = GameState.Paused;
            Time.timeScale = 0;
            audioManager.PauseBGM();
        }
    }

    /// <summary>
    /// 继续游戏
    /// </summary>
    public void ResumeGame()
    {
        if (gameState == GameState.Paused)
        {
            gameState = GameState.Playing;
            Time.timeScale = 1;
            audioManager.ResumeBGM();
        }
    }

    /// <summary>
    /// 处理微信小游戏进入后台
    /// </summary>
    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            // 进入后台
            if (gameState == GameState.Playing)
            {
                PauseGame();
            }
        }
        // 退出后台不需要自动继续，让玩家点击继续
    }

    /// <summary>
    /// 低内存处理 - 微信小游戏可能被系统回收
    /// </summary>
    private void OnLowMemory()
    {
        // 卸载未使用的资源
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
    }
}
