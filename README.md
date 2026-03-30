# 陷阵之志 微信小游戏版 - Into the Breach WeChat Mini Game

基于 Unity 开发的类似《陷阵之志》（Into the Breach）的回合制机甲策略游戏，可直接发布到微信小游戏。

## 游戏特色

- ✅ 完整复刻《陷阵之志》核心玩法：
  - 8x8 网格棋盘
  - 玩家回合 + 敌人行动预测机制
  - 回合制机甲战斗
  - 需要保护建筑物（能量核心）
  - 敌人下一步可预测

- ✅ 适配微信小游戏：
  - 触摸操作优化
  - 自动屏幕适配
  - 微信登录集成
  - 分享功能
  - 广告变现支持

- ✅ 多种机甲类型：
  - 格斗型 - 近战高伤害
  - 狙击型 - 远程攻击
  - 坦克型 - 高血量高防御

- ✅ 多种敌人类型：
  - 喷吐者 - 远程攻击
  - 甲虫 - 近战冲撞
  - 萤火虫 - 飞行单位
  - BOSS

## 项目结构

```
IntoTheBreachWechat/
├── Assets/
│   ├── Scripts/
│   │   ├── Core/              # 核心系统
│   │   │   ├── GridManager.cs     # 网格管理
│   │   │   ├── Cell.cs            # 单元格
│   │   │   └── InputManager.cs    # 输入管理
│   │   ├── Gameplay/           # 游戏逻辑
│   │   │   ├── BaseUnit.cs        # 单位基类
│   │   │   ├── PlayerMech.cs      # 玩家机甲
│   │   │   ├── EnemyUnit.cs       # 敌人单位
│   │   │   └── TurnManager.cs     # 回合管理
│   │   ├── UI/                # UI
│   │   │   └── GameUIManager.cs   # 游戏UI管理
│   │   └── WeChat/            # 微信小游戏适配
│   │       ├── WeChatGameManager.cs    # 微信游戏管理
│   │       └── WeChatAdManager.cs      # 广告管理
│   ├── Prefabs/              # 预制件
│   ├── Scenes/               # 场景
│   └── Sprites/              # 图片资源
├── ProjectSettings/          # Unity 项目设置
└── README.md
```

## 开发环境要求

1. Unity 2021.3 或更高版本（推荐 2022 LTS）
2. 安装 WeChat Mini Game SDK for Unity
   - 下载地址：https://github.com/wechatdevelop/unity-wechat-minigame
   - 或者通过 Package Manager 导入

## 构建发布流程

### 1. 导入项目
- 打开 Unity Hub
- 点击「打开」，选择 `IntoTheBreachWechat` 文件夹
- 等待项目导入

### 2. 导入微信小游戏SDK
- 下载 WeChat Mini Game SDK
- 导入到 Unity 项目中
- SDK 会自动配置好环境

### 3. 设置场景
- 打开 `Assets/Scenes/MainGame.unity`（需要你在Unity中创建并设置好引用）
- 将各个脚本挂载到对应 GameObject：
  - `GridManager` -> Grid 对象
  - `TurnManager` -> GameManager 对象
  - `InputManager` -> GameManager 对象
  - `WeChatGameManager` -> WeChatManager 对象

### 4. 构建设置
- 打开 `File -> Build Settings`
- 选择 `WebGL` 平台
- 点击 `Switch Platform`
- 在 `Player Settings` 中：
  - 设置 `Resolution and Presentation` -> 分辨率适配手机
  - 设置 `Other Settings` -> `Rendering Color Space` -> Gamma
  - 勾选 `WebGL -> Memory Size` -> 设置为 512MB 或更高

### 5. 构建输出
- 点击 `Build`，输出到 `Build` 文件夹
- 打开微信开发者工具
- 导入这个文件夹
- 填写你的小游戏 AppID
- 点击上传即可提交审核

### 6. 微信后台配置
- 登录微信公众平台
- 进入你的小游戏
- 在「开发」->「开发设置」中：
  - 配置服务器域名（如果需要联网）
  - 开通广告单元（如果要变现）
  - 填写游戏基本信息

## 发布注意事项

### 包体积控制
- 图片资源建议压缩后再导入
- 使用 Unity 的 Sprite Packer 打包
- 音乐音效使用压缩格式
- 微信小游戏限制包体积为 4MB（包体积超过需要分包）

### 分包配置
如果包体积超过 4MB，可以配置分包：
1. 在 `Assets/StreamingAssets` 文件夹存放资源
2. 在微信开发者工具中配置分包
3. 游戏启动时下载资源

### 审核注意事项
- 需要提供用户协议和隐私政策链接
- 如果接入广告，需要在游戏中展示广告说明
- 分享功能不要诱导分享
- 不要包含违规内容

## 游戏玩法说明

1. **玩家回合**：你控制三个机甲轮流行动
   - 每个机甲可以移动一次 + 攻击一次
   - 点击机甲选中，点击目标格子移动或攻击
   - 可以看到敌人下一回合的行动目标
   - 安排好站位保护建筑

2. **敌人回合**：敌人按照预告的行动执行
   - 如果你的机甲在敌人攻击范围内，会被攻击
   - 如果建筑被摧毁太多，游戏失败

3. **胜利条件**：消灭所有敌人即为胜利

## 核心机制

- **敌人行动可见**：在你行动前，就能看到每个敌人下一回合要去哪里、攻击谁
- **推进机制**：近战攻击可以把敌人推开，撞到墙壁或建筑会造成额外伤害
- **保护建筑**：建筑不仅是胜利条件，也会提供能量/分数
- **回合制策略**：每一步都要考虑后果

## 开发扩展

你可以继续扩展：
- 添加更多机甲类型和技能
- 添加更多敌人类型
- 添加关卡系统
- 添加道具系统
- 添加机甲升级系统
- 添加不同地图布局

## License

MIT License
