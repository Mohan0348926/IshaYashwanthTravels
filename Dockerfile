FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /app

COPY . .

RUN dotnet restore IshaYashwanthTravels/IshaYashwanthTravels.csproj

RUN dotnet publish IshaYashwanthTravels/IshaYashwanthTravels.csproj \
    -c Release \
    -o /out \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

WORKDIR /app

ENV DOTNET_USE_POLLING_FILE_WATCHER=true

COPY --from=build /out .

ENTRYPOINT ["dotnet", "IshaYashwanthTravels.dll"]
