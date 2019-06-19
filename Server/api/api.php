<?php
declare (strict_types = 1);

require_once 'Router.php';
require_once 'Database.php';
require_once 'PlayerManager.php';
require_once 'CityManager.php';
require_once 'Debug.php';

$database = new Database();
$router = new Router($database);
$cityManager = new CityManager($database);
$playerManager = new PlayerManager($database, $cityManager);
$debug = new Debug($cityManager);

$router->post('api/register', function () use ($playerManager) {
    try {
        $username = $_POST['username'];
        $password = $_POST['password'];
        $playerManager->createPlayer($username, $password);        
    } catch (\Throwable $th) {
        return array(
            'Success' => false,
            'Error' => $th->getMessage()
        );
    }
    return array(
        'Success' => true,
    );
}, false);
$router->get('api/login', function () {
    $res['Success'] = true;
    return $res;
});
$router->get('api/player/{id}', function ($id) use ($playerManager) {
    if ($id == 'me') {
        $user = $playerManager->getMe();
    } else {
        $user = $playerManager->getById($id);
    }
    if (is_null($user)) {
        return array(
            'Success' => false
        );
    }
    return array(
        'Success' => true,
        'Player' => $user
    );
});
$router->get('api/player/{id}/city', function ($id) use ($cityManager) {
    return array(
        'Success' => true,
        'City' => $cityManager->getCityByPlayerId($id)
    );
});
$router->get('api/health', function () use ($database) {
    return array(
        'Database' => $database->isConnected() ? 'connected' : 'failure'
    );
});
$router->get('api/test', function () use ($debug) {
    header('Content-type:text/html;charset=utf-8');
    echo $debug->createTerrainVisualization();
    die();
}, false);

$router->process($database);
