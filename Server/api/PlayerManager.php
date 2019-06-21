<?php

require_once 'Database.php';

class PlayerManager
{
    private $db;
    private $cityManager;

    public function __construct(Database $db, CityManager $cityManager)
    {
        $this->db = $db;
        $this->cityManager = $cityManager;
    }

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

    public function getById(int $id)
    {
        try {
            return array(
                'Success' => true,
                'Player' => $this->db->findPlayerById($id)
            );
        } catch (Exception $e) {
            return array(
                'Success' => false,
                'Error' => $e->getMessage()
            );
        }
    }

    public function getMe()
    {
        $reqHeaders = apache_request_headers();
        $creds = explode(':', base64_decode(substr($reqHeaders['Authorization'], 6)));
        $creds[0];

        $user = $this->db->findPlayerByName($creds[0]);

        try {
            return array(
                'Success' => true,
                'Player' => $this->db->findPlayerByName($creds[0])
            );
        } catch (Exception $e) {
            return array(
                'Success' => false,
                'Error' => $e->getMessage()
            );
        }
    }

    public function createPlayer($username, $password)
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
