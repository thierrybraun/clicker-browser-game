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
    public $tickDuration;
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
        return $user;
    }

    public function findPlayerPassword($name)
    {
        $stmt = $this->pdo->prepare('SELECT password FROM player WHERE name=? LIMIT 1');
        $stmt->execute([$name]);
        $user = $stmt->fetch();
        return $user;
    }

    public function findPlayerById(int $id): Player
    {
        $stmt = $this->pdo->prepare('SELECT * FROM player WHERE id=? LIMIT 1');
        $stmt->execute([$id]);
        $user = $stmt->fetchObject('Player');
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
        $stmt = $this->pdo->prepare('INSERT INTO city(id,  idPlayer, tickDuration, food, wood, metal) VALUES (NULL, ?, ?, 0, 0, 0)');
        $stmt->execute([$playerId, 10]);
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
        return $stmt->fetchObject('City');
    }

    public function findFieldsByCityId(int $cityId): array
    {
        $stmt = $this->pdo->prepare('SELECT * FROM field WHERE idCity=? ORDER BY y, x');
        $stmt->execute([$cityId]);
        return $stmt->fetchAll(PDO::FETCH_CLASS, 'Field');
    }
}
