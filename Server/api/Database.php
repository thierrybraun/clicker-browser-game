<?php

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

    public function findPlayerByName(string $name)
    {
        $stmt = $this->pdo->prepare('SELECT * FROM player WHERE name=? LIMIT 1');
        $stmt->execute([$name]);
        $user = $stmt->fetch();
        if (!is_null($user)) unset($user['password']);
        return $user;
    }

    public function findPlayerPassword($name)
    {
        $stmt = $this->pdo->prepare('SELECT password FROM player WHERE name=? LIMIT 1');
        $stmt->execute([$name]);
        $user = $stmt->fetch();
        return $user;
    }

    public function findPlayerById(int $id)
    {
        $stmt = $this->pdo->prepare('SELECT * FROM player WHERE id=? LIMIT 1');
        $stmt->execute([$id]);
        $user = $stmt->fetch();
        if (!is_null($user)) unset($user['password']);
        return $user;
    }

    public function createPlayer(string $name, string $password): int
    {
        $this->pdo->beginTransaction();
        $stmt = $this->pdo->prepare('INSERT INTO player(id, name, password) VALUES (NULL, ?, ?)');
        $stmt->execute([$name, $password]);

        $id = (int)$this->pdo->lastInsertId();
        $this->createCity($id);

        $this->pdo->commit();
        return $id;
    }

    public function createCity(int $playerId): int
    {
        $stmt = $this->pdo->prepare('INSERT INTO city(id,  idPlayer, tickDuration, food, wood, metal) VALUES (NULL, ?, ?, 0, 0, 0)');
        $stmt->execute([$playerId, 10]);

        $cityId = (int)$this->pdo->lastInsertId();

        for ($i = 0; $i < 10; $i++) {
            for ($j = 0; $j < 10; $j++) {
                $this->createField($cityId, $j, $i, 0, 0);
            }
        }

        return $cityId;
    }

    public function createField(int $cityId, int $x, int $y, int $type, int $resource): int
    {
        $stmt = $this->pdo->prepare('INSERT INTO field(id, idCity, x, y, fieldType, resourceType, buildingType, buildingLevel) VALUES (NULL, ?, ?, ?, ?, ?, 0, 0)');
        $stmt->execute([$cityId, $x, $y, $type, $resource]);
        return (int)$this->pdo->lastInsertId();
    }

    public function findCityByPlayerId(int $playerId)
    {
        $stmt = $this->pdo->prepare('SELECT * FROM city WHERE idPlayer=?');
        $stmt->execute([$playerId]);
        return $stmt->fetch();
    }

    public function findFieldsByCityId(int $cityId)
    {
        $stmt = $this->pdo->prepare('SELECT * FROM field WHERE idCity=? ORDER BY y, x');
        $stmt->execute([$cityId]);
        return $stmt->fetchAll();
    }
}
