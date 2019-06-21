<?php

class Admin
{
    private $database;

    public function __construct(Database $database)
    {
        $this->database = $database;
    }

    private function check_credentials()
    {
        $reqHeaders = apache_request_headers();
        if (!isset($reqHeaders['Authorization'])) throw new Exception('Incomplete credentials');
        $auth = $reqHeaders['Authorization'];
        if (empty($auth)) throw new Exception('Incomplete credentials');
        if (!substr($auth, 0, 4) === 'Basic') throw new Exception('Not authorized');
        $userCredentials = explode(':', base64_decode(substr($auth, 6)));

        if (getenv('admin_credentials') == '') error_log("Admin password could not be read. Please set it in the environment variable 'admin_credentials");
        $adminCredentials = array_filter(explode(':', getenv('admin_credentials')));
        if ($userCredentials[0] !== trim($adminCredentials[0]) || $userCredentials[1] !== trim($adminCredentials[1])) throw new Exception("Not authorized");        
    }

    public function setup()
    {
        $this->check_credentials();
        $this->database->setup();
        return array('Success' => true);
    }
}
