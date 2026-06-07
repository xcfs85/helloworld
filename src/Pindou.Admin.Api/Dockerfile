# 基础镜像
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Pindou.sln", "./"]
COPY ["src/Pindou.Domain/Pindou.Domain.csproj", "src/Pindou.Domain/"]
COPY ["src/Pindou.Application/Pindou.Application.csproj", "src/Pindou.Application/"]
COPY ["src/Pindou.Infrastructure/Pindou.Infrastructure.csproj", "src/Pindou.Infrastructure/"]
COPY ["src/Pindou.Shared/Pindou.Shared.csproj", "src/Pindou.Shared/"]
COPY ["src/Pindou.Api/Pindou.Api.csproj", "src/Pindou.Api/"]
COPY ["src/Pindou.Admin.Api/Pindou.Admin.Api.csproj", "src/Pindou.Admin.Api/"]

RUN dotnet restore "src/Pindou.Admin.Api/Pindou.Admin.Api.csproj"
COPY . .
WORKDIR /src/src/Pindou.Admin.Api
RUN dotnet publish "Pindou.Admin.Api.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Pindou.Admin.Api.dll"]
