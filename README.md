# Clicker Browser Game

## Introduction
In this game, the player creates and manages a city. He builds production buildings, collects resources and improves his buildings. If resources were produced, the player can collect them by clicking on the appearing symbol. In his absence, the game continues and new resources are continually being produced.

## Getting started
### Requirements
* Unity3D 2019.1.5
* docker
* git lfs

### Installation
#### Building the client with Unity:
In a terminal:
```
Unity -batchmode -executeMethod WebGLBuilder.build -projectPath ./Client -nographics -quit
```

In Unity:
* Generate PHP classes using the main menu entry under PHP
* Select WebGL platform
* Build to Client/build

#### Building the backend
Build the backend docker container:
```
docker build -t projekt-php-2019 .
```
Without docker:
* Copy Server/api to <site>/api
* Copy Client/build/* to <site>/
* Create a database with utf8mb4 and add the user specified in Server/api/Database.php
* Modify Server/api/Database.php connection variables to your configuration
* Add `SetEnv admin\_credentials admin:<adminpassword>` to .htaccess

#### Run the application:
```
docker-compose -f docker-compose.yml up
```

#### Create database tables
Finally setup the database with a POST request to `/admin/setup`. Admin credentials can be found in `docker-compose.yml` under `admin_credentials`.

The mysql password (required to login in phpmyadmin) can also be found in `docker-compose.yml`.

The current configuration create both a production version (`https://localhost`) and a development version (`https://localhost/dev`). The development version is accessing the api files directly from source, while the production version is using the files from the container.

To remove the development version, delete the following lines from the Dockerfile:
```
VOLUME /var/www/site/dev
VOLUME /var/www/site/dev/api
```
