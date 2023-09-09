FROM mcr.microsoft.com/dotnet/sdk:6.0 as build

WORKDIR /app

COPY *.csproj ./

RUN dotnet restore

COPY . .

RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS runtime

WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "KafkaWorkerService.dll"]