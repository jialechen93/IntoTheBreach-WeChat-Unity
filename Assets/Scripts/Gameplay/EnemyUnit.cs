using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 敌人单位
/// 陷阵之志特点：敌人下一回合的移动和攻击都是可见可预测的
/// </summary>
public class EnemyUnit : BaseUnit
{
    [Header("敌人类型")]
    public enemyType type;
    
    public enum enemyType
    {
        Spitter,    // 喷吐者 - 远程攻击
        Beetle,     // 甲虫 - 近战冲撞
        Firefly,    // 萤火虫 - 飞行
        Boss        // BOSS
    }

    [Header("下一回合行动预测")]
    public Vector2Int nextMoveTarget;  // 下一回合要移动到哪里
    public Vector2Int nextAttackTarget; // 下一回合要攻击哪里
    public bool hasPlannedAction = false;

    [Header("预览标记")]
    public GameObject movePreviewPrefab;
    public GameObject attackPreviewPrefab;
    private GameObject movePreviewInstance;
    private GameObject attackPreviewInstance;

    protected override void Start()
    {
        base.Start();
    }

    /// <summary>
    /// 规划下一回合行动（在玩家回合时显示）
    /// </summary>
    public void PlanAction(Transform playerMechs, List<Cell> buildings)
    {
        // 找到最近的目标（玩家机甲优先，其次建筑）
        BaseUnit target = FindClosestTarget(playerMechs, buildings);
        
        if (target != null)
        {
            // 计算移动路径，预测下一步
            PredictMovement(target);
            // 预测攻击
            PredictAttack(target);
            
            // 创建预览标记
            CreatePreviewMarkers();
            hasPlannedAction = true;
        }
    }

    /// <summary>
    /// 找到最近的目标
    /// </summary>
    private BaseUnit FindClosestTarget(Transform playerMechs, List<Cell> buildings)
    {
        float closestDistance = float.MaxValue;
        BaseUnit closestTarget = null;

        // 先找玩家机甲
        foreach (Transform mechTransform in playerMechs)
        {
            PlayerMech mech = mechTransform.GetComponent<PlayerMech>();
            if (mech != null && mech.IsAlive())
            {
                float dist = Vector2.Distance(
                    new Vector2(currentX, currentY),
                    new Vector2(mech.currentX, mech.currentY)
                );
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    closestTarget = mech;
                }
            }
        }

        // 如果没有找到机甲，找建筑
        if (closestTarget == null && buildings != null)
        {
            foreach (Cell building in buildings)
            {
                if (building.isBuilding && building.buildingHealth > 0)
                {
                    float dist = Vector2.Distance(
                        new Vector2(currentX, currentY),
                        new Vector2(building.X, building.Y)
                    );
                    if (dist < closestDistance)
                    {
                        closestDistance = dist;
                        // 这里我们只需要位置，用一个假target带位置信息
                        // 在实际实现中可以改进
                    }
                }
            }
        }

        return closestTarget;
    }

    /// <summary>
    /// 预测移动 - 向目标靠近一步
    /// </summary>
    private void PredictMovement(BaseUnit target)
    {
        int dx = target.currentX - currentX;
        int dy = target.currentY - currentY;
        
        nextMoveTarget = new Vector2Int(currentX, currentY);
        
        // 如果不在攻击范围内，向目标移动一步
        float dist = Mathf.Abs(dx) + Mathf.Abs(dy);
        if (dist > attackRange)
        {
            // 选择x或y方向移动（曼哈顿距离）
            if (Mathf.Abs(dx) >= Mathf.Abs(dy))
            {
                nextMoveTarget.x += dx > 0 ? 1 : -1;
            }
            else
            {
                nextMoveTarget.y += dy > 0 ? 1 : -1;
            }
        }
    }

    /// <summary>
    /// 预测攻击
    /// </summary>
    private void PredictAttack(BaseUnit target)
    {
        // 如果已经在攻击范围内，直接攻击目标
        int dx = target.currentX - currentX;
        int dy = target.currentY - currentY;
        float dist = Mathf.Abs(dx) + Mathf.Abs(dy);
        
        if (dist <= attackRange)
        {
            nextAttackTarget = new Vector2Int(target.currentX, target.currentY);
        }
        else
        {
            // 移动后攻击
            int finalX = nextMoveTarget.x;
            int finalY = nextMoveTarget.y;
            dx = target.currentX - finalX;
            dy = target.currentY - finalY;
            dist = Mathf.Abs(dx) + Mathf.Abs(dy);
            
            if (dist <= attackRange)
            {
                nextAttackTarget = new Vector2Int(target.currentX, target.currentY);
            }
            else
            {
                nextAttackTarget = Vector2Int.zero;
            }
        }
    }

    /// <summary>
    /// 创建预览标记，让玩家看到敌人下一回合的行动
    /// </summary>
    private void CreatePreviewMarkers()
    {
        // 清除旧标记
        ClearPreviews();
        
        // 如果移动和攻击位置不同，显示移动标记
        if (nextMoveTarget.x != currentX || nextMoveTarget.y != currentY)
        {
            Vector3 worldPos = GridManager.Instance.GetWorldPosition(nextMoveTarget.x, nextMoveTarget.y);
            movePreviewInstance = Instantiate(movePreviewPrefab, worldPos, Quaternion.identity, transform);
        }

        // 显示攻击标记
        if (nextAttackTarget.x != 0 || nextAttackTarget.y != 0)
        {
            Vector3 worldPos = GridManager.Instance.GetWorldPosition(nextAttackTarget.x, nextAttackTarget.y);
            attackPreviewInstance = Instantiate(attackPreviewPrefab, worldPos, Quaternion.identity, transform);
        }
    }

    /// <summary>
    /// 清除预览标记
    /// </summary>
    private void ClearPreviews()
    {
        if (movePreviewInstance != null)
            Destroy(movePreviewInstance);
        if (attackPreviewInstance != null)
            Destroy(attackPreviewInstance);
    }

    /// <summary>
    /// 执行规划好的行动（敌人回合）
    /// </summary>
    public void ExecutePlannedAction()
    {
        // 清除预览
        ClearPreviews();
        hasPlannedAction = false;
        
        // 移动
        if (nextMoveTarget.x != currentX || nextMoveTarget.y != currentY)
        {
            MoveTo(nextMoveTarget.x, nextMoveTarget.y);
        }
        
        // 攻击
        if (nextAttackTarget.x != 0 || nextAttackTarget.y != 0)
        {
            Cell targetCell = GridManager.Instance.GetCell(nextAttackTarget.x, nextAttackTarget.y);
            if (targetCell != null && targetCell.HasUnit())
            {
                Attack(targetCell.unitOnCell);
            }
            else if (targetCell != null && targetCell.isBuilding)
            {
                targetCell.TakeDamage(attackDamage);
            }
        }
    }

    /// <summary>
    /// 攻击
    /// </summary>
    public override void Attack(BaseUnit target)
    {
        if (target != null && target.IsAlive())
        {
            target.TakeDamage(attackDamage);
        }
    }

    protected override void Die()
    {
        ClearPreviews();
        base.Die();
    }
}
