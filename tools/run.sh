#!/bin/bash
docker="docker"

if [ "$1" == "$docker" ];
then
    echo "Building web api applicaiton"
    docker-compose build

    echo "Up web api applicaiton"
    docker-compose up
else
    echo 2
    dotnet run -p src/maav.webapi/ 
fi