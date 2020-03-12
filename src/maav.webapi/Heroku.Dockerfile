FROM mcr.microsoft.com/dotnet/core/runtime-deps:3.1-buster-slim AS base
WORKDIR /api

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
COPY . .
WORKDIR /src/maav.webapi
RUN dotnet publish MAAV.WebAPI.csproj -c Release -o /api -r linux-x64 --self-contained

FROM base AS final
WORKDIR /app
COPY --from=build /api .

CMD ASPNETCORE_URLS=http://*:$PORT dotnet MAAV.WebAPI.Server.dll