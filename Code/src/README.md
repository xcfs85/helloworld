# 拼豆（Pindou）项目框架

> 本目录为拼豆项目的工程源码与设计文档。

## 1. 目录结构

```
src/
├── 01.Domain/                # 领域层：实体、枚举
├── 02.Application/           # 应用层：DTO、Service 接口
├── 03.Infrastructure/        # 基础设施层：DB、缓存、外部服务
├── 04.Shared/                # 公共工具、扩展、Attribute
├── 05.Api/                   # 移动端 HTTP API（端口 5000）
├── 06.Admin.Api/             # 后台管理 HTTP API（端口 5100）
├── admin-web/                # 后台管理 Web（Vue 3 + Element Plus）
├── mobile-app/               # 移动端 App（Flutter）
└── docs/                     # 项目框架设计文档
```

## 2. 关键文档

- [项目框架设计说明书](docs/项目框架设计说明书.md)：包含总体技术架构、解决方案组织、各层职责、关键设计模式、数据库要点、安全设计、部署架构、CI/CD、测试策略等。

## 3. 相关设计文档

- `../03-总体设计/总体架构设计文档_ v0.2. md`
- `../03-总体设计/后台设计/数据库设计说明书_ v1.0. md`
- `../04-概要设计/`、`../05-详细设计/`

## 4. 快速开始

```bash
# 启动基础服务
docker compose up -d pindou-db pindou-redis

# 启动移动端 API
cd 05.Api/Pindou.Api && dotnet run

# 启动后台 API
cd 06.Admin.Api/Pindou.Admin.Api && dotnet run

# 启动后台 Web
cd admin-web && npm install && npm run dev

# 启动移动端
cd mobile-app && flutter run
```

或一键执行：

```bash
bash ../deploy/scripts/dev.sh
```

## 5. 端口约定

| 服务 | 端口 |
| --- | --- |
| Pindou.Api（移动端） | 5000 |
| Pindou.Admin.Api（后台） | 5100 |
| Admin Web（开发服务器） | 8080 |
| PostgreSQL | 5432 |
| Redis | 6379 |
| Nginx（生产入口） | 80/443 |
