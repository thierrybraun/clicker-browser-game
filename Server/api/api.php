<?php
declare (strict_types = 1);

require_once 'Router.php';
require_once 'Database.php';
require_once 'PlayerManager.php';
require_once 'CityManager.php';
require_once 'Debug.php';
require_once 'Admin.php';

$tickDuration = 10;

$database = new Database();
$router = new Router($database);
$cityManager = new CityManager($database, $tickDuration);
$playerManager = new PlayerManager($database, $cityManager);
$debug = new Debug($cityManager);
$admin = new Admin($database);

$router->get('api/login', function () {
    return array('Success' => true);
});
$router->post('api/register', [$playerManager, 'register'], false);
$router->get('api/player/me', [$playerManager, 'getMe']);
$router->get('api/player/{id}', [$playerManager, 'getById']);
$router->get('api/player/{id}/city', [$cityManager, 'getCityByPlayerId']);
$router->post('api/city/{id}/building', [$cityManager, 'createBuilding']);
$router->get('api/city/{id}', [$cityManager, 'getCityById']);
$router->get('api/city/{id}/stash/{x}/{y}', [$cityManager, 'getStash']);
$router->post('api/city/{id}/collect/{x}/{y}', [$cityManager, 'collect']);
$router->post('api/city/{id}/upgrade', [$cityManager, 'upgrade']);

$router->get('api/version', function () {
    return array('Version' => 'indev');
}, false);

$router->get('api/test', function () use ($debug) {
    header('Content-type:text/html;charset=utf-8');
    echo $debug->createTerrainVisualization();
    die();
}, false);

$router->post('api/admin/setup', [$admin, 'setup'], false);

$router->process($database);
