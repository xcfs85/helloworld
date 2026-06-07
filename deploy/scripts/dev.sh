#!/bin/bash
# 拼豆项目一键启动脚本 (开发环境)

set -e
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "$SCRIPT_DIR/../.."

echo "======================================="
echo "  拼豆项目启动脚本"
echo "======================================="

# 启动基础服务
if command -v docker-compose &> /dev/null; then
    echo ">>> 启动 Docker 服务 (PostgreSQL, Redis)"
    docker-compose up -d pindou-db pindou-redis
elif command -v docker &> /dev/null; then
    echo ">>> 启动 Docker 服务 (PostgreSQL, Redis)"
    docker compose up -d pindou-db pindou-redis
fi

# 启动 API
echo ">>> 启动 Pindou.Api (端口:5000)"
cd src/Pindou.Api
dotnet run --no-launch-profile &
API_PID=$!
cd ../..

# 启动 Admin API
echo ">>> 启动 Pindou.Admin.Api (端口:5100)"
cd src/Pindou.Admin.Api
dotnet run --no-launch-profile &
ADMIN_PID=$!
cd ../..

# 启动 Admin Web
echo ">>> 启动 Admin Web (端口:8080)"
cd src/admin-web
if [ ! -d node_modules ]; then
    npm install --legacy-peer-deps
fi
npm run dev &
WEB_PID=$!
cd ../..

echo "======================================="
echo "  启动完成"
echo "  Pindou.Api:        http://localhost:5000"
echo "  Pindou.Admin.Api:  http://localhost:5100"
echo "  Admin Web:         http://localhost:8080"
echo "  PostgreSQL:        localhost:5432 (pindou/Pindou@2026)"
echo "  Redis:             localhost:6379 (Pindou@Redis2026)"
echo "======================================="

trap "kill $API_PID $ADMIN_PID $WEB_PID 2>/dev/null || true" EXIT
wait
