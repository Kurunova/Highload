FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["SocialNetwork.DbMigrator/SocialNetwork.DbMigrator.csproj", "SocialNetwork.DbMigrator/"]
RUN dotnet restore "SocialNetwork.DbMigrator/SocialNetwork.DbMigrator.csproj"
COPY . .
WORKDIR "/src/SocialNetwork.DbMigrator"
RUN dotnet build "SocialNetwork.DbMigrator.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "SocialNetwork.DbMigrator.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SocialNetwork.DbMigrator.dll"]
