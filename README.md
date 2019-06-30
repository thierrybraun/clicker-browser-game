# Clicker Browser Game, a school project

## Introduction
In this game, the player creates and manages a city. He builds production buildings, collects resources and improves his buildings. If resources were produced, the player can collect them by clicking on the appearing symbol. In his absence, the game continues and new resources are continually being produced.

## Getting started
### Requirements
* Unity3D 2019.1.5
* docker

### Installation
Build the client with Unity:
```
Unity -batchmode -executeMethod WebGLBuilder.build -projectPath ./Client -nographics -quit
```

Build the backend docker container:
```
docker build -t projekt-php-2019 .
```

Run the application:
```
docker-compose -f docker-compose.yml up
```

The current configuration create both a production version (`https://localhost`) and a development version (`https://localhost/dev`). The development version is accessing the api files directly from source, while the production version is using the files from the container.

To remove the development version, delete the following lines from the Dockerfile:
```
VOLUME /var/www/site/dev
VOLUME /var/www/site/dev/api
```
