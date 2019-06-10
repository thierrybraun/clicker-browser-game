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

    public function findPlayerPassword($name) {
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
}
