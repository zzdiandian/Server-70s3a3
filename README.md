# DNF70 70s3a3 协作工程

以 FG175 已交付的145份隔离源码为历史基线；当前src含暗黑城等变更，共150份开发候选源码，联合修复候选为155份，供共同开发、审查与合并。这里不是根目录旧 ACT4 工程，也不是完整游戏安装包。

- `src/Tools/VmTestKit/`：公共服务开发源码；当前变更不等同于已发布玩家版本。
- `data/`：稳定服务所需目录和表；以实际文件哈希为准。
- `research/tower/`：152 份未发布的塔研究源码及数据，独立构建，不能直接覆盖稳定服务。
- `docs/MERGE_BOUNDARIES.md`：模块边界、共享文件和冲突处理。
- `CONTRIBUTING.md`：新增功能与合并流程。
- `docs/FEATURE_STATUS.md`：已交付范围和真实未完成项。
- `baseline/manifest.json`：导入来源及逐文件 SHA256；这是导入快照，不是每次修改后重写的清单。

## 编译与检查

Windows、Python 3.10+、.NET Framework C# 编译器。优先使用 Framework v3.5；没有时可用 v4 编译器做编译检查，输出不保证与历史二进制相同。

```powershell
python scripts/check_repo.py
python scripts/build.py --smoke
python scripts/build.py --research --smoke
```

只编译并执行无参数返回码检查，不启动游戏、监听服务或读取玩家存档。稳定输出在 `build/stable/`，塔草稿在 `build/tower-research/`。研究区不会自动加入稳定编译。

CI 只证明结构检查、编译和无参数检查通过。真实请求、原收包、保存重登及真人游戏验收需要在独立环境完成，并在 PR 中附结果，不能把 CI 绿灯写成副本已可用。

原版客户端、PVF、账号存档、密码/令牌、玩家日志和可执行文件不入库。需要游戏测试时，自行提供匹配的本地资源并校验上述版本；本仓库不自动安装、启动或迁移游戏。当前启动器及客户端二进制仍在原本地交付目录。

## 协作

每人一个 `feature/<主题>` 或 `fix/<主题>` 分支，提交 PR。`main` 保持可编译的稳定状态；研究改动先进入 `research/`。仓库管理员按 `docs/GITHUB_SETTINGS.md` 设置分支保护，并在 GitHub 仓库设置中添加朋友。

本仓库尚未声明开源许可证。历史导入、改写代码和原版资源的权属不同，不能因为仓库公开就推定所有内容可自由再发布。

当前合并范围、联合构建和未开放条件见 [本批集成记录](docs/INTEGRATION-2026-10-15.md) 与 [155源候选](research/integration/README.md)。原基线manifest不改写，原玩家FG175/v48保持不变。

塔的后续接续使用[162源独立联合候选](research/tower-integration/README.md)，保留上述155源修复；不替换公共源码或玩家入口，完整塔仍未发布。
