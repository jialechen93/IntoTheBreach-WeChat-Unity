using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 网格管理器 - 管理游戏棋盘网格
/// 类似陷阵之志的网格系统
/// </summary>
public class GridManager : MonoBehaviour
{
    public static GridManager Instance;
    
    [Header("网格设置")]
    public int width = 8;
    public int height = 8;
    public float cellSize = 1f;
    public GameObject cellPrefab;
    
    private Cell[,] grid;
    private Transform gridParent;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        CreateGrid();
    }

    /// <summary>
    /// 创建网格
    /// </summary>
    void CreateGrid()
    {
        grid = new Cell[width, height];
        gridParent = new GameObject("Grid").transform;
        gridParent.SetParent(transform);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 worldPos = GetWorldPosition(x, y);
                GameObject cellObj = Instantiate(cellPrefab, worldPos, Quaternion.identity, gridParent);
                cellObj.name = $"Cell_{x}_{y}";
                
                Cell cell = cellObj.GetComponent<Cell>();
                if (cell == null) cell = cellObj.AddComponent<Cell>();
                cell.SetPosition(x, y);
                
                grid[x, y] = cell;
            }
        }

        CenterGrid();
    }

    /// <summary>
    /// 获取网格坐标对应的世界坐标
    /// </summary>
    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(
            (x - width / 2f + 0.5f) * cellSize,
            (y - height / 2f + 0.5f) * cellSize,
            0
        );
    }

    /// <summary>
    /// 从世界坐标获取网格坐标
    /// </summary>
    public Vector2Int GetGridPosition(Vector3 worldPos)
    {
        int x = Mathf.FloorToInt((worldPos.x / cellSize) + width / 2f);
        int y = Mathf.FloorToInt((worldPos.y / cellSize) + height / 2f);
        return new Vector2Int(x, y);
    }

    /// <summary>
    /// 获取单元格
    /// </summary>
    public Cell GetCell(int x, int y)
    {
        if (IsValidCell(x, y)) return grid[x, y];
        return null;
    }

    /// <summary>
    /// 验证坐标是否有效
    /// </summary>
    public bool IsValidCell(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }

    /// <summary>
    /// 将网格居中
    /// </summary>
    void CenterGrid()
    {
        gridParent.localPosition = Vector3.zero;
    }

    /// <summary>
    /// 获取相邻单元格
    /// </summary>
    public List<Cell> GetAdjacentCells(int x, int y)
    {
        List<Cell> adjacent = new List<Cell>();
        
        // 上下左右四个方向
        if (IsValidCell(x + 1, y)) adjacent.Add(GetCell(x + 1, y));
        if (IsValidCell(x - 1, y)) adjacent.Add(GetCell(x - 1, y));
        if (IsValidCell(x, y + 1)) adjacent.Add(GetCell(x, y + 1));
        if (IsValidCell(x, y - 1)) adjacent.Add(GetCell(x, y - 1));
        
        return adjacent;
    }
}
