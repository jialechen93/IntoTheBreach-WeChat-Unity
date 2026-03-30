using UnityEngine;

/// <summary>
/// 单个单元格
/// </summary>
public class Cell : MonoBehaviour
{
    public int X { get; private set; }
    public int Y { get; private set; }
    
    [Header("视觉设置")]
    public SpriteRenderer spriteRenderer;
    public Color defaultColor = Color.gray;
    public Color highlightedColor = Color.yellow;
    public Color selectedColor = Color.green;
    public Color blockedColor = Color.red;
    
    // 单元格上的单位
    public BaseUnit unitOnCell { get; private set; }
    
    // 是否是建筑（需要保护）
    public bool isBuilding = false;
    public int buildingHealth = 3;
    
    // 是否可通行
    public bool isWalkable => !isBuilding || buildingHealth <= 0;

    private void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetPosition(int x, int y)
    {
        X = x;
        Y = y;
    }

    public void SetUnit(BaseUnit unit)
    {
        unitOnCell = unit;
    }

    public void ClearUnit()
    {
        unitOnCell = null;
    }

    public bool HasUnit()
    {
        return unitOnCell != null;
    }

    public void Highlight(bool highlight)
    {
        if (spriteRenderer == null) return;
        
        spriteRenderer.color = highlight ? highlightedColor : defaultColor;
    }

    public void Select()
    {
        if (spriteRenderer == null) return;
        spriteRenderer.color = selectedColor;
    }

    public void ResetColor()
    {
        if (spriteRenderer == null) return;
        
        if (isBuilding && buildingHealth > 0)
            spriteRenderer.color = Color.blue;
        else if (isBuilding && buildingHealth <= 0)
            spriteRenderer.color = blockedColor;
        else
            spriteRenderer.color = defaultColor;
    }

    public void TakeDamage(int damage)
    {
        if (!isBuilding) return;
        
        buildingHealth -= damage;
        if (buildingHealth <= 0)
        {
            ResetColor();
        }
    }

    public bool IsOccupied()
    {
        return HasUnit() || (isBuilding && buildingHealth > 0);
    }
}
