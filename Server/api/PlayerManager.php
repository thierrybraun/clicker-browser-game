<?php

declare(strict_types=1);

require_once 'Database.php';
require_once 'generated/GetPlayerResponse.php';

class PlayerManager
{
    /** @var Database */
    private $db;
    /** @var CityManager */
    private $cityManager;

    /**
     * @param Database $db
     * @param CityManager $cityManager
     */
    public function __construct(Database $db, CityManager $cityManager)
    {
        $this->db = $db;
        $this->cityManager = $cityManager;
    }

    /**
     * Register new user
     *
     * @return void
     */
    public function register()
    {
        try {
            $username = $_POST['username'];
            $password = $_POST['password'];
            $this->createPlayer($username, $password);
        } catch (\Throwable $th) {
            return array(
                'Success' => false,
                'Error' => $th->getMessage()
            );
        }
        return array(
            'Success' => true,
        );
    }

    /**
     * Get player by id
     *
     * @param integer $id
     * @return array
     */
    public function getById(int $id)
    {
        return new GetPlayerResponse(true, null, $this->db->findPlayerById($id));
    }

    /**
     * Get player data with credentials from request
     *
     * @return array
     */
    public function getMe()
    {
        $reqHeaders = apache_request_headers();
        $creds = explode(':', base64_decode(substr($reqHeaders['Authorization'], 6)));
        return new GetPlayerResponse(true, null, $this->db->findPlayerByName($creds[0]));
    }

    /**
     * Create a player
     *
     * @param string $username
     * @param string $password
     * @return void
     */
    public function createPlayer(string $username, string $password)
    {
        if (trim($username) === '') throw new Exception('Name can\'t be blank');
        if (trim($password) === '') throw new Exception('Password can\'t be blank');

        try {
            $existing = $this->db->findPlayerByName($username);
        } catch (Exception $e) { }
        if ($existing) throw new Exception('Name already taken');

        $id = $this->db->createPlayer($username, password_hash($password, PASSWORD_DEFAULT));
        $this->cityManager->createCity($id);
    }
}
