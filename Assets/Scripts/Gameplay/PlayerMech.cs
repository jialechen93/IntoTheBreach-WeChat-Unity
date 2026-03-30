using UnityEngine;

/// <summary>
/// 玩家机甲单位
/// </summary>
public class PlayerMech : BaseUnit
{
    [Header("机甲特殊属性")]
    public int armor;
    public mechType type;
    
    public enum mechType
    {
        Fighter,   // 格斗型 - 近战高伤害
        Sniper,   // 狙击型 - 远程攻击
        Tank      // 坦克型 - 高血量高防御
    }

    // 已经移动过了
    public bool hasMovedThisTurn { get; private set; }
    // 已经攻击过了
    public bool hasAttackedThisTurn { get; private set; }

    protected override void Start()
    {
        base.Start();
        ResetTurn();
    }

    /// <summary>
    /// 重置回合状态
    /// </summary>
    public void ResetTurn()
    {
        hasMovedThisTurn = false;
        hasAttackedThisTurn = false;
    }

    /// <summary>
    /// 标记已移动
    /// </summary>
    public void MarkMoved()
    {
        hasMovedThisTurn = true;
    }

    /// <summary>
    /// 标记已攻击
    /// </summary>
    public void MarkAttacked()
    {
        hasAttackedThisTurn = true;
    }

    /// <summary>
    /// 是否已完成本回合行动
    /// </summary>
    public bool IsActionComplete()
    {
        // 只要移动和攻击都做了，或者都不需要做，就完成了
        return (hasMovedThisTurn || moveRange <= 0) && (hasAttackedThisTurn || attackRange <= 0);
    }

    /// <summary>
    /// 受到伤害 - 考虑护甲
    /// </summary>
    protected override void TakeDamage(int damage)
    {
        int actualDamage = Mathf.Max(1, damage - armor);
        base.TakeDamage(actualDamage);
    }

    /// <summary>
    /// 攻击目标
    /// </summary>
    public override void Attack(BaseUnit target)
    {
        if (target != null && target.IsAlive())
        {
            target.TakeDamage(attackDamage);
            MarkAttacked();
        }
    }

    /// <summary>
    /// 推进攻击 - 把敌人推走（陷阵之志特色机制）
    /// </summary>
    public void PushAttack(BaseUnit target, int directionX, int directionY, int pushDistance = 1)
    {
        // 先造成伤害
        Attack(target);
        
        // 如果目标还活着，尝试推进
        if (target.IsAlive())
        {
            TryPushTarget(target, directionX, directionY, pushDistance);
        }
    }

    /// <summary>
    /// 尝试推进目标
    /// </summary>
    private void TryPushTarget(BaseUnit target, int dirX, int dirY, int distance)
    {
        int targetX = target.currentX;
        int targetY = target.currentY;
        
        for (int i = 0; i < distance; i++)
        {
            int newX = targetX + dirX;
            int newY = targetY + dirY;
            
            if (GridManager.Instance.IsValidCell(newX, newY))
            {
                Cell cell = GridManager.Instance.GetCell(newX, newY);
                if (!cell.IsOccupied())
                {
                    // 可以推过去
                    Cell oldCell = GridManager.Instance.GetCell(targetX, targetY);
                    oldCell.ClearUnit();
                    target.SetPosition(newX, newY);
                    targetX = newX;
                    targetY = newY;
                }
                else if (cell.isBuilding)
                {
                    // 撞到建筑，建筑掉血
                    cell.TakeDamage(1);
                    break;
                }
                else
                {
                    // 撞到其他单位，停下
                    break;
                }
            }
        }
    }
}
