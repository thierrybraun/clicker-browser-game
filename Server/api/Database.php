<?php
declare (strict_types = 1);

class Player
{
    /** @var integer */
    public $id;
    /** @var string */
    public $name;
    public function __set($name, $value)
    { }
}

class City
{
    /** @var integer */
    public $id;
    /** @var integer */
    public $idPlayer;
    /** @var integer */
    public $food;
    /** @var integer */
    public $wood;
    /** @var integer */
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
    /** @var integer */
    public $id;
    /** @var integer */
    public $idCity;
    /** @var integer */
    public $x;
    /** @var integer */
    public $y;
    /** @var integer */
    public $fieldType;
    /** @var integer */
    public $resourceType;
    /** @var integer */
    public $buildingType;
    /** @var integer */
    public $buildingLevel;
    /** @var integer */
    public $food;
    /** @var integer */
    public $wood;
    /** @var integer */
    public $metal;
    /** @var integer */
    public $lastQuery;
}

class Database
{
    /** @var PDO */
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

    /**
     * Create database tables if not existing
     *
     * @return void
     */
    public function setup()
    {
        $this->pdo->query('CREATE TABLE IF NOT EXISTS `player` ( 
            `id` INT NOT NULL AUTO_INCREMENT , 
            `name` VARCHAR(255) NOT NULL , 
            `password` VARCHAR(255) NOT NULL , 
            PRIMARY KEY (`id`), UNIQUE (`name`)) 
            ENGINE = InnoDB;');

        $this->pdo->query('CREATE TABLE IF NOT EXISTS `city` ( 
                `id` INT NOT NULL AUTO_INCREMENT , 
                `idPlayer` INT NOT NULL , 
                `food` INT DEFAULT 0, 
                `wood` INT DEFAULT 0, 
                `metal` INT DEFAULT 0 , 
                PRIMARY KEY (`id`), 
                INDEX idPlayerIndex (`idPlayer`),
                FOREIGN KEY (idPlayer)
                    REFERENCES player(id)
                    ON DELETE RESTRICT
            ) ENGINE = InnoDB;');

        $this->pdo->query('CREATE TABLE IF NOT EXISTS `field` ( 
                `id` INT NOT NULL AUTO_INCREMENT , 
                `idCity` INT NOT NULL , 
                `x` INT, 
                `y` INT, 
                `fieldType` INT DEFAULT 0, 
                `resourceType` INT DEFAULT 0, 
                `buildingType` INT DEFAULT 0,
                `buildingLevel` INT DEFAULT 0,
                `food` INT DEFAULT 0, 
                `wood` INT DEFAULT 0, 
                `metal` INT DEFAULT 0, 
                `lastQuery` INT DEFAULT 0,
                PRIMARY KEY (`id`),
                INDEX idCityIndex (`idCity`),
                FOREIGN KEY (idCity)
                    REFERENCES city(id)
                    ON DELETE RESTRICT
            ) ENGINE = InnoDB;');
    }

    /**
     * @return boolean
     */
    public function isConnected()
    {
        return !is_null($this->pdo);
    }

    /**
     * @param string $name
     * @return Player
     */
    public function findPlayerByName(string $name): Player
    {
        $stmt = $this->pdo->prepare('SELECT * FROM player WHERE name=? LIMIT 1');
        $stmt->execute([$name]);
        $user = $stmt->fetchObject('Player');
        if ($user == false) throw new Exception("User '$name' not found");
        return $user;
    }

    /**
     * @param string $name
     * @return array
     */
    public function findPlayerPassword(string $name)
    {
        $stmt = $this->pdo->prepare('SELECT password FROM player WHERE name=? LIMIT 1');
        $stmt->execute([$name]);
        $user = $stmt->fetch();
        if ($user == false) throw new Exception("User '$name' not found");
        return $user;
    }

