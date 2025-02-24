FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Poll.Api/Poll.Api.csproj", "Poll.Api/"]
COPY ["Poll.Infrastructure/Poll.Infrastructure.csproj", "Poll.Infrastructure/"]
COPY ["Poll.Core/Poll.Core.csproj", "Poll.Core/"]
RUN dotnet restore "Poll.Api/Poll.Api.csproj"
COPY . .
WORKDIR "/src/Poll.Api"
RUN dotnet build "Poll.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Poll.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Poll.Api.dll"]
