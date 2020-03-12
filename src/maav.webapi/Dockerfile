FROM mcr.microsoft.com/dotnet/core/runtime-deps:3.1-buster-slim AS base
WORKDIR /api

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
COPY . .
RUN dotnet publish /src/maav.webapi/MAAV.WebAPI.csproj -c Release -o /api -r linux-x64 --self-contained

FROM base AS final
WORKDIR /app
COPY --from=build /api .
ENV ASPNETCORE_URLS=http://*:80

ENTRYPOINT [ "./MAAV.WebAPI" ]
