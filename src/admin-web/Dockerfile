# 构建阶段
FROM node:20-alpine AS build
WORKDIR /app
COPY src/admin-web/package*.json ./
RUN npm install --legacy-peer-deps
COPY src/admin-web/ ./
RUN npm run build

# 运行阶段
FROM nginx:alpine AS runtime
COPY --from=build /app/dist /usr/share/nginx/html
COPY src/admin-web/nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
