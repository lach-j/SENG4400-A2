FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["A2.Dashboard/A2.Dashboard.csproj", "A2.Dashboard/"]
RUN dotnet restore "A2.Dashboard/A2.Dashboard.csproj"
COPY . .
WORKDIR "/src/A2.Dashboard"
RUN dotnet build "A2.Dashboard.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "A2.Dashboard.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "A2.Dashboard.dll"]
