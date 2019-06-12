<?php

require_once 'Database.php';

class CityManager
{
    private $db;

    public function __construct(Database $db)
    {
        $this->db = $db;
    }

    public function getCityByPlayerId($playerId)
    {
        $city = $this->db->findCityByPlayerId($playerId);
        if (!$city) throw new Exception("No city found for player '$playerId'");
        $fields = $this->db->findFieldsByCityId($city['id']);

        $city['height'] = 10;
        $city['width'] = 10;
        $city['fields'] = $fields;
        return $city;
    }
}
