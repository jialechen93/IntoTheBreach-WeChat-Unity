using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 随机关图生成器 - 每次生成不同的地图布局
/// </summary>
public class MapGenerator : MonoBehaviour
{
    public GridManager gridManager;
    
    [Header("生成设置")]
    public int gridWidth = 8;
    public int gridHeight = 8;
    public int minDistanceBetweenEnemies = 2;
    public int minDistanceFromPlayerStart = 3;
    
    [Header("预制件")]
    public EnemyUnit[] enemyPrefabs;
    public Cell cellPref;
    public PlayerMech[] playerMechPrefabs;
    
    [Header("生成父物体")]
    public Transform playerMechsParent;
    public Transform enemiesParent;
    public Transform buildingsParent;
    
    private System.Random random;
    private int seed;

    private void Awake()
    {
        if (gridManager == null)
            gridManager = FindObjectOfType<GridManager>();
    }

    /// <summary>
    /// 生成随机地图
    /// </summary>
    public void GenerateRandomMap(int enemyCount, int buildingCount)
    {
        // 使用当前时间作为种子，也可以固定种子调试
        seed = System.DateTime.Now.Millisecond;
        random = new System.Random(seed);
        
        // 清理旧地图
        ClearExistingMap();
        
        // 放置玩家机甲（固定在下方三格）
        PlacePlayerMechs();
        
        // 放置建筑（随机分布在中间）
        List<Cell> placedBuildings = PlaceBuildings(buildingCount);
        
        // 放置敌人（随机分布在上方，不能太近）
        PlaceEnemies(enemyCount);
        
        // 把建筑列表传给TurnManager
        TurnManager turnManager = FindObjectOfType<TurnManager>();
        if (turnManager != null)
        {
            turnManager.buildingCells = placedBuildings;
        }
        
        Debug.Log($"随机地图生成完成: {enemyCount} 敌人, {buildingCount} 建筑, 种子: {seed}");
    }

    /// <summary>
    /// 清理旧地图
    /// </summary>
    void ClearExistingMap()
    {
        // 清理玩家
        foreach (Transform child in playerMechsParent)
            Destroy(child.gameObject);
        
        // 清理敌人
        foreach (Transform child in enemiesParent)
            Destroy(child.gameObject);
        
        // 清理建筑标记
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Cell cell = gridManager.GetCell(x, y);
                if (cell != null)
                {
                    cell.isBuilding = false;
                    cell.buildingHealth = 0;
                    cell.ResetColor();
                }
            }
        }
    }

    /// <summary>
    /// 放置玩家机甲 - 固定在下方
    /// </summary>
    void PlacePlayerMechs()
    {
        // 三个玩家机甲分别放在下方三个位置
        int[] xPositions = new int[] { 2, 4, 6 };
        int yPosition = 1;
        
        for (int i = 0; i < playerMechPrefabs.Length && i < xPositions.Length; i++)
        {
            PlayerMech prefab = GetUpgradedMechPrefab(i);
            PlayerMech newMech = Instantiate(prefab, playerMechsParent);
            
            // 应用升级
            if (UpgradeManager.Instance != null)
            {
                UpgradeManager.Instance.ApplyUpgradesToMech(newMech, i);
            }
            
            newMech.SetPosition(xPositions[i], yPosition);
        }
    }

    /// <summary>
    /// 获取升级后的机甲预制件（实际上是复制后修改属性）
    /// </summary>
    PlayerMech GetUpgradedMechPrefab(int index)
    {
        // 这里我们直接实例化然后让UpgradeManager修改属性
        return playerMechPrefabs[index];
    }

    /// <summary>
    /// 随机放置建筑
    /// </summary>
    List<Cell> PlaceBuildings(int count)
    {
        List<Cell> buildings = new List<Cell>();
        
        for (int i = 0; i < count; i++)
        {
            Cell cell = GetRandomFreeCellInArea(2, 6, 2, 5);
            if (cell != null)
            {
                cell.isBuilding = true;
                cell.buildingHealth = 3;
                cell.ResetColor();
                buildings.Add(cell);
            }
        }
        
        return buildings;
    }

    /// <summary>
    /// 随机放置敌人
    /// </summary>
    void PlaceEnemies(int count)
    {
        LevelManager levelManager = LevelManager.Instance;
        int baseHealth = 1;
        if (levelManager != null)
            baseHealth = levelManager.GetEnemyBaseHealth();
        
        for (int i = 0; i < count; i++)
        {
            // 在上方区域找位置
            Cell cell = GetRandomEnemyCell();
            if (cell == null) continue;
            
            // 随机选择敌人类型
            EnemyUnit prefab = enemyPrefabs[random.Next(enemyPrefabs.Length)];
            EnemyUnit enemy = Instantiate(prefab, enemiesParent);
            
            // 根据关卡调整血量
            enemy.maxHealth = baseHealth;
            enemy.currentHealth = baseHealth;
            
            enemy.SetPosition(cell.X, cell.Y);
        }
    }

    /// <summary>
    /// 在指定区域获取一个随机空闲格子
    /// </summary>
    Cell GetRandomFreeCellInArea(int minX, int maxX, int minY, int maxY)
    {
        for (int attempts = 0; attempts < 100; attempts++)
        {
            int x = random.Next(minX, maxX + 1);
            int y = random.Next(minY, maxY + 1);
            
            Cell cell = gridManager.GetCell(x, y);
            if (cell != null && !cell.IsOccupied() && !cell.isBuilding)
            {
                return cell;
            }
        }
        return null;
    }

    /// <summary>
    /// 获取一个适合放敌人的随机格子
    /// </summary>
    Cell GetRandomEnemyCell()
    {
        // 敌人放在上方 y > 4 的区域
        for (int attempts = 0; attempts < 100; attempts++)
        {
            int x = random.Next(0, gridWidth);
            int y = random.Next(4, gridHeight);
            
            Cell cell = gridManager.GetCell(x, y);
            if (cell == null || cell.IsOccupied()) continue;
            
            // 检查距离玩家起始位置不能太近
            bool tooCloseToPlayer = false;
            int[] playerX = new int[] { 2, 4, 6 };
            int playerY = 1;
            
            foreach (int px in playerX)
            {
                int dist = Mathf.Abs(x - px) + Mathf.Abs(y - playerY);
                if (dist < minDistanceFromPlayerStart)
                {
                    tooCloseToPlayer = true;
                    break;
                }
            }
            if (tooCloseToPlayer) continue;
            
            // 检查距离其他敌人不能太近
            bool tooCloseToOtherEnemy = false;
            foreach (Transform enemy in enemiesParent)
            {
                EnemyUnit enemyUnit = enemy.GetComponent<EnemyUnit>();
                if (enemyUnit != null)
                {
                    int dist = Mathf.Abs(x - enemyUnit.currentX) + Mathf.Abs(y - enemyUnit.currentY);
                    if (dist < minDistanceBetweenEnemies)
                    {
                        tooCloseToOtherEnemy = true;
                        break;
                    }
                }
            }
            if (tooCloseToOtherEnemy) continue;
            
            return cell;
        }
        
        return null;
    }

    /// <summary>
    /// 设置种子（用于重现地图）
    /// </summary>
    public void SetSeed(int newSeed)
    {
        seed = newSeed;
        random = new System.Random(seed);
    }
}
