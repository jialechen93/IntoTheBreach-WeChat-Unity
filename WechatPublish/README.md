# 微信小游戏发布上架指南

本文档帮你一步步发布到微信小游戏后台

## 前置准备

### 1. 注册微信公众号/小程序
- 打开 https://mp.weixin.qq.com/
- 注册「小程序」，选择「游戏」类目
- 认证需要 300 元认证费

### 2. 获了 AppID
- 登录后 → 开发 → 开发设置
- 复制你的 `AppID`
- 在 `project.config.json` 中替换 `touristappid` 为你的 AppID

## Unity 构建输出

### 1. 切换平台
- `File → Build Settings` → 选择 `WebGL`
- 点击 `Switch Platform`

### 2. Player Settings
- `Edit → Project Settings → Player`
- **Other Settings**:
  - `Scripting Runtime Version`: `.NET 4.x`
  - `Api Compatibility Level`: `.NET 4.x`
  - `Color Space`: `Gamma`
  - `Allow unsafe code`: 勾选
- **Resolution and Presentation**:
  - `Default Canvas Width`: 1080
  - `Default Canvas Height`: 1920
  - `Run in background`: 不勾选
- **WebGL**:
  - `Memory Size`: `1024` (MB)
  - `Enable Exceptions`: `None` (发布版选None减小体积)

### 3. 构建
- 点击 `Build`
- 输出目录选择 `Build/` 文件夹（就是当前这个目录）
- 等待构建完成

## 微信开发者工具导入

### 1. 下载微信开发者工具
- https://developers.weixin.qq.com/miniprogram/dev/devtools/download.html
- 安装并登录

### 2. 导入项目
- 点击「导入项目」
- 目录选择 `Build/` 文件夹
- AppID 填写你的
- 点击导入

### 3. 配置域名（重要！）
如果你没有联网请求，可以跳过。如果有请求（比如登录、云端存档）：
- 微信公众平台 → 开发 → 开发设置
- 「服务器域名」→ 「request合法域名」添加你的服务器域名
- 开发阶段可以勾选「不校验合法域名」

## 提审上架

### 1. 测试
- 在微信开发者工具测试一遍所有功能：
  - ✅ 游戏能正常启动
  - ✅ 点击操作正常
  - ✅ 胜利/失败/升级正常
  - ✅ 分享按钮正常
  - ✅ 暂停菜单正常

### 2. 填写版本信息
- 微信公众平台 → 管理 → 版本管理
- 点击「提交审核」
- 需要填写：
  - **功能介绍**: 类似陷阵之志的回合制机甲策略游戏，保护城市，击退敌人！每次地图都不一样，通关升级你的机甲。
  - **截图**: 准备 4-6 张游戏截图（尺寸 1080x1920）
  - **图标**: 准备 512x512 png 图标
  - **版本号**: 1.0.0
  - **更新说明**: 首发版本，回合制机甲策略游戏

### 3. 提交审核
- 提交后等待审核，一般 1-3 天会出结果
- 审核通过后就可以点击「发布」，正式上线了！

## 需要准备的材料

| 材料 | 尺寸要求 | 说明 |
|------|---------|------|
| 游戏图标 | 512x512 PNG | 游戏logo |
| 启动封面 | 1080x1920 PNG | 游戏启动图 |
| 游戏截图 | 1080x1920 PNG × 4-6张 | 不同游戏画面 |
| 用户协议 | 文本 | 用户隐私协议 |
| 隐私政策 | 文本 | 符合微信要求 |

## 审核注意事项

1. **不能有诱导分享** - 不要说"分享得好礼"、"分享复活"这种
2. **必须有隐私政策** - 如果收集用户信息，必须有隐私政策链接
3. **不能有违规内容** - 政治、色情、赌博这些都不行
4. **适配手机屏幕** - 确保在手机上能正常触摸操作

## 广告变现（可选）

如果你想要变现：
1. 在微信公众平台开通流量主
2. 在 `WeChatAdManager.cs` 中填写你的广告单元ID
3. 可以在游戏失败、关卡结束插屏广告
4. 可以做"看广告复活"用激励视频

详细已经写在 `Assets/Scripts/WeChat/WeChatAdManager.cs` 里了。

## 常见问题

**Q: 包体积超过 4MB 怎么办？**
A: 使用微信分包功能，把资源放到分包里，或者压缩图片和音频。

**Q: 提示 "找不到 WeChatWASM" 怎么办？**
A: 说明你没导入微信SDK，回去看导入步骤。

**Q: 点击没反应怎么办？**
A: 检查 EventSystem 是否在场景中，这是 Unity UI 必需的。

**Q: 卡在 90% 加载不开怎么办？**
A: 内存不够，在 Player Settings 把 WebGL Memory Size 调到 1024MB。
