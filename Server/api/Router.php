<?php
declare (strict_types = 1);

abstract class Method
{
    const Get = 'GET';
    const Post = 'POST';
}

class Route
{
    public $method;
    public $path;
    public $callback;
    public $requiresAuth;

    public function __construct(string $method, string $path, callable $callback, bool $requiresAuth = true)
    {
        $this->method = $method;
        $this->path = splitPath($path);
        $this->callback = $callback;
        $this->requiresAuth = $requiresAuth;
    }
}

class Router
{
    private $routes = array();
    private $db;

    public function __construct(Database $db)
    {
        $this->db = $db;
    }

    public function process()
    {
        $path = splitPath($_SERVER['REQUEST_URI']);
        while ($path[0] !== 'api' && count($path) > 0) {
            array_shift($path);
            $path = array_values($path);
        }
        $method = $_SERVER['REQUEST_METHOD'];
        foreach ($this->routes as $route) {
            if ($this->pathFits($path, $route->path) && $route->method === $method) {

                if ($route->requiresAuth) {
                    $reqHeaders = apache_request_headers();
                    if (!isset($reqHeaders['Authorization']) || !$this->isAuthorized($reqHeaders['Authorization'])) {
                        header('Content-type:application/json;charset=utf-8');
                        echo json_encode(array(
                            'Success' => false,
                            'Error' => 'NotAuthorized'
                        ));
                        return;
                    }
                }

                header('Content-type:application/json;charset=utf-8');
                $res = call_user_func($route->callback, ...$this->getPathVariables($path, $route->path));
                echo json_encode($res);
                return;
            }
        }

        header("HTTP/1.0 404 Not Found");
    }

    private function isAuthorized($auth)
    {
        if (empty($auth)) return false;
        if (!substr($auth, 0, 4) === 'Basic') return false;
        $creds = explode(':', base64_decode(substr($auth, 6)));

        $user = $this->db->findPlayerPassword($creds[0]);
        if (is_null($user)) {
            return false;
        }

        if (password_verify($creds[1], $user['password'])) {
            return true;
        } else {
            return false;
        }
    }

    private function pathFits(array $reqPath, array $routePath)
    {
        if (count($reqPath) !== count($routePath)) return false;

        for ($i = 0; $i < count($routePath); $i++) {
            $part = $routePath[$i];
            if ($this->isPathVariable($part)) {
                continue;
            } else {
                if ($part !== $reqPath[$i]) {
                    return false;
                }
            }
        }

        return true;
    }

    private function getPathVariables(array $reqPath, array $routePath)
    {
        $res = array();
        for ($i = 0; $i < count($routePath); $i++) {
            $part = $routePath[$i];
            if ($this->isPathVariable($part)) {
                $res[] = $reqPath[$i];
            }
        }
        return $res;
    }

    private function isPathVariable(string $part)
    {
        $first = substr($part, 0, 1);
        $last = substr($part, strlen($part) - 1, 1);
        return $first === '{' && $last === '}';
    }

    public function get(string $path, callable $callback, bool $requiresAuth = true)
    {
        $this->routes[] = new Route(Method::Get, $path, $callback, $requiresAuth);
    }

    public function post(string $path, callable $callback, bool $requiresAuth = true)
    {
        $this->routes[] = new Route(Method::Post, $path, $callback, $requiresAuth);
    }
}

function splitPath(string $path)
{
    return array_values(array_filter(explode('/', $path), function ($e) {
        return $e !== '';
    }));
}
