<?php
declare (strict_types = 1);

require_once 'Database.php';
require_once 'Perlin.php';

class CityManager
{
    /** @var Database */
    private $db;
    /** @var integer */
    private $tickDuration;

    /**
     * @param Database $db
     * @param integer $tickDuration
     */
    public function __construct(Database $db, int $tickDuration)
    {
        $this->db = $db;
        $this->tickDuration = $tickDuration;
    }

    /**
     * @param int $playerId
     * @return array
     */
    public function getCityByPlayerId(int $playerId)
    {
        $city = $this->db->findCityByPlayerId($playerId);
        if (!$city) throw new Exception("No city found for player '$playerId'");
        $fields = $this->db->findFieldsByCityId($city->id);

        $size = sqrt(count($fields));
        $city->height = $size;
        $city->width = $size;
        $city->fields = $fields;

        return array(
            'Success' => true,
            'City' => $city
        );
    }

    /**
     * @param int $cityId
     * @return array
     */
    public function getCityById(int $cityId)
    {
        $city = $this->db->findCityById($cityId);
        if (!$city) throw new Exception("No city found: '$cityId'");
        $fields = $this->db->findFieldsByCityId($city->id);

        $size = sqrt(count($fields));
        $city->height = $size;
        $city->width = $size;
        $city->fields = $fields;

        return array(
            'Success' => true,
            'City' => $city
        );
    }

    /**
     * @param integer $cityId
     * @param integer $x
     * @param integer $y
     * @return array
     */
    public function collect(int $cityId, int $x, int $y)
    {
        $field = $this->db->findField($cityId, $x, $y);
        $city = $this->db->findCityById($cityId);

        $city->food += $field->food;
        $city->wood += $field->wood;
        $city->metal += $field->metal;
        $field->food = 0;
        $field->wood = 0;
        $field->metal = 0;

        $this->db->saveField($field);
        $this->db->saveCity($city);

        return array(
            'Success' => true,
            'CityResources' => array(
                'Food' => $city->food,
                'Wood' => $city->wood,
                'Metal' => $city->metal,
            ),
            'Resources' => array(
                'Food' => $field->food,
                'Wood' => $field->wood,
                'Metal' => $field->metal,
            )
        );
    }

    /**
     * @param int $cityId
     * @return array
     */
    public function upgrade(int $cityId)
    {
        $x = (int)$_POST['x'];
        $y = (int)$_POST['y'];

        $field = $this->db->findField($cityId, $x, $y);
        $city = $this->db->findCityById($cityId);

        $level = $field->buildingLevel + 1;
        $cost = array(
            'food' => 0 * $level,
            'wood' => 3 * $level,
            'metal' => 3 * $level
        );
        if ($field->buildingType == BuildingType::Mine) {
            $cost = array(
                'food' => 0 * $level,
                'wood' => 3 * $level,
                'metal' => 0 * $level
            );
        }

        if ($city->food < $cost['food'] || $city->wood < $cost['wood'] || $city->metal < $cost['metal']) {
            throw new Exception("Not enough resources");
        }

        $city->food -= $cost['food'];
        $city->wood -= $cost['wood'];
        $city->metal -= $cost['metal'];
        $field->buildingLevel++;

        $this->db->saveField($field);
        $this->db->saveCity($city);

        return array('Success' => true);
    }

    /**
     * @param integer $cityId
     * @param integer $x
     * @param integer $y
     * @return array
     */
    public function getStash(int $cityId, int $x, int $y)
    {
        $now = time();
        $field = $this->db->findField($cityId, $x, $y);
        if ($field->lastQuery == 0) $field->lastQuery = $now;
        $ticks = floor($this->getPassedTicks($field->lastQuery, $now));
        $secondsUntilNextUpdate = (int)ceil(max($field->lastQuery + $this->tickDuration - $now, 0));
        $secondsFromLastQuery = (int)min($now - $field->lastQuery, $this->tickDuration);

        $field->lastQuery = $field->lastQuery + ($this->tickDuration * $ticks);
        $level = $field->buildingLevel;
        $production = array(
            'food' => $level,
            'wood' => $level,
            'metal' => $level
        );

        switch ($field->buildingType) {
            case BuildingType::Applefarm:
                $field->food += $ticks * $production['food'];
                break;
            case BuildingType::Fishingboat:
                $field->food += $ticks * $production['food'];
                break;
            case BuildingType::House:
                //Food += GetPassedTicks(lastQuery, lastUpdateTime);
                break;
            case BuildingType::Lumberjack:
                $field->wood += $ticks * $production['wood'];
                break;
            case BuildingType::Mine:
                $field->metal += $ticks * $production['metal'];
                break;
        }

        $this->db->saveField($field);

        return array(
            'Success' => true,
            'Resources' => array(
                'Food' => $field->food,
                'Wood' => $field->wood,
                'Metal' => $field->metal,
            ),
            'SecondsUntilNextUpdate' => $secondsUntilNextUpdate,
            'SecondsFromLastUpdate' => $secondsFromLastQuery
        );
    }

    /**
     * Calculate passed ticks between two timestamps
     *
     * @param integer $last
     * @param integer $now
     * @return integer
     */
    private function getPassedTicks(int $last, int $now): int
    {
        return (int)floor(($now - $last) / $this->tickDuration);
    }

    /**
     * @param int $cityId
     * @return array
     */
    public function createBuilding(int $cityId)
    {
        $b = $_POST['building'];
        $x = (int)$_POST['x'];
        $y = (int)$_POST['y'];

        if (!isset($b) || !isset($x) || !isset($y)) {
            throw new Exception("Invalid data");
        }

        $field = $this->db->findField($cityId, $x, $y);
        if ($field->buildingType == BuildingType::None) {
            $field->buildingType = (int)$b;
            $field->buildingType = $b;
            $field->buildingLevel = 1;
            $this->db->saveField($field);
            return array('Success' => true);
        } else {
            throw new Exception("Building already exists");
        }
    }

    /**
     * @param integer $playerId
     * @return integer city id
     */
    public function createCity(int $playerId)
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

    /**
     * Create a new random terrain
     *
     * @param integer $seed
     * @return array
     */
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
