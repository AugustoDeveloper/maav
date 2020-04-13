#!/bin/sh

# Adapted from https://toedter.com/2018/06/02/heroku-docker-deployment-update/

APP_NAME=$1
HEROKU_AUTH_TOKEN=$2
IMAGE_ID=$(docker inspect registry.heroku.com/$APP_NAME/web --format={{.Id}})
payload='{"updates":[{"type":"web","docker_image":"'"$IMAGE_ID"'"}]}'

echo "App: $APP_NAME T: $HEROKU_AUTH_TOKEN I: $IMAGE_ID"

curl -n -X PATCH https://api.heroku.com/apps/$APP_NAME/formation \
-d "$payload" \
-H "Content-Type: application/json" \
-H "Accept: application/vnd.heroku+json; version=3.docker-releases" \
-H "Authorization: Bearer $HEROKU_AUTH_TOKEN"