using UnityEngine;

/// <summary>
/// 视觉效果管理器 - 高亮预览、动画、特效
/// </summary>
public class VisualEffects : MonoBehaviour
{
    public static VisualEffects Instance;
    
    [Header("高亮设置")]
    public Color moveHighlightColor = new Color(0.2f, 0.8f, 0.2f, 0.5f);
    public Color attackHighlightColor = new Color(0.8f, 0.2f, 0.2f, 0.5f);
    public Color validTargetColor = new Color(1f, 1f, 0f, 0.5f);
    
    [Header("推送效果")]
    public float pushAnimationDuration = 0.3f;
    public AnimationCurve pushMovementCurve;
    
    [Header"引用")]
    public GridManager gridManager;
    
    private Cell[,] originalColors;
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    /// <summary>
    /// 高亮可移动范围
    /// </summary>
    public void HighlightMovableArea(PlayerMech mech)
    {
        if (mech.hasMovedThisTurn) return;
        
        // BFS 搜索所有可达格子
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
                
                CheckAndHighlight(pos.x + 1, pos.y, visited, queue, moveHighlightColor);
                CheckAndHighlight(pos.x - 1, pos.y, visited, queue, moveHighlightColor);
                CheckAndHighlight(pos.x, pos.y + 1, visited, queue, moveHighlightColor);
                CheckAndHighlight(pos.x, pos.y - 1, visited, queue, moveHighlightColor);
            }
            steps++;
        }
    }

    /// <summary>
    /// 高亮攻击范围
    /// </summary>
    public void HighlightAttackRange(PlayerMech mech)
    {
        if (!mech.hasMovedThisTurn || mech.hasAttackedThisTurn) return;
        
        // 曼哈顿距离搜索攻击范围
        for (int x = 0; x < gridManager.width; x++)
        {
            for (int y = 0; y < gridManager.height; y++)
            {
                int dist = Mathf.Abs(x - mech.currentX) + Mathf.Abs(y - mech.currentY);
                if (dist > 0 && dist <= mech.attackRange)
                {
                    Cell cell = gridManager.GetCell(x, y);
                    if (cell != null)
                    {
                        // 如果有敌人，显示红色目标
                        if (cell.HasUnit() && cell.unitOnCell is EnemyUnit)
                        {
                            SetCellTemporaryColor(cell, validTargetColor);
                        }
                        else
                        {
                            SetCellTemporaryColor(cell, attackHighlightColor);
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// 检查并高亮
    /// </summary>
    void CheckAndHighlight(int x, int y, bool[,] visited, 
        System.Collections.Generic.Queue<Vector2Int> queue, Color color)
    {
        if (gridManager.IsValidCell(x, y) && !visited[x, y])
        {
            visited[x, y] = true;
            Cell cell = gridManager.GetCell(x, y);
            if (cell != null && !cell.IsOccupied())
            {
                SetCellTemporaryColor(cell, color);
                queue.Enqueue(new Vector2Int(x, y));
            }
        }
    }

    /// <summary>
    /// 设置临时颜色
    /// </summary>
    void SetCellTemporaryColor(Cell cell, Color color)
    {
        if (cell == null || cell.spriteRenderer == null) return;
        cell.spriteRenderer.color = color;
    }

    /// <summary>
    /// 清除所有高亮
    /// </summary>
    public void ClearAllHighlights()
    {
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

    /// <summary>
    /// 播放单位推送动画（陷阵之志特色）
    /// </summary>
    public void PlayPushAnimation(BaseUnit unit, Vector2Int startPos, Vector2Int endPos, System.Action onComplete)
    {
        StartCoroutine(PushAnimationCoroutine(unit, startPos, endPos, onComplete));
    }

    private System.Collections.IEnumerator PushAnimationCoroutine(
        BaseUnit unit, Vector2Int startPos, Vector2Int endPos, System.Action onComplete)
    {
        float startTime = Time.time;
        Vector3 startWorldPos = gridManager.GetWorldPosition(startPos.x, startPos.y);
        Vector3 endWorldPos = gridManager.GetWorldPosition(endPos.x, endPos.y);
        
        while (Time.time - startTime < pushAnimationDuration)
        {
            float t = (Time.time - startTime) / pushAnimationDuration;
            float curveT = pushMovementCurve.Evaluate(t);
            unit.transform.position = Vector3.Lerp(startWorldPos, endWorldPos, curveT);
            yield return null;
        }
        
        unit.transform.position = endWorldPos;
        onComplete?.Invoke();
    }
}