    /**
     * @param integer $id
     * @return Player
     */
    public function findPlayerById(int $id): Player
    {
        $stmt = $this->pdo->prepare('SELECT * FROM player WHERE id=? LIMIT 1');
        $stmt->execute([$id]);
        $user = $stmt->fetchObject('Player');
        if ($user == false) throw new Exception("User '$id' not found");
        return $user;
    }

    /**
     * @param string $name
     * @param string $password
     * @return integer player id
     */
    public function createPlayer(string $name, string $password): int
    {
        $stmt = $this->pdo->prepare('INSERT INTO player(id, name, password) VALUES (NULL, ?, ?)');
        if (!$stmt->execute([$name, $password])) throw new Exception("Could not create player '$name'");

        $id = (int)$this->pdo->lastInsertId();
        return $id;
    }

    /**
     * @param integer $playerId
     * @return integer city id
     */
    public function createCity(int $playerId): int
    {
        $stmt = $this->pdo->prepare('INSERT INTO city(id, idPlayer, food, wood, metal) VALUES (NULL, ?, 10, 10, 10)');
        $stmt->execute([$playerId]);
        $cityId = (int)$this->pdo->lastInsertId();
        return $cityId;
    }

    /**
     * @param integer $cityId
     * @param integer $x
     * @param integer $y
     * @param integer $type
     * @param integer $resource
     * @return integer field id
     */
    public function createField(int $cityId, int $x, int $y, int $type, int $resource): int
    {
        $stmt = $this->pdo->prepare('INSERT INTO field(id, idCity, x, y, fieldType, resourceType, buildingType, buildingLevel) VALUES (NULL, ?, ?, ?, ?, ?, 0, 0)');
        $stmt->execute([$cityId, $x, $y, $type, $resource]);
        return (int)$this->pdo->lastInsertId();
    }

    /**
     * @param integer $playerId
     * @return City
     */
    public function findCityByPlayerId(int $playerId): City
    {
        $stmt = $this->pdo->prepare('SELECT * FROM city WHERE idPlayer=?');
        $stmt->execute([$playerId]);
        $city = $stmt->fetchObject('City');
        if ($city == false) throw new Exception("City for user '$playerId' not found");
        return $city;
    }

    /**
     * @param integer $cityId
     * @return City
     */
    public function findCityById(int $cityId): City
    {
        $stmt = $this->pdo->prepare('SELECT * FROM city WHERE id=?');
        $stmt->execute([$cityId]);
        $city = $stmt->fetchObject('City');
        if ($city == false) throw new Exception("City '$cityId' not found");
        return $city;
    }

    /**
     * @param integer $cityId
     * @return Field[]
     */
    public function findFieldsByCityId(int $cityId): array
    {
        $stmt = $this->pdo->prepare('SELECT * FROM field WHERE idCity=? ORDER BY y, x');
        $stmt->execute([$cityId]);
        return $stmt->fetchAll(PDO::FETCH_CLASS, 'Field');
    }

    /**
     * @param int $cityId
     * @param int $x
     * @param int $y
     * @return Field
     */
    public function findField(int $cityId, int $x, int $y): Field
    {
        $stmt = $this->pdo->prepare('SELECT * FROM field WHERE idCity=? AND x=? AND y=? LIMIT 1');
        $stmt->execute([$cityId, $x, $y]);
        $field = $stmt->fetchObject('Field');
        if ($field == false) throw new Exception("Field (city=$cityId,x=$x,y=$y) not found");
        return $field;
    }

    /**
     * @param Field $field
     * @return void
     */
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

    /**
     * @param City $city
     * @return void
     */
    public function saveCity(City $city)
    {
        $stmt = $this->pdo->prepare('UPDATE city SET id=?,idPlayer=?,food=?,wood=?,metal=? WHERE id=?');
        $stmt->execute([
            $city->id,
            $city->idPlayer,
            $city->food,
            $city->wood,
            $city->metal,
            $city->id
        ]);
    }
}
