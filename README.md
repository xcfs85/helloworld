# 拼豆 (Pindou)

> 拼豆手工制作辅助应用 - 照片转拼豆图案，智能色号推荐

[![.NET](https://img.shields.io/badge/.NET-8.0-blue)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/license-MIT-green)](LICENSE)

## 项目简介

拼豆(Pindou)是一款面向拼豆手工爱好者的移动应用，核心功能包括：

- 📸 **照片转拼豆图案** - 智能识别照片并转换为拼豆图纸
- 🎨 **多色号支持** - 支持MARD、漫漫、丫丫、HAMA等主流品牌色板
- 🛒 **色号推荐** - 基于图案智能推荐采购清单
- 👥 **社区分享** - 用户作品展示与交流（规划中）
- 💎 **会员服务** - 会员专属权益与高级功能

## 产品路线图

| 版本 | 时间 | 功能 |
|------|------|------|
| MVP | 0-2个月 | 注册登录、照片转拼豆(MARD)、色号推荐、会员订阅 |
| V1.0 | 2-4个月 | 论坛、经典拼图模板、MARD+漫漫+丫丫色板 |
| V1.5 | 4-6个月 | 引入HAMA高端色板 |
| V2.0 | 6个月+ | 电商导流、全部品牌+自定义色板 |

## 技术架构

### 后端技术栈

```
┌─────────────────────────────────────────────────────────┐
│                      应用层 (Application)                 │
│         DTOs / Service Interfaces / Common              │
├─────────────────────────────────────────────────────────┤
│                       领域层 (Domain)                     │
│            Entities / Enums / Interfaces                 │
├─────────────────────────────────────────────────────────┤
│                   基础设施层 (Infrastructure)              │
│      Repositories / Data / External Services            │
├─────────────────────────────────────────────────────────┤
│                      共享层 (Shared)                      │
│          Extensions / Attributes / Utilities            │
└─────────────────────────────────────────────────────────┘
```

- **框架**: .NET 8.0 + ASP.NET Core
- **架构模式**: DDD (Domain-Driven Design) 分层架构
- **数据库**: PostgreSQL 8 + Redis 7
- **认证**: JWT
- **AI服务**: 阿里云 GPU (按量付费)

### 项目结构

```
pindou/
├── Code/                          # 项目源码
│   ├── src/
│   │   ├── Pindou.Api/            # 移动端 API (端口 5000)
│   │   ├── Pindou.Admin.Api/      # 后台管理 API (端口 5100)
│   │   ├── Pindou.Application/    # 应用层
│   │   ├── Pindou.Domain/         # 领域层
│   │   ├── Pindou.Infrastructure/ # 基础设施层
│   │   ├── Pindou.Shared/         # 共享工具
│   │   └── Pindou.Tests/          # 单元测试
│   ├── Pindou.sln                 # 解决方案文件
│   └── docker-compose.yml         # 容器编排
├── UI/                            # 前端原型
│   ├── APP/                       # 移动端原型
│   └── Back/                      # 管理后台原型
├── 01-拼豆软件产品规划/             # 产品规划文档
├── 02-需求分析/                    # 需求规格说明书
├── 03-总体设计/                    # 架构设计文档
├── 04-概要设计/                    # 模块概要设计
├── 05-详细设计/                    # 模块详细设计
├── deploy/                        # 部署脚本
└── README.md                      # 项目说明
```

### 端口约定

| 服务 | 端口 |
|------|------|
| Pindou.Api（移动端） | 5000 |
| Pindou.Admin.Api（后台） | 5100 |
| Admin Web（开发） | 8080 |
| PostgreSQL | 5432 |
| Redis | 6379 |

## 快速开始

### 前置要求

- .NET 8.0 SDK
- Docker & Docker Compose
- PostgreSQL 8+ / Redis 7+ (可使用Docker)

### 本地开发

```bash
# 1. 克隆项目
git clone https://github.com/xcfs85/helloworld.git
cd helloworld/pindou

# 2. 启动基础服务
docker compose up -d pindou-db pindou-redis

# 3. 还原并编译
cd Code
dotnet restore
dotnet build

# 4. 启动移动端 API
cd src/Pindou.Api
dotnet run

# 5. 启动后台管理 API (新终端)
cd src/Pindou.Admin.Api
dotnet run
```

### Docker 部署

```bash
# 启动所有服务
docker compose up -d

# 查看日志
docker compose logs -f

# 停止服务
docker compose down
```

## 核心功能模块

### 移动端 API (Pindou.Api)

| 模块 | 描述 |
|------|------|
| AuthController | 用户注册、登录、Token管理 |
| UserController | 用户信息、资料修改 |
| DiagramController | 拼豆图纸生成、查询、导出 |
| TemplateController | 模板管理 |
| MemberController | 会员订阅、权益 |
| CommunityController | 社区帖子、评论（规划中） |
| MessagingController | 消息推送 |

### 后台管理 API (Pindou.Admin.Api)

| 模块 | 描述 |
|------|------|
| AuthController | 管理员登录 |
| UserController | 用户管理 |
| ContentController | 内容审核 |
| TemplateController | 模板管理 |
| StatisticsController | 数据统计 |
| SystemController | 系统配置 |

## 设计文档

- [产品规划](01-拼豆软件产品规划/产品规划_v0.2.md)
- [需求规格说明书](02-需求分析/需求规格说明书_v0.1.md)
- [总体架构设计](03-总体设计/总体架构设计文档_ v0.2. md)
- [数据库设计](03-总体设计/后台设计/数据库设计说明书_ v1.0. md)
- [各模块详细设计](05-详细设计/)

## 贡献指南

欢迎提交 Issue 和 Pull Request！

## 许可证

MIT License - 详见 [LICENSE](LICENSE) 文件

---

Made with ❤️ by Pindou Team