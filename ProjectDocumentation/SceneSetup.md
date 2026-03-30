# 场景搭建步骤

## 层级结构建议

```
MainCamera (主相机)
└── MainCamera 自带音频监听器
GameManager
├── GridManager (GridManager 组件)
│   └── Grid (运行时自动生成)
├── TurnManager (TurnManager 组件)
│   ├── PlayerMechs (父物体)
│   │   ├── Mech1 (PlayerMech 组件 + 碰撞器 + SpriteRenderer)
│   │   ├── Mech2 (PlayerMech 组件 + 碰撞器 + SpriteRenderer)
│   │   └── Mech3 (PlayerMech 组件 + 碰撞器 + SpriteRenderer)
│   ├── Enemies (父物体)
│   │   ├── Enemy1 (EnemyUnit 组件 + 碰撞器 + SpriteRenderer)
│   │   └── ...根据关卡需要添加
│   └── Buildings
│       ├── Building1 (在对应 Cell 上设置 isBuilding = true)
│       └── ...
InputManager (InputManager 组件)
WeChatManager
├── WeChatGameManager (WeChatGameManager 组件)
└── WeChatAdManager (WeChatAdManager 组件)
Canvas (UI)
├── TurnText (Text - 显示回合信息)
├── EndTurnButton (Button - 结束回合按钮)
└── GameOverPanel
    ├── TitleText (Text)
    ├── DescText (Text)
    ├── RestartButton (Button)
    ├── BackButton (Button)
    └── ShareButton (Button)
EventSystem (Unity UI 自动生成)
```

## 组件参数设置参考

### GridManager
- width: 8
- height: 8
- cellSize: 1
- cellPrefab: 一个带 SpriteRenderer 和 Cell 组件的预制件

### Cell Prefab
- SpriteRenderer: 一个方形精灵，灰色
- Cell 组件: 无需修改默认参数

### PlayerMech 参数示例
- 格斗型 (Fighter)
  - maxHealth: 4
  - attackDamage: 2
  - moveRange: 3
  - attackRange: 1
  - armor: 1

- 狙击型 (Sniper)
  - maxHealth: 3
  - attackDamage: 3
  - moveRange: 3
  - attackRange: 3
  - armor: 0

- 坦克型 (Tank)
  - maxHealth: 6
  - attackDamage: 1
  - moveRange: 2
  - attackRange: 1
  - armor: 2

### EnemyUnit 参数示例
- Spitter (喷吐者)
  - maxHealth: 2
  - attackDamage: 1
  - moveRange: 2
  - attackRange: 2

- Beetle (甲虫)
  - maxHealth: 3
  - attackDamage: 2
  - moveRange: 3
  - attackRange: 1

## 相机设置

- Projection: Orthographic
- Size: 5 ~ 6（根据网格大小调整）
- Position: (0, 0, -10)
- Background Color: 深灰色 (0.2, 0.2, 0.2)

## 层级排序（Sorting Layer）

建议设置：
- Background: 0
- Grid: 1
- Units: 2
- UI: 3

这样可以保证正确的遮挡关系。

## 开局地图示例

一个简单的第一关布局：

```
网格大小：8x8
玩家机甲：3个，放置在下方 (2,1), (4,1), (6,1)
敌人：3个，放置在上方 (2,6), (4,6), (6,6)
建筑：2-3个，放置在中间位置
```

这样玩家在下，敌人在上，建筑在中间，玩家需要保护建筑不被敌人摧毁。

## 碰撞器设置

- 每个单位需要添加一个 2D 碰撞器（BoxCollider2D）
- 单元格不需要碰撞器，我们是通过格子坐标计算的
- IsTrigger: 不需要，我们没有用到物理碰撞

## 微信SDK需要注意

1. 导入 WeChatSDK 后，在 Player Settings 中：
   - Scripting Runtime Version: .NET 4.x
   - Api Compatibility Level: .NET 4.x

2. 构建 WebGL 时：
   - Enable Exceptions: Full Exceptions（开发阶段，发布可以改成 None）
   - WebGL templates: 选择 WeChat 提供的模板

3. 在微信开发者工具中：
   - 开启「不校验合法域名」开发调试
   - 发布前一定要配置好合法域名
