<?php
declare (strict_types = 1);

require_once 'Router.php';
require_once 'Database.php';
require_once 'PlayerManager.php';

$database = new Database();
$router = new Router($database);
$playerManager = new PlayerManager($database);

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
$router->get('api/player/{id}/city', function ($id) {
    return array(
        'Success' => true,
        'City' => json_decode(file_get_contents('city.json'))
    );
});
$router->get('api/health', function () use ($database) {
    return array(
        'Database' => $database->isConnected() ? 'connected' : 'failure'
    );
});

$router->process($database);