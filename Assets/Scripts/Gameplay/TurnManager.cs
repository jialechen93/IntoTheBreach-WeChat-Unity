using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// 回合管理器 - 控制玩家回合和敌人回合的流程
/// 陷阵之志核心机制：玩家行动 -> 敌人展示下一步 -> 敌人执行行动
/// </summary>
public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;
    
    [Header("引用")]
    public GridManager gridManager;
    public Transform playerMechsParent;
    public Transform enemiesParent;
    public List<Cell> buildingCells;
    
    [Header("UI")]
    public Text turnText;
    public Button endTurnButton;
    
    [Header("游戏状态")]
    public int currentTurn = 1;
    public TurnState currentState;
    
    public enum TurnState
    {
        PlayerTurn,    // 玩家正在行动
        EnemiesPlanning, // 敌人规划完成（玩家已经看到所有敌人下一步）
        EnemiesTurn  // 敌人执行行动
    }

    private List<PlayerMech> playerMechs = new List<PlayerMech>();
    private List<EnemyUnit> enemies = new List<EnemyUnit>();
    private int actingMechIndex = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // 收集玩家机甲
        CollectPlayerMechs();
        // 收集敌人
        CollectEnemies();
        
        // 开始玩家回合
        StartPlayerTurn();
    }

    /// <summary>
    /// 收集所有玩家机甲
    /// </summary>
    public void CollectPlayerMechs()
    {
        playerMechs.Clear();
        foreach (Transform child in playerMechsParent)
        {
            PlayerMech mech = child.GetComponent<PlayerMech>();
            if (mech != null) playerMechs.Add(mech);
        }
    }

    /// <summary>
    /// 收集所有敌人
    /// </summary>
    public void CollectEnemies()
    {
        enemies.Clear();
        foreach (Transform child in enemiesParent)
        {
            EnemyUnit enemy = child.GetComponent<EnemyUnit>();
            if (enemy != null) enemies.Add(enemy);
        }
    }

    /// <summary>
    /// 开始玩家回合
    /// </summary>
    void StartPlayerTurn()
    {
        currentState = TurnState.PlayerTurn;
        UpdateTurnText();
        
        // 重置所有玩家机甲状态
        foreach (PlayerMech mech in playerMechs)
        {
            if (mech.IsAlive())
            {
                mech.ResetTurn();
            }
        }
        
        actingMechIndex = GetNextAliveMechIndex();
        endTurnButton.interactable = true;
        
        InputManager.Instance.SelectNextActingMech();
    }

    /// <summary>
    /// 获取下一个可行动的机甲索引
    /// </summary>
    int GetNextAliveMechIndex()
    {
        for (int i = 0; i < playerMechs.Count; i++)
        {
            if (playerMechs[i].IsAlive() && !playerMechs[i].IsActionComplete())
            {
                return i;
            }
        }
        return -1;
    }

    /// <summary>
    /// 当前机甲完成行动，切换到下一个
    /// </summary>
    public void CompleteCurrentMechAction()
    {
        actingMechIndex = GetNextAliveMechIndex();
        
        // 如果所有机甲都行动完毕，结束玩家回合
        if (actingMechIndex < 0)
        {
            EndPlayerTurn();
        }
        else
        {
            InputManager.Instance.SelectNextActingMech();
        }
    }

    /// <summary>
    /// 获取当前可行动机甲
    /// </summary>
    public PlayerMech GetCurrentActingMech()
    {
        if (actingMechIndex >= 0 && actingMechIndex < playerMechs.Count)
        {
            return playerMechs[actingMechIndex];
        }
        return null;
    }

    /// <summary>
    /// 结束玩家回合
    /// </summary>
    public void EndPlayerTurn()
    {
        endTurnButton.interactable = false;
        currentState = TurnState.EnemiesPlanning;
        
        // 让所有敌人规划行动并显示预览
        foreach (EnemyUnit enemy in enemies)
        {
            if (enemy.IsAlive())
            {
                enemy.PlanAction(playerMechsParent, buildingCells);
            }
        }
        
        // 短暂延迟后开始敌人回合
        Invoke(nameof(StartEnemiesTurn), 1.5f);
    }

    /// <summary>
    /// 开始敌人回合
    /// </summary>
    void StartEnemiesTurn()
    {
        currentState = TurnState.EnemiesTurn;
        UpdateTurnText();
        
        // 敌人按顺序执行规划好的行动
        StartCoroutine(ExecuteEnemiesTurn());
    }

    /// <summary>
    /// 协程执行敌人回合，一个一个来，有动画效果
    /// </summary>
    IEnumerator<WaitForSeconds> ExecuteEnemiesTurn()
    {
        foreach (EnemyUnit enemy in enemies)
        {
            if (enemy.IsAlive())
            {
                enemy.ExecutePlannedAction();
                yield return new WaitForSeconds(0.5f);
            }
        }
        
        yield return new WaitForSeconds(0.5f);
        
        // 检查游戏结束条件
        if (CheckGameOver())
        {
            // 游戏结束
            GameOver();
        }
        else if (CheckVictory())
        {
            // 胜利
            Victory();
        }
        else
        {
            // 下一回合
            currentTurn++;
            StartPlayerTurn();
        }
    }

    /// <summary>
    /// 检查游戏结束（所有玩家机甲死亡，或所有建筑被摧毁）
    /// </summary>
    bool CheckGameOver()
    {
        // 检查所有玩家机甲是否死亡
        bool allPlayerDead = true;
        foreach (PlayerMech mech in playerMechs)
        {
            if (mech.IsAlive())
            {
                allPlayerDead = false;
                break;
            }
        }
        if (allPlayerDead) return true;
        
        // 检查所有建筑是否被摧毁
        int aliveBuildings = 0;
        foreach (Cell cell in buildingCells)
        {
            if (cell.isBuilding && cell.buildingHealth > 0)
            {
                aliveBuildings++;
            }
        }
        if (aliveBuildings <= 0) return true;
        
        return false;
    }

    /// <summary>
    /// 检查胜利（所有敌人死亡）
    /// </summary>
    bool CheckVictory()
    {
        foreach (EnemyUnit enemy in enemies)
        {
            if (enemy.IsAlive())
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// 游戏结束
    /// </summary>
    void GameOver()
    {
        turnText.text = "游戏失败！";
        endTurnButton.interactable = false;
        Debug.Log("Game Over!");
        GameUIManager.Instance.ShowGameOver(false);
    }

    /// <summary>
    /// 胜利
    /// </summary>
    void Victory()
    {
        if (LevelManager.Instance != null)
        {
            // 完成关卡，显示升级界面
            LevelManager.Instance.CompleteLevel();
        }
        else
        {
            // 没有关卡管理器，直接显示游戏结束
            turnText.text = "回合 " + currentTurn + " - 胜利！";
            endTurnButton.interactable = false;
            Debug.Log("Victory!");
            GameUIManager.Instance.ShowGameOver(true);
        }
    }

    /// <summary>
    /// 更新回合文字
    /// </summary>
    void UpdateTurnText()
    {
        string stateStr = currentState == TurnState.PlayerTurn ? "你的回合" : "敌人行动";
        turnText.text = $"回合 {currentTurn} - {stateStr}";
    }

    /// <summary>
    /// 获取所有活着的敌人
    /// </summary>
    public List<EnemyUnit> GetAliveEnemies()
    {
        List<EnemyUnit> alive = new List<EnemyUnit>();
        foreach (EnemyUnit enemy in enemies)
        {
            if (enemy.IsAlive()) alive.Add(enemy);
        }
        return alive;
    }

    /// <summary>
    /// 按钮点击：结束回合
    /// </summary>
    public void OnEndTurnButtonClicked()
    {
        EndPlayerTurn();
    }
}
