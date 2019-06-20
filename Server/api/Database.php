<?php

class Player
{
    public $id;
    public $name;
    public function __set($name, $value)
    { }
}

class City
{
    public $id;
    public $idPlayer;
    public $food;
    public $wood;
    public $metal;
}

abstract class FieldType
{
    const Plain = 0;
    const Hills = 1;
    const Water = 2;
}
abstract class BuildingType
{
    const None = 0;
    const House = 1;
    const Applefarm = 2;
    const Fishingboat = 3;
    const Lumberjack = 4;
    const Mine = 5;
}

abstract class ResourceType
{
    const  None = 0;
    const  Apples = 1;
    const  Fish = 2;
    const  Forest = 3;
    const  Ore = 4;
}

class Field
{
    public $id;
    public $idCity;
    public $x;
    public $y;
    public $fieldType;
    public $resourceType;
    public $buildingType;
    public $buildingLevel;
    public $food;
    public $wood;
    public $metal;
    public $lastQuery;
}

class Database
{
    private $pdo = null;

    public function __construct()
    {
        $host = "db";
        $db   = 'game';
        $user = 'test';
        $pass = 'test';
        $charset = 'utf8mb4';

        $dsn = "mysql:host=$host;dbname=$db;charset=$charset";
        $options = [
            PDO::ATTR_ERRMODE            => PDO::ERRMODE_EXCEPTION,
            PDO::ATTR_DEFAULT_FETCH_MODE => PDO::FETCH_ASSOC,
            PDO::ATTR_EMULATE_PREPARES   => false,
        ];
        try {
            $this->pdo = new PDO($dsn, $user, $pass, $options);
        } catch (\PDOException $e) {
            throw new \PDOException($e->getMessage(), (int)$e->getCode());
        }
    }

    public function isConnected()
    {
        return !is_null($this->pdo);
    }

    public function findPlayerByName(string $name): Player
    {
        $stmt = $this->pdo->prepare('SELECT * FROM player WHERE name=? LIMIT 1');
        $stmt->execute([$name]);
        $user = $stmt->fetchObject('Player');
        if ($user == false) throw new Exception("User '$name' not found");
        return $user;
    }

    public function findPlayerPassword($name)
    {
        $stmt = $this->pdo->prepare('SELECT password FROM player WHERE name=? LIMIT 1');
        $stmt->execute([$name]);
        $user = $stmt->fetch();
        if ($user == false) throw new Exception("User '$name' not found");
        return $user;
    }

    public function findPlayerById(int $id): Player
    {
        $stmt = $this->pdo->prepare('SELECT * FROM player WHERE id=? LIMIT 1');
        $stmt->execute([$id]);
        $user = $stmt->fetchObject('Player');
        if ($user == false) throw new Exception("User '$id' not found");
        return $user;
    }

    public function createPlayer(string $name, string $password): int
    {
        $stmt = $this->pdo->prepare('INSERT INTO player(id, name, password) VALUES (NULL, ?, ?)');
        $stmt->execute([$name, $password]);

        $id = (int)$this->pdo->lastInsertId();
        return $id;
    }

    public function createCity(int $playerId): int
    {
        $stmt = $this->pdo->prepare('INSERT INTO city(id, idPlayer, food, wood, metal) VALUES (NULL, ?, 0, 0, 0)');
        $stmt->execute([$playerId]);
        $cityId = (int)$this->pdo->lastInsertId();
        return $cityId;
    }

    public function createField(int $cityId, int $x, int $y, int $type, int $resource): int
    {
        $stmt = $this->pdo->prepare('INSERT INTO field(id, idCity, x, y, fieldType, resourceType, buildingType, buildingLevel) VALUES (NULL, ?, ?, ?, ?, ?, 0, 0)');
        $stmt->execute([$cityId, $x, $y, $type, $resource]);
        return (int)$this->pdo->lastInsertId();
    }

    public function findCityByPlayerId(int $playerId): City
    {
        $stmt = $this->pdo->prepare('SELECT * FROM city WHERE idPlayer=?');
        $stmt->execute([$playerId]);
        $city = $stmt->fetchObject('City');
        if ($city == false) throw new Exception("City for user '$playerId' not found");
        return $city;
    }

    public function findCityById(int $cityId): City
    {
        $stmt = $this->pdo->prepare('SELECT * FROM city WHERE id=?');
        $stmt->execute([$cityId]);
        $city = $stmt->fetchObject('City');
        if ($city == false) throw new Exception("City '$cityId' not found");
        return $city;
    }

    public function findFieldsByCityId(int $cityId): array
    {
        $stmt = $this->pdo->prepare('SELECT * FROM field WHERE idCity=? ORDER BY y, x');
        $stmt->execute([$cityId]);
        return $stmt->fetchAll(PDO::FETCH_CLASS, 'Field');
    }

    public function findField($cityId, $x, $y): Field
    {
        $stmt = $this->pdo->prepare('SELECT * FROM field WHERE idCity=? AND x=? AND y=? LIMIT 1');
        $stmt->execute([$cityId, $x, $y]);
        $field = $stmt->fetchObject('Field');
        if ($field == false) throw new Exception("Field (city=$cityId,x=$x,y=$y) not found");
        return $field;
    }

    public function saveField(Field $field)
    {
        $stmt = $this->pdo->prepare('UPDATE field SET id=?,idCity=?,x=?,y=?,fieldType=?,resourceType=?,buildingType=?,buildingLevel=?,food=?,wood=?,metal=?,lastQuery=? WHERE id=?');
        $stmt->execute([
            $field->id,
            $field->idCity,
            $field->x,
            $field->y,
            $field->fieldType,
            $field->resourceType,
            $field->buildingType,
            $field->buildingLevel,
            $field->food,
            $field->wood,
            $field->metal,
            $field->lastQuery,
            $field->id
        ]);
    }
}
