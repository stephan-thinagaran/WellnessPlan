FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Directory.Build.props", "."]
COPY ["Directory.Packages.props", "."]
COPY ["nuget.config", "."]
COPY ["WellnessPlan.sln", "."]
COPY ["src/WellnessPlan.WebApi/WellnessPlan.WebApi.csproj", "src/WellnessPlan.WebApi/"]
RUN dotnet restore "src/WellnessPlan.WebApi/WellnessPlan.WebApi.csproj"
COPY . .
WORKDIR "/src/src/WellnessPlan.WebApi"
RUN dotnet build "WellnessPlan.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WellnessPlan.WebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WellnessPlan.WebApi.dll"]
