# 项目专属配置 - Pindou 拼豆应用

## 加载知识库

### C# 高级开发工程师
```
C:\Users\xcf\Documents\提示词\C#高级开发工程师.md
```

## 项目技术栈

- **框架**: .NET 8.0 + ASP.NET Core
- **架构模式**: DDD (Domain-Driven Design) 分层架构
- **数据库**: PostgreSQL 8 + Redis 7
- **ORM**: SqlSugar
- **认证**: JWT
- **AI服务**: 阿里云 GPU

## 项目结构

```
pindou/Code/
├── src/
│   ├── Pindou.Api/            # 移动端 API (端口 5000)
│   ├── Pindou.Admin.Api/      # 后台管理 API (端口 5100)
│   ├── Pindou.Application/    # 应用层
│   ├── Pindou.Domain/         # 领域层
│   ├── Pindou.Infrastructure/ # 基础设施层
│   ├── Pindou.Shared/         # 共享工具
│   └── Pindou.Tests/          # 单元测试
└── Pindou.sln                 # 解决方案文件
```

## 端口约定

| 服务 | 端口 |
|------|------|
| Pindou.Api（移动端） | 5000 |
| Pindou.Admin.Api（后台） | 5100 |
| Admin Web（开发） | 80 |
| PostgreSQL | 5432 |
| Redis | 6379 |

## 本地开发命令

```bash
# 启动基础服务
docker compose up -d pindou-db pindou-redis

# 编译项目
cd Code
dotnet build
```

## 开发约定

1. 代码修改前先说明思路
2. 优先使用原生 API，避免引入不必要的依赖
3. 代码中不要输出任何密钥或 token 等敏感信息
4. 开发完成必须编译项目确保通过