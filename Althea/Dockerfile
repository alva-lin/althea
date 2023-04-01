FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["backend/", "."]
RUN dotnet restore "Althea/Althea.csproj" /ignoreprojectextensions:.dcproj

FROM build AS publish
WORKDIR "/src/Althea"
RUN dotnet publish "Althea.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Althea.dll"]
