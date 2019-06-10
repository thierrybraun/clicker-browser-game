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
}
