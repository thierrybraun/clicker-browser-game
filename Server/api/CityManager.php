<?php

declare(strict_types=1);

require_once 'Database.php';
require_once 'Perlin.php';

foreach (scandir(dirname(__FILE__) . '/generated') as $filename) {
    $path = dirname(__FILE__) . '/generated/' . $filename;
    if (is_file($path)) {
        require_once $path;
    }
}

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

        return new GetCityResponse(true, null, $city);
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

        return new GetCityResponse(true, null, $city);
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

        $city->currency->Food += $field->food;
        $city->currency->Wood += $field->wood;
        $city->currency->Metal += $field->metal;
        $field->food = 0;
        $field->wood = 0;
        $field->metal = 0;

        $this->db->saveField($field);
        $this->db->saveCity($city);
        return new CollectResourcesResponse($city->currency, new Currency($field->food, $field->wood, $field->metal), true);
    }

    public function getCostRange(Currency $baseCost, int $currentLevel, int $targetLevel)
    {
        $currencyTypes = array();
        foreach (new Currency() as $key => $value) {
            $currencyTypes[] = $key;
        }
        $current = new Currency();
        foreach ($currencyTypes as $c) {
            $current->$c = 0;
        }
        for ($i = $currentLevel + 1; $i <= $targetLevel; $i++) {
            foreach ($currencyTypes as $c) {
                $current->$c += $baseCost->$c * $i;
            }
        }
        return $current;
    }

    /**
     * @param int $cityId
     * @return array
     */
    public function upgrade(int $cityId)
    {
        $x = (int) $_POST['x'];
        $y = (int) $_POST['y'];
        $targetLevel = (int) $_POST['targetLevel'];

        $field = $this->db->findField($cityId, $x, $y);
        $city = $this->db->findCityById($cityId);

        $prodFuncName = BuildingFunctions::$costFunctions[$field->buildingType];
        $prodFunc = new $prodFuncName();
        if ($prodFunc->Function !== 'CostFunctionLinear') {
            throw new Exception("Unsupported function: " . $prodFunc->Function);
        }

        $base = $prodFunc->Base;

        $cost = $this->getCostRange($base, $field->buildingLevel, $targetLevel);
        foreach ($cost as $key => $value) {
            if ($city->currency->$key < $value) throw new Exception("Not enough $key");
        }
        foreach ($cost as $key => $value) {
            $city->currency->$key -= $value;
        }
        $field->buildingLevel = $targetLevel;

        $this->db->saveField($field);
        $this->db->saveCity($city);

        return new UpgradeResponse(true);
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
        if ($field->buildingType === BuildingType::None) return new GetStashForTileResponse(false, "No building on tile");
        if ($field->lastQuery == 0) $field->lastQuery = $now;
        $ticks = floor($this->getPassedTicks($field->lastQuery, $now));
        $secondsUntilNextUpdate = (int) ceil(max($field->lastQuery + $this->tickDuration - $now, 0));
        $secondsFromLastQuery = (int) min($now - $field->lastQuery, $this->tickDuration);

        $field->lastQuery = $field->lastQuery + ($this->tickDuration * $ticks);
        $level = $field->buildingLevel;
        $prodFuncName = BuildingFunctions::$productionFunctions[$field->buildingType];
        $prodFunc = new $prodFuncName();
        if ($prodFunc->Function !== 'ProductionFunctionLinear') {
            throw new Exception("Unsupported function: " . $prodFunc->Function);
        }

        $base = $prodFunc->Base;
        foreach (new Currency() as $key => $value) {
            $lk = strtolower($key);
            $field->$lk += $base->$key * $level * $ticks;
        }

        $this->db->saveField($field);

        return new GetStashForTileResponse(true, null, new Currency($field->food, $field->wood, $field->metal), $secondsUntilNextUpdate, $secondsFromLastQuery);
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
        return (int) floor(($now - $last) / $this->tickDuration);
    }

    /**
     * @param int $cityId
     * @return array
     */
    public function createBuilding(int $cityId)
    {
        $b = $_POST['building'];
        $x = (int) $_POST['x'];
        $y = (int) $_POST['y'];

        if (!isset($b) || !isset($x) || !isset($y)) {
            throw new Exception("Invalid data");
        }

        $field = $this->db->findField($cityId, $x, $y);
        if ($field->buildingType == BuildingType::None) {
            $field->buildingType = (int) $b;
            $field->buildingType = $b;
            $field->buildingLevel = 1;
            $this->db->saveField($field);
            return new CreateBuildingResponse(true);
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
        $cityId = (int) $this->db->createCity($playerId);

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
