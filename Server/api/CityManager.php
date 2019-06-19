<?php

require_once 'Database.php';
require_once 'Perlin.php';

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
        $fields = $this->db->findFieldsByCityId($city->id);

        $size = sqrt(count($fields));
        $city->height = $size;
        $city->width = $size;
        $city->fields = $fields;
        return $city;
    }

    public function createCity($playerId)
    {
        $cityId = (int)$this->db->createCity($playerId);

        $terrain = $this->createTerrain($cityId);

        for ($i = 0; $i < count($terrain); $i++) {
            for ($j = 0; $j < count($terrain[$i]); $j++) {
                $field = $terrain[$i][$j];
                $type = $field[0];
                $res = ResourceType::None;
                if (count($field) > 1) {
                    $res = $field[1];
                }
                $this->db->createField($cityId, $j, $i, $type, $res);
            }
        }

        return $cityId;
    }

    public function createTerrain($seed)
    {
        $gen = new Perlin($seed);
        $forest = new Perlin($seed + 1);
        $smooth = 4;
        $gridsize = 20;
        $terrain = array();
        $max = 0;
        $min = 1;
        for ($y = 0; $y < $gridsize; $y += 1) {
            $terrain[$y] = array();
            for ($x = 0; $x < $gridsize; $x += 1) {
                mt_srand($seed + ($y * $gridsize + $x));
                $num = ($gen->noise($x, $y, 0, $smooth)  + 1) / 2;
                if ($num < $min) $min = $num;
                if ($num > $max) $max = $num;

                if ($num < 0.3) {
                    $terrain[$y][$x] = array(FieldType::Water);
                    if (mt_rand(0, 1) == 0) {
                        $terrain[$y][$x][] = ResourceType::Fish;
                    }
                } else if ($num < 0.7) {
                    $terrain[$y][$x] = array(FieldType::Plain);
                    $f = ($forest->noise($x, $y, 0, $smooth)  + 1) / 2;
                    if ($f < 0.3) {
                        $terrain[$y][$x][] = ResourceType::Forest;
                    } else if (mt_rand(0, 1) == 0) {
                        $terrain[$y][$x][] = ResourceType::Apples;
                    }
                } else {
                    $terrain[$y][$x] = array(FieldType::Hills);
                    if (mt_rand(0, 1) == 0) {
                        $terrain[$y][$x][] = ResourceType::Ore;
                    }
                }
            }
        }
        return $terrain;
    }
}
