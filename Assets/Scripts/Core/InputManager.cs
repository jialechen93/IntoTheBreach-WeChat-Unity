using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 输入管理器 - 处理玩家点击、移动、攻击等输入
/// 适配手机触摸屏和微信小游戏
/// </summary>
public class InputManager : MonoBehaviour
{
    public static InputManager Instance;
    
    [Header("引用")]
    public Camera mainCamera;
    public GridManager gridManager;
    public TurnManager turnManager;
    
    [Header("状态")]
    public InputState currentState;
    public enum InputState
    {
        Idle,           // 等待选择机甲
        SelectingMech,  // 已选机甲，等待选择移动或攻击目标
        Moving,         // 移动中
        Attacking       // 攻击中
    }

    private PlayerMech selectedMech;
    private Cell highlightedCell;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        currentState = InputState.Idle;
        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    private void Update()
    {
        if (turnManager.currentState != TurnManager.TurnState.PlayerTurn)
            return;

        // 处理点击输入（同时支持鼠标和触摸）
        if (Input.GetMouseButtonDown(0) && !IsPointerOverUI())
        {
            HandleClick();
        }
        
        #if UNITY_IOS || UNITY_ANDROID
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began && !IsPointerOverUI())
            {
                HandleTouch(touch);
            }
        }
        #endif
    }

    /// <summary>
    /// 处理鼠标点击
    /// </summary>
    void HandleClick()
    {
        Vector2 worldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        ProcessWorldPositionClick(worldPos);
    }

    /// <summary>
    /// 处理触摸
    /// </summary>
    void HandleTouch(Touch touch)
    {
        Vector2 worldPos = mainCamera.ScreenToWorldPoint(touch.position);
        ProcessWorldPositionClick(worldPos);
    }

    /// <summary>
    /// 处理世界位置点击
    /// </summary>
    void ProcessWorldPositionClick(Vector2 worldPos)
    {
        Vector2Int gridPos = gridManager.GetGridPosition(worldPos);
        
        if (!gridManager.IsValidCell(gridPos.x, gridPos.y))
            return;

        Cell cell = gridManager.GetCell(gridPos.x, gridPos.y);
        
        switch (currentState)
        {
            case InputState.Idle:
                //  idle状态，点击格子，如果有玩家机甲，选中它
                if (cell != null && cell.HasUnit() && cell.unitOnCell is PlayerMech)
                {
                    PlayerMech mech = (PlayerMech)cell.unitOnCell;
                    if (mech.IsAlive() && !mech.IsActionComplete())
                    {
                        SelectMech(mech);
                    }
                }
                break;
                
            case InputState.SelectingMech:
                // 已选中机甲，点击目标格子
                if (cell == null) break;
                
                // 如果点击自己，取消选中
                if (cell.X == selectedMech.currentX && cell.Y == selectedMech.currentY)
                {
                    DeselectMech();
                    break;
                }
                
                // 如果点击的是另一个玩家机甲，切换选中
                if (cell.HasUnit() && cell.unitOnCell is PlayerMech)
                {
                    PlayerMech newMech = (PlayerMech)cell.unitOnCell;
                    if (newMech.IsAlive() && !newMech.IsActionComplete())
                    {
                        DeselectMech();
                        SelectMech(newMech);
                    }
                    break;
                }
                
                // 如果还能移动，并且目标格子可走，移动过去
                if (!selectedMech.hasMovedThisTurn && selectedMech.CanMoveTo(cell.X, cell.Y))
                {
                    MoveSelectedMech(cell);
                    break;
                }
                
                // 不能移动了，看看能不能攻击这里
                if (selectedMech.hasMovedThisTurn && !selectedMech.hasAttackedThisTurn)
                {
                    // 检查是否在攻击范围内
                    int dist = Mathf.Abs(cell.X - selectedMech.currentX) + 
                               Mathf.Abs(cell.Y - selectedMech.currentY);
                               
                    if (dist <= selectedMech.attackRange)
                    {
                        TryAttackCell(cell);
                    }
                }
                break;
        }
        
        // 清除高亮
        ClearHighlight();
    }

    /// <summary>
    /// 选中机甲
    /// </summary>
    void SelectMech(PlayerMech mech)
    {
        selectedMech = mech;
        currentState = InputState.SelectingMech;
        
        // 使用 VisualEffects 高亮范围
        VisualEffects effects = FindObjectOfType<VisualEffects>();
        if (effects != null)
        {
            effects.ClearAllHighlights();
            effects.HighlightMovableArea(mech);
            if (mech.hasMovedThisTurn)
            {
                effects.HighlightAttackRange(mech);
            }
        }
        else
        {
            // 兼容旧代码
            HighlightMovableArea(mech);
        }
        
        // 选中格子变色
        Cell cell = gridManager.GetCell(mech.currentX, mech.currentY);
        if (cell != null)
        {
            cell.Select();
            highlightedCell = cell;
        }
    }

    /// <summary>
    /// 取消选中
    /// </summary>
    void DeselectMech()
    {
        if (highlightedCell != null)
        {
            highlightedCell.ResetColor();
        }
        
        selectedMech = null;
        currentState = InputState.Idle;
        ClearHighlight();
    }

    /// <summary>
    /// 高亮可移动区域
    /// </summary>
    void HighlightMovableArea(PlayerMech mech)
    {
        if (mech.hasMovedThisTurn) return;
        
        // BFS 搜索可移动范围
        bool[,] visited = new bool[gridManager.width, gridManager.height];
        System.Collections.Generic.Queue<Vector2Int> queue = 
            new System.Collections.Generic.Queue<Vector2Int>();
            
        queue.Enqueue(new Vector2Int(mech.currentX, mech.currentY));
        visited[mech.currentX, mech.currentY] = true;
        
        int steps = 0;
        while (queue.Count > 0 && steps < mech.moveRange)
        {
            int count = queue.Count;
            for (int i = 0; i < count; i++)
            {
                Vector2Int pos = queue.Dequeue();
                
                // 检查四个方向
                CheckAndEnqueue(pos.x + 1, pos.y, visited, queue);
                CheckAndEnqueue(pos.x - 1, pos.y, visited, queue);
                CheckAndEnqueue(pos.x, pos.y + 1, visited, queue);
                CheckAndEnqueue(pos.x, pos.y - 1, visited, queue);
            }
            steps++;
        }
    }

    /// <summary>
    /// 检查并加入队列
    /// </summary>
    void CheckAndEnqueue(int x, int y, bool[,] visited, System.Collections.Generic.Queue<Vector2Int> queue)
    {
        if (gridManager.IsValidCell(x, y) && !visited[x, y])
        {
            visited[x, y] = true;
            Cell cell = gridManager.GetCell(x, y);
            if (cell != null && !cell.IsOccupied())
            {
                cell.Highlight(true);
                queue.Enqueue(new Vector2Int(x, y));
            }
        }
    }

    /// <summary>
    /// 清除所有高亮
    /// </summary>
    void ClearHighlight()
    {
        VisualEffects effects = FindObjectOfType<VisualEffects>();
        if (effects != null)
        {
            effects.ClearAllHighlights();
        }
        else
        {
            // 兼容旧代码
            for (int x = 0; x < gridManager.width; x++)
            {
                for (int y = 0; y < gridManager.height; y++)
                {
                    Cell cell = gridManager.GetCell(x, y);
                    if (cell != null)
                    {
                        cell.ResetColor();
                    }
                }
            }
        }
    }

    /// <summary>
    /// 移动选中机甲到目标格子
    /// </summary>
    void MoveSelectedMech(Cell targetCell)
    {
        if (selectedMech == null) return;
        
        selectedMech.MoveTo(targetCell.X, targetCell.Y);
        selectedMech.MarkMoved();
        
        ClearHighlight();
        targetCell.Select();
        highlightedCell = targetCell;
        
        // 如果移动后攻击范围内有可攻击目标，可以继续攻击
        // 如果不能攻击了，完成这个机甲的行动
        if (selectedMech.attackRange <= 0 || selectedMech.IsActionComplete())
        {
            turnManager.CompleteCurrentMechAction();
            DeselectMech();
        }
    }

    /// <summary>
    /// 尝试攻击目标格子
    /// </summary>
    void TryAttackCell(Cell cell)
    {
        if (cell.HasUnit())
        {
            // 攻击格子上的单位
            selectedMech.Attack(cell.unitOnCell);
            
            if (!cell.unitOnCell.IsAlive())
            {
                cell.ClearUnit();
            }
        }
        else if (cell.isBuilding && cell.buildingHealth > 0)
        {
            // 攻击建筑（这种情况很少见）
            cell.TakeDamage(selectedMech.attackDamage);
        }
        
        selectedMech.MarkAttacked();
        
        // 完成这个机甲的行动
        turnManager.CompleteCurrentMechAction();
        DeselectMech();
    }

    /// <summary>
    /// 选中下一个可行动机甲
    /// </summary>
    public void SelectNextActingMech()
    {
        PlayerMech nextMech = turnManager.GetCurrentActingMech();
        if (nextMech != null)
        {
            SelectMech(nextMech);
        }
    }

    /// <summary>
    /// 检查是否点击到UI
    /// </summary>
    bool IsPointerOverUI()
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }

    /// <summary>
    /// 适配微信小游戏屏幕自适应
    /// </summary>
    public void AdaptToScreen()
    {
        float screenRatio = (float)Screen.width / (float)Screen.height;
        float targetRatio = 9.0f / 16.0f; // 手机竖屏
        
        if (screenRatio >= targetRatio)
        {
            // 更宽，调整高度
            mainCamera.orthographicSize = (gridManager.height * gridManager.cellSize + 2) / 2f;
        }
        else
        {
            // 更高，调整宽度
            float unitsPerPixel = (gridManager.width * gridManager.cellSize + 2) / 2f / screenRatio;
            mainCamera.orthographicSize = unitsPerPixel;
        }
    }
}
