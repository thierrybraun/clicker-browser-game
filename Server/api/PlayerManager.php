<?php

require_once 'Database.php';

class PlayerManager
{
    private $db;

    public function __construct(Database $db)
    {
        $this->db = $db;
    }

    public function getById(int $id)
    {
        return $this->db->findPlayerById($id);
    }

    public function getMe()
    {
        $reqHeaders = apache_request_headers();
        $creds = explode(':', base64_decode(substr($reqHeaders['Authorization'], 6)));
        $creds[0];

        return $this->db->findPlayerByName($creds[0]);
    }

    public function createPlayer($username, $password)
    {
        if (trim($username) === '') throw new Exception('Name can\'t be blank');
        if (trim($password) === '') throw new Exception('Password can\'t be blank');

        $existing = $this->db->findPlayerByName($username);
        if ($existing) throw new Exception('Name already taken');

        $this->db->createPlayer($username, password_hash($password, PASSWORD_DEFAULT));
    }
}
