# Prefabs 目录

这里放预制件：

## 需要创建的预制件：

### 1. 玩家机甲预制件

创建三个不同类型的机甲预制件：
- `FighterMech.prefab` - 格斗型
  - PlayerMech 组件
  - maxHealth: 4
  - attackDamage: 2
  - moveRange: 3
  - attackRange: 1
  - armor: 1
  - type: Fighter

- `SniperMech.prefab` - 狙击型
  - PlayerMech 组件
  - maxHealth: 3
  - attackDamage: 3
  - moveRange: 3
  - attackRange: 3
  - armor: 0
  - type: Sniper

- `TankMech.prefab` - 坦克型
  - PlayerMech 组件
  - maxHealth: 6
  - attackDamage: 1
  - moveRange: 2
  - attackRange: 1
  - armor: 2
  - type: Tank

### 2. 敌人预制件

- `SpitterEnemy.prefab` - 喷吐者
  - EnemyUnit 组件
  - maxHealth: 2
  - attackDamage: 1
  - moveRange: 2
  - attackRange: 2
  - type: Spitter

- `BeetleEnemy.prefab` - 甲虫
  - EnemyUnit 组件
  - maxHealth: 3
  - attackDamage: 2
  - moveRange: 3
  - attackRange: 1
  - type: Beetle

- `FireflyEnemy.prefab` - 萤火虫
  - EnemyUnit 组件
  - maxHealth: 2
  - attackDamage: 1
  - moveRange: 4
  - attackRange: 2
  - type: Firefly

### 3. 预览标记预制件

- `MovePreview.prefab` - 敌人下一步移动位置预览（蓝色半透明）
- `AttackPreview.prefab` - 敌人下一步攻击位置预览（红色半透明）

### 4. Cell 预制件

- `Cell.prefab` - 带 SpriteRenderer 和 Cell 组件
