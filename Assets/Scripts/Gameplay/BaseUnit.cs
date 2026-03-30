using UnityEngine;

/// <summary>
/// 基础单位类 - 所有单位（玩家机甲、敌人）都继承自此类
/// </summary>
public abstract class BaseUnit : MonoBehaviour
{
    [Header("基础属性")]
    public string unitName;
    public int maxHealth;
    public int currentHealth;
    public int attackDamage;
    public int moveRange;
    public int attackRange;
    
    [Header("当前位置")]
    public int currentX;
    public int currentY;
    
    [Header("视觉")]
    public SpriteRenderer spriteRenderer;
    
    protected TurnManager turnManager;

    protected virtual void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
            
        turnManager = FindObjectOfType<TurnManager>();
    }

    protected virtual void Start()
    {
        currentHealth = maxHealth;
    }

    /// <summary>
    /// 设置单位位置
    /// </summary>
    public virtual void SetPosition(int x, int y)
    {
        currentX = x;
        currentY = y;
        
        Cell cell = GridManager.Instance.GetCell(x, y);
        if (cell != null)
        {
            transform.position = GridManager.Instance.GetWorldPosition(x, y);
            cell.SetUnit(this);
        }
    }

    /// <summary>
    /// 移动单位
    /// </summary>
    public virtual bool MoveTo(int x, int y)
    {
        if (!CanMoveTo(x, y)) return false;
        
        // 清除旧位置
        Cell oldCell = GridManager.Instance.GetCell(currentX, currentY);
        if (oldCell != null) oldCell.ClearUnit();
        
        // 设置新位置
        SetPosition(x, y);
        return true;
    }

    /// <summary>
    /// 检查是否可以移动到此位置
    /// </summary>
    public virtual bool CanMoveTo(int x, int y)
    {
        if (!GridManager.Instance.IsValidCell(x, y)) return false;
        
        Cell cell = GridManager.Instance.GetCell(x, y);
        return cell != null && !cell.IsOccupied();
    }

    /// <summary>
    /// 受到伤害
    /// </summary>
    public virtual void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// 死亡
    /// </summary>
    protected virtual void Die()
    {
        Cell cell = GridManager.Instance.GetCell(currentX, currentY);
        if (cell != null) cell.ClearUnit();
        
        Destroy(gameObject);
    }

    /// <summary>
    /// 是否存活
    /// </summary>
    public bool IsAlive()
    {
        return currentHealth > 0;
    }

    /// <summary>
    /// 攻击
    /// </summary>
    public abstract void Attack(BaseUnit target);
}
