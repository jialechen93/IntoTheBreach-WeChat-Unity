# GitHub Actions 自动构建

你可以使用 GitHub Actions 免费自动构建 Unity WebGL，不需要本地安装 Unity！

## 设置步骤

### 1. 获取 Unity 授权文件

1. 登录 https://license.unity3d.com/
2. 下载你的 `Unity license file` (`Unity_license.ulf`)
3. 复制文件内容

### 2. 在 GitHub 设置 Secrets

在你的 GitHub 仓库 → `Settings → Secrets and variables → Actions → New repository secret`

添加这三个 Secrets：

| Name | Value |
|------|-------|
| `UNITY_EMAIL` | 你登录 Unity 的邮箱 |
| `UNITY_PASSWORD` | 你登录 Unity 的密码 |
| `UNITY_LICENSE` | 你的 `Unity_license.ulf` 文件内容 |

### 3. 触发构建

- 每次 push 到 main 分支自动构建
- 或者手动在 `GitHub → Actions → Build Unity WebGL → Run workflow` 触发

### 4. 下载构建结果

构建完成后，在 `Actions → Latest run → Artifacts → WebGL-Build` 下载 zip

解压后，把里面的文件放到 `Build/` 文件夹，就可以直接用微信开发者工具导入了！

## 优点

- ✅ 不需要本地安装 Unity
- ✅ 完全免费（GitHub Actions 免费配额够用）
- ✅ 推完代码等一会儿就能下载构建好的文件

## 注意

- Unity 免费版许可证就可以用
- 第一次构建需要下载 Unity，比较慢，后面有缓存就快了
