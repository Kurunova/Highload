FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

RUN apt-get update && apt-get install -y zabbix-agent

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["SocialNetworkApi/SocialNetworkApi.csproj", "SocialNetworkApi/"]
RUN dotnet restore "SocialNetworkApi/SocialNetworkApi.csproj"
COPY . .
WORKDIR "/src/SocialNetworkApi"
RUN dotnet build "SocialNetworkApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "SocialNetworkApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
#ENTRYPOINT ["dotnet", "SocialNetworkApi.dll"]

CMD service zabbix-agent start && dotnet SocialNetworkApi.dll