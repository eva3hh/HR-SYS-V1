FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["SmartHRAPI/SmartHRAPI.csproj", "SmartHRAPI/"]
RUN dotnet restore "SmartHRAPI/SmartHRAPI.csproj"

COPY . .
WORKDIR "/src/SmartHRAPI"
RUN dotnet build "SmartHRAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SmartHRAPI.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 5000
ENTRYPOINT ["dotnet", "SmartHRAPI.dll"]
