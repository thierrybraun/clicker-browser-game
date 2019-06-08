<?php
declare (strict_types = 1);

require 'Router.php';
require 'DataLoader.php';

$dataLoader = new DataLoader();

$router = new Router();
$router->get('api/login', function () {
    $res['Success'] = true;
    return $res;
});
$router->get('api/player/{id}', function ($id) use ($dataLoader) {
    return array(
        'Success' => true,
        'Player' => $dataLoader->GetPlayer($id == 'me' ? 0 : $id)
    );
});
$router->get('api/player/{id}/city', function ($id) {
    return array(
        'Success' => true,
        'City' => json_decode(file_get_contents('city.json'))
    );
});

$router->process();
