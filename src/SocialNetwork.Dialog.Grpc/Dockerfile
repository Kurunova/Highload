FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["SocialNetwork.Dialog.Grpc/SocialNetwork.Dialog.Grpc.csproj", "SocialNetwork.Dialog.Grpc/"]
RUN dotnet restore "SocialNetwork.Dialog.Grpc/SocialNetwork.Dialog.Grpc.csproj"
COPY . .
WORKDIR "/src/SocialNetwork.Dialog.Grpc"
RUN dotnet build "SocialNetwork.Dialog.Grpc.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "SocialNetwork.Dialog.Grpc.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SocialNetwork.Dialog.Grpc.dll"]
