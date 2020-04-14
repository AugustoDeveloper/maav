FROM mcr.microsoft.com/dotnet/core/runtime-deps:3.1-buster-slim AS base
WORKDIR /api
COPY ./app .
RUN chmod 777 ./MAAV.WebAPI
CMD ASPNETCORE_URLS=http://*:$PORT ./MAAV.WebAPI